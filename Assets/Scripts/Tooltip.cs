using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject _renderedToolTip;
    Text _title;
    Text _description;
    Text _stats;

    void Start()
    {
        _renderedToolTip = transform.Find("ToolTip").gameObject;
        _title = _renderedToolTip.transform.Find("Title").GetComponent<Text>();
        _description = _renderedToolTip.transform.Find("Description").GetComponent<Text>();
        _stats = _renderedToolTip.transform.Find("CurrentStats").GetComponent<Text>();

    }

    public void Select(string title, string description, string stats)
    {
        _renderedToolTip.SetActive(true);

        _title.text = title;
        _stats.text = stats;
        _description.text = description;


        transform.position = Input.mousePosition;
    }

    // Update is called once per frame
    public void Deselect()
    {
        _renderedToolTip.SetActive(false);
    }

    internal void Select(string description, string stats)
    {
        throw new NotImplementedException();
    }
}
