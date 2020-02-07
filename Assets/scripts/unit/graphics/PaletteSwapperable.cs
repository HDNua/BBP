using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(SpriteRenderer))]
/// <summary>
/// 
/// </summary>
public class PaletteSwapperable : MonoBehaviour
{
    #region 컨트롤러가 사용할 Unity 컴포넌트를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    SpriteRenderer _renderer;

    #endregion



    #region 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    Texture2D _MainTex;
    /// <summary>
    /// 
    /// </summary>
    Texture2D _colorSwapTex;
    /// <summary>
    /// 
    /// </summary>
    Color[] _spriteColors;

    /// <summary>
    /// 
    /// </summary>
    Color[] _inputColors;

    #endregion



    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        // !!!!! IMPORTANT !!!!!
        // SpriteRenderer를 이 시점에 가져오지 않으면 이후의 과정이 동작하지 않는다
        _renderer = GetComponent<SpriteRenderer>();

        // 스왑 텍스쳐를 초기화 합니다.
        InitColorSwapTex();
    }

    #endregion



    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public void InitColorSwapTex()
    {
        Texture2D colorSwapTex = new Texture2D(256, 1, TextureFormat.RGBA32, false, false);
        colorSwapTex.filterMode = FilterMode.Point;

        // 
        for (int i = 0; i < colorSwapTex.width; ++i)
            colorSwapTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        colorSwapTex.Apply();

        // 
        _renderer.material.SetTexture("_SwapTex", colorSwapTex);

        // 
        _spriteColors = new Color[colorSwapTex.width];
        _colorSwapTex = colorSwapTex;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="color"></param>
    public void SwapColor(int index, Color color)
    {
        _spriteColors[index] = color;
        _colorSwapTex.SetPixel(index, 0, color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static Color ColorFromInt(int c, float alpha = 1.0f)
    {
        int r = (c >> 16) & 0x000000FF;
        int g = (c >> 8) & 0x000000FF;
        int b = c & 0x000000FF;

        Color ret = ColorFromIntRGB(r, g, b);
        ret.a = alpha;

        return ret;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Color ColorFromIntRGB(int r, int g, int b)
    {
        return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, 1.0f);
    }

    #endregion
}