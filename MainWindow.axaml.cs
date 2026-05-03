using Avalonia;
using Avalonia.Controls;
using CatchAndEarn.Controller;
using CatchAndEarn.Model;
using System;
using System.Threading.Tasks;

namespace CatchAndEarn;

public partial class MainWindow : Window
{
    private GameController? gameController;

    private SkillCheck? skillCheck;
    private GameState state = GameState.Idle;

    private FishingService fishingService = new FishingService();
    private Fish? currentFish;

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

    private void StartGame_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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
    }

    private void BackToMenu_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        GamePanel.IsVisible = false;
        MenuPanel.IsVisible = true;
    }

    private void Exit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private async void CatchButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (state == GameState.Idle)
        {
            state = GameState.Waiting;
            ResultText.Text = "Ожидание поклёвки...";

            await Task.Delay(new Random().Next(1000, 2000));

            // выбираем рыбу
            var fish = fishingService.TryCatchFish();

            if (fish == null)
            {
                ResultText.Text = "Не клюёт";
                state = GameState.Idle;
                return;
            }

            currentFish = fish;

            skillCheck = new SkillCheck(fish.Difficulty);

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
                SuccessZone.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.LimeGreen);

                if (currentFish != null)
                {
                    ResultText.Text = gameController!.RewardFish(currentFish);
                }

                UpdateCoins();
            }
            else
            {
                SuccessZone.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Red);
                ResultText.Text = "Рыба сорвалась";
            }

            await Task.Delay(600);

            SkillCheckPanel.IsVisible = false;

            SuccessZone.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Green);

            await Task.Delay(200);

            state = GameState.Idle;
            ResultText.Text = "Нажми Ловить";
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

        CoinsText.Text = $"Монеты: {gameController.GetCoins()}";
    }
}