using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    SpriteRenderer _Renderer
    {
        get { return GetComponent<SpriteRenderer>(); }
    }

    #endregion



    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 컨트롤러가 없는 개체라면 PaletteUser의 LateUpdate()에서 UpdateColor()를 실행합니다.
    /// </summary>
    public bool _noController = false;
    /// <summary>
    /// 인덱스 텍스쳐입니다.
    /// </summary>
    public Texture2D _indexTexture;
    /// <summary>
    /// 팔레트 텍스쳐 리스트입니다. 0번은 기본 팔레트입니다.
    /// </summary>
    public Texture2D[] _paletteTextures;

    /// <summary>
    /// 공용 알파 값입니다.
    /// </summary>
    public float _commonAlpha = 1;

    /// <summary>
    /// 팔레트 리스트 인덱스입니다.
    /// </summary>
    public int _paletteIndex = 0;

    /// <summary>
    /// 팔레트 스왑 기능을 사용합니다.
    /// </summary>
    public bool _usePaletteSwap = true;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 팔레트 스왑이 적용된 텍스쳐입니다. ColorSwap shader와 같이 사용합니다.
    /// </summary>
    Texture2D _colorSwapTexture;

    /// <summary>
    /// 팔레트 컬러 인덱스입니다.
    /// </summary>
    int[] _indexes;
    /// <summary>
    /// 팔레트 리스트입니다.
    /// </summary>
    uint[][] _palettes;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    private void Awake()
    {

    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    private void Start()
    {
        if (_usePaletteSwap)
        {
            // !!!!! IMPORTANT !!!!!
            /// SpriteRenderer를 이 시점에 가져오지 않으면 이후의 과정이 동작하지 않습니다!
            ///_Renderer = GetComponent<SpriteRenderer>();

            // 스왑 텍스쳐 및 팔레트를 초기화 합니다.
            InitColorSwapTexture();
            InitPalette();
        }
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    private void LateUpdate()
    {
        if (_usePaletteSwap)
        {
            if (_noController)
            {
                UpdateColor();
            }
        }
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

        // 렌더에 초기화한 스왑 텍스쳐를 적용합니다.
        _Renderer.material.SetTexture("_SwapTex", colorSwapTexture);
        _colorSwapTexture = colorSwapTexture;
    }
    /// <summary>
    /// 팔레트와 인덱스를 초기화 합니다.
    /// </summary>
    void InitPalette()
    {
        // _indexTexture를 사용하여 인덱스 리스트를 생성합니다
        Color[] indexPixels = _indexTexture.GetPixels();
        int paletteWidth = indexPixels.Length;
        _indexes = new int[paletteWidth];
        for (int i = 0; i < paletteWidth; ++i)
        {
            _indexes[i] = (int)(UIntFromColor(indexPixels[i]) & 0xFF);
        }

        // _paletteSprites를 사용하여 목적 팔레트를 생성합니다.
        int numOfPaletteSprites = _paletteTextures.Length;
        _palettes = new uint[numOfPaletteSprites][];
        if (_palettes == null)
        {
            throw new Exception("WHY IS THIS NOT INITIALIZED???");
        }
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

        //
        if (_palettes == null)
        {
            throw new Exception("WHY IS THIS NOT INITIALIZED???");
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
    public static uint UIntFromColor(Color color)
    {
        int a = (int)(color.a * 255);
        int r = (int)(color.r * 255);
        int g = (int)(color.g * 255);
        int b = (int)(color.b * 255);
        return (uint)((a << 24) | (r << 16) | (g << 8) | (b << 0));
    }

    #endregion





    #region 공용 메서드를 정의합니다.
    /// <summary>
    /// 팔레트 인덱스를 업데이트 합니다.
    /// </summary>
    /// <param name="paletteIndex">새 타겟 팔레트의 인덱스입니다.</param>
    public void UpdatePaletteIndex(int paletteIndex)
    {
        _paletteIndex = paletteIndex;
    }
    /// <summary>
    /// PaletteUser의 색상을 업데이트 합니다. 컨트롤러의 LateUpdate()에서 호출됩니다.
    /// 중요: 그레이스케일 팔레트에서 0과 255를 사용하지 마십시오.
    /// </summary>
    public void UpdateColor()
    {
        // 타겟 팔레트를 가져옵니다.
        if (_palettes == null)
        {
            InitColorSwapTexture();
            InitPalette();
            if (_palettes == null)
            {
                throw new Exception("IMPOSSIBLE");
            }
        }
        uint[] colors = _palettes[_paletteIndex];

        // 새 팔레트 값으로 색상을 덮어씌웁니다.
        for (int i = 0; i < _indexes.Length; ++i)
        {
            int index = _indexes[i];
            int colorValue = (int)(colors[i]);
            _colorSwapTexture.SetPixel(index, 0, ColorFromInt(colorValue, _commonAlpha));
        }
        _colorSwapTexture.Apply();
    }
    /// <summary>
    /// 텍스쳐를 활성화합니다.
    /// </summary>
    public void EnableTexture()
    {
        _Renderer.enabled = true;
    }
    /// <summary>
    /// 텍스쳐를 비활성화합니다.
    /// </summary>
    public void DisableTexture()
    {
        _Renderer.enabled = false;
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("PaletteUser로 대체되었습니다. 다음 커밋에서 발견하면 즉시 삭제하십시오.")]
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

    #endregion
}
