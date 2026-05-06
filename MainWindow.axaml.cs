using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using CatchAndEarn.Controller;
using CatchAndEarn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool isTestMode = false;

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
        StartGame(false);
    }

    private void StartTestGame_Click(object? sender, RoutedEventArgs e)
    {
        StartGame(true);
    }

    private void StartGame(bool testMode)
    {
        isTestMode = testMode;
        
        if (gameController == null)
        {
            var player = new Player();
            gameController = new GameController(player);
        }
        else
        {
            gameController.GetPlayer().ClearUpgrades();
            gameController.GetPlayer().CurrentLake = 1;
        }

        MenuPanel.IsVisible = false;
        GamePanel.IsVisible = true;
        WinPanel.IsVisible = false;

        LakeText.Text = "Озеро 1";
        ResultText.Text = testMode ? "ТЕСТОВЫЙ РЕЖИМ (дешёвые улучшения)" : "Нажми Ловить";

        UpdateCoins();
        UpdateCollection();
        UpdateShopUI();
    }

    private void BackToMenu_Click(object? sender, RoutedEventArgs e)
    {
        GamePanel.IsVisible = false;
        CollectionPanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        WinPanel.IsVisible = false;
        MenuPanel.IsVisible = true;
        isTestMode = false;
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

        int lake = gameController.GetPlayer().CurrentLake;
        var fishes = fishingService.GetFishesForLake(lake);

        var text = new StringBuilder();
        text.AppendLine($"--- Озеро {lake} ---");
        foreach (var fish in fishes)
        {
            string status = gameController.HasCaughtFish(fish.Name) ? "✓" : "";
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

        var upgradeDefs = GetUpgradeDefinitions(currentLake);

        // Сортировка от дешевых к дорогим
        var sortedDefs = upgradeDefs.OrderBy(x => x.Cost).ToList();

        foreach (var def in sortedDefs)
        {
            var panel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10 };

            bool isPurchased = gameController != null && gameController.GetPlayer().HasUpgrade(def.Name);
            bool isDisabled = isPurchased || (def.Name == "Ключ от Озера 2" && currentLake >= 2);

            string displayText = $"{def.Name} — {def.Desc} — {def.Cost} монет";
            if (def.Name == "Ключ от Озера 2" && currentLake >= 2)
                displayText = $"{def.Name} — Уже открыто";

            var info = new TextBlock
            {
                Text = displayText,
                Foreground = Brushes.White,
                Width = 260,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };

            var button = new Button
            {
                Content = isPurchased ? "Куплено" : "Купить",
                IsEnabled = !isDisabled,
                Width = 100
            };

            button.Click += (_, __) =>
            {
                if (gameController == null) return;

                if (def.Name == "Ключ от Озера 2")
                {
                    if (gameController.GetPlayer().SpendCoins(def.Cost))
                    {
                        gameController.GetPlayer().CurrentLake = 2;
                        gameController.GetPlayer().ClearUpgrades();
                        LakeText.Text = "Озеро 2";
                        ResultText.Text = "Добро пожаловать в Озеро 2! Улучшения сброшены.";
                        UpdateCoins();
                        UpdateShopUI();
                        UpdateCollection();
                    }
                }
                else if (def.Name == "Корона Победителя")
                {
                    if (gameController.GetPlayer().SpendCoins(def.Cost))
                    {
                        gameController.GetPlayer().PurchaseUpgrade(def.Name);
                        ShowWinScreen();
                    }
                }
                else
                {
                    if (gameController.BuyUpgrade(def.Name, def.Cost))
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

    private List<(string Name, string Desc, int Cost)> GetUpgradeDefinitions(int lake)
    {
        if (isTestMode)
        {
            return lake == 1 ? new List<(string, string, int)>
            {
                ("Ключ от Озера 2", "Открывает второе озеро. Улучшения можно купить заново.", 100),
                ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2 раза", 10),
                ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 3%", 10),
                ("Бонус монет", "Получай на 15% больше монет", 10),
                ("Скоростная реакция", "Замедляет движение маркера на 30%", 10),
                ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 10)
            } : new List<(string, string, int)>
            {
                ("Корона Победителя", "Покупка завершает игру с победой.", 100),
                ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2.5 раза", 10),
                ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 6%", 10),
                ("Бонус монет", "Получай на 25% больше монет", 10),
                ("Скоростная реакция", "Замедляет движение маркера на 50%", 10),
                ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 10)
            };
        }
        else
        {
            return lake == 1 ? new List<(string, string, int)>
            {
                ("Ключ от Озера 2", "Открывает второе озеро. Улучшения можно купить заново.", 500),
                ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2 раза", 50),
                ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 3%", 50),
                ("Бонус монет", "Получай на 15% больше монет", 25),
                ("Скоростная реакция", "Замедляет движение маркера на 30%", 100),
                ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 200)
            } : new List<(string, string, int)>
            {
                ("Корона Победителя", "Покупка завершает игру с победой.", 1000),
                ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2.5 раза", 50),
                ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 6%", 50),
                ("Бонус монет", "Получай на 25% больше монет", 25),
                ("Скоростная реакция", "Замедляет движение маркера на 50%", 100),
                ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 200)
            };
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

            int currentLake = gameController.GetPlayer().CurrentLake;
            bool goldenLureActive = gameController.GetPlayer().HasUpgrade("Золотая приманка");
            double lureBonus = currentLake == 1 ? 3.0 : 6.0;

            var fish = fishingService.TryCatchFish(currentLake, goldenLureActive, lureBonus, excludeUltraRare: false);

            if (fish == null)
            {
                ResultText.Text = "Не клюёт";
                state = GameState.Idle;
                return;
            }

            currentFish = fish;

            double zoneBonus = gameController.GetPlayer().HasUpgrade("Широкая зона") ? (currentLake == 1 ? 2.0 : 2.5) : 1.0;
            double markerSpeedBonus = gameController.GetPlayer().HasUpgrade("Скоростная реакция") ? (currentLake == 1 ? 0.7 : 0.5) : 1.0;

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
                    int currentLake = gameController.GetPlayer().CurrentLake;
                    bool lure = gameController.GetPlayer().HasUpgrade("Золотая приманка");
                    double lb = currentLake == 1 ? 3.0 : 6.0;
                    var secondFish = fishingService.TryCatchFish(currentLake, lure, lb, excludeUltraRare: true);
                    if (secondFish != null)
                        resultMessage += "\n" + gameController.RewardFish(secondFish);
                    else
                        resultMessage += "\nТрофейная сетка: вторая рыба не клюнула";
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

    private void ShowWinScreen()
    {
        GamePanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        CollectionPanel.IsVisible = false;
        WinCoinsText.Text = $"Итоговые монеты: {gameController.GetCoins()}";
        WinPanel.IsVisible = true;
    }
}