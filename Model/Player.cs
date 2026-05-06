using System.Collections.Generic;

namespace CatchAndEarn.Model;

public class Player
{
    public int Coins { get; private set; } = 0;
    public HashSet<string> CaughtFish { get; private set; } = new();
    public HashSet<string> PurchasedUpgrades { get; private set; } = new();
    public int CurrentLake { get; set; } = 1;

    public void AddCoins(int amount) => Coins += amount;

    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            return true;
        }
        return false;
    }

    public void PurchaseUpgrade(string upgradeName)
    {
        PurchasedUpgrades.Add(upgradeName);
    }

    public void ClearUpgrades()
    {
        PurchasedUpgrades.Clear();
    }

    public bool HasUpgrade(string upgradeName)
    {
        return PurchasedUpgrades.Contains(upgradeName);
    }
}