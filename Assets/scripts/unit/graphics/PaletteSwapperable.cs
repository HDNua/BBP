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
    #region 컨트롤러가 사용할 Unity 개체를 보관합니다.
    /// <summary>
    /// 
    /// </summary>
    SpriteRenderer _spriteRenderer;

    #endregion



    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
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
    public ColorDictElem[] _inputColors;

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<uint, Color> _spriteColorDict;

    /// <summary>
    /// 
    /// </summary>
    public int _step = 0;

    /// <summary>
    /// 
    /// </summary>
    public Texture2D _1;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    void Awake()
    {
        // !!!!! IMPORTANT !!!!!
        // SpriteRenderer를 이 시점에 가져오지 않으면 이후의 과정이 동작하지 않는다
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 
        InitColorSwapTex();
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    public void Start()
    {
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    private void Update()
    {
        if (_step == 1)
        {
            foreach (uint key in _spriteColorDict.Keys)
            {
                SwapColor(key, TextureIndexFromColorKey(key), ColorFromIntRGB(255, 0, 0));
            }
            _colorSwapTex.Apply();
        }
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    private void FixedUpdate()
    {
        if (_step == 2)
        {
            foreach (uint key in _spriteColorDict.Keys)
            {
                SwapColor(key, TextureIndexFromColorKey(key), ColorFromIntRGB(0, 255, 0));
            }
            _colorSwapTex.Apply();
        }
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    void LateUpdate()
    {
        if (_step == 3)
        {
            // 
            Dictionary<uint, Color> inputColorDict = new Dictionary<uint, Color>();
            for (int i = 0; i < _inputColors.Length; ++i)
            {
                ColorDictElem elem = _inputColors[i];
                inputColorDict.Add(elem.key, elem.color);
            }

            // 
            foreach (uint key in inputColorDict.Keys)
            {
                SwapColor(key, TextureIndexFromColorKey(key), inputColorDict[key]);
            }
            _colorSwapTex.Apply();
        }
    }

    #endregion





    #region 보조 메서드를 정의합니다.
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

        // 




        // 
        for (int i = 0; i < 13; ++i)
        {
            _inputColors[i] = new ColorDictElem(palettes[i] >> 8, _inputColors[i].color);
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
        _colorSwapTex.SetPixel(textureIndex, 0, color);
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
            ColorDictElem elem = _inputColors[i];
            if (elem.key == key)
                return i;
        }
        return -1;
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





    #region 구형 정의를 보관합니다.
    [Obsolete("")]
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<uint, Color> _inputColorDict;

    #endregion
}