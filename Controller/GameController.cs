using CatchAndEarn.Model;

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

        if (player.HasCaught(fish.Name))
            return $"Пойман {fish.Name}, но ты уже ловил его раньше";

        player.CatchFish(fish.Name, 10);
        return $"Пойман новый вид: {fish.Name}! +10 монет";
    }

    public int GetCoins()
    {
        return player.Coins;
    }
}