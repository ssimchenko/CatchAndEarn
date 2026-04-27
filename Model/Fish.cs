namespace CatchAndEarn.Model;

public class Fish
{
    public string Name { get; }
    public double Chance { get; }

    public Fish(string name, double chance)
    {
        Name = name;
        Chance = chance;
    }
}