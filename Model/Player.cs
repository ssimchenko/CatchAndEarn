using System.Collections.Generic;

namespace CatchAndEarn.Model;

public class Player
{
    public int Coins { get; private set; }
    public int CurrentLake { get; set; } = 1;

    private readonly HashSet<string> caughtFish = new();
    private readonly HashSet<string> upgrades = new();

    public bool HasCaught(string fishName)
    {
        return caughtFish.Contains(fishName);
    }

    public void CatchFish(string fishName, int reward)
    {
        caughtFish.Add(fishName);
        AddCoins(reward);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Coins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0)
            return false;

        if (Coins < amount)
            return false;

        Coins -= amount;
        return true;
    }

    public bool HasUpgrade(string upgradeName)
    {
        return upgrades.Contains(upgradeName);
    }

    public void PurchaseUpgrade(string upgradeName)
    {
        if (string.IsNullOrWhiteSpace(upgradeName))
            return;

        upgrades.Add(upgradeName);
    }

    public void ClearUpgrades()
    {
        upgrades.Clear();
    }
}