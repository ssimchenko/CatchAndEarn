using System;

namespace CatchAndEarn.Model;

// сущность рыбы: название, шанс, награда, сложность
public class Fish
{
    public string Name { get; }
    public double Chance { get; }
    public int Reward { get; }
    public double Difficulty { get; }

    // расчёт сложности: чем меньше шанс, тем выше сложность (0..1)
    public Fish(string name, double chance, int reward)
    {
        Name = name;
        Chance = chance;
        Reward = reward;

        // формула: 1 - (chance / 25), ограничиваем диапазоном [0, 1]
        Difficulty = Math.Clamp(1.0 - (chance / 25.0), 0, 1);
    }
}