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
        if (caughtFish.Contains(fishName))
            return;

        caughtFish.Add(fishName);
        Coins += reward;
    }
}