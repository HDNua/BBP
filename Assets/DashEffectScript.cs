using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 
/// </summary>
public class DashEffectScript : MonoBehaviour
{
    float _time = 0;
    float _maxTime = 1;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        PaletteUser paletteUser = GetComponent<PaletteUser>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        //
        _time += Time.deltaTime;

        // 
        //spriteRenderer.color = Color.clear;

        //
        float rate = 1 - _time / _maxTime;
        //paletteUser.UpdatePaletteAlpha(rate);
        //paletteUser._commonAlpha = rate;
        paletteUser.UpdateColor();
    }
}
