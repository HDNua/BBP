using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 
/// </summary>
public struct SwapInfoOld
{
    /// <summary>
    /// 
    /// </summary>
    public uint srcColorKey;
    /// <summary>
    /// 
    /// </summary>
    public Color dstColorValue;



    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public SwapInfoOld(uint key, Color value)
    {
        this.srcColorKey = key;
        this.dstColorValue = value;
    }
}
