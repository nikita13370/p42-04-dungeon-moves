// Карта подземелья.
// Координаты клетки — (x, y): x — номер столбца слева направо, y — номер строки сверху вниз.
// Левая верхняя клетка — (0, 0). Чем ниже клетка, тем больше y.

public class GameMap
{
    public const char WallTile = '#';           // стена
    public const char FloorTile = '.';          // пол
    public const char ExitTile = '>';           // выход из подземелья
    public const char ShopTile = '$';           // здесь стоит торговец

    private readonly string[] _rows;            // _rows[y] — строка карты номер y, _rows[y][x] — её символ номер x

    public int Width { get; }
    public int Height { get; }

    public GameMap(string[] rows)
    {
        _rows = rows;
        Height = rows.Length;
        Width = rows[0].Length;
    }

    // Клетка (x, y) находится внутри карты.
    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    // Символ клетки (x, y). Вызывать только для клеток внутри карты.
    public char TileAt(int x, int y)
    {
        return _rows[y][x];
    }

    // Клетка (x, y) — стена. Всё, что за краем карты, тоже считается стеной.
    public bool IsWall(int x, int y)
    {
        if (!IsInside(x, y))
            return true;
        return TileAt(x, y) == WallTile;
    }

    public bool IsExit(int x, int y)
    {
        return IsInside(x, y) && TileAt(x, y) == ExitTile;
    }

    public bool IsShop(int x, int y)
    {
        return IsInside(x, y) && TileAt(x, y) == ShopTile;
    }
}
