using System;
using UnityEngine;



[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PaletteUser))]
/// <summary>
/// 효과 스크립트입니다.
/// </summary>
public class EffectScript : MonoBehaviour
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// AudioSource 개체입니다.
    /// </summary>
    AudioSource _audioSource = null;
    /// <summary>
    /// Animator 개체입니다.
    /// </summary>
    protected Animator _animator;
    /// <summary>
    /// 팔레트 사용자입니다.
    /// </summary>
    protected PaletteUser _paletteUser;

    #endregion





    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 효과 애니메이션의 클립 길이를 가져옵니다.
    /// </summary>
    public float _clipLength = 0;

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

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator)
        {
            if (_animator.enabled)
            {
                // 효과 개체에서 애니메이션은 유일하게 하나 존재합니다.
                // 해당 애니메이션의 길이로 초기화 합니다.
                var clips = _animator.runtimeAnimatorController.animationClips;
                _clipLength = clips[0].length;
            }
        }
        _paletteUser = GetComponent<PaletteUser>();
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    protected virtual void Start()
    {

    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected virtual void Update()
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
    protected virtual void LateUpdate()
    {
        // 
        if (_paletteUser)
        {
            if (GetComponent<SpriteRenderer>().color != Color.clear)
            {
                _paletteUser.UpdateColor();
            }
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