using CatchAndEarn.Model;
using System;

namespace CatchAndEarn.Controller;

public class GameController
{
    private readonly Player player;
    private readonly FishingService fishingService;

    public GameController(Player player, FishingService fishingService)
    {
        this.player = player;
        this.fishingService = fishingService;
    }

    public string Catch()
    {
        var fish = fishingService.TryCatchFish();

        if (fish == null)
            return "Не клюёт";

        bool isNew = !player.HasCaught(fish.Name);

        int reward = isNew ? fish.Reward : Math.Max(1, fish.Reward / 10);

        player.CatchFish(fish.Name, reward);

        if (isNew)
            return $"Пойман новый вид: {fish.Name}! +{reward} монет";
        else
            return $"Пойман {fish.Name} повторно (+{reward} монет)";
    }

    public int GetCoins()
    {
        return player.Coins;
    }
}