using System;

public class GridPosition
{
    public int x { get; set; }
    public int y { get; set; }

    public bool IsAdjacent(GridPosition other)
    {
        return Math.Abs(x - other.x) <= 1 && Math.Abs(y - other.y) <= 1;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               y == position.y;
    }

    public override int GetHashCode()
    {
        int hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }

    public int GridDistance(GridPosition other)
    {
        return (int)Math.Sqrt(Math.Pow(x - other.x, 2) + Math.Pow(y - other.y, 2));
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}