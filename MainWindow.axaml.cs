using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
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
            var fishingService = new FishingService();
            gameController = new GameController(player, fishingService);
        }

        MenuPanel.IsVisible = false;
        GamePanel.IsVisible = true;

        ResultText.Text = "Нажми Ловить";
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

            skillCheck = new SkillCheck();
            skillCheck.OnUpdate += UpdateSkillCheckUI;

            skillCheck.Start();

            SkillCheckPanel.IsVisible = true;

            state = GameState.SkillCheck;
            ResultText.Text = "ЛОВИ!";
        }
        else if (state == GameState.SkillCheck && skillCheck != null)
        {
            bool success = skillCheck.TryHit();

            state = GameState.Result;

            // меняем цвет зоны
            if (success)
            {
                SuccessZone.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.LimeGreen);
            }
            else
            {
                SuccessZone.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Red);
            }

            ResultText.Text = success ? "Поймал рыбу!" : "Рыба сорвалась";

            // даём время увидеть изменение
            await Task.Delay(600);

            // скрываем шкалу
            SkillCheckPanel.IsVisible = false;

            // возвращаем исходный цвет
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

        // движение маркера
        Marker.Margin = new Thickness(skillCheck.Position * width, 0, 0, 0);

        // зона успеха
        double zoneWidth = (skillCheck.ZoneEnd - skillCheck.ZoneStart) * width;
        double zoneStart = skillCheck.ZoneStart * width;

        SuccessZone.Width = zoneWidth;
        SuccessZone.Margin = new Thickness(zoneStart, 0, 0, 0);
    }
}