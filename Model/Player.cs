using System.Collections.Generic;

namespace CatchAndEarn.Model;

<<<<<<< HEAD
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
=======
// состояние игрока: монеты, озеро, коллекция, улучшения
public class Player
{
    public int Coins { get; private set; }
    public int CurrentLake { get; set; } = 1; // начинаем с первого озера

    private readonly HashSet<string> caughtFish = new(); // пойманные виды
    private readonly HashSet<string> upgrades = new(); // купленные бусты

    // проверка: ловил ли игрок эту рыбу
    public bool HasCaught(string fishName) => caughtFish.Contains(fishName);

    // добавить рыбу в коллекцию и начислить награду
    public void CatchFish(string fishName, int reward)
    {
        caughtFish.Add(fishName);
        AddCoins(reward);
    }

    // пополнить баланс (защита от отрицательных значений)
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
    }

    // списать монеты: проверка баланса и валидация суммы
    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || Coins < amount) return false;
        Coins -= amount;
        return true;
    }

    // проверка наличия улучшения
    public bool HasUpgrade(string upgradeName) => upgrades.Contains(upgradeName);

    // добавить улучшение в список (игнорируем пустые строки)
    public void PurchaseUpgrade(string upgradeName)
    {
        if (string.IsNullOrWhiteSpace(upgradeName)) return;
        upgrades.Add(upgradeName);
    }

    // полный сброс улучшений (при переходе на озеро 2)
    public void ClearUpgrades() => upgrades.Clear();
>>>>>>> 784d05e
}