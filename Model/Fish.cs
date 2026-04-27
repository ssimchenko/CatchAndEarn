namespace CatchAndEarn.Model;

public class Fish
{
    public string Name { get; }
    public double Chance { get; }
    public int Reward { get; }

    // новая переменная
    public double Difficulty { get; }

    public Fish(string name, double chance, int reward)
    {
        Name = name;
        Chance = chance;
        Reward = reward;

        // чем меньше шанс — тем сложнее
        Difficulty = 1.0 - (chance / 25.0);
    }
}