namespace CatchAndEarn.Model;

public class Fish
{
    public string Name { get; }
    public double Chance { get; }
    public int Reward { get; }

    public Fish(string name, double chance, int reward)
    {
        Name = name;
        Chance = chance;
        Reward = reward;
    }
}