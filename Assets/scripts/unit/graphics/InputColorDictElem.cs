using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
/// <summary>
/// 
/// </summary>
public struct InputColorDictElem
{
    public uint key;
    public Color color;

    public InputColorDictElem(uint key, Color color)
    {
        this.key = key;
        this.color = color;
    }
}