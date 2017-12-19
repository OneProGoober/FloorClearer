using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    //If true, the key opens a door along the main path, else the extraneous path.
    private bool main;
    private int keyNumber;

    public void SetKeyStats(int keyNumber, bool main)
    {
        this.main = main;
        this.keyNumber = keyNumber;
    }
}
