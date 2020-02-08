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



    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public Texture2D[] _paletteTextures;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public int _paletteWidth;

    /// <summary>
    /// 
    /// </summary>
    public Texture2D _MainTex;
    /// <summary>
    /// 
    /// </summary>
    public Texture2D _colorSwapTexture;

    /// <summary>
    /// 
    /// </summary>
    public Color[][] _palettes;
    /// <summary>
    /// 
    /// </summary>
    public int _paletteIndex = 0;

    /// <summary>
    /// 
    /// </summary>
    public Color[] _defaultPalette;

    /// <summary>
    /// SwapInfo[]의 배열입니다.
    /// SwapInfo[0]: 디폴트 스왑 정보입니다. key에 대한 color가 자신과 같습니다.
    /// </summary>
    //SwapInfo[][] _swapDicts;


    #endregion





    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    void Awake()
    {
        // !!!!! IMPORTANT !!!!!
        // SpriteRenderer를 이 시점에 가져오지 않으면 이후의 과정이 동작하지 않는다
        _renderer = GetComponent<SpriteRenderer>();

        // 스왑 텍스쳐를 초기화 합니다.
        InitColorSwapTexture();
    }
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        // 
        // _valuesArray = new uint[][] { _values0, _values1 };
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    void LateUpdate()
    {
        // 
        _valuesArray = new uint[][] { _values0, _values1 };
        uint[] values = _valuesArray[_valuesArrayIndex];

        //
        for (int i = 0; i < _indexes.Length; ++i)
        {
            int index = _indexes[i];
            int value = (int)(values[i] >> 8);
            _colorSwapTexture.SetPixel(index, 0, ColorFromInt(value));
        }
        _colorSwapTexture.SetPixel(_startIndex, 0, ColorFromInt((int)(values[_endIndex] >> 8)));
        _colorSwapTexture.Apply();
    }

    public Color[] _outColors;
    public int _startIndex;
    public int _endIndex;
    public int[] _indexes =
    {
        124, 120, 172, 208, 240,
        188, 144, 108, 68, 220,
        104, 128, 176
    };
    public uint[] _values0 =
    {
        0x3068C8ff, 0x2040d0ff, 0x70a0e8ff, 0xb8d8e8ff, 0xe8f0f8ff,
        0x98c0e0ff, 0x4088e0ff, 0x3048a8ff, 0x102078ff, 0xc8e8f0ff,
        0x10a8c0ff, 0x30c0c0ff, 0x78e0e0ff
    };
    public uint[] _values1 =
    {
        0xC830A5FF, 0xD020BFFF, 0xE870D3FF, 0xE8B8E5FF, 0xF8E8F7FF,
        0xE098DDFF, 0xE040DEFF, 0xA83099FF, 0x78105AFF, 0xF0C8E6FF,
        0xC01062FF, 0xC0306BFF, 0xCB0387FF
    };
    public uint[][] _valuesArray;
    public int _valuesArrayIndex = 0;

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public void InitColorSwapTexture()
    {
        Color[] defaultPixels = _paletteTextures[0].GetPixels();
        _defaultPalette = defaultPixels;

        //
        _paletteWidth = 256; // defaultPixels.Length;
        Texture2D colorSwapTexture = new Texture2D
            (_paletteWidth, 1, TextureFormat.RGBA32, false, false);
        colorSwapTexture.filterMode = FilterMode.Point;

        // 
        for (int i = 0; i < _paletteWidth; ++i)
            colorSwapTexture.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        colorSwapTexture.Apply();

        // 
        _renderer.material.SetTexture("_SwapTex", colorSwapTexture);
        _colorSwapTexture = colorSwapTexture;

        // _paletteSprites를 사용하여 목적 팔레트를 생성합니다.
        int numOfPaletteSprites = _paletteTextures.Length;
        _palettes = new Color[numOfPaletteSprites][];
        //_swapDicts = new SwapInfo[numOfPaletteSprites][];

        /*
        for (int i = 0; i < numOfPaletteSprites; ++i)
        {
            Color[] pixels = _paletteTextures[i].GetPixels();
            SwapInfo[] swapDict = new SwapInfo[_paletteWidth];
            for (int j = 0; j < _paletteWidth; ++j)
            {
                Color color = pixels[j];
                if (color.a == 0)
                {
                    color = new Color(0, 0, 0);
                }
                uint key = UIntFromColor(defaultPixels[j]);
                if (defaultPixels[j].a == 0)
                {
                    key = 0;
                }

                color.a = 1f;

                // 
                swapDict[j] = new SwapInfo(key & 0x00FFFFFF, color);
            }

            //
            _swapDicts[i] = swapDict;
        }
        */
    }







    /// <summary>
    /// 팔레트를 초기화 합니다.
    /// </summary>
    /// <param name="palette"></param>
    /// <param name="colorKeys"></param>
    /// <param name="colorValues"></param>
    void InitPalette(Color[] palette, uint[] colorKeys, Color[] colorValues)
    {
        for (int i = 0; i < colorKeys.Length; ++i)
        {
            uint key = colorKeys[i];
            Color value = colorValues[i];
            palette[i] = value;
        }
    }

    /// <summary>
    /// 팔레트에서 지정된 키 값을 갖는 픽셀들의 색상을 변경합니다.
    /// </summary>
    /// <param name="palette"></param>
    /// <param name="key"></param>
    /// <param name="color"></param>
    public void SwapColor(int index, Color color)
    {
        //palette[index] = color;
        _colorSwapTexture.SetPixel(index, 0, color);
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private static uint UIntFromColor(Color color)
    {
        int a = (int)(color.a * 256);
        int r = (int)(color.r * 256);
        int g = (int)(color.g * 256);
        int b = (int)(color.b * 256);
        return (uint)((a << 24) | (r << 16) | (g << 8) | (b << 0));
    }

    #endregion





    #region 공용 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public void UpdateColor()
    {

    }

    #endregion





    #region 구형 정의를 보관합니다.
    /*
    /// <summary>
    /// 
    /// </summary>
    /// <param name="swapInfos"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    int TextureIndexFromColorKey(SwapInfo[] swapInfos, uint key)
    {
        for (int i = 0; i < swapInfos.Length; ++i)
        {
            SwapInfo elem = swapInfos[i];
            if (elem.srcColorKey == key)
                return i;
        }
        return -1;
    }
    */

    #endregion
}