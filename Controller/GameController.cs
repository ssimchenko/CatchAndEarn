using CatchAndEarn.Model;
using System;

namespace CatchAndEarn.Controller;

public class GameController
{
    private readonly Player player;

    public GameController(Player player)
    {
        this.player = player;
    }

    public string RewardFish(Fish fish)
    {
        bool isNew = !player.HasCaught(fish.Name);

        int reward = isNew ? fish.Reward : Math.Max(1, fish.Reward / 10);

        player.CatchFish(fish.Name, reward);

        if (isNew)
            return $"Пойман новый вид: {fish.Name}! +{reward} монет";

        return $"Пойман {fish.Name} повторно (+{reward} монет)";
    }

    public int GetCoins()
    {
        return player.Coins;
    }
}