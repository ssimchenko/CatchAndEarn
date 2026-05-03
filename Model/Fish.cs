using System;

namespace CatchAndEarn.Model;

public class Fish
{
    public string Name { get; }
    public double Chance { get; }
    public int Reward { get; }
    public double Difficulty { get; }

    public Fish(string name, double chance, int reward)
    {
        Name = name;
        Chance = chance;
        Reward = reward;

        Difficulty = 1.0 - (chance / 25.0);
        Difficulty = Math.Clamp(Difficulty, 0, 1);
    }
}