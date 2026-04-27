using System;
using System.Collections.Generic;
using System.Linq;

namespace CatchAndEarn.Model;

public class FishingService
{
    private readonly Random random = new();

    private readonly List<Fish> fishes = new()
    {
        new Fish("Карась", 25, 5),
        new Fish("Окунь", 18, 8),
        new Fish("Щука", 13, 12),
        new Fish("Тунец", 10, 15),
        new Fish("Рыба-кот", 8, 20),
        new Fish("Рыба-единорог", 6, 30),
        new Fish("Тропическая барракуда", 5, 40),
        new Fish("Сом", 4, 50),
        new Fish("Демоническая рыба", 3, 70),
        new Fish("Дори", 2, 90),
        new Fish("Немо", 1.5, 120),
        new Fish("Акула", 1, 150),
        new Fish("Проклятая рыба", 0.8, 200),
        new Fish("Мифическая рыба", 0.5, 300),
        new Fish("Легендарная Емельяновка", 0.1, 1000)
    };

    public Fish? TryCatchFish()
    {
        if (random.NextDouble() < 0.05)
            return null;

        double totalChance = fishes.Sum(f => f.Chance);

        double roll = random.NextDouble() * totalChance;
        double current = 0;

        foreach (var fish in fishes)
        {
            current += fish.Chance;

            if (roll <= current)
                return fish;
        }

        return fishes[0];
    }
}