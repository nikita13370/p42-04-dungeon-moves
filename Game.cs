// Игра целиком: карта, игрок, монстры, предметы на полу, торговец.
// Главный метод — Execute: выполняет одну команду игрока.

public class Game
{
    public GameMap Map { get; }
    public Player Player { get; }
    public Monster[] Monsters { get; }
    public FloorItem[] Loot { get; }            // предметы на полу
    public Shop Shop { get; }
    public int Turn { get; private set; }       // сколько ходов сделано
    public bool IsWon { get; private set; }     // игрок дошёл до выхода

    // Игра закончена: победа или игрок погиб.
    public bool IsOver => IsWon || !Player.IsAlive;

    // Новая игра: всё расставляется так, как записано в Levels.
    public Game()
    {
        Map = new GameMap(Levels.MapRows);
        Player = new Player(Levels.StartX, Levels.StartY);
        Monsters = Levels.CreateMonsters();
        Loot = Levels.CreateLoot();
        Shop = new Shop(Levels.CreateGoods());
    }

    // Выполняет одну команду игрока и возвращает сообщение для него.
    // Команды w, a, s, d, use и buy тратят ход: после них ходят монстры.
    // Команды i, shop, help и неизвестные команды ход не тратят.
    public string Execute(string line)
    {
        if (IsOver)
            return "Игра окончена.";

        string word = Commands.Word(line);
        int number = Commands.Number(line);
        string message;

        switch (word)
        {
            case "w": message = Actions.Move(this, 0, -1); break;
            case "s": message = Actions.Move(this, 0, 1); break;
            case "a": message = Actions.Move(this, -1, 0); break;
            case "d": message = Actions.Move(this, 1, 0); break;
            case "use": message = Actions.UseItem(this, number); break;
            case "buy": message = Actions.Buy(this, number); break;

            case "i": return Renderer.BagText(Player);
            case "shop": return Renderer.ShopText(Shop);
            case "help": return Commands.Help;
            default: return "Неизвестная команда. Введите help.";
        }

        Turn++;

        if (!IsWon)
        {
            string attacks = MonstersTurn();
            if (attacks != "")
                message += " " + attacks;
        }

        if (!Player.IsAlive)
            message += " Вы погибли.";

        return message;
    }

    // Ход монстров: игрока бьёт каждый живой монстр, который стоит вплотную к нему.
    // Возвращает сообщения об ударах; если никто не ударил — пустую строку.
    private string MonstersTurn()
    {
        string message = "";

        for (int i = 0; i < Monsters.Length; i++)
        {
            Monster monster = Monsters[i];
            if (!monster.IsAlive)
                continue;
            if (!IsNextToPlayer(monster))
                continue;

            if (message != "")
                message += " ";
            message += Combat.MonsterAttacks(monster, Player);
        }

        return message;
    }

    // Монстр стоит вплотную к игроку: на соседней клетке сверху, снизу, слева или справа (не по диагонали).
    private bool IsNextToPlayer(Monster monster)
    {
        int dx = Math.Abs(monster.X - Player.X);
        int dy = Math.Abs(monster.Y - Player.Y);
        return dx + dy == 1;
    }

    // Живой монстр на клетке (x, y). Если такого нет — null.
    public Monster FindMonsterAt(int x, int y)
    {
        for (int i = 0; i < Monsters.Length; i++)
        {
            Monster monster = Monsters[i];
            if (monster.IsAlive && monster.X == x && monster.Y == y)
                return monster;
        }
        return null;
    }

    // Предмет, который лежит на полу на клетке (x, y). Если такого нет — null.
    public FloorItem FindLootAt(int x, int y)
    {
        for (int i = 0; i < Loot.Length; i++)
        {
            FloorItem loot = Loot[i];
            if (!loot.IsTaken && loot.X == x && loot.Y == y)
                return loot;
        }
        return null;
    }

    // Игрок дошёл до выхода.
    public void Win()
    {
        IsWon = true;
    }
}
