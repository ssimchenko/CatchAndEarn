using System;

namespace CatchAndEarn.Model;

public class Upgrade
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Cost { get; set; }
    public bool Purchased { get; set; } = false;

    public Upgrade(string name, string description, int cost)
    {
        Name = name;
        Description = description;
        Cost = cost;
    }
}