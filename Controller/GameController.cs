using System;
using CatchAndEarn.Model;
using System.Collections.Generic;

namespace CatchAndEarn.Controller;

public class GameController
{
    private readonly Player player;

    public GameController(Player player)
    {
        this.player = player;
    }

    public int GetCoins() => player.Coins;

    public bool HasCaughtFish(string fishName) => player.CaughtFish.Contains(fishName);

    public string RewardFish(Fish fish)
    {
        int reward = fish.Reward;
        if (HasCaughtFish(fish.Name))
            reward = Math.Max(1, reward / 10);

        if (player.HasUpgrade("Бонус монет"))
            reward = (int)Math.Round(reward * 1.1);

        player.AddCoins(reward);
        player.CaughtFish.Add(fish.Name);
        return $"Поймана {fish.Name}! +{reward} монет";
    }

    public bool BuyUpgrade(Upgrade upgrade)
    {
        if (upgrade.Purchased) return false;

        if (player.SpendCoins(upgrade.Cost))
        {
            player.PurchaseUpgrade(upgrade);
            return true;
        }

        return false;
    }

    public Player GetPlayer() => player;
}