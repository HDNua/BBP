using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Groundable))]
/// <summary>
/// 스마슈를 정의합니다.
/// </summary>
public class EnemyBossSmashuUnit : EnemyBossUnit
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 숨고르기 시간입니다.
    /// </summary>
    public float TIME_WAIT = 0.4f;

    /// <summary>
    /// 대타격 공격의 대미지입니다.
    /// </summary>
    public int DAMAGE_DAETAKYOK = 20;

    #endregion



    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// 지상에 착지할 수 있는 유닛입니다.
    /// </summary>
    Groundable _Groundable
    {
        get { return GetComponent<Groundable>(); }
    }
    /// <summary>
    /// 환세 전투 관리자입니다.
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

    #endregion





    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 등장 상태라면 참입니다.
    /// </summary>
    public bool Appearing
    {
        get { return _appearing; }
        set { _Animator.SetBool("Appearing", _appearing = value); }
    }
    /// <summary>
    /// 등장 상태라면 참입니다.
    /// </summary>
    public bool Disappearing
    {
        get { return _disappearing; }
        set { _Animator.SetBool("Disappearing", _disappearing = value); }
    }
    /// <summary>
    /// 대미지를 받았다면 참입니다.
    /// </summary>
    public bool IsAlmostDead
    {
        get { return _Animator.GetBool("IsAlmostDead"); }
        set { _Animator.SetBool("IsAlmostDead", value); }
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
        if (Appearing == false && IsAlive())
        {
            MakeAttackable();
        }
        else
        {
            MakeUnattackable();
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





    #region "등장" 행동을 정의합니다.
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    Coroutine _coroutineAppear;

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
        StartAction();
        _coroutineAppear = StartCoroutine(CoroutineAppear());
    }
    /// <summary>
    /// 등장을 중지합니다.
    /// </summary>
    void StopAppearing()
    {
        Appearing = false;
        EndAction();
    }

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
        _coroutineAppear = null;
        yield break;
    }

    #endregion





    #region "퇴장" 행동을 정의합니다.
    /// <summary>
    /// 사라지기 코루틴입니다.
    /// </summary>
    Coroutine _coroutineDisappear;

    /// <summary>
    /// 캐릭터를 사라지게 합니다.
    /// </summary>
    public override void Disappear()
    {
        // 상태를 정의합니다.
        Disappearing = true;
        IsAlmostDead = false;

        // 
        StartAction();
        _coroutineDisappear = StartCoroutine(CoroutineDisappear());
    }
    /// <summary>
    /// 퇴장을 끝냅니다.
    /// </summary>
    public void EndDisappear()
    {
        EndAction();
        _BattleManager._smashuUnit = null;
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

    #endregion





    #region "사망" 행동을 정의합니다.
    /// <summary>
    /// 사망 코루틴입니다. 스마슈는 실제로 사망하지 않고 그저 사라지기만 합니다.
    /// </summary>
    Coroutine _coroutineDead;

    /// <summary>
    /// 캐릭터를 죽입니다.
    /// </summary>
    public override void Dead()
    {
        //
        if (IsDead == false)
        {
            IsDead = true;
            IsAlmostDead = true;

            // 
            if (_coroutineAppear != null) StopCoroutine(_coroutineAppear);

            // 
            if (_coroutinePattern != null) StopCoroutine(_coroutinePattern);

            // 
            StopAllCoroutines();
            _coroutineDead = StartCoroutine(CoroutineDead());
            _coroutineInvencible = StartCoroutine(CoroutineInvencible(999));
        }
    }

    /// <summary>
    /// 사망 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDead()
    {
        // 
        MakeUnattackable();

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





    #region "대타격" 행동을 정의합니다.
    /// <summary>
    /// 대타격 행동 중이라면 참입니다.
    /// </summary>
    bool _doingDaetakyok;
    /// <summary>
    /// 대타격 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingDaetakyok
    {
        get { return _doingDaetakyok; }
        private set { _Animator.SetBool("DoingDaetakyok", _doingDaetakyok = value); }
    }

    /// <summary>
    /// 대타격 코루틴입니다.
    /// </summary>
    Coroutine _coroutineDaetakyok;

    /// <summary>
    /// 대타격 행동을 시작합니다.
    /// </summary>
    public void DoDaetakyok()
    {
        DoingDaetakyok = true;

        // 대타격 코루틴을 시작합니다.
        StartAction();
        _coroutineDaetakyok = StartCoroutine(CoroutineDaetakyok());
    }
    /// <summary>
    /// 대타격 행동을 중지합니다.
    /// </summary>
    public void StopDaetakyok()
    {
        DoingDaetakyok = false;
        _damage = _defaultDamage;

        // 
        EndAction();
    }

    /// <summary>
    /// 대타격 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDaetakyok()
    {
        yield return false;
        RunAction();
        EnemyUnit attackRange = _attackRanges[0];

        // 대타격 시작 모션시까지 플레이어를 쳐다보면서 준비합니다.
        attackRange.gameObject.SetActive(false);
        LookPlayer();
        while (IsAnimatorInState("DaetakyokBeg"))
        {
            yield return false;
        }
        RunEndRequest = false;

        // 대타격 진행 시에는 몸에 닿았을 때의 대미지도 칼로 베었을 때와 같게 합니다.
        // 그렇게 해야 플레이어가 칼에 맞는 대신 몸통에 박치기하지 않을 것입니다.
        _damage = DAMAGE_DAETAKYOK;
        attackRange._damage = DAMAGE_DAETAKYOK;

        // 플레이어를 쳐다보는 방향으로 대타격을 진행합니다.
        float x = transform.localScale.x;
        float dirX = x;

        // 
        Transform dstPosition;
        if (transform.position.x == _BattleManager._positions[7].position.x)
        {
            dstPosition = _BattleManager._positions[9];
        }
        else if (transform.position.x == _BattleManager._positions[9].position.x)
        {
            dstPosition = _BattleManager._positions[7];
        }
        else
        {
            dstPosition = (dirX > 0) ? _BattleManager._positions[7] : _BattleManager._positions[9];
        }
        Vector3 dv = dstPosition.position - transform.position;

        //
        Velocity = new Vector2(x > 0 ? 10f : -10f, 0);
        while (Mathf.Abs(dv.x) >= THRESHOLD_NEAR_DIST)
        {
            dv = dstPosition.position - transform.position;
            Vector3 playerPos = _StageManager.GetCurrentPlayerPosition();
            Vector3 diffBetweenPlayer = playerPos - transform.position;
            if (THRESHOLD_NEAR_DIST >= Mathf.Abs(diffBetweenPlayer.x))
            {
                break;
            }
            yield return false;
        }

        // Run 애니메이션의 종료 시점을 모르는 경우 대기를 위해 사용합니다.
        RunEndRequest = true;
        while (_Animator.GetBool("RunEndRequest") == false)
        {
            yield return false;
        }

        // 공격을 진행합니다.
        Velocity = Vector2.zero;
        SoundEffects[1].Play();
        attackRange.gameObject.SetActive(true);
        while (IsAnimatorInState("DaetakyokRun"))
        {
            yield return false;
        }

        // 공격을 끝냅니다.
        while (IsAnimatorInState("DaetakyokEnd"))
        {
            yield return false;
        }
        RunEndRequest = false;
        attackRange.gameObject.SetActive(false);

        // 대타격을 종료합니다.
        StopDaetakyok();
        StopCoroutine(_coroutineDaetakyok);
        _coroutineDaetakyok = null;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    public float THRESHOLD_NEAR_DIST = 1f;
    /// <summary>
    /// 
    /// </summary>
    bool _runEndRequest = false;
    /// <summary>
    /// 
    /// </summary>
    public bool RunEndRequest
    {
        get { return _runEndRequest; }
        private set { _Animator.SetBool("RunEndRequest", _runEndRequest = value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public EnemyUnit[] _attackRanges;

    #endregion





    #region "쾌진격" 행동을 정의합니다.
    /// <summary>
    /// 쾌진격 행동 중이라면 참입니다.
    /// </summary>
    bool _doingKwaejinkyok;
    /// <summary>
    /// 쾌진격 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingKwaejinkyok
    {
        get { return _doingKwaejinkyok; }
        private set { _Animator.SetBool("DoingKwaejinkyok", _doingKwaejinkyok = value); }
    }

    /// <summary>
    /// 쾌진격 코루틴입니다.
    /// </summary>
    Coroutine _coroutineKwaejinkyok;

    /// <summary>
    /// 쾌진격 행동을 시작합니다.
    /// </summary>
    public void DoKwaejinkyok()
    {
        DoingKwaejinkyok = true;

        // 쾌진격 코루틴을 시작합니다.
        StartAction();
        _coroutineKwaejinkyok = StartCoroutine(CoroutineKwaejinkyok());
    }
    /// <summary>
    /// 쾌진격 행동을 중지합니다.
    /// </summary>
    public void StopKwaejinkyok()
    {
        DoingKwaejinkyok = false;
        EndAction();
    }

    /// <summary>
    /// 쾌진격 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineKwaejinkyok()
    {
        yield return false;
        RunAction();

        // 
        yield return false;

        // 
        StopKwaejinkyok();
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





    #region 구형 정의를 보관합니다.
    [Obsolete("이거 쓰긴 쓰나요?")]
    /// <summary>
    /// 스마슈의 패턴입니다.
    /// </summary>
    int Pattern
    {
        set { _Animator.SetInteger("Pattern", value); }
    }

    [Obsolete("행동 이후의 행동은 행동의 끝이 아닌 패턴에서 정의해야 합니다.")]
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


    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 패턴 코루틴입니다.
    /// </summary>
    Coroutine _coroutinePattern;

    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
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
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 패턴을 중지합니다.
    /// </summary>
    void StopPattern()
    {
        Pattern = 0;
    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 페이즈 1의 패턴 1을 실행합니다.
    /// </summary>
    void DoPattern11()
    {
        _coroutinePattern = StartCoroutine(CoroutinePattern11());
    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 페이즈 1의 패턴 1이 실행된 후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterPattern11()
    {
        Disappear();
    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 페이즈 1의 패턴 2를 실행합니다.
    /// </summary>
    void DoPattern12()
    {
        _coroutinePattern = StartCoroutine(CoroutinePattern12());
    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 페이즈 1의 패턴 2가 실행된 후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterPattern12()
    {
        Disappear();
    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 
    /// </summary>
    void DoPattern21()
    {

    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 
    /// </summary>
    void DoPattern22()
    {

    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 
    /// </summary>
    void DoPattern31()
    {

    }
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
    /// <summary>
    /// 
    /// </summary>
    void DoPattern32()
    {

    }

    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
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
    [Obsolete("패턴의 정의는 유닛이 아닌 전투 관리자에 있어야 합니다.")]
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

    #endregion
}