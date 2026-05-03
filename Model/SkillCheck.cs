using System;
using Avalonia.Threading;

namespace CatchAndEarn.Model;

public class SkillCheck
{
    private readonly DispatcherTimer timer;
    private readonly double markerSpeed;
    private readonly bool isZoneMoving;
    private readonly double zoneMoveSpeed;

    public double Position { get; private set; } = 0;
    private double markerDirection = 1;

    public double ZoneStart { get; private set; }
    public double ZoneEnd { get; private set; }
    private double zoneDirection = 1;

    public bool IsActive { get; private set; }

    public event Action? OnUpdate;

    public SkillCheck(double difficulty)
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += Update;

        double minZone = 0.035;
        double maxZone = 0.18;

        double zoneSize = maxZone - difficulty * (maxZone - minZone);
        zoneSize = Math.Clamp(zoneSize, minZone, maxZone);

        double center = 0.5;

        ZoneStart = center - zoneSize / 2;
        ZoneEnd = center + zoneSize / 2;

        markerSpeed = 0.012 + difficulty * 0.028;

        // Движение зоны включается для сложных рыб:
        // Демоническая рыба и все, кто реже неё.
        isZoneMoving = difficulty >= 0.88;

        // Чем сложнее рыба, тем быстрее двигается зона.
        zoneMoveSpeed = 0.002 + difficulty * 0.004;
    }

    public void Start()
    {
        Position = 0;
        markerDirection = 1;
        zoneDirection = 1;
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
        UpdateMarker();

        if (isZoneMoving)
            UpdateZone();

        OnUpdate?.Invoke();
    }

    private void UpdateMarker()
    {
        Position += markerSpeed * markerDirection;

        if (Position >= 1)
        {
            Position = 1;
            markerDirection = -1;
        }

        if (Position <= 0)
        {
            Position = 0;
            markerDirection = 1;
        }
    }

    private void UpdateZone()
    {
        double zoneSize = ZoneEnd - ZoneStart;

        ZoneStart += zoneMoveSpeed * zoneDirection;
        ZoneEnd += zoneMoveSpeed * zoneDirection;

        if (ZoneEnd >= 1)
        {
            ZoneEnd = 1;
            ZoneStart = ZoneEnd - zoneSize;
            zoneDirection = -1;
        }

        if (ZoneStart <= 0)
        {
            ZoneStart = 0;
            ZoneEnd = ZoneStart + zoneSize;
            zoneDirection = 1;
        }
    }

    public bool TryHit()
    {
        if (!IsActive)
            return false;

        Stop();

        return Position >= ZoneStart && Position <= ZoneEnd;
    }
}