using System;
using Avalonia.Threading;

namespace CatchAndEarn.Model;

public class SkillCheck
{
    private readonly DispatcherTimer timer;

    public double Position { get; private set; } = 0; // 0..1
    private double direction = 1; // 1 вправо, -1 влево

    public double ZoneStart { get; } = 0.4;
    public double ZoneEnd { get; } = 0.6;

    public bool IsActive { get; private set; }

    public event Action? OnUpdate;

    public SkillCheck()
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
        timer.Tick += Update;
    }

    public void Start()
    {
        Position = 0;
        direction = 1;
        IsActive = true;
        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();
        IsActive = false;
    }

    private void Update(object? sender, EventArgs e)
    {
        Position += 0.01 * direction;

        if (Position >= 1)
        {
            Position = 1;
            direction = -1;
        }

        if (Position <= 0)
        {
            Position = 0;
            direction = 1;
        }

        OnUpdate?.Invoke();
    }

    public bool TryHit()
    {
        if (!IsActive)
            return false;

        Stop();

        return Position >= ZoneStart && Position <= ZoneEnd;
    }
}