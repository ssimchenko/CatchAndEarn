using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using CatchAndEarn.Controller;
using CatchAndEarn.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CatchAndEarn;

public partial class MainWindow : Window
{
    private GameController? gameController;
    private SkillCheck? skillCheck;
    private GameState state = GameState.Idle;

    private readonly FishingService fishingService = new();
    private Fish? currentFish;
    private readonly Random random = new();

    private readonly List<Upgrade> upgrades = new()
    {
        new Upgrade("Ключ от Озера 2", "Открывает второе озеро. Улучшения можно купить заново.", 1000),
        new Upgrade("Широкая зона", "Увеличивает зону успеха в skillcheck в 3 раза", 10),
        new Upgrade("Золотая приманка", "Повышает шанс поймать редкую рыбу на 5%", 10),
        new Upgrade("Бонус монет", "Получай на 10% больше монет с каждой рыбы", 10),
        new Upgrade("Скоростная реакция", "Замедляет движение маркера на 50%", 10),
        new Upgrade("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 10)
    };

    public MainWindow()
    {
        InitializeComponent();
    }

    enum GameState
    {
        Idle,
        Waiting,
        SkillCheck,
        Result
    }

    private void StartGame_Click(object? sender, RoutedEventArgs e)
    {
        if (gameController == null)
        {
            var player = new Player();
            gameController = new GameController(player);
        }

        MenuPanel.IsVisible = false;
        GamePanel.IsVisible = true;

        int lake = gameController.GetPlayer().CurrentLake;
        LakeText.Text = lake == 1 ? "Озеро 1" : "Озеро 2";
        ResultText.Text = "Нажми Ловить";

        UpdateCoins();
        UpdateCollection();
        UpdateShopUI();
    }

    private void BackToMenu_Click(object? sender, RoutedEventArgs e)
    {
        GamePanel.IsVisible = false;
        MenuPanel.IsVisible = true;
        CollectionPanel.IsVisible = false;
        ShopPanel.IsVisible = false;
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleCollection_Click(object? sender, RoutedEventArgs e)
    {
        CollectionPanel.IsVisible = !CollectionPanel.IsVisible;
        UpdateCollection();
    }

    private void UpdateCollection()
    {
        if (gameController == null) return;

        var text = new StringBuilder();
        foreach (var fish in fishingService.GetAllFishes())
        {
            string status = gameController.HasCaughtFish(fish.Name) ? "✓" : "✕";
            text.AppendLine($"{status} {fish.Name} — {fish.Chance:0.#}%");
        }

        CollectionText.Text = text.ToString();
    }

    private void ToggleShop_Click(object? sender, RoutedEventArgs e)
    {
        ShopPanel.IsVisible = !ShopPanel.IsVisible;
        UpdateShopUI();
    }

    private void UpdateShopUI()
    {
        ShopList.Children.Clear();

        int currentLake = gameController != null ? gameController.GetPlayer().CurrentLake : 1;

        foreach (var upgrade in upgrades)
        {
            var panel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10 };

            string displayText = $"{upgrade.Name} — {upgrade.Description} — {upgrade.Cost} монет";
            if (upgrade.Name == "Ключ от Озера 2" && currentLake >= 2)
            {
                displayText = $"{upgrade.Name} — Уже открыто";
            }

            var info = new TextBlock
            {
                Text = displayText,
                Foreground = Brushes.White,
                Width = 260,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };

            var button = new Button
            {
                Content = upgrade.Purchased ? "Куплено" : "Купить",
                IsEnabled = !(upgrade.Purchased || (upgrade.Name == "Ключ от Озера 2" && currentLake >= 2)),
                Width = 100
            };

            button.Click += (_, __) =>
            {
                if (gameController == null) return;

                if (upgrade.Name == "Ключ от Озера 2")
                {
                    if (currentLake >= 2) return;

                    if (gameController.GetPlayer().SpendCoins(upgrade.Cost))
                    {
                        upgrade.Purchased = true;
                        gameController.GetPlayer().CurrentLake = 2;
                        LakeText.Text = "Озеро 2";
                        ResultText.Text = "Добро пожаловать в Озеро 2! Улучшения сброшены и доступны для покупки заново.";

                        foreach (var u in upgrades)
                            if (u.Name != "Ключ от Озера 2") u.Purchased = false;

                        UpdateCoins();
                        UpdateShopUI();
                    }
                }
                else
                {
                    if (gameController.BuyUpgrade(upgrade))
                    {
                        UpdateCoins();
                        UpdateShopUI();
                    }
                }
            };

            panel.Children.Add(info);
            panel.Children.Add(button);

            ShopList.Children.Add(panel);
        }
    }

    private async void CatchButton_Click(object? sender, RoutedEventArgs e)
    {
        if (gameController == null) return;

        if (state == GameState.Idle)
        {
            state = GameState.Waiting;
            ResultText.Text = "Ожидание поклёвки...";

            await Task.Delay(random.Next(1000, 2000));

            bool goldenLureActive = gameController.GetPlayer().HasUpgrade("Золотая приманка");
            double lureBonus = gameController.GetPlayer().CurrentLake == 1 ? 5.0 : 10.0;
            var fish = fishingService.TryCatchFish(goldenLureActive, lureBonus);

            if (fish == null)
            {
                ResultText.Text = "Не клюёт";
                state = GameState.Idle;
                return;
            }

            currentFish = fish;

            int currentLake = gameController.GetPlayer().CurrentLake;

            double zoneBonus = 1.0;
            if (gameController.GetPlayer().HasUpgrade("Широкая зона"))
                zoneBonus = currentLake == 1 ? 3.0 : 5.0;

            double markerSpeedBonus = 1.0;
            if (gameController.GetPlayer().HasUpgrade("Скоростная реакция"))
                markerSpeedBonus = currentLake == 1 ? 0.5 : 0.3;

            skillCheck = new SkillCheck(fish.Difficulty, zoneBonus, markerSpeedBonus);
            skillCheck.OnUpdate += UpdateSkillCheckUI;
            skillCheck.Start();

            SkillCheckPanel.IsVisible = true;
            state = GameState.SkillCheck;
            ResultText.Text = $"ЛОВИ! ({fish.Name})";
        }
        else if (state == GameState.SkillCheck && skillCheck != null)
        {
            bool success = skillCheck.TryHit();
            state = GameState.Result;

            if (success)
            {
                SuccessZone.Background = new SolidColorBrush(Colors.LimeGreen);

                string resultMessage = "";
                if (currentFish != null)
                    resultMessage = gameController.RewardFish(currentFish);

                bool trophyNetActive = gameController.GetPlayer().HasUpgrade("Трофейная сетка");
                if (trophyNetActive)
                {
                    bool lure = gameController.GetPlayer().HasUpgrade("Золотая приманка");
                    double lb = gameController.GetPlayer().CurrentLake == 1 ? 5.0 : 10.0;
                    var secondFish = fishingService.TryCatchFish(lure, lb);
                    if (secondFish != null)
                    {
                        resultMessage += "\n" + gameController.RewardFish(secondFish);
                    }
                    else
                    {
                        resultMessage += "\nТрофейная сетка: вторая рыба не клюнула";
                    }
                }

                ResultText.Text = resultMessage;
                UpdateCoins();
                UpdateCollection();
            }
            else
            {
                SuccessZone.Background = new SolidColorBrush(Colors.Red);
                ResultText.Text = "Рыба сорвалась";
            }

            await Task.Delay(600);
            SkillCheckPanel.IsVisible = false;
            SuccessZone.Background = new SolidColorBrush(Colors.Green);

            await Task.Delay(200);
            state = GameState.Idle;
            ResultText.Text = "Нажми Ловить";
        }
    }

    private void UpdateSkillCheckUI()
    {
        if (skillCheck == null) return;

        double width = SkillCheckPanel.Bounds.Width;

        Marker.Margin = new Thickness(skillCheck.Position * width, 0, 0, 0);

        double zoneWidth = (skillCheck.ZoneEnd - skillCheck.ZoneStart) * width;
        double zoneStart = skillCheck.ZoneStart * width;

        SuccessZone.Width = zoneWidth;
        SuccessZone.Margin = new Thickness(zoneStart, 0, 0, 0);
    }

    private void UpdateCoins()
    {
        if (gameController == null) return;
        CoinsText.Text = $"Монеты: {gameController.GetCoins()}";
    }
}