using Avalonia;
using Avalonia.Controls;
<<<<<<< HEAD
using Avalonia.Media;
using Avalonia.Interactivity;
=======
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
>>>>>>> 784d05e
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
<<<<<<< HEAD
    private bool isTestMode = false;
=======

    private bool isMusicMuted = false;
    private bool isSfxMuted = false;
    private bool isAllMuted = false;
>>>>>>> 784d05e

    public MainWindow()
    {
        InitializeComponent();
<<<<<<< HEAD
    }

    enum GameState
=======
        this.Closing += MainWindow_Closing;
        UpdateSoundIcons();
    }

    private void MainWindow_Closing(object? sender, Avalonia.Controls.WindowClosingEventArgs e)
    {
        AudioService.StopAll();
    }

    private enum GameState
>>>>>>> 784d05e
    {
        Idle,
        Waiting,
        SkillCheck,
        Result
    }

    private void StartGame_Click(object? sender, RoutedEventArgs e)
    {
<<<<<<< HEAD
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
=======
        _ = AudioService.PlayAsync("click.wav");
        StartGame();
>>>>>>> 784d05e
    }

    private void BackToMenu_Click(object? sender, RoutedEventArgs e)
    {
<<<<<<< HEAD
=======
        _ = AudioService.PlayAsync("click.wav");
        
>>>>>>> 784d05e
        GamePanel.IsVisible = false;
        CollectionPanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        WinPanel.IsVisible = false;
        MenuPanel.IsVisible = true;
<<<<<<< HEAD
        isTestMode = false;
=======

        SkillCheckPanel.IsVisible = false;

        Lake1Background.IsVisible = true;
        Lake2Background.IsVisible = false;
        Lake1Sign.IsVisible = true;
        Lake2Sign.IsVisible = false;

        ResultText.Text = "";
        state = GameState.Idle;

        AudioService.StopBackgroundMusic();
>>>>>>> 784d05e
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
<<<<<<< HEAD
        Close();
    }

    private void ToggleCollection_Click(object? sender, RoutedEventArgs e)
    {
        CollectionPanel.IsVisible = !CollectionPanel.IsVisible;
        UpdateCollection();
=======
        _ = AudioService.PlayAsync("click.wav");
        Close();
    }

    private async void StartGame()
    {
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
        ResultText.Text = "";
        state = GameState.Idle;

        UpdateCoins();
        UpdateCollection();
        UpdateShopUI();

        await AudioService.PlayBackgroundMusicAsync("background_music_calm.wav");
    }

    private void ToggleCollection_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        if (gameController == null) return;

        UpdateCollection();
        bool shouldOpen = !CollectionPanel.IsVisible;
        ShopPanel.IsVisible = false;
        CollectionPanel.IsVisible = shouldOpen;
    }

    private void ToggleShop_Click(object? sender, RoutedEventArgs e)
    {
        _ = AudioService.PlayAsync("click.wav");
        if (gameController == null) return;

        UpdateShopUI();
        bool shouldOpen = !ShopPanel.IsVisible;
        CollectionPanel.IsVisible = false;
        ShopPanel.IsVisible = shouldOpen;
>>>>>>> 784d05e
    }

    private void UpdateCollection()
    {
        if (gameController == null) return;

        int lake = gameController.GetPlayer().CurrentLake;
        var fishes = fishingService.GetFishesForLake(lake);
<<<<<<< HEAD

        var text = new StringBuilder();
        text.AppendLine($"--- Озеро {lake} ---");
        foreach (var fish in fishes)
        {
            string status = gameController.HasCaughtFish(fish.Name) ? "✓" : "";
=======
        string lakeName = GetLakeName(lake);

        var text = new StringBuilder();
        text.AppendLine($"--- {lakeName} ---");

        foreach (var fish in fishes)
        {
            string status = gameController.HasCaughtFish(fish.Name) ? "✓" : "✕";
>>>>>>> 784d05e
            text.AppendLine($"{status} {fish.Name} — {fish.Chance:0.#}%");
        }

        CollectionText.Text = text.ToString();
    }

<<<<<<< HEAD
    private void ToggleShop_Click(object? sender, RoutedEventArgs e)
    {
        ShopPanel.IsVisible = !ShopPanel.IsVisible;
        UpdateShopUI();
    }
=======
    private string GetLakeName(int lake) => lake == 1 ? "Глубокий пруд" : "Море C#";
>>>>>>> 784d05e

    private void UpdateShopUI()
    {
        ShopList.Children.Clear();
<<<<<<< HEAD
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
=======
        if (gameController == null) return;

        int currentLake = gameController.GetPlayer().CurrentLake;
        var upgrades = GetUpgradeDefinitions(currentLake)
            .OrderBy(u => u.Cost)
            .ToList();

        foreach (var upgrade in upgrades)
            AddShopItem(upgrade.Name, upgrade.Desc, upgrade.Cost, currentLake);
>>>>>>> 784d05e
    }

    private List<(string Name, string Desc, int Cost)> GetUpgradeDefinitions(int lake)
    {
<<<<<<< HEAD
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
=======
        if (lake == 1)
        {
            return new List<(string, string, int)>
            {
                ("Ключ от Моря C#", "Открывает Море C#. Улучшения сбрасываются.", 500),
>>>>>>> 784d05e
                ("Широкая зона", "Увеличивает зону успеха в skillcheck в 2 раза", 50),
                ("Золотая приманка", "Повышает шанс поймать редкую рыбу на 3%", 50),
                ("Бонус монет", "Получай на 15% больше монет", 25),
                ("Скоростная реакция", "Замедляет движение маркера на 30%", 100),
                ("Трофейная сетка", "Позволяет ловить сразу две рыбы с одного броска", 200)
<<<<<<< HEAD
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
=======
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
        if (gameController == null) return;

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

        var rootPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        var textPanel = new StackPanel { Spacing = 4, Width = 280 };

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
        if (gameController == null) return;

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
>>>>>>> 784d05e
    }

    private async void CatchButton_Click(object? sender, RoutedEventArgs e)
    {
        if (gameController == null) return;

        if (state == GameState.Idle)
        {
<<<<<<< HEAD
            state = GameState.Waiting;
            ResultText.Text = "Ожидание поклёвки...";
=======
            _ = AudioService.PlayAsync("click.wav");
            _ = AudioService.PlayAsync("cast.wav");
            
            state = GameState.Waiting;
            ResultText.Text = "Ожидание поклёвки...";

>>>>>>> 784d05e
            await Task.Delay(random.Next(1000, 2000));

            int currentLake = gameController.GetPlayer().CurrentLake;
            bool goldenLureActive = gameController.GetPlayer().HasUpgrade("Золотая приманка");
            double lureBonus = currentLake == 1 ? 3.0 : 6.0;

            var fish = fishingService.TryCatchFish(currentLake, goldenLureActive, lureBonus, excludeUltraRare: false);

            if (fish == null)
            {
<<<<<<< HEAD
=======
                _ = AudioService.PlayAsync("fail.wav");
>>>>>>> 784d05e
                ResultText.Text = "Не клюёт";
                state = GameState.Idle;
                return;
            }

<<<<<<< HEAD
            currentFish = fish;

            double zoneBonus = gameController.GetPlayer().HasUpgrade("Широкая зона") ? (currentLake == 1 ? 2.0 : 2.5) : 1.0;
            double markerSpeedBonus = gameController.GetPlayer().HasUpgrade("Скоростная реакция") ? (currentLake == 1 ? 0.7 : 0.5) : 1.0;
=======
            _ = AudioService.PlayAsync("bite.wav");
            currentFish = fish;

            double zoneBonus = gameController.GetPlayer().HasUpgrade("Широкая зона")
                ? currentLake == 1 ? 2.0 : 2.5 : 1.0;
            double markerSpeedBonus = gameController.GetPlayer().HasUpgrade("Скоростная реакция")
                ? currentLake == 1 ? 0.7 : 0.5 : 1.0;
>>>>>>> 784d05e

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
<<<<<<< HEAD
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
=======
                _ = AudioService.PlayAsync("success.wav");
                _ = AudioService.PlayAsync("coin.wav");
                
                SuccessZone.Background = new SolidColorBrush(Colors.LimeGreen);

                string resultMessage = currentFish != null ? gameController.RewardFish(currentFish) : "";

                if (gameController.GetPlayer().HasUpgrade("Трофейная сетка"))
                {
                    int currentLake = gameController.GetPlayer().CurrentLake;
                    bool goldenLureActive = gameController.GetPlayer().HasUpgrade("Золотая приманка");
                    double lureBonus = currentLake == 1 ? 3.0 : 6.0;

                    var secondFish = fishingService.TryCatchFish(currentLake, goldenLureActive, lureBonus, excludeUltraRare: true);
>>>>>>> 784d05e
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
<<<<<<< HEAD
=======
                _ = AudioService.PlayAsync("fail.wav");
>>>>>>> 784d05e
                SuccessZone.Background = new SolidColorBrush(Colors.Red);
                ResultText.Text = "Рыба сорвалась";
            }

            await Task.Delay(600);
            SkillCheckPanel.IsVisible = false;
            SuccessZone.Background = new SolidColorBrush(Colors.Green);
            await Task.Delay(200);
<<<<<<< HEAD
            state = GameState.Idle;
            ResultText.Text = "Нажми Ловить";
=======

            state = GameState.Idle;
            if (!WinPanel.IsVisible) ResultText.Text = "";
>>>>>>> 784d05e
        }
    }

    private void UpdateSkillCheckUI()
    {
        if (skillCheck == null) return;
<<<<<<< HEAD
        double width = SkillCheckPanel.Bounds.Width;
        Marker.Margin = new Thickness(skillCheck.Position * width, 0, 0, 0);
        double zoneWidth = (skillCheck.ZoneEnd - skillCheck.ZoneStart) * width;
        double zoneStart = skillCheck.ZoneStart * width;
=======

        double width = SkillCheckPanel.Bounds.Width;
        Marker.Margin = new Thickness(skillCheck.Position * width, 0, 0, 0);

        double zoneWidth = (skillCheck.ZoneEnd - skillCheck.ZoneStart) * width;
        double zoneStart = skillCheck.ZoneStart * width;

>>>>>>> 784d05e
        SuccessZone.Width = zoneWidth;
        SuccessZone.Margin = new Thickness(zoneStart, 0, 0, 0);
    }

    private void UpdateCoins()
    {
        if (gameController == null) return;
<<<<<<< HEAD
        CoinsText.Text = $"Монеты: {gameController.GetCoins()}";
=======
        CoinsText.Text = gameController.GetCoins().ToString();
>>>>>>> 784d05e
    }

    private void ShowWinScreen()
    {
<<<<<<< HEAD
        GamePanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        CollectionPanel.IsVisible = false;
        WinCoinsText.Text = $"Итоговые монеты: {gameController.GetCoins()}";
        WinPanel.IsVisible = true;
=======
        if (gameController == null) return;

        GamePanel.IsVisible = false;
        ShopPanel.IsVisible = false;
        CollectionPanel.IsVisible = false;

        WinCoinsText.Text = $"Итоговые монеты: {gameController.GetCoins()}";
        WinPanel.IsVisible = true;

        AudioService.StopBackgroundMusic();
    }

    private void AllMuteButton_Click(object? sender, RoutedEventArgs e)
    {
        bool newState = !isAllMuted;
        isMusicMuted = newState;
        isSfxMuted = newState;
        isAllMuted = newState;
        
        AudioService.IsMusicEnabled = !isMusicMuted;
        AudioService.IsSfxEnabled = !isSfxMuted;
        
        if (isAllMuted) AudioService.StopBackgroundMusic();
        else if (gameController != null && GamePanel.IsVisible && !WinPanel.IsVisible)
            _ = AudioService.PlayBackgroundMusicAsync("background_music_calm.wav");
            
        UpdateSoundIcons();
    }

    private void MusicMuteButton_Click(object? sender, RoutedEventArgs e)
    {
        isMusicMuted = !isMusicMuted;
        AudioService.IsMusicEnabled = !isMusicMuted;
        
        if (isMusicMuted) AudioService.StopBackgroundMusic();
        else if (gameController != null && GamePanel.IsVisible && !WinPanel.IsVisible)
            _ = AudioService.PlayBackgroundMusicAsync("background_music_calm.wav");
            
        UpdateSoundIcons();
    }

    private void SfxMuteButton_Click(object? sender, RoutedEventArgs e)
    {
        isSfxMuted = !isSfxMuted;
        AudioService.IsSfxEnabled = !isSfxMuted;
        UpdateSoundIcons();
    }

    private void UpdateSoundIcons()
    {
        isAllMuted = isMusicMuted && isSfxMuted;
        
        AllMuteButton.Opacity = isAllMuted ? 1.0 : 0.5;
        SfxMuteButton.Opacity = isSfxMuted ? 1.0 : 0.5;
        MusicMuteButton.Opacity = isMusicMuted ? 1.0 : 0.5;
>>>>>>> 784d05e
    }
}