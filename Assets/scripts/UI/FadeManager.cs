using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 페이드 관리자입니다.
/// </summary>
public class FadeManager : MonoBehaviour
{
    #region 상수를 정의합니다.

    #endregion



    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// GUITexture를 대신하는 페이더 패널입니다.
    /// </summary>
    public UnityEngine.UI.Image _faderPanel;

    #endregion





    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 페이딩 목적 색상입니다. Fader는 기본으로 black입니다.
    /// </summary>
    public Color _colorDst = Color.black;
    /// <summary>
    /// 기본 페이딩 목적 색상입니다. Fader는 기본으로 black입니다.
    /// </summary>
    public Color _defaultColorDst = Color.black;

    /// <summary>
    /// 페이딩 시작 임계점입니다.
    /// </summary>
    public float _thresholdSrc = 0.01f;
    /// <summary>
    /// 페이딩 끝 임계점입니다.
    /// </summary>
    public float _thresholdDst = 0.99f;

    /// <summary>
    /// 페이드 인 속도입니다. 통상적으로 페이드 아웃의 1/3입니다.
    /// </summary>
    public float _fadeInSpeed = 1f;
    /// <summary>
    /// 페이드 아웃 속도입니다. 통상적으로 페이드 인의 3배입니다.
    /// </summary>
    public float _fadeOutSpeed = 3f;
    /// <summary>
    /// 기본 페이드 인 속도입니다. 통상적으로 페이드 아웃의 1/3입니다.
    /// </summary>
    public float _defaultFadeInSpeed = 1f;
    /// <summary>
    /// 기본 페이드 아웃 속도입니다. 통상적으로 페이드 인의 3배입니다.
    /// </summary>
    public float _defaultFadeOutSpeed = 3f;


    /// <summary>
    /// 페이드 인이 요청되었다면 참입니다.
    /// </summary>
    bool _fadeInRequested = false;
    /// <summary>
    /// 페이드 아웃이 요청되었다면 참입니다.
    /// </summary>
    bool _fadeOutRequested = false;

    /// <summary>
    /// 페이드 인이 끝났다면 참입니다.
    /// </summary>
    public bool FadeInEnded { get { return (_faderPanel.color == Color.clear); } }
    /// <summary>
    /// 페이드 아웃이 끝났다면 참입니다.
    /// </summary>
    public bool FadeOutEnded { get { return (_faderPanel.color == _colorDst); } }

    /// <summary>
    /// 페이더 알파 값입니다.
    /// </summary>
    public float Alpha { get { return _faderPanel.color.a; } }

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 페이드 인/아웃 관리자입니다.
    /// </summary>
    public static FadeManager Instance
    {
        get
        {
            return GameObject.FindGameObjectWithTag("FadeManager")
                .GetComponent<FadeManager>();
        }
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected void Start()
    {
        _faderPanel.enabled = true;
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected void Update()
    {
        if (_fadeInRequested)
        {
            FadeToClear();
            if (_faderPanel.color.a <= _thresholdSrc)
            {
                _faderPanel.color = Color.clear;
                _faderPanel.enabled = false;
                _fadeInRequested = false;
            }
        }
        else if (_fadeOutRequested)
        {
            _faderPanel.enabled = true;
            FadeToDestination();

            if (_faderPanel.color.a >= _thresholdDst)
            {
                _faderPanel.color = _colorDst;
                _fadeOutRequested = false;
            }
        }
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    protected void FixedUpdate()
    {

    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    protected void LateUpdate()
    {

    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 페이드인 효과를 한 단계 진행합니다.
    /// </summary>
    void FadeToClear()
    {
        _faderPanel.color = Color.Lerp(_faderPanel.color, Color.clear, _fadeInSpeed * Time.deltaTime);
    }
    /// <summary>
    /// 페이드아웃 효과를 한 단계 진행합니다.
    /// </summary>
    void FadeToDestination()
    {
        _faderPanel.color = Color.Lerp(_faderPanel.color, _colorDst, _fadeOutSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 페이드인 효과를 처리합니다.
    /// </summary>
    public void FadeIn()
    {
        _fadeInRequested = true;
        _fadeOutRequested = false;
    }
    /// <summary>
    /// 페이드 인 효과를 처리합니다.
    /// </summary>
    /// <param name="fadeSpeed">페이드 인 속도입니다.</param>
    public void FadeIn(float fadeSpeed, float thresSrc = 0)
    {
        _fadeInSpeed = fadeSpeed;
        if (thresSrc != 0)
        {
            _thresholdSrc = thresSrc;
        }
        FadeIn();
    }
    /// <summary>
    /// 페이드아웃 효과를 처리합니다.
    /// </summary>
    public void FadeOut()
    {
        _fadeInRequested = false;
        _fadeOutRequested = true;
    }
    /// <summary>
    /// 페이드아웃 효과를 처리합니다.
    /// </summary>
    /// <param name="fadeSpeed">페이드아웃 속도입니다.</param>
    public void FadeOut(float fadeSpeed, float thresDst = 0)
    {
        _fadeOutSpeed = fadeSpeed;
        if (thresDst != 0)
        {
            _thresholdDst = thresDst;
        }
        FadeOut();
    }

    /// <summary>
    /// 페이더를 초기화합니다.
    /// </summary>
    public void ResetToDefault()
    {
        _fadeInSpeed = _defaultFadeInSpeed;
        _fadeOutSpeed = _defaultFadeOutSpeed;
        _colorDst = _defaultColorDst;
    }

    /// <summary>
    /// 페이딩 색상을 변경합니다.
    /// </summary>
    /// <param name="color">새 색상입니다.</param>
    public void ChangeFadeColor(Color color)
    {
        _faderPanel.color = color;
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("이건 뭔가요?")]
    /// <summary>
    /// 페이딩 텍스쳐의 색상을 변경합니다.
    /// </summary>
    /// <param name="colorSrc">시작 색상입니다.</param>
    /// <param name="colorDst">끝 색상입니다.</param>
    /// <param name="thresDst">끝 색상으로 인지하는 임계점입니다.</param>
    public void ChangeFadeTextureColor(Color colorSrc, Color colorDst, float thresDst)
    {
        _faderPanel.color = colorSrc;
        _colorDst = colorDst;
        _thresholdDst = thresDst;
    }
    [Obsolete("_fadeInSpeed, _fadeOutSpeed로 대체되었습니다.")]
    /// <summary>
    /// 페이드 인/아웃 속도입니다.
    /// </summary>
    public float _fadeSpeed = 1.5f;

    #endregion
}
