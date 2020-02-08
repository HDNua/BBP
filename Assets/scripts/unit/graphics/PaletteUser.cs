using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[RequireComponent(typeof(SpriteRenderer))]
/// <summary>
/// 팔레트 스왑 사용자입니다.
/// </summary>
public class PaletteUser : MonoBehaviour
{
    #region 컨트롤러가 사용할 Unity 컴포넌트를 정의합니다.
    /// <summary>
    /// SpriteRenderer입니다.
    /// </summary>
    SpriteRenderer _renderer;

    #endregion



    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public Texture2D _indexTexture;
    /// <summary>
    /// 팔레트입니다. 0번은 기본 팔레트입니다.
    /// </summary>
    public Texture2D[] _paletteTextures;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 팔레트 스왑이 적용된 텍스쳐입니다. ColorSwap shader와 같이 사용합니다.
    /// </summary>
    public Texture2D _colorSwapTexture;

    /// <summary>
    /// 팔레트 컬러 인덱스입니다.
    /// </summary>
    public int[] _indexes =
    {
        124, 120, 172, 208, 240,
        188, 144, 108, 68, 220,
        104, 128, 176
    };
    /// <summary>
    /// 팔레트 리스트입니다.
    /// </summary>
    uint[][] _palettes;
    /// <summary>
    /// 팔레트 리스트 인덱스입니다.
    /// </summary>
    public int _paletteIndex = 0;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    void Awake()
    {
        // !!!!! IMPORTANT !!!!!
        // SpriteRenderer를 이 시점에 가져오지 않으면 이후의 과정이 동작하지 않습니다!
        _renderer = GetComponent<SpriteRenderer>();

        // 스왑 텍스쳐를 초기화 합니다.
        InitColorSwapTexture();
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    private void Start()
    {
        /*
        _indexes = new int[] {
            124, 120, 172, 208, 240,
            188, 144, 108, 68, 220,
            104, 128, 176
        };
        _paletteArray = new uint[][] { _values0, _values1 };
        */
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    void LateUpdate()
    {
        // 
        ///uint[] colors = _paletteArray[_paletteIndex];
        uint[] colors = _palettes[_paletteIndex];

        //
        for (int i = 0; i < _indexes.Length; ++i)
        {
            int index = _indexes[i];
            int colorValue = (int)(colors[i]);
            _colorSwapTexture.SetPixel(index, 0, ColorFromInt(colorValue));
        }
        _colorSwapTexture.Apply();
    }

    #endregion


    


    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 스왑 텍스쳐를 초기화 합니다.
    /// </summary>
    public void InitColorSwapTexture()
    {
        Texture2D colorSwapTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false, false)
        {
            filterMode = FilterMode.Point
        };
        for (int i = 0; i < 256; ++i)
            colorSwapTexture.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        colorSwapTexture.Apply();

        // 
        _renderer.material.SetTexture("_SwapTex", colorSwapTexture);
        _colorSwapTexture = colorSwapTexture;

        // _indexTexture를 사용하여 인덱스 리스트를 생성합니다
        int paletteWidth = _indexTexture.width;
        Color[] indexPixels = _indexTexture.GetPixels();
        _indexes = new int[paletteWidth];
        for (int i = 0; i < paletteWidth; ++i)
        {
            _indexes[i] = (int)(UIntFromColor(indexPixels[i]) & 0xFF);
        }

        // _paletteSprites를 사용하여 목적 팔레트를 생성합니다.
        int numOfPaletteSprites = _paletteTextures.Length;
        _palettes = new uint[numOfPaletteSprites][];
        for (int i = 0; i < numOfPaletteSprites; ++i)
        {
            Color[] paletteTexture = _paletteTextures[i].GetPixels();
            uint[] palette = new uint[paletteWidth];
            for (int j = 0; j < paletteWidth; ++j)
            {
                palette[j] = UIntFromColor(paletteTexture[j]);
            }
            _palettes[i] = palette;
        }
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
    /// 정수 값으로부터 색상을 가져옵니다.
    /// </summary>
    /// <param name="c">RGB 색상 정수 값입니다.</param>
    /// <param name="alpha">알파 값입니다.</param>
    /// <returns>RGBA Color를 반환합니다.</returns>
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
    /// RGB 각각의 정수 값으로부터 색상을 가져옵니다.
    /// </summary>
    /// <param name="r">R 값입니다.</param>
    /// <param name="g">G 값입니다.</param>
    /// <param name="b">B 값입니다.</param>
    /// <returns>RGBA Color를 반환합니다.</returns>
    public static Color ColorFromIntRGB(int r, int g, int b)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, 1.0f);
    }
    /// <summary>
    /// Color에 해당하는 정수 값을 ARGB 형태로 가져옵니다.
    /// </summary>
    /// <param name="color">정수 값을 가져올 Color입니다.</param>
    /// <returns>RGBA 정수 값입니다.</returns>
    private static uint UIntFromColor(Color color)
    {
        int a = (int)(color.a * 256);
        int r = (int)(color.r * 256);
        int g = (int)(color.g * 256);
        int b = (int)(color.b * 256);
        return (uint)((a << 24) | (r << 16) | (g << 8) | (b << 0));
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("인덱스와 팔레트로 대체되었습니다.")]
    /// <summary>
    /// 
    /// </summary>
    public uint[][] _paletteArray;

    [Obsolete("인덱스와 팔레트로 대체되었습니다.")]
    /// <summary>
    /// 
    /// </summary>
    public uint[] _values0 =
    {
        0x3068C8ff, 0x2040d0ff, 0x70a0e8ff, 0xb8d8e8ff, 0xe8f0f8ff,
        0x98c0e0ff, 0x4088e0ff, 0x3048a8ff, 0x102078ff, 0xc8e8f0ff,
        0x10a8c0ff, 0x30c0c0ff, 0x78e0e0ff
    };
    [Obsolete("인덱스와 팔레트로 대체되었습니다.")]
    /// <summary>
    /// 
    /// </summary>
    public uint[] _values1 =
    {
        0xC830A5FF, 0xD020BFFF, 0xE870D3FF, 0xE8B8E5FF, 0xF8E8F7FF,
        0xE098DDFF, 0xE040DEFF, 0xA83099FF, 0x78105AFF, 0xF0C8E6FF,
        0xC01062FF, 0xC0306BFF, 0xCB0387FF
    };

    #endregion
}
