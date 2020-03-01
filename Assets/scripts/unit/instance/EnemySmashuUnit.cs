using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Groundable))]
/// <summary>
/// 스마슈를 정의합니다.
/// </summary>
public class EnemySmashuUnit : EnemyUnit
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// 지상에 착지할 수 있는 유닛입니다.
    /// </summary>
    Groundable _Groundable
    {
        get { return GetComponent<Groundable>(); }
    }
    /// <summary>
    /// 
    /// </summary>
    HwanseBattleManager _BattleManager
    {
        get { return (HwanseBattleManager)BattleManager.Instance; }
    }

    #endregion





    #region Groundable 컴포넌트를 구현합니다.
    /// <summary>
    /// 지상에 있다면 참입니다.
    /// </summary>
    bool Landed
    {
        get { return _Groundable.Landed; }
        set { _Groundable.Landed = value; }
    }
    /// <summary>
    /// 점프 상태라면 참입니다.
    /// </summary>
    bool Jumping
    {
        get { return _Groundable.Jumping; }
        set { _Groundable.Jumping = value; }
    }
    /// <summary>
    /// 낙하 상태라면 참입니다.
    /// </summary>
    bool Falling
    {
        get { return _Groundable.Falling; }
        set { _Groundable.Falling = value; }
    }
    /// <summary>
    /// 개체의 속도 벡터를 구합니다.
    /// </summary>
    Vector2 Velocity
    {
        get { return _Groundable.Velocity; }
        set { _Groundable.Velocity = value; }
    }

    /// <summary>
    /// 지상에 착륙합니다.
    /// </summary>
    void Land()
    {
        _Groundable.Land();
    }
    /// <summary>
    /// 점프합니다.
    /// </summary>
    void Jump()
    {
        _Groundable.Jump();
    }
    /// <summary>
    /// 낙하합니다.
    /// </summary>
    void Fall()
    {
        _Groundable.Fall();
    }

    /// <summary>
    /// 중력 가속도를 반영하여 종단 속도를 업데이트 합니다.
    /// </summary>
    void UpdateVy()
    {
        _Groundable.UpdateVy();
    }

    #endregion





    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 등장 준비 시간입니다.
    /// 등장 준비 시간 이후부터 스마슈가 반짝이며 나타납니다.
    /// </summary>
    public float _appearReadyTime = 0.9f;
    /// <summary>
    /// 퇴장 준비 시간입니다.
    /// 퇴장 준비 시간 이후부터 스마슈가 반짝이며 사라집니다.
    /// </summary>
    public float _disappearReadyTime = 0.9f;
    /// <summary>
    /// 피격 시 사라지기 전까지 피격 모션을 재생할 시간입니다.
    /// </summary>
    public float _damagedTime = 1.6f;

    #endregion





    #region 상태 필드를 정의합니다.
    /// <summary>
    /// 등장 상태라면 참입니다.
    /// </summary>
    bool _appearing = false;
    /// <summary>
    /// 사라지기 상태라면 참입니다.
    /// </summary>
    bool _disappearing = false;

    /// <summary>
    /// Groundable 컴포넌트가 활성화된 상태라면 참입니다.
    /// </summary>
    bool _isGroundableNow = true;

    /// <summary>
    /// 페이즈입니다.
    /// </summary>
    public int _phase = 0;

    #endregion





    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 등장 상태라면 참입니다.
    /// </summary>
    bool Appearing
    {
        get { return _appearing; }
        set { _Animator.SetBool("Appearing", _appearing = value); }
    }
    /// <summary>
    /// 등장 상태라면 참입니다.
    /// </summary>
    bool Disappearing
    {
        get { return _disappearing; }
        set { _Animator.SetBool("Disappearing", _disappearing = value); }
    }
    /// <summary>
    /// 대미지를 받았다면 참입니다.
    /// </summary>
    bool Damaged
    {
        get { return _Animator.GetBool("Damaged"); }
        set { _Animator.SetBool("Damaged", value); }
    }

    /// <summary>
    /// 스마슈의 패턴입니다.
    /// </summary>
    int Pattern
    {
        set { _Animator.SetInteger("Pattern", value); }
    }

    /// <summary>
    /// 등장 풀때기 효과입니다.
    /// </summary>
    GameObject EffectNinjaGrass
    {
        get { return _effects[0]; }
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 색상을 없애고 시작합니다.
        Color color;
        color = _Renderer.color;
        color.a = 0f;
        _Renderer.color = color;

        // 등장합니다.
        Appear();
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // 
        if (Appearing == false)
        {
            _damage = 3;
        }
        else
        {
            _damage = 0;
        }
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    protected override void FixedUpdate()
    {
        // 
        if (_isGroundableNow)
        {
            // 점프 중이라면
            if (Jumping)
            {
                if (Velocity.y <= 0)
                {
                    Fall();
                }
                else
                {
                    UpdateVy();
                }
            }
            // 떨어지고 있다면
            else if (Falling)
            {
                if (Landed)
                {
                    Land();
                }
                else
                {
                    UpdateVy();
                }
            }
            // 그 외의 경우
            else
            {
                if (Landed == false)
                {
                    Fall();
                }
            }
        }
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    protected override void LateUpdate()
    {
        _PaletteUser.UpdateColor();
    }

    #endregion





    #region 패턴 메서드를 정의합니다.
    /// <summary>
    /// 등장합니다.
    /// </summary>
    public override void Appear()
    {
        // 상태를 정의합니다.
        Appearing = true;

        // 효과음을 재생합니다.
        SoundEffects[0].Play();

        // 내용을 정의합니다.
        _coroutineAppear = StartCoroutine(CoroutineAppear());
    }
    /// <summary>
    /// 등장을 중지합니다.
    /// </summary>
    void StopAppearing()
    {
        Appearing = false;
    }
    /// <summary>
    /// 등장이 끝난 이후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterAppear()
    {
        int patternOffset = 0; // Random.Range(0, 2);
        int patternBase;

        // 
        switch (_phase)
        {
            case 0:
                patternBase = 11;
                break;
            case 1:
                patternBase = 21;
                break;
            default:
                patternBase = 31;
                break;
        }

        // 
        DoPattern(patternBase + patternOffset);
    }
    /// <summary>
    /// 캐릭터를 사라지게 합니다.
    /// </summary>
    public override void Disappear()
    {
        // 상태를 정의합니다.
        Disappearing = true;
        Damaged = false;

        // 
        _coroutineDisappear = StartCoroutine(CoroutineDisappear());
    }
    /// <summary>
    /// 
    /// </summary>
    public void EndDisappear()
    {
        _BattleManager._smashuUnit = null;
    }

    #endregion





    #region 패턴을 정의합니다.
    /// <summary>
    /// 패턴을 수행합니다.
    /// </summary>
    /// <param name="patternIndex">수행할 패턴의 인덱스입니다.</param>
    void DoPattern(int patternIndex)
    {
        // 상태를 정의합니다.
        Pattern = patternIndex;

        // 
        switch (patternIndex)
        {
            case 11:
                DoPattern11();
                break;
            case 12:
                DoPattern12();
                break;
            case 21:
                DoPattern21();
                break;
            case 22:
                DoPattern22();
                break;
            case 31:
                DoPattern31();
                break;
            case 32:
                DoPattern32();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 패턴을 중지합니다.
    /// </summary>
    void StopPattern()
    {
        Pattern = 0;
    }
    /// <summary>
    /// 페이즈 1의 패턴 1을 실행합니다.
    /// </summary>
    void DoPattern11()
    {
        _coroutinePattern = StartCoroutine(CoroutinePattern11());
    }
    /// <summary>
    /// 페이즈 1의 패턴 1이 실행된 후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterPattern11()
    {
        Disappear();
    }
    /// <summary>
    /// 페이즈 1의 패턴 2를 실행합니다.
    /// </summary>
    void DoPattern12()
    {
        _coroutinePattern = StartCoroutine(CoroutinePattern12());
    }
    /// <summary>
    /// 페이즈 1의 패턴 2가 실행된 후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterPattern12()
    {
        Disappear();
    }
    /// <summary>
    /// 
    /// </summary>
    void DoPattern21()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    void DoPattern22()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    void DoPattern31()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    void DoPattern32()
    {

    }

    #endregion





    #region 행동 메서드를 정의합니다.
    /// <summary>
    /// 캐릭터를 죽입니다.
    /// </summary>
    public override void Dead()
    {
        //
        if (IsDead == false)
        {
            IsDead = true;
            Damaged = true;

            // 
            if (_coroutineAppear != null) StopCoroutine(_coroutineAppear);
            if (_coroutinePattern != null) StopCoroutine(_coroutinePattern);

            // 
            ///StopAllCoroutines();
            _coroutineDead = StartCoroutine(CoroutineDead());
            _coroutineInvencible = StartCoroutine(CoroutineInvencible(999));
        }
    }


    /// <summary>
    /// 쾌진격 기술을 사용합니다.
    /// </summary>
    void DoSkillKwaejinkyuk()
    {

    }
    /// <summary>
    /// 쾌진격 기술의 사용을 중지합니다.
    /// </summary>
    void StopSkillKwaejinkyuk()
    {

    }
    /// <summary>
    /// 쾌진격 기술이 끝난 이후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterSkillKwaejinkyuk()
    {

    }

    #endregion





    #region 코루틴 메서드를 정의합니다.
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    Coroutine _coroutineAppear;
    /// <summary>
    /// 패턴 코루틴입니다.
    /// </summary>
    Coroutine _coroutinePattern;
    /// <summary>
    /// 사라지기 코루틴입니다.
    /// </summary>
    Coroutine _coroutineDisappear;
    /// <summary>
    /// 사망 코루틴입니다. 스마슈는 실제로 사망하지 않고 그저 사라지기만 합니다.
    /// </summary>
    Coroutine _coroutineDead;

    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineAppear()
    {
        // 
        gameObject.tag = "Untagged";
        _PaletteUser.DisableTexture();

        // 닌자 등장 풀때기 효과를 생성합니다.
        GameObject effectGrassObject = Instantiate(
            EffectNinjaGrass, 
            transform.position, 
            transform.rotation);
        effectGrassObject.SetActive(true);
        EffectScript effectGrass = effectGrassObject.GetComponent<EffectScript>();
        float effectClipLength = effectGrass._clipLength;

        // 
        float time = 0;
        while (time < _appearReadyTime)
        {
            time += Time.deltaTime;
            yield return false;
        }

        // 
        gameObject.tag = "Enemy";

        // 
        bool blink = false;
        while (time < effectClipLength)
        {
            time += TIME_30FPS + Time.deltaTime;

            if (blink)
            {
                _PaletteUser.EnableTexture();
            }
            else
            {
                _PaletteUser.DisableTexture();
            }
            blink = !blink;

            // 30 FPS 간격으로 반짝이게 합니다.
            yield return new WaitForSeconds(TIME_30FPS);
        }

        // 효과를 제거하고 스마슈의 색상을 원래대로 돌립니다.
        effectGrass.RequestDestroy();
        _PaletteUser.EnableTexture();

        // 등장을 끝냅니다.
        StopAppearing();
        PerformActionAfterAppear();
        _coroutineAppear = null;
        yield break;
    }
    /// <summary>
    /// 페이즈 1의 패턴 1 코루틴입니다.
    /// </summary>
    IEnumerator CoroutinePattern11()
    {
        // 
        yield return new WaitForSeconds(TIME_30FPS);
        while (IsAnimatorInState("Pattern111"))
        {
            yield return false;
        }

        // 
        yield return new WaitForSeconds(TIME_30FPS);
        while (IsAnimatorInState("Pattern112"))
        {
            yield return false;
        }

        // 
        yield return new WaitForSeconds(TIME_30FPS);
        while (IsAnimatorInState("Pattern113"))
        {
            yield return false;
        }

        // 
        yield return new WaitForSeconds(TIME_30FPS);

        // 패턴을 끝냅니다.
        StopPattern();
        PerformActionAfterPattern11();
        _coroutinePattern = null;
        yield break;
    }
    /// <summary>
    /// 페이즈 1의 패턴 2 코루틴입니다.
    /// </summary>
    IEnumerator CoroutinePattern12()
    {
        // 
        while (IsAnimatorInState("Pattern121"))
        {
            yield return false;
        }

        // 
        while (IsAnimatorInState("Pattern122"))
        {
            yield return false;
        }

        // 
        while (IsAnimatorInState("Pattern12E"))
        {
            yield return false;
        }

        // 패턴을 끝냅니다.
        StopPattern();
        PerformActionAfterPattern12();
        _coroutinePattern = null;
        yield break;
    }
    /// <summary>
    /// 사라지기 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDisappear()
    {
        float time = 0;
        while (time < _disappearReadyTime)
        {
            time += Time.deltaTime;

            // 30 FPS 간격으로 반짝이게 합니다.
            yield return false;
        }

        // 
        gameObject.tag = "Untagged";

        // 효과음을 재생합니다.
        SoundEffects[0].Play();

        // 닌자 등장 풀때기 효과를 생성합니다.
        GameObject effectGrassObject = Instantiate(
            EffectNinjaGrass,
            transform.position,
            transform.rotation);
        effectGrassObject.SetActive(true);
        EffectScript effectGrass = effectGrassObject.GetComponent<EffectScript>();
        float effectClipLength = effectGrass._clipLength;

        // 
        bool blink = false;
        time = 0;
        while (time < effectClipLength)
        {
            time += TIME_30FPS + Time.deltaTime;

            if (blink)
            {
                _PaletteUser.EnableTexture();
            }
            else
            {
                _PaletteUser.DisableTexture();
            }
            blink = !blink;

            // 30 FPS 간격으로 반짝이게 합니다.
            yield return new WaitForSeconds(TIME_30FPS);
        }

        // 효과를 제거하고 스마슈의 색상을 없앱니다.
        effectGrass.RequestDestroy();
        _PaletteUser.DisableTexture();

        // 퇴장을 끝냅니다.
        EndDisappear();
        Destroy(gameObject);
        yield break;
    }
    /// <summary>
    /// 사망 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDead()
    {
        // 
        gameObject.tag = "Untagged";

        // 
        bool blink = false;
        float time = 0;
        while (time < _damagedTime)
        {
            time += TIME_30FPS + Time.deltaTime;

            if (blink)
            {
                _PaletteUser.EnableTexture();
            }
            else
            {
                _PaletteUser.DisableTexture();
            }
            blink = !blink;

            // 30 FPS 간격으로 반짝이게 합니다.
            yield return new WaitForSeconds(TIME_30FPS);
        }

        // 효과음을 재생합니다.
        SoundEffects[0].Play();

        // 닌자 등장 풀때기 효과를 생성합니다.
        GameObject effectGrassObject = Instantiate(
            EffectNinjaGrass,
            transform.position,
            transform.rotation);
        effectGrassObject.SetActive(true);
        EffectScript effectGrass = effectGrassObject.GetComponent<EffectScript>();
        float effectClipLength = effectGrass._clipLength;

        // 
        blink = false;
        time = 0;
        while (time < effectClipLength)
        {
            time += TIME_30FPS + Time.deltaTime;

            if (blink)
            {
                _PaletteUser.EnableTexture();
            }
            else
            {
                _PaletteUser.DisableTexture();
            }
            blink = !blink;

            // 30 FPS 간격으로 반짝이게 합니다.
            yield return new WaitForSeconds(TIME_30FPS);
        }

        // 효과를 제거하고 스마슈의 색상을 없앱니다.
        effectGrass.RequestDestroy();
        _PaletteUser.DisableTexture();

        // 등장을 끝냅니다.
        _coroutineDead = null;
        EndDisappear();
        Destroy(gameObject);
        yield break;
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 폭발 효과를 생성합니다. (주의: 효과 0번은 폭발 개체여야 합니다.)
    /// </summary>
    protected virtual void CreateExplosion(Vector3 position)
    {
        Instantiate(DataBase.Instance.Explosion1Effect, position, transform.rotation)
            .gameObject.SetActive(true);
    }

    #endregion
}