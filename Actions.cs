// Действия игрока: шаг, использование предмета, покупка.
// Каждый метод возвращает сообщение, которое увидит игрок.

public static class Actions
{
    // Шаг на соседнюю клетку: dx и dy — на сколько меняются x и y (например, шаг вправо — dx = 1, dy = 0).
    //   — в стену пройти нельзя;
    //   — если на клетке живой монстр, игрок не идёт, а бьёт его;
    //   — в выход без ключа пройти нельзя;
    //   — если на клетке лежит предмет, игрок подбирает его;
    //   — если игрок встал на выход, игра выиграна.
    public static string Move(Game game, int dx, int dy)
    {
        Player player = game.Player;
        int newX = player.X + dx;
        int newY = player.Y + dy;

        if (game.Map.IsWall(newX, newY))
            return "Там стена.";

        Monster monster = game.FindMonsterAt(newX, newY);
        if (monster != null)
            return Combat.PlayerAttacks(player, monster);

        bool isExit = game.Map.IsExit(newX, newY);
        if (isExit && !player.Bag.Has(Item.Key))
            return "Выход заперт. Нужен ключ.";

        player.MoveTo(newX, newY);
        string message = "Вы перешли на клетку (" + newX + ", " + newY + ").";

        FloorItem loot = game.FindLootAt(newX, newY);
        if (loot != null)
            message += " " + PickUp(player, loot);

        if (isExit)
        {
            game.Win();
            message += " Вы открыли дверь ключом и вышли из подземелья!";
        }

        return message;
    }

    // Игрок подбирает предмет с пола. Если в сумке нет места, предмет остаётся лежать.
    static string PickUp(Player player, FloorItem loot)
    {
        if (!player.Bag.Add(loot.Item))
            return "Здесь лежит: " + loot.Item.Name + ", но в сумке нет места.";

        loot.Take();
        return "Вы подобрали: " + loot.Item.Name + ".";
    }

    // Игрок использует предмет из сумки. number — номер предмета, как его видит игрок:
    // первый предмет в сумке — номер 1, второй — номер 2 и так далее.
    //   — зелье лечит и исчезает из сумки;
    //   — оружие игрок берёт в руки; оружие, которое было в руках, возвращается в сумку;
    //   — ключ использовать нельзя: он сам сработает на выходе.
    public static string UseItem(Game game, int number)
    {
        Player player = game.Player;
        int index = number - 1;

        Item item = player.Bag.Get(index);
        if (item == null)
            return "В сумке нет предмета с номером " + number + ".";

        if (item.Kind == Item.Potion)
        {
            player.Bag.RemoveAt(index);
            player.Heal(item.Power);
            return "Вы выпили: " + item.Name + ". Здоровье: " + player.Health + ".";
        }

        if (item.Kind == Item.Weapon)
        {
            Item old = player.Weapon;
            player.Bag.RemoveAt(index);
            player.Equip(item);
            if (old != null)
                player.Bag.Add(old);
            return "Вы взяли в руки: " + item.Name + ". Атака: " + player.Attack + ".";
        }

        return item.Name + ": этот предмет нельзя использовать.";
    }

    // Покупка товара номер number. Покупать можно, только стоя на клетке торговца.
    public static string Buy(Game game, int number)
    {
        Player player = game.Player;
        if (!game.Map.IsShop(player.X, player.Y))
            return "Торговца здесь нет. Он стоит на клетке $.";

        return game.Shop.Buy(player, number);
    }
}
