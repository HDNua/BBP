using System;
using UnityEngine;



[RequireComponent(typeof(Animator))]
/// <summary>
/// 효과 스크립트입니다.
/// </summary>
public class EffectScript : MonoBehaviour
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    AudioSource _audioSource = null;
    Animator _animator;

    #endregion



    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 효과 애니메이션의 클립 길이를 가져옵니다.
    /// </summary>
    public float _clipLength;

    #endregion





    #region 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 효과 종료가 요청되었습니다.
    /// </summary>
    bool _endRequested = false;
    /// <summary>
    /// 효과 개체 종료가 요청되었습니다.
    /// </summary>
    public bool EndRequested
    {
        get { return _endRequested; }
        private set { _animator.SetBool("EndRequested", _endRequested = value); }
    }
    /// <summary>
    /// 효과 개체 삭제가 요청되었습니다.
    /// </summary>
    bool _destroyRequested = false;
    /// <summary>
    /// 효과 개체 삭제가 요청되었습니다.
    /// </summary>
    public bool DestroyRequested
    {
        get { return _destroyRequested; }
        private set { _destroyRequested = value; }
    }
    
    /// <summary>
    /// 팔레트 변경 요청이 들어왔다면 참입니다.
    /// </summary>
    bool _paletteChangeRequested = false;
    /// <summary>
    /// 기본 색상 팔레트입니다.
    /// </summary>
    Color[] _defaultPalette = null;
    /// <summary>
    /// 현재 팔레트입니다.
    /// </summary>
    Color[] _currentPalette = null;
    
    #endregion

    



    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    void Awake()
    {
        _animator = GetComponent<Animator>();

        // 효과 개체에서 애니메이션은 유일하게 하나 존재합니다.
        // 해당 애니메이션의 길이로 초기화 합니다.
        var clips = _animator.runtimeAnimatorController.animationClips;
        _clipLength = clips[0].length;
    }
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    void Start()
    {

    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    void Update()
    {
        if (_destroyRequested == false)
            return;

        // 애니메이션이 재생중이거나 음원 재생중이라면
        if (_animator.enabled || _audioSource && _audioSource.isPlaying)
        {

        }
        // 모든 재생이 끝난 경우 파괴합니다.
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    void LateUpdate()
    {
        if (_paletteChangeRequested)
        {
            UpdateTextureColor();
        }
    }

    #endregion

    



    #region 프레임 이벤트 핸들러를 정의합니다.
    /// <summary>
    /// 효과를 끝냅니다.
    /// </summary>
    void FE_EndEffect()
    {
        RequestDestroy();
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="srcColors"></param>
    /// <param name="dstColors"></param>
    /// <returns></returns>
    protected Texture2D GetColorUpdatedTexture(Texture2D texture, Color[] srcColors, Color[] dstColors)
    {
        Color[] texturePixelArray = texture.GetPixels();
        Color[] pixels = new Color[texturePixelArray.Length];

        // 모든 픽셀을 돌면서 색상을 업데이트합니다.
        for (int pixelIndex = 0, pixelCount = texturePixelArray.Length; pixelIndex < pixelCount; ++pixelIndex)
        {
            Color currentPixelColor = texturePixelArray[pixelIndex];
            pixels[pixelIndex] = currentPixelColor;
            if (currentPixelColor.a == 1)
            {
                for (int targetIndex = 0, targetPixelCount = srcColors.Length;
                    targetIndex < targetPixelCount;
                    ++targetIndex)
                {
                    Color srcColor = srcColors[targetIndex];
                    if (Mathf.Approximately(currentPixelColor.r, srcColor.r) &&
                        Mathf.Approximately(currentPixelColor.g, srcColor.g) &&
                        Mathf.Approximately(currentPixelColor.b, srcColor.b) &&
                        Mathf.Approximately(currentPixelColor.a, srcColor.a))
                    {
                        pixels[pixelIndex] = dstColors[targetIndex];
                        break;
                    }
                }
            }
            else
            {
                pixels[pixelIndex] = currentPixelColor;
            }
        }

        // 텍스쳐를 복제하고 새 픽셀 팔레트로 덮어씌웁니다.
        Texture2D cloneTexture = new Texture2D(texture.width, texture.height);
        cloneTexture.filterMode = FilterMode.Point;
        cloneTexture.SetPixels(pixels);
        cloneTexture.Apply();

        // 완성된 텍스쳐를 반환합니다.
        return cloneTexture;
    }
    /// <summary>
    /// 필드 색상표를 바탕으로 텍스쳐 색상을 업데이트합니다.
    /// </summary>
    protected void UpdateTextureColor()
    {
        // 
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Texture2D cloneTexture = GetColorUpdatedTexture
            (renderer.sprite.texture, _defaultPalette, _currentPalette);

        // 새 텍스쳐를 렌더러에 반영합니다.
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", cloneTexture);
        renderer.SetPropertyBlock(block);
    }

    #endregion
    




    #region 외부에서 호출 가능한 메서드를 정의합니다.
    /// <summary>
    /// 효과 객체를 x 반전합니다.
    /// </summary>
    public void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    /// <summary>
    /// 효과 객체 파괴를 요청합니다.
    /// </summary>
    public void RequestDestroy()
    {
        _animator.enabled = false;
        GetComponent<SpriteRenderer>().color = Color.clear;
        DestroyRequested = true;
    }
    /// <summary>
    /// 효과 객체 종료를 요청합니다.
    /// </summary>
    public void RequestEnd()
    {
        if (_animator.parameters.Length > 0)
        {
            EndRequested = true;
        }
        else
        {
            RequestDestroy();
        }
    }
    /// <summary>
    /// AudioSource 컴포넌트의 clip을 설정합니다. AudioSource가 없으면 생성합니다.
    /// </summary>
    /// <param name="audioClip">붙일 clip입니다.</param>
    public void AttachSound(AudioClip audioClip)
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = audioClip;
    }
    /// <summary>
    /// AudioSource에 설정된 효과음을 재생합니다.
    /// </summary>
    public void PlayEffectSound()
    {
        _audioSource.Play();
    }
    /// <summary>
    /// AudioSource의 clip을 설정하고 재생합니다. AudioSource가 없으면 생성합니다.
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlayEffectSound(AudioClip audioClip)
    {
        AttachSound(audioClip);
        PlayEffectSound();
    }
    
    /// <summary>
    /// 텍스쳐를 업데이트합니다.
    /// </summary>
    /// <param name="defaultPalette">기본 색상 팔레트입니다.</param>
    /// <param name="targetPalette">타겟 색상 팔레트입니다.</param>
    public void RequestUpdateTexture(Color[] defaultPalette, Color[] targetPalette)
    {
        _defaultPalette = defaultPalette;
        _currentPalette = targetPalette;
        _paletteChangeRequested = true;
    }

    /// <summary>
    /// 애니메이터가 지정된 문자열의 상태인지 확인합니다.
    /// </summary>
    /// <param name="stateName">재생 중인지 확인하려는 상태의 이름입니다.</param>
    /// <param name="layerIndex">애니메이터 레이어 인덱스입니다. 기본값은 0입니다.</param>
    /// <returns>애니메이터가 지정된 문자열의 상태라면 true를 반환합니다.</returns>
    public bool IsAnimatorInState(string stateName, int layerIndex = 0)
    {
        return _animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
    }

    #endregion





    #region 구형 정의를 보관합니다.


    #endregion
}