using System;
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
    public InputColorDictElem[] _inputColors;

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<uint, Color> _inputColorDict;

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<uint, Color> _spriteColorDict;


    /// <summary>
    /// 
    /// </summary>
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
        Texture2D colorSwapTex = new Texture2D(
            _inputColors.Length, 1, TextureFormat.RGBA32, false, false);
        colorSwapTex.filterMode = FilterMode.Point;

        for (int i = 0; i < colorSwapTex.width; ++i)
            colorSwapTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        colorSwapTex.Apply();
        _spriteRenderer.material.SetTexture("_SwapTex", colorSwapTex);

        //_spriteColors = new Color[colorSwapTex.width];
        _spriteColorDict = new Dictionary<uint, Color>();
        _colorSwapTex = colorSwapTex;

        // 
        _inputColorDict = new Dictionary<uint, Color>();
        InitInputColorDict();
    }

    /// <summary>
    /// 
    /// </summary>
    void InitInputColorDict()
    {
        uint[] palettes = {
            0x3068C8ff, 0x2040d0ff, 0x70a0e8ff, 0xb8d8e8ff,
            0xe8f0f8ff, 0x98c0e0ff, 0x4088e0ff, 0x3048a8ff,
            0x102078ff, 0xc8e8f0ff, 0x10a8c0ff, 0x30c0c0ff,
            0x78e0e0ff,
        };

        for (int i = 0; i < 13; ++i)
        {
            _inputColors[i] = new InputColorDictElem(palettes[i] >> 8, _inputColors[i].color);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictIndex"></param>
    /// <param name="textureIndex"></param>
    /// <param name="color"></param>
    public void SwapColor(uint dictIndex, int textureIndex, Color color)
    {
        uint realDictIndex = dictIndex;

        //_spriteColors[index] = color;

        if (_spriteColorDict.ContainsKey(realDictIndex))
        {
            _spriteColorDict[realDictIndex] = color;
        }
        else
        {
            _spriteColorDict.Add(realDictIndex, color);
        }
        _colorSwapTex.SetPixel(textureIndex, 0, color);

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
            foreach (uint key in _spriteColorDict.Keys)
            {
                SwapColor(key, TextureIndexFromColorKey(key), ColorFromIntRGB(255, 0, 0));
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
            foreach (uint key in _spriteColorDict.Keys)
            {
                SwapColor(key, TextureIndexFromColorKey(key), ColorFromIntRGB(0, 255, 0));
            }
            _colorSwapTex.Apply();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int TextureIndexFromColorKey(uint key)
    {
        for (int i = 0; i < _inputColors.Length; ++i)
        {
            InputColorDictElem elem = _inputColors[i];
            if (elem.key == key)
                return i;
        }
        return -1;
    }
    
    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        if (step == 3)
        {
            // 
            _inputColorDict = new Dictionary<uint, Color>();
            for (int i = 0; i < _inputColors.Length; ++i)
            {
                InputColorDictElem elem = _inputColors[i];
                _inputColorDict.Add(elem.key, elem.color);
            }

            // 
            foreach (uint key in _inputColorDict.Keys)
            {
                SwapColor(key, TextureIndexFromColorKey(key), _inputColorDict[key]);
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



    #region 구형 정의를 보관합니다.
    [Obsolete("_spriteColorDict로 대체되었습니다.")]
    /// <summary>
    /// 
    /// </summary>
    public Color[] _spriteColors;
    [Obsolete("_inputColorDict로 대체되었습니다.")]
    /// <summary>
    /// 
    /// </summary>
    public Color[] _inputColorArrayDep;


    #endregion
}
