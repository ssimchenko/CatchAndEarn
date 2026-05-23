using System;

namespace CatchAndEarn.Model;

<<<<<<< HEAD
=======
// сущность рыбы: название, шанс, награда, сложность
>>>>>>> 784d05e
public class Fish
{
    public string Name { get; }
    public double Chance { get; }
    public int Reward { get; }
    public double Difficulty { get; }

<<<<<<< HEAD
=======
    // расчёт сложности: чем меньше шанс, тем выше сложность (0..1)
>>>>>>> 784d05e
    public Fish(string name, double chance, int reward)
    {
        Name = name;
        Chance = chance;
        Reward = reward;

<<<<<<< HEAD
        Difficulty = 1.0 - (chance / 25.0);
        Difficulty = Math.Clamp(Difficulty, 0, 1);
=======
        // формула: 1 - (chance / 25), ограничиваем диапазоном [0, 1]
        Difficulty = Math.Clamp(1.0 - (chance / 25.0), 0, 1);
>>>>>>> 784d05e
    }
}