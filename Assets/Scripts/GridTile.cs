using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    private GameObject _possibleDirections;
    public GridPosition Position { get; private set; }

    private void Start()
    {
        _possibleDirections = transform.Find("PossibleDirections").gameObject;
    }

    public void SetPosition(GridPosition p)
    {
        // Prevents accidentally altering it
        Position = Position ?? p;
    }
    public void UpdateViableLocations(IEnumerable<GridPosition> _occupiedPositions)
    {
        _possibleDirections = _possibleDirections ?? transform.Find("PossibleDirections").gameObject;
        foreach (var direction in _possibleDirections.GetComponentsInChildren<Transform>())
        {
            direction.gameObject.SetActive(false);
        }

        var enabledDirections = new List<string> { "North", "South", "East", "West", "NorthWest", "NorthEast", "SouthEast", "SouthWest" };

        foreach (var p in _occupiedPositions)
        {
            if (Position.IsAdjacent(p))
            {
                string direction = "";

                if ((p.y - Position.y) > 0)
                {
                    direction += "North";
                }
                else if ((p.y - Position.y) < 0)
                {
                    direction += "South";
                }

                if ((p.x - Position.x) > 0)
                {
                    direction += "East";
                }
                else if ((p.x - Position.x) < 0)
                {
                    direction += "West";
                }

                enabledDirections.Remove(direction);
            }

        }

        foreach (var direction in enabledDirections)
        {
            _possibleDirections.transform.Find(direction).gameObject.SetActive(true);
        }

    }

    public void TogglePossibleLocations(bool enabled)
    {
        _possibleDirections.SetActive(enabled);
    }
}