using System;
using Avalonia.Threading;

namespace CatchAndEarn.Model;

public class SkillCheck
{
    private readonly DispatcherTimer timer;

    public double Position { get; private set; } = 0;
    private double direction = 1;

    public double ZoneStart { get; private set; }
    public double ZoneEnd { get; private set; }

    public bool IsActive { get; private set; }

    public event Action? OnUpdate;

    private readonly double speed;

    public SkillCheck(double difficulty)
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += Update;

        // зона
        double minZone = 0.05;
        double maxZone = 0.3;

        double zoneSize = maxZone - (difficulty * (maxZone - minZone));
        zoneSize = Math.Clamp(zoneSize, 0.05, 0.3);

        double center = 0.5;

        ZoneStart = center - zoneSize / 2;
        ZoneEnd = center + zoneSize / 2;

        // скорость
        speed = 0.006 + (difficulty * 0.02);
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
        Position += speed * direction;

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