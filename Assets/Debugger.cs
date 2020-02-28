using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



/// <summary>
/// 디버거 개체입니다.
/// </summary>
public class Debugger : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public AnimationClip[] animationClips;

    #endregion



    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    void Awake()
    {

    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    void Start()
    {
        SavePalette();
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    void Update()
    {

    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    void FixedUpdate()
    {
        
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    void LateUpdate()
    {

    }

    #endregion





    #region 테스트 진입점입니다.
    /// <summary>
    /// 
    /// </summary>
    void PrintAnimationClipLength()
    {
        foreach (AnimationClip clip in animationClips)
        {
            Debug.Log(clip.length);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void SavePalette()
    {
        /*
        int[] INVENCIBLE_COLOR_PALETTE =
            {
                0x88D8F8, 0x90D8F8, 0x98D8F8, 0xA0D8F8,
                0xA8E0F8, 0xB0E0F8, 0xB8E0F8,
                0xC0E0F8, 0xC8E8F8, 0xD0E8F8, 0xD8E8F8,
                0xE0F0F8, 0xE8F0F8, 0xF0F0F8, 0xF8F8F8,
                0x88D8F8, 0x90D8F8, 0x98D8F8, 0xA0D8F8,
                0xA8E0F8, 0xB0E0F8, 0xB8E0F8,
                0xC0E0F8, 0xC8E8F8, 0xD0E8F8, 0xD8E8F8,
                0xE0F0F8, 0xE8F0F8, 0xF0F0F8, 0xF8F8F8
            };
        for (int i = 0; i < palette.Length; ++i)
        {
            Color color = ColorFromInt(palette[i]);
            texture.SetPixel(i, 0, color);
        }
        */

        Color[][] playerXPalettes =
        {
            ZColorPalette.DefaultPalette,
            ZColorPalette.InvenciblePalette,
            ZColorPalette.DashEffectColorPalette,
        };
        Color[][] enemyPalettes =
        {
            //EnemyColorPalette.EnemyRinshanPalette,
        };

        // 
        for (int i = 0; i < playerXPalettes.Length; ++i)
        {
            string filename = string.Format("PlayerZPalettes{0:D2}.png", i);
            SavePalette(playerXPalettes[i], filename);
        }
        // 
        for (int i = 0; i < enemyPalettes.Length; ++i)
        {
            string filename = string.Format("EnemyPalettes{0:D2}.png", i);
            SavePalette(enemyPalettes[i], filename);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="palette"></param>
    /// <param name="filename"></param>
    void SavePalette(Color[] palette, string filename)
    {
        Texture2D texture = new Texture2D(palette.Length, 1);
        texture.SetPixels(palette);
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);
    }

    #endregion





    #region 보조 메서드를 정의합니다.
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
        int a = (int)(color.a * 256);
        int r = (int)(color.r * 256);
        int g = (int)(color.g * 256);
        int b = (int)(color.b * 256);
        return (uint)((a << 24) | (r << 16) | (g << 8) | (b << 0));
    }

    #endregion
}
