using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
/// <summary>
/// 
/// </summary>
public struct ColorDictElem
{
    /// <summary>
    /// 
    /// </summary>
    public uint key;
    /// <summary>
    /// 
    /// </summary>
    public Color color;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="color"></param>
    public ColorDictElem(uint key, Color color)
    {
        this.key = key;
        this.color = color;
    }
}