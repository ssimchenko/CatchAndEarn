using System;
using Avalonia.Threading;

namespace CatchAndEarn.Model;

// мини-игра на реакцию: маркер, зона, таймер, событие для ui
public class SkillCheck
{
    private readonly DispatcherTimer timer;
    private readonly double markerSpeed;
    private readonly bool isZoneMoving;
    private readonly double zoneMoveSpeed;

    public double Position { get; private set; } = 0; // позиция маркера [0..1]
    private double markerDirection = 1; // направление движения

    public double ZoneStart { get; private set; } // границы зоны успеха
    public double ZoneEnd { get; private set; }
    private double zoneDirection = 1; // направление движения зоны

    public bool IsActive { get; private set; }
    public event Action? OnUpdate; // оповещение view об изменении координат

    // расчёт параметров на основе сложности и бонусов
    public SkillCheck(double difficulty, double zoneBonus = 1.0, double markerSpeedBonus = 1.0)
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 fps
        timer.Tick += Update;

        double minZone = 0.035;
        double maxZone = 0.18;

        maxZone *= zoneBonus;
        maxZone = Math.Clamp(maxZone, minZone, 0.6);

        // чем выше сложность, тем уже зона
        double zoneSize = maxZone - difficulty * (maxZone - minZone);
        zoneSize = Math.Clamp(zoneSize, minZone, maxZone);

        ZoneStart = 0.5 - zoneSize / 2;
        ZoneEnd = 0.5 + zoneSize / 2;

        // скорость маркера растёт с сложностью
        markerSpeed = (0.008 + difficulty * 0.018) * markerSpeedBonus;

        // зона двигается только при высокой сложности
        isZoneMoving = difficulty >= 0.88;
        zoneMoveSpeed = 0.002 + difficulty * 0.004;
    }

    // сброс позиций и запуск таймера
    public void Start()
    {
        Position = 0;
        markerDirection = 1;
        zoneDirection = 1;
        IsActive = true;
        timer.Start();
    }

    // остановка таймера
    public void Stop()
    {
        timer.Stop();
        IsActive = false;
    }

    // тик таймера: обновление позиций и уведомление ui
    private void Update(object? sender, EventArgs e)
    {
        UpdateMarker();
        if (isZoneMoving) UpdateZone();
        OnUpdate?.Invoke();
    }

    // движение маркера в пределах [0, 1] с отражением от границ
    private void UpdateMarker()
    {
        Position += markerSpeed * markerDirection;
        if (Position >= 1) { Position = 1; markerDirection = -1; }
        if (Position <= 0) { Position = 0; markerDirection = 1; }
    }

    // плавное смещение зоны успеха в пределах [0, 1]
    private void UpdateZone()
    {
        double zoneSize = ZoneEnd - ZoneStart;
        ZoneStart += zoneMoveSpeed * zoneDirection;
        ZoneEnd += zoneMoveSpeed * zoneDirection;

        if (ZoneEnd >= 1) { ZoneEnd = 1; ZoneStart = ZoneEnd - zoneSize; zoneDirection = -1; }
        if (ZoneStart <= 0) { ZoneStart = 0; ZoneEnd = ZoneStart + zoneSize; zoneDirection = 1; }
    }

    // проверка попадания и завершение мини-игры
    public bool TryHit()
    {
        if (!IsActive) return false;
        Stop();
        return Position >= ZoneStart && Position <= ZoneEnd;
    }
}