using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{

    public static event Action<Selectable, GridPosition> OnAttemptedMove = delegate { };

    private Vector3 _gridOriginInWorldSpace;
    public bool CanMove { get; set; }

    private Vector3 _lastPosition;

    public Character Character { get; private set; }

    void Awake()
    {
        CanMove = true;
        _lastPosition = transform.position;
        Character = GetComponentInChildren<Character>();
        Character.GetComponent<PositionHandler>().CanMove = false;
        Character.GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnMouseDrag()
    {
        if (CanMove)
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        if (CanMove)
        {
            var currentX = transform.position.x;
            var currentY = transform.position.y;

            var possibleXs = new List<float>
            {
                Mathf.Round(currentX) + 0.5f,
                Mathf.Round(currentX) - 0.5f
            };

            var possibleYs = new List<float>
            {
                Mathf.Round(currentY) + 0.5f,
                Mathf.Round(currentY) - 0.5f
            };

            var newPosition = new Vector3(possibleXs.OrderBy(p => Mathf.Abs(p - currentX)).First(), possibleYs.OrderBy(p => Mathf.Abs(p - currentY)).First(), transform.position.z);

            var newGridPosition = GridFromWorldPosition(newPosition);
            transform.position = _lastPosition;
            OnAttemptedMove(this, newGridPosition);

        }
    }

    private GridPosition GridFromWorldPosition(Vector3 position)
    {
        return new GridPosition { x = (int)(position.x - _gridOriginInWorldSpace.x), y = (int)(position.y - _gridOriginInWorldSpace.y) };
    }

    public void Initialise(Vector3 gridOriginInWorldSpace)
    {
        _gridOriginInWorldSpace = gridOriginInWorldSpace;
        Character.PositionHandler.Initialise(gridOriginInWorldSpace);
    }

    private Vector3 WorldFromGridPosition(GridPosition position)
    {
        return new Vector3(position.x + _gridOriginInWorldSpace.x, position.y + _gridOriginInWorldSpace.y, transform.position.z);
    }
}