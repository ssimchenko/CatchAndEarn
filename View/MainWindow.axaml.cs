using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using CatchAndEarn.Controller;
using CatchAndEarn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatchAndEarn.View;

public partial class MainWindow : Window
{
    private GameController? gameController;
    private SkillCheck? skillCheck;
    private GameState state = GameState.Idle;

    private readonly FishingService fishingService = new();
    private Fish? currentFish;
    private readonly Random random = new();
    private bool isTestMode;

    public MainWindow()
    {
        InitializeComponent();
        this.Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, Avalonia.Controls.WindowClosingEventArgs e)
    {
        AudioService.StopAll();
    }
    private enum GameState
    {
        Idle,
        Waiting,
        SkillCheck,
        Result
    }

    private void StartGame_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        StartGame(false);
    }

    private void StartTestGame_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        StartGame(true);
    }

    private async void StartGame(bool testMode)
    {
        isTestMode = testMode;

        var player = new Player();
        gameController = new GameController(player);

        MenuPanel.IsVisible = false;
        GamePanel.IsVisible = true;
        WinPanel.IsVisible = false;

        CollectionPanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        SkillCheckPanel.IsVisible = false;

        Lake1Background.IsVisible = true;
        Lake2Background.IsVisible = false;

        Lake1Sign.IsVisible = true;
        Lake2Sign.IsVisible = false;

        LakeText.Text = "Глубокий пруд";
        ResultText.Text = testMode ? "ТЕСТОВЫЙ РЕЖИМ" : "";

        state = GameState.Idle;

        UpdateCoins();
        UpdateCollection();
        UpdateShopUI();

        // Запускаем фоновую музыку
        await AudioService.PlayBackgroundMusicAsync("background_music_calm.wav");
    }

    private void BackToMenu_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        
        GamePanel.IsVisible = false;
        CollectionPanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        WinPanel.IsVisible = false;
        MenuPanel.IsVisible = true;

        SkillCheckPanel.IsVisible = false;

        Lake1Background.IsVisible = true;
        Lake2Background.IsVisible = false;

        Lake1Sign.IsVisible = true;
        Lake2Sign.IsVisible = false;

        ResultText.Text = "";
        state = GameState.Idle;
        isTestMode = false;

        // Останавливаем фоновую музыку
        AudioService.StopBackgroundMusic();
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        Close();
    }

    private void ToggleCollection_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        
        if (gameController == null)
            return;

        UpdateCollection();

        bool shouldOpen = !CollectionPanel.IsVisible;

        ShopPanel.IsVisible = false;
        CollectionPanel.IsVisible = shouldOpen;
    }

    private void ToggleShop_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        
        if (gameController == null)
            return;

        UpdateShopUI();

        bool shouldOpen = !ShopPanel.IsVisible;

        CollectionPanel.IsVisible = false;
        ShopPanel.IsVisible = shouldOpen;
    }

    private void UpdateCollection()
    {
        if (gameController == null)
            return;

        int lake = gameController.GetPlayer().CurrentLake;
        var fishes = fishingService.GetFishesForLake(lake);

        string lakeName = GetLakeName(lake);

        var text = new StringBuilder();
        text.AppendLine($"--- {lakeName} ---");

        foreach (var fish in fishes)
        {
            string status = gameController.HasCaughtFish(fish.Name) ? "✓" : "✕";
            text.AppendLine($"{status} {fish.Name} — {fish.Chance:0.#}%");
        }

        CollectionText.Text = text.ToString();
    }

    private string GetLakeName(int lake)
    {
        if (lake == 1)
            return "Глубокий пруд";

        return "Море C#";
    }

    private void UpdateShopUI()
    {
        ShopList.Children.Clear();

        if (gameController == null)
            return;

        int currentLake = gameController.GetPlayer().CurrentLake;
        var upgrades = GetUpgradeDefinitions(currentLake)
            .OrderBy(upgrade => upgrade.Cost)
            .ToList();

        foreach (var upgrade in upgrades)
        {
            AddShopItem(upgrade.Name, upgrade.Desc, upgrade.Cost, currentLake);
        }
    }

    private List<(string Name, string Desc, int Cost)> GetUpgradeDefinitions(int lake)
    {
        if (isTestMode)
        {
            if (lake == 1)
            {
                return new List<(string, string, int)>
                {
                    ("Ключ от Моря C#", "Открывает Море C#. Улучшения можно купить заново.", 100),
                    ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2 раза", 10),
                    ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 3%", 10),
                    ("Бонус монет", "Получай на 15% больше монет", 10),
                    ("Скоростная реакция", "Замедляет движение маркера на 30%", 10),
                    ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 10)
                };
            }

            return new List<(string, string, int)>
            {
                ("Корона Победителя", "Покупка завершает игру с победой.", 100),
                ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2.5 раза", 10),
                ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 6%", 10),
                ("Бонус монет", "Получай на 25% больше монет", 10),
                ("Скоростная реакция", "Замедляет движение маркера на 50%", 10),
                ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 10)
            };
        }

        if (lake == 1)
        {
            return new List<(string, string, int)>
            {
                ("Ключ от Моря C#", "Открывает Море C#. Улучшения можно купить заново.", 500),
                ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2 раза", 50),
                ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 3%", 50),
                ("Бонус монет", "Получай на 15% больше монет", 25),
                ("Скоростная реакция", "Замедляет движение маркера на 30%", 100),
                ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 200)
            };
        }

        return new List<(string, string, int)>
        {
            ("Корона Победителя", "Покупка завершает игру с победой.", 1000),
            ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2.5 раза", 50),
            ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 6%", 50),
            ("Бонус монет", "Получай на 25% больше монет", 25),
            ("Скоростная реакция", "Замедляет движение маркера на 50%", 100),
            ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 200)
        };
    }

    private void AddShopItem(string name, string description, int cost, int currentLake)
    {
        if (gameController == null)
            return;

        bool isPurchased = gameController.GetPlayer().HasUpgrade(name);
        bool isLakeKeyDisabled = name == "Ключ от Моря C#" && currentLake >= 2;
        bool isDisabled = isPurchased || isLakeKeyDisabled;

        var itemBorder = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#8C2A2018")),
            BorderBrush = new SolidColorBrush(Color.Parse("#6D4D32")),
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 8)
        };

        var rootPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12
        };

        var textPanel = new StackPanel
        {
            Spacing = 4,
            Width = 280
        };

        string title = isLakeKeyDisabled ? $"{name} — уже открыто" : name;

        var titleBlock = new TextBlock
        {
            Text = title,
            Foreground = new SolidColorBrush(Color.Parse("#FFF1C9")),
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            TextWrapping = TextWrapping.Wrap
        };

        var descBlock = new TextBlock
        {
            Text = description,
            Foreground = new SolidColorBrush(Color.Parse("#F8E9C8")),
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            TextWrapping = TextWrapping.Wrap
        };

        var priceBlock = new TextBlock
        {
            Text = $"Цена: {cost} монет",
            Foreground = new SolidColorBrush(Color.Parse("#FFD45C")),
            FontSize = 15,
            FontWeight = FontWeight.Bold
        };

        textPanel.Children.Add(titleBlock);

        if (!isLakeKeyDisabled)
        {
            textPanel.Children.Add(descBlock);
            textPanel.Children.Add(priceBlock);
        }

        rootPanel.Children.Add(textPanel);
        rootPanel.Children.Add(CreateBuyButton(name, cost, isPurchased, isDisabled));

        itemBorder.Child = rootPanel;
        ShopList.Children.Add(itemBorder);
    }

    private Button CreateBuyButton(string upgradeName, int cost, bool isPurchased, bool isDisabled)
    {
        var buttonText = new TextBlock
        {
            Text = isPurchased ? "Куплено" : "Купить",
            Foreground = new SolidColorBrush(Color.Parse("#FFF1C9")),
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        var buttonView = new Border
        {
            Background = new SolidColorBrush(isDisabled ? Color.Parse("#4A3A2D") : Color.Parse("#7A5635")),
            BorderBrush = new SolidColorBrush(isDisabled ? Color.Parse("#6D5A45") : Color.Parse("#B48A5A")),
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(10, 6),
            Child = buttonText
        };

        var button = new Button
        {
            Width = 95,
            Height = 42,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            Content = buttonView,
            IsEnabled = !isDisabled,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };

        button.Click += (_, _) => BuyUpgrade(upgradeName, cost);

        return button;
    }

    private void BuyUpgrade(string upgradeName, int cost)
    {
        if (gameController == null)
            return;

        if (upgradeName == "Ключ от Моря C#")
        {
            if (!gameController.GetPlayer().SpendCoins(cost))
            {
                ResultText.Text = "Недостаточно монет";
                return;
            }

            _ = AudioService.PlayAsync("unlock_lake.wav");

            gameController.GetPlayer().CurrentLake = 2;
            gameController.GetPlayer().ClearUpgrades();

            Lake1Background.IsVisible = false;
            Lake2Background.IsVisible = true;

            Lake1Sign.IsVisible = false;
            Lake2Sign.IsVisible = true;

            LakeText.Text = "Море C#";
            ResultText.Text = "Открыто Море C#. Улучшения сброшены.";

            CollectionPanel.IsVisible = false;
            ShopPanel.IsVisible = true;

            UpdateCoins();
            UpdateShopUI();
            UpdateCollection();
            return;
        }

        if (upgradeName == "Корона Победителя")
        {
            if (!gameController.GetPlayer().SpendCoins(cost))
            {
                ResultText.Text = "Недостаточно монет";
                return;
            }

            _ = AudioService.PlayAsync("win.wav");

            gameController.GetPlayer().PurchaseUpgrade(upgradeName);
            UpdateCoins();
            ShowWinScreen();
            return;
        }

        if (!gameController.BuyUpgrade(upgradeName, cost))
        {
            ResultText.Text = "Недостаточно монет или улучшение уже куплено";
            return;
        }

        _ = AudioService.PlayAsync("buy.wav");

        ResultText.Text = $"Куплено: {upgradeName}";

        UpdateCoins();
        UpdateShopUI();
    }

    private async void CatchButton_Click(object? sender, RoutedEventArgs e)
    {
        if (gameController == null)
            return;

        if (state == GameState.Idle)
        {
            _ = AudioService.PlayAsync("click.wav");
            _ = AudioService.PlayAsync("cast.wav");
            
            state = GameState.Waiting;
            ResultText.Text = "Ожидание поклёвки...";

            await Task.Delay(random.Next(1000, 2000));

            int currentLake = gameController.GetPlayer().CurrentLake;
            bool goldenLureActive = gameController.GetPlayer().HasUpgrade("Золотая приманка");
            double lureBonus = currentLake == 1 ? 3.0 : 6.0;

            var fish = fishingService.TryCatchFish(
                currentLake,
                goldenLureActive,
                lureBonus,
                excludeUltraRare: false);

            if (fish == null)
            {
                _ = AudioService.PlayAsync("fail.wav");
                ResultText.Text = "Не клюёт";
                state = GameState.Idle;
                return;
            }

            _ = AudioService.PlayAsync("bite.wav");

            currentFish = fish;

            double zoneBonus = gameController.GetPlayer().HasUpgrade("Широкая зона")
                ? currentLake == 1 ? 2.0 : 2.5
                : 1.0;

            double markerSpeedBonus = gameController.GetPlayer().HasUpgrade("Скоростная реакция")
                ? currentLake == 1 ? 0.7 : 0.5
                : 1.0;

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
                _ = AudioService.PlayAsync("success.wav");
                _ = AudioService.PlayAsync("coin.wav");
                
                SuccessZone.Background = new SolidColorBrush(Colors.LimeGreen);

                string resultMessage = "";

                if (currentFish != null)
                    resultMessage = gameController.RewardFish(currentFish);

                if (gameController.GetPlayer().HasUpgrade("Трофейная сетка"))
                {
                    int currentLake = gameController.GetPlayer().CurrentLake;
                    bool goldenLureActive = gameController.GetPlayer().HasUpgrade("Золотая приманка");
                    double lureBonus = currentLake == 1 ? 3.0 : 6.0;

                    var secondFish = fishingService.TryCatchFish(
                        currentLake,
                        goldenLureActive,
                        lureBonus,
                        excludeUltraRare: true);

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
                _ = AudioService.PlayAsync("fail.wav");
                
                SuccessZone.Background = new SolidColorBrush(Colors.Red);
                ResultText.Text = "Рыба сорвалась";
            }

            await Task.Delay(600);

            SkillCheckPanel.IsVisible = false;
            SuccessZone.Background = new SolidColorBrush(Colors.Green);

            await Task.Delay(200);

            state = GameState.Idle;

            if (!WinPanel.IsVisible)
                ResultText.Text = "";
        }
    }

    private void UpdateSkillCheckUI()
    {
        if (skillCheck == null)
            return;

        double width = SkillCheckPanel.Bounds.Width;

        Marker.Margin = new Thickness(skillCheck.Position * width, 0, 0, 0);

        double zoneWidth = (skillCheck.ZoneEnd - skillCheck.ZoneStart) * width;
        double zoneStart = skillCheck.ZoneStart * width;

        SuccessZone.Width = zoneWidth;
        SuccessZone.Margin = new Thickness(zoneStart, 0, 0, 0);
    }

    private void UpdateCoins()
    {
        if (gameController == null)
            return;

        CoinsText.Text = gameController.GetCoins().ToString();
    }

    private void ShowWinScreen()
    {
        if (gameController == null)
            return;

        GamePanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        CollectionPanel.IsVisible = false;

        WinCoinsText.Text = $"Итоговые монеты: {gameController.GetCoins()}";
        WinPanel.IsVisible = true;

        // Останавливаем музыку на экране победы (опционально)
        AudioService.StopBackgroundMusic();
    }
}