using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Obsolete("더 안 쓸 것 같습니다.")]
[Serializable]
/// <summary>
/// 
/// </summary>
public struct ColorDictElem
{
    public uint key;
    public Color color;

    public ColorDictElem(uint key, Color color)
    {
        this.key = key;
        this.color = color;
    }
}