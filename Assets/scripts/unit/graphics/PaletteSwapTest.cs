using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 팔레트 스왑 테스트 모듈입니다.
/// https://gamedevelopment.tutsplus.com/tutorials/how-to-use-a-shader-to-dynamically-swap-a-sprites-colors--cms-25129#post_comments
/// </summary>
public class PaletteSwapTest : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    SpriteRenderer _spriteRenderer;

    /// <summary>
    /// 
    /// </summary>
    public Texture2D _MainTex;
    /// <summary>
    /// 
    /// </summary>
    public Texture2D _colorSwapTex;
    /// <summary>
    /// 
    /// </summary>
    public Color[] _spriteColors;

    /// <summary>
    /// 
    /// </summary>
    public Color[] _inputColors;


    void Awake()
    {
        // !!!!! IMPORTANT !!!!!
        // SpriteRenderer를 이 시점에 가져오지 않으면 이후의 과정이 동작하지 않는다
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 
        InitColorSwapTex();
        //SwapDemoColors();
    }
    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        // InitColorSwapTex();
    }

    /// <summary>
    /// 
    /// </summary>
    public void InitColorSwapTex()
    {
        Texture2D colorSwapTex = new Texture2D(256, 1, TextureFormat.RGBA32, false, false);
        colorSwapTex.filterMode = FilterMode.Point;

        for (int i = 0; i < colorSwapTex.width; ++i)
            colorSwapTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));

        colorSwapTex.Apply();

        _spriteRenderer.material.SetTexture("_SwapTex", colorSwapTex);

        _spriteColors = new Color[colorSwapTex.width];
        _colorSwapTex = colorSwapTex;

        _inputColors = new Color[colorSwapTex.width];
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

        //mSpriteColors[index] = color;
        //mColorSwapTex.SetPixel(index, 0, color);
    }



    /// <summary>
    /// 
    /// </summary>
    public int step = 0;



    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (step == 1)
        {
            for (int i = 0; i < _spriteColors.Length; ++i)
            {
                SwapColor(i, ColorFromIntRGB(255, 0, 0));
            }
            _colorSwapTex.Apply();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        if (step == 2)
        {
            for (int i = 0; i < _spriteColors.Length; ++i)
            {
                SwapColor(i, ColorFromIntRGB(0, 255, 0));
            }
            _colorSwapTex.Apply();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        if (step == 3)
        {
            for (int i = 0; i < _spriteColors.Length; ++i)
            {
                SwapColor(i, ColorFromIntRGB(0, 0, 255));
            }
            _colorSwapTex.Apply();
        }
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
}
