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
        new Upgrade("Широкая зона", "Увеличивает зону успеха в skillcheck в 3 раза", 10),
        new Upgrade("Золотая приманка", "Повышает шанс поймать редкую рыбу на 5%", 10),
        new Upgrade("Бонус монет", "Получай на 10% больше монет с каждой рыбы", 50),
        new Upgrade("Скоростная реакция", "Замедляет движение маркера на 15%", 60),
        new Upgrade("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 70)
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

        ResultText.Text = "Нажми Ловить";

        UpdateCoins();
        UpdateCollection();
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

        foreach (var upgrade in upgrades)
        {
            var panel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10 };

            var info = new TextBlock
            {
                Text = $"{upgrade.Name} — {upgrade.Description} — {upgrade.Cost} монет",
                Foreground = Brushes.White,
                Width = 260,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };

            var button = new Button
            {
                Content = upgrade.Purchased ? "Куплено" : "Купить",
                IsEnabled = !upgrade.Purchased,
                Width = 100
            };

            button.Click += (_, __) =>
            {
                if (gameController != null && gameController.BuyUpgrade(upgrade))
                {
                    UpdateCoins();
                    UpdateShopUI();
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

            var fish = fishingService.TryCatchFish();

            if (fish == null)
            {
                ResultText.Text = "Не клюёт";
                state = GameState.Idle;
                return;
            }

            currentFish = fish;

            double zoneBonus = 1.0;
            if (gameController.GetPlayer().HasUpgrade("Широкая зона"))
                zoneBonus = 3.0;

            skillCheck = new SkillCheck(fish.Difficulty, zoneBonus);
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

                if (currentFish != null)
                    ResultText.Text = gameController.RewardFish(currentFish);

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