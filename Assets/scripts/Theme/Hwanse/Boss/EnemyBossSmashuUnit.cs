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
    public int DAMAGE_DAETAKYUK = 20;
    /// <summary>
    /// 쾌진격 공격의 대미지입니다.
    /// </summary>
    public int DAMAGE_KWAEJINKYUK = 15;

    /// <summary>
    /// 플레이어와 스마슈 사이가 가깝다고 인지할 수 있는 간격입니다.
    /// </summary>
    public float THRESHOLD_NEAR_DIST = 1f;

    /// <summary>
    /// 대타격 속도입니다.
    /// </summary>
    public float SPEED_DAETAKYUK = 10f;
    /// <summary>
    /// 쾌진격 속도입니다.
    /// </summary>
    public float SPEED_KWAEJINKYUK = 15f;

    #endregion



    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// 지상에 착지할 수 있는 유닛입니다.
    /// </summary>
    Groundable _groundable
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
        get { return _groundable.Landed; }
        set { _groundable.Landed = value; }
    }
    /// <summary>
    /// 점프 상태라면 참입니다.
    /// </summary>
    bool Jumping
    {
        get { return _groundable.Jumping; }
        set { _groundable.Jumping = value; }
    }
    /// <summary>
    /// 낙하 상태라면 참입니다.
    /// </summary>
    bool Falling
    {
        get { return _groundable.Falling; }
        set { _groundable.Falling = value; }
    }
    /// <summary>
    /// 개체의 속도 벡터를 구합니다.
    /// </summary>
    Vector2 Velocity
    {
        get { return _groundable.Velocity; }
        set { _groundable.Velocity = value; }
    }

    /// <summary>
    /// 착지가 막혀있다면 참입니다.
    /// </summary>
    public bool _landBlocked = false;
    /// <summary>
    /// 착지가 불가능하다면 참입니다.
    /// </summary>
    bool LandBlocked
    {
        get { return _landBlocked; }
        set { _landBlocked = value; }
    }

    /// <summary>
    /// 지상에 착륙합니다.
    /// </summary>
    void Land()
    {
        _groundable.Land();
    }
    /// <summary>
    /// 점프합니다.
    /// </summary>
    void Jump()
    {
        _groundable.Jump();
    }
    /// <summary>
    /// 낙하합니다.
    /// </summary>
    void Fall()
    {
        _groundable.Fall();
    }

    /// <summary>
    /// 중력 가속도를 반영하여 종단 속도를 업데이트 합니다.
    /// </summary>
    void UpdateVy()
    {
        _groundable.UpdateVy();
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

    /// <summary>
    /// 스마슈의 근거리 베기 공격 범위입니다.
    /// </summary>
    public EnemyUnit[] _attackRanges;


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
    /// 행동의 RUN 상태에 대한 종료 요청을 받았습니다.
    /// </summary>
    bool _runEndRequest = false;

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
    /// 행동의 RUN 상태에 대한 종료 요청을 받았습니다.
    /// </summary>
    public bool RunEndRequest
    {
        get { return _runEndRequest; }
        private set { _Animator.SetBool("RunEndRequest", _runEndRequest = value); }
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
                if (Landed)
                {
                    if (LandBlocked)
                    {
                        UpdateVy();
                    }
                    else
                    {
                        Land();
                    }
                }
                else if (Velocity.y <= 0)
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
                    if (LandBlocked)
                    {
                        UpdateVy();
                    }
                    else
                    {
                        Land();
                    }
                }
                else
                {
                    UpdateVy();
                }
            }
            // 그 외의 경우
            else
            {
                if (LandBlocked)
                {
                    UpdateVy();
                }
                else if (Landed == false)
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
        // 초기 설정을 진행합니다.
        MakeUnattackable(); // 스마슈가 플레이어를 공격하지 못하게 합니다.
        gameObject.tag = "Untagged"; // 플레이어가 스마슈를 공격하지 못하게 합니다.
        _PaletteUser.DisableTexture(); // 텍스쳐를 잠깐 가립니다.

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
            MakeUnattackable();
            yield return false;
        }

        // 중간 설정을 진행합니다.
        gameObject.tag = "Enemy"; // 플레이어가 스마슈를 공격할 수 있게 합니다.
        MakeAttackable(); // 스마슈가 플레이어를 공격할 수 있게 합니다.

        // 등장 시 깜빡이는 부분입니다.
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
        // 초기화를 진행합니다.
        _hasBulletImmunity = true;
        gameObject.tag = "Untagged"; // 플레이어가 스마슈를 때릴 수 없게 합니다.
        MakeUnattackable(); // 스마슈가 플레이어를 때릴 수 없게 합니다.
        yield return false;
        RunAction();

        // 퇴장 준비 시간입니다.
        float time = 0;
        while (time < _disappearReadyTime)
        {
            time += Time.deltaTime;

            // 30 FPS 간격으로 반짝이게 합니다.
            yield return false;
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
        EndAction();
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
            StopAllCoroutines();
            StartAction();
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
        Velocity = Vector2.zero;
        MakeUnattackable();
        yield return false;
        RunAction();

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
        EndAction();
        Destroy(gameObject);
        yield break;
    }

    #endregion





    #region "세레모니" 행동을 정의합니다.
    /// <summary>
    /// 세레모니 행동 중이라면 참입니다.
    /// </summary>
    bool _doingCeremony;
    /// <summary>
    /// 세레모니 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingCeremony
    {
        get { return _doingCeremony; }
        private set { _Animator.SetBool("Win", _doingCeremony = value); }
    }
    /// <summary>
    /// 세레모니 행동입니다.
    /// </summary>
    public void Ceremony()
    {
        DoingCeremony = true;
        _hasBulletImmunity = true;

        // 세레모니 코루틴을 시작합니다.
        StartAction();
        _coroutineCeremony = StartCoroutine(CoroutineCeremony());
    }
    /// <summary>
    /// 세레모니를 중지합니다.
    /// </summary>
    public void StopCeremonying()
    {
        DoingCeremony = false;
        _hasBulletImmunity = false;

        // 행동을 종료합니다.
        EndAction();
    }

    /// <summary>
    /// 세레모니 코루틴입니다.
    /// </summary>
    Coroutine _coroutineCeremony;
    /// <summary>
    /// 세레모니 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineCeremony()
    {
        yield return false;
        RunAction();

        // 
        while (IsAnimatorInState("Win"))
        {
            yield return false;
        }

        // 세레모니 상태를 끝냅니다.
        StopCeremonying();
        _coroutineCeremony = null;
        yield break;
    }

    #endregion





    #region "대타격" 행동을 정의합니다.
    /// <summary>
    /// 대타격 행동 중이라면 참입니다.
    /// </summary>
    bool _doingDaetakyuk;
    /// <summary>
    /// 대타격 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingDaetakyuk
    {
        get { return _doingDaetakyuk; }
        private set { _Animator.SetBool("DoingDaetakyuk", _doingDaetakyuk = value); }
    }

    /// <summary>
    /// 대타격 코루틴입니다.
    /// </summary>
    Coroutine _coroutineDaetakyuk;

    /// <summary>
    /// 대타격 행동을 시작합니다.
    /// </summary>
    public void DoDaetakyuk()
    {
        DoingDaetakyuk = true;

        // 대타격 코루틴을 시작합니다.
        StartAction();
        _coroutineDaetakyuk = StartCoroutine(CoroutineDaetakyuk());
    }
    /// <summary>
    /// 대타격 행동을 중지합니다.
    /// </summary>
    public void StopDaetakyuk()
    {
        DoingDaetakyuk = false;
        _damage = _defaultDamage;

        // 
        EndAction();
    }

    /// <summary>
    /// 대타격 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDaetakyuk()
    {
        yield return false;
        RunAction();
        EnemyUnit attackRange = _attackRanges[0];

        // 대타격 시작 모션시까지 플레이어를 쳐다보면서 준비합니다.
        attackRange.gameObject.SetActive(false);
        LookPlayer();
        while (IsAnimatorInState("DaetakyukBeg"))
        {
            yield return false;
        }
        RunEndRequest = false;

        // 대타격 진행 시에는 몸에 닿았을 때의 대미지도 칼로 베었을 때와 같게 합니다.
        // 그렇게 해야 플레이어가 칼에 맞는 대신 몸통에 박치기하지 않을 것입니다.
        _damage = DAMAGE_DAETAKYUK;
        attackRange._damage = DAMAGE_DAETAKYUK;

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
        Velocity = new Vector2(x > 0 ? SPEED_DAETAKYUK : -SPEED_DAETAKYUK, 0);
        while (Mathf.Abs(dv.x) >= THRESHOLD_NEAR_DIST)
        {
            dv = dstPosition.position - transform.position;
            Vector3 playerPos = _StageManager.GetCurrentPlayerPosition();
            Vector3 diffBetweenPlayer = playerPos - transform.position;

            if (THRESHOLD_NEAR_DIST >= Mathf.Abs(diffBetweenPlayer.x))
            {
                if (THRESHOLD_NEAR_DIST >= Mathf.Abs(diffBetweenPlayer.y))
                {
                    // 대타격으로 플레이어를 때릴 수 있다면 멈춥니다.
                    break;
                }
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
        while (IsAnimatorInState("DaetakyukRun"))
        {
            // 저는 위에서 한 번만 정의해주면 값이 바뀔 줄 알았는데 아니네요... 왜죠
            _damage = DAMAGE_DAETAKYUK;
            attackRange._damage = DAMAGE_DAETAKYUK;
            yield return false;
        }

        // 공격을 끝냅니다.
        while (IsAnimatorInState("DaetakyukEnd1"))
        {
            // 저는 위에서 한 번만 정의해주면 값이 바뀔 줄 알았는데 아니네요... 왜죠
            _damage = DAMAGE_DAETAKYUK;
            attackRange._damage = DAMAGE_DAETAKYUK;
            yield return false;
        }
        RunEndRequest = false;
        attackRange.gameObject.SetActive(false);

        // 
        while (IsAnimatorInState("DaetakyukEnd2"))
        {
            _damage = _defaultDamage;
            attackRange._damage = 0;
            yield return false;
        }
        _damage = _defaultDamage;

        // 대타격을 종료합니다.
        StopDaetakyuk();
        StopCoroutine(_coroutineDaetakyuk);
        _coroutineDaetakyuk = null;
        yield break;
    }

    #endregion





    #region "쾌진격" 행동을 정의합니다.
    /// <summary>
    /// 쾌진격 행동 중이라면 참입니다.
    /// </summary>
    bool _doingKwaejinkyuk;
    /// <summary>
    /// 쾌진격 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingKwaejinkyuk
    {
        get { return _doingKwaejinkyuk; }
        private set { _Animator.SetBool("DoingKwaejinkyuk", _doingKwaejinkyuk = value); }
    }

    /// <summary>
    /// 쾌진격 코루틴입니다.
    /// </summary>
    Coroutine _coroutineKwaejinkyuk;

    /// <summary>
    /// 쾌진격 행동을 시작합니다.
    /// </summary>
    public void DoKwaejinkyuk()
    {
        DoingKwaejinkyuk = true;

        // 쾌진격 코루틴을 시작합니다.
        StartAction();
        _coroutineKwaejinkyuk = StartCoroutine(CoroutineKwaejinkyuk());
    }
    /// <summary>
    /// 쾌진격 행동을 중지합니다.
    /// </summary>
    public void StopKwaejinkyuk()
    {
        DoingKwaejinkyuk = false;
        EndAction();
    }

    /// <summary>
    /// 쾌진격 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineKwaejinkyuk()
    {
        yield return false;
        RunAction();

        // 초기 설정을 진행합니다.
        LookPlayer();
        while (IsAnimatorInState("KwaejinkyukBeg"))
        {
            yield return false;
        }

        // 
        EnemyUnit attackRange0 = _attackRanges[1];
        EnemyUnit attackRange1 = _attackRanges[2];

        // 
        Vector3 playerPos = _StageManager.GetCurrentPlayerPosition();
        Vector3 dv = (playerPos - transform.position).normalized;
        SoundEffects[2].Play();

        _isGroundableNow = false;
        LandBlocked = true;

        Velocity = dv * SPEED_KWAEJINKYUK;
        LookPlayer();

        _groundable._whatIsWall = 0;
        _groundable._whatIsGround = 0;

        // https://forum.unity.com/threads/child-doesnt-follow-parent.314583/
        attackRange0.gameObject.SetActive(true);
        attackRange1.gameObject.SetActive(true);
        while (IsAnimatorInState("KwaejinkyukRun"))
        {
            _damage = DAMAGE_KWAEJINKYUK;
            yield return false;
        }

        // 행동을 종료합니다.
        StopKwaejinkyuk();
        _coroutineKwaejinkyuk = null;
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


    #endregion
}