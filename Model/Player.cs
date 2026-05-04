using System.Collections.Generic;

namespace CatchAndEarn.Model;

public class Player
{
    public int Coins { get; private set; } = 0;
    public HashSet<string> CaughtFish { get; private set; } = new();

    public List<Upgrade> PurchasedUpgrades { get; private set; } = new();

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

    public void PurchaseUpgrade(Upgrade upgrade)
    {
        if (!PurchasedUpgrades.Contains(upgrade))
        {
            PurchasedUpgrades.Add(upgrade);
            upgrade.Purchased = true;
        }
    }

    public bool HasUpgrade(string upgradeName)
    {
        return PurchasedUpgrades.Exists(u => u.Name == upgradeName);
    }
}