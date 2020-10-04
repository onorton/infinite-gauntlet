using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiCounter : MonoBehaviour
{
    private Text _number;

    void Awake()
    {
        _number = transform.Find("Number").gameObject.GetComponent<Text>();
    }

    public void UpdateCounter(int count)
    {
        _number.text = $"{count}";
    }
}
