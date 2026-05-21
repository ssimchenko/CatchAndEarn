using System;
using System.Collections.Generic;
using System.Linq;

namespace CatchAndEarn.Model;

// сервис ловли: пулы рыб, взвешенный рандом, бонусы
public class FishingService
{
    private readonly Random random = new();

    // рыбы первого озера: шанс (1..25), награда (5..1000)
    private readonly List<Fish> lake1Fishes = new()
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

    // рыбы второго озера: чуть выше награды, свои названия
    private readonly List<Fish> lake2Fishes = new()
    {
        new Fish("Призрачный пескарь", 25, 6),
        new Fish("Лавовый окунь", 18, 10),
        new Fish("Ледяной сом", 13, 14),
        new Fish("Глубинный угорь", 10, 18),
        new Fish("Электрический скат", 8, 25),
        new Fish("Кристаллическая форель", 6, 35),
        new Fish("Кровавая пиранья", 5, 45),
        new Fish("Теневой лосось", 4, 55),
        new Fish("Астральная медуза", 3, 75),
        new Fish("Космический карп", 2, 95),
        new Fish("Звёздная акула", 1.5, 130),
        new Fish("Хранитель бездны", 1, 160),
        new Fish("Эфирный дракон-рыба", 0.8, 220),
        new Fish("Легенда морей", 0.5, 320),
        new Fish("Мифический Юлерн", 0.1, 1100)
    };

    // вернуть список рыб для указанного озера
    public IReadOnlyList<Fish> GetFishesForLake(int lakeId) => lakeId == 1 ? lake1Fishes : lake2Fishes;

    // основной алгоритм ловли: взвешенный рандом с бонусами
    // excludeUltraRare — исключает рыб с шансом 0.1% (для трофейной сетки)
    public Fish? TryCatchFish(int lakeId, bool goldenLureActive = false, double rareFishBonus = 5.0, bool excludeUltraRare = false)
    {
        var fishes = GetFishesForLake(lakeId);
        
        // фильтрация ультраредких для второго броска
        if (excludeUltraRare)
            fishes = fishes.Where(f => f.Chance > 0.1).ToList();
        
        if (fishes.Count == 0) return null;
        
        // 5% шанс что вообще не клюнет
        if (random.NextDouble() < 0.05) return null;

        // расчёт суммарного шанса с учётом бонуса приманки
        double totalChance = 0;
        foreach (var fish in fishes)
        {
            double chance = fish.Chance;
            if (goldenLureActive && fish.Chance <= 5.0)
                chance += rareFishBonus;
            totalChance += chance;
        }

        if (totalChance == 0) return null;

        // взвешенный рандом: выбираем рыбу по накопленной вероятности
        double roll = random.NextDouble() * totalChance;
        double current = 0;

        foreach (var fish in fishes)
        {
            double chance = fish.Chance;
            if (goldenLureActive && fish.Chance <= 5.0)
                chance += rareFishBonus;

            current += chance;
            if (roll <= current) return fish;
        }

        return fishes[0]; // фолбэк на первую рыбу
    }
}