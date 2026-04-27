using System;
using System.Collections.Generic;

namespace CatchAndEarn.Model;

public class FishingService
{
    private readonly Random random = new();

    private readonly List<Fish> fishes = new()
{
    new Fish("Карась", 20),
    new Fish("Окунь", 15),
    new Fish("Щука", 12),
    new Fish("Тунец", 10),
    new Fish("Рыба-кот", 8),
    new Fish("Рыба-единорог", 7),
    new Fish("Тропическая барракуда", 6),
    new Fish("Сом", 5),
    new Fish("Демоническая рыба", 4),
    new Fish("Дори", 3),
    new Fish("Немо", 2),
    new Fish("Акула", 1),
    new Fish("Проклятая рыба", 0.9),
    new Fish("Мифическая рыба", 1),
    new Fish("Легендарная Емельяновка", 0.1)
};

    public Fish? TryCatchFish()
    {
        var roll = random.Next(0, 100);

        // 5% — не клюёт
        if (roll < 5)
            return null;

        // выбор рыбы из 95%
        var fishRoll = random.NextDouble() * 95;
        double current = 0;

        foreach (var fish in fishes)
        {
            current += fish.Chance;

            if (fishRoll < current)
                return fish;
        }

        return fishes[0];
    }
}