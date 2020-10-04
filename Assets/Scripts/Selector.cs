using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class Selector : MonoBehaviour
{
    private List<Selectable> _selectables;

    void Awake()
    {
        _selectables = transform.Find("Characters").GetComponentsInChildren<Selectable>().ToList();
    }

    public void Initialise(Vector3 gridOriginInWorldSpace)
    {
        foreach (var s in _selectables)
        {
            s.Initialise(gridOriginInWorldSpace);
        }
    }

    public void PreventPlayerMoves()
    {
        foreach (var s in _selectables)
        {
            s.CanMove = false;
        }
    }
}



