using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBox : MonoBehaviour
{

    public GameObject NextBox;

    // Update is called once per frame
    public void Ok()
    {
        if (NextBox != null)
        {
            NextBox.SetActive(true);
        }
        gameObject.SetActive(false);

    }
}
