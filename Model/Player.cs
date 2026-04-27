using System.Collections.Generic;

namespace CatchAndEarn.Model;

public class Player
{
    public int Coins { get; private set; }

    private readonly HashSet<string> caughtFish = new();

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
}