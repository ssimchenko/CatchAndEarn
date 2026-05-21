using CatchAndEarn.Model;
using System;

namespace CatchAndEarn.Controller;

// контроллер логики: награды, магазин, бонусы
public class GameController
{
    private readonly Player player;

    public GameController(Player player)
    {
        this.player = player;
    }

    // доступ к данным игрока для view
    public Player GetPlayer()
    {
        return player;
    }

    public bool HasCaughtFish(string fishName)
    {
        return player.HasCaught(fishName);
    }

    public int GetCoins()
    {
        return player.Coins;
    }

    public void AddCoins(int amount)
    {
        player.AddCoins(amount);
    }

    public bool SpendCoins(int amount)
    {
        return player.SpendCoins(amount);
    }

    // расчет награды: первая поимка = полная, повтор = 10%
    public string RewardFish(Fish fish)
    {
        bool isNew = !player.HasCaught(fish.Name);
        int reward = isNew ? fish.Reward : Math.Max(1, fish.Reward / 10);

        // применяем множитель за улучшение
        reward = ApplyCoinBonus(reward);

        player.CatchFish(fish.Name, reward);

        return isNew
            ? $"Пойман новый вид: {fish.Name}! +{reward} монет"
            : $"Пойман {fish.Name} повторно (+{reward} монет)";
    }

    // покупка улучшения: проверка наличия и баланса
    public bool BuyUpgrade(string upgradeName, int cost)
    {
        if (player.HasUpgrade(upgradeName)) return false;
        if (!player.SpendCoins(cost)) return false;

        player.PurchaseUpgrade(upgradeName);
        return true;
    }

    // множитель монет зависит от озера (1.15x или 1.25x)
    private int ApplyCoinBonus(int reward)
    {
        if (!player.HasUpgrade("Бонус монет")) return reward;

        double multiplier = player.CurrentLake == 1 ? 1.15 : 1.25;
        return Math.Max(1, (int)Math.Ceiling(reward * multiplier));
    }
}