using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Groundable))]
/// <summary>
/// 린샹을 정의합니다.
/// </summary>
public class EnemyBossRinshanUnit : EnemyBossUnit
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 문과 가깝다고 느낄 거리입니다.
    /// </summary>
    public float THRESHOLD_NEAR_DOOR = 1f;

    /// <summary>
    /// 방어 시간입니다.
    /// </summary>
    public float TIME_GUARD = 3f;
    /// <summary>
    /// 수경 행동 시간입니다.
    /// </summary>
    public float TIME_SUKYEONG_RUN = 1f;
    /// <summary>
    /// 대폭진 행동 시간입니다.
    /// </summary>
    public float TIME_DAEPOKJIN_RUN = 1f;

    /// <summary>
    /// 영상뢰화 간격입니다.
    /// </summary>
    public float[] TIME_ROIHWA_INTERVAL = { 0.10f, 0.20f, 0.30f, 0.40f, 0.50f, };
    /// <summary>
    /// 영상뢰화가 끝나는 시간입니다.
    /// </summary>
    public float TIME_ROIHWA_END = 1f;

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
        SoundEffects[4].Play();
    }
    /// <summary>
    /// 점프합니다.
    /// </summary>
    void Jump()
    {
        ///_Groundable.Jump();
        Jumping = true;
        Landed = false;
        LandBlocked = true;
        SoundEffects[3].Play();
    }
    /// <summary>
    /// 낙하합니다.
    /// </summary>
    void Fall()
    {
        _Groundable.Fall();
        Landed = false;
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
    /// 방어 상태라면 참입니다.
    /// </summary>
    bool _guarding = false;

    /// <summary>
    /// X축 속력입니다.
    /// </summary>
    public float _movingSpeedX = 1;
    /// <summary>
    /// Y축 속력입니다.
    /// </summary>
    public float _movingSpeedY = 2;

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
    /// 행동의 RUN 상태에 대한 종료 요청을 받았습니다.
    /// </summary>
    bool _runEndRequest = false;
    /// <summary>
    /// 행동의 RUN 상태에 대한 종료 요청을 받았습니다.
    /// </summary>
    public bool RunEndRequest
    {
        get { return _runEndRequest; }
        private set { _Animator.SetBool("RunEndRequest", _runEndRequest = value); }
    }
    
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
    public bool IsKnockouted
    {
        get { return _Animator.GetBool("IsKnockouted"); }
        set { _Animator.SetBool("IsKnockouted", value); }
    }
    /// <summary>
    /// 방어 중이라면 참입니다.
    /// </summary>
    bool Guarding
    {
        get { return _guarding; }
        set { _Animator.SetBool("Guarding", _guarding = value); }
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 등장합니다.
        Appear();
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    protected override void FixedUpdate()
    {
        // 점프 중이라면
        if (Jumping)
        {
            if (Landed)
            {
                if (LandBlocked)
                {
                    Landed = false;
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
                    Landed = false;
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
            if (Landed == false)
            {
                Fall();
            }
        }

        // 디버깅용 구문입니다.
        _Groundable._position = transform.position;
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
        transform.position = _BattleManager._rinshanSpawnPosition.position;
        yield return false;
        RunAction();

        // Groundable 개체의 올바른 초기화는 일단 바닥에 닿은 것을 기준으로 합니다.
        Fall();
        while (Landed == false)
        {
            yield return false;
        }

        // 대시 애니메이션을 추가합니다.
        GameObject dashEffectObject = Instantiate(_effects[0], transform.position, transform.rotation, transform);

        // 
        float v0 = 10;
        float begPosX = transform.position.x;
        float endPosX = _BattleManager._rinshanSpawnEndPosition.position.x;
        float dist = Mathf.Abs(endPosX - begPosX);
        float time = 1f;
        float passTime = 0;

        // 착지 완료 후 왼쪽에서 보스 문을 박차고 들어옵니다.
        // 소환 후 등장 지점까지 이동합니다.
        Velocity = new Vector2(v0, 0);
        SoundEffects[0].Play();

        bool explosionOn = false;
        Vector3 doorPos = _BattleManager._bossRoomDoor.transform.position;
        while (Velocity.x > 0)
        {
            if (explosionOn == false && Mathf.Abs(doorPos.x - transform.position.x) < THRESHOLD_NEAR_DOOR)
            {
                _BattleManager.RequestDestroyDoor();
                explosionOn = true;
            }
            yield return false;

            // 
            Velocity = new Vector2(v0 - ((2 * dist) / (time * time)) * passTime, 0);
            passTime += Time.deltaTime;
        }
        transform.position = new Vector3(_BattleManager._rinshanSpawnEndPosition.position.x, transform.position.y, transform.position.z);
        Velocity = Vector2.zero;

        // 수경 등을 이용하여 들어온 문을 막습니다.

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
        IsKnockouted = false;

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

        // 퇴장을 끝냅니다.
        EndDisappear();
        Destroy(gameObject);
        yield break;
    }

    #endregion





    #region "방어" 행동을 정의합니다.
    /// <summary>
    /// 방어 행동입니다.
    /// </summary>
    public void Guard()
    {
        Guarding = true;
        _hasBulletImmunity = true;

        // 막기 코루틴을 시작합니다.
        StartAction();
        _coroutineGuard = StartCoroutine(CoroutineGuard());
    }
    /// <summary>
    /// 방어를 중지합니다.
    /// </summary>
    public void StopGuarding()
    {
        Guarding = false;
        _hasBulletImmunity = false;

        // 행동을 종료합니다.
        EndAction();
    }

    /// <summary>
    /// 방어 코루틴입니다.
    /// </summary>
    Coroutine _coroutineGuard;
    /// <summary>
    /// 방어 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineGuard()
    {
        yield return false;
        RunAction();

        // 
        float time = 0;
        while (time < TIME_GUARD)
        {
            time += Time.deltaTime;
            yield return false;
        }

        // 막기 상태를 끝냅니다.
        StopGuarding();
        _coroutineGuard = null;
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





    #region "기절" 행동을 정의합니다.
    /// <summary>
    /// 기절 코루틴입니다. 린샹은 실제로 사망하지 않고 그저 무적 상태로 일정 시간 방어만 합니다.
    /// </summary>
    Coroutine _coroutineDead;

    /// <summary>
    /// 캐릭터를 기절시킵니다. 린샹은 실제로 사망하지 않고 그저 무적 상태로 일정 시간 방어만 합니다.
    /// </summary>
    public override void Dead()
    {
        //
        if (IsDead == false)
        {
            IsDead = true;
            IsKnockouted = true;
            StopGuarding();
            StopDaepokjin();
            StopSukyeong();
            StopRoihwa();

            // 
            if (_coroutineAppear != null) StopCoroutine(_coroutineAppear);

            // 
            StopAllCoroutines();
            _coroutineDead = StartCoroutine(CoroutineDead());
            _coroutineInvencible = StartCoroutine(CoroutineInvencible(TIME_INVENCIBLE));
        }
    }

    /// <summary>
    /// 기절 코루틴입니다.
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
        IsKnockouted = false;
        _health = _maxHealth;
        _PaletteUser.UpdatePaletteIndex(0);

        // 재정비 상태에 들어갑니다.
        _hasBulletImmunity = true;
        Guarding = true;
        while (IsAnimatorInState("Regroup") == false)
        {
            yield return false;
        }

        // Guard를 호출하는 대신 직접 Guarding을 조작하는 이유는,
        // Guard 함수가 코루틴을 생성하는 함수이기 때문입니다.
        // 여기서 Guarding 파라미터는 Regroup AnimatorState로 넘어가기 위한 조건일 뿐
        // 실제로 방어 행동을 목적으로 사용되지는 않았기 때문에 서로 별개입니다.
        time = 0;
        while (IsAnimatorInState("Regroup"))
        {
            yield return false;
            if (time >= TIME_GUARD)
            {
                break;
            }
            time += Time.deltaTime;
        }
        Guarding = false;
        _hasBulletImmunity = false;

        // 기절 상태를 끝냅니다.
        MakeAttackable();
        IsDead = false;
        _coroutineDead = null;
        yield break;
    }

    #endregion





    #region "수경" 행동을 정의합니다.
    /// <summary>
    /// 수경 행동 중이라면 참입니다.
    /// </summary>
    bool _doingSukyeong;
    /// <summary>
    /// 수경 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingSukyeong
    {
        get { return _doingSukyeong; }
        private set { _Animator.SetBool("DoingSukyeong", _doingSukyeong = value); }
    }

    /// <summary>
    /// 수경 코루틴입니다.
    /// </summary>
    Coroutine _coroutineSukyeong;

    /// <summary>
    /// 수경 행동을 시작합니다.
    /// </summary>
    public void DoSukyeong()
    {
        DoingSukyeong = true;

        // 대타격 코루틴을 시작합니다.
        StartAction();
        _coroutineSukyeong = StartCoroutine(CoroutineSukyeong());
    }
    /// <summary>
    /// 수경 행동을 중지합니다.
    /// </summary>
    public void StopSukyeong()
    {
        DoingSukyeong = false;

        // 
        EndAction();
    }

    /// <summary>
    /// 수경 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineSukyeong()
    {
        yield return false;
        RunAction();

        // 수경을 준비합니다.
        while (IsAnimatorInState("SukyeongBeg"))
        {
            yield return false;
        }

        // 
        float time = 0;
        while (IsAnimatorInState("SukyeongRun"))
        {
            yield return false;
            if (time >= TIME_SUKYEONG_RUN)
            {
                break;
            }
            time += Time.deltaTime;
        }

        // 수경을 종료합니다.
        StopSukyeong();
        StopCoroutine(_coroutineSukyeong);
        _coroutineSukyeong = null;
        yield break;
    }

    #endregion





    #region "대폭진" 행동을 정의합니다.
    /// <summary>
    /// 대폭진 행동 중이라면 참입니다.
    /// </summary>
    bool _doingDaepokjin;
    /// <summary>
    /// 대폭진 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingDaepokjin
    {
        get { return _doingDaepokjin; }
        private set { _Animator.SetBool("DoingDaepokjin", _doingDaepokjin = value); }
    }

    /// <summary>
    /// 대폭진 코루틴입니다.
    /// </summary>
    Coroutine _coroutineDaepokjin;

    /// <summary>
    /// 대폭진 행동을 시작합니다.
    /// </summary>
    public void DoDaepokjin()
    {
        DoingDaepokjin = true;

        // 대폭진 코루틴을 시작합니다.
        StartAction();
        _coroutineDaepokjin = StartCoroutine(CoroutineDaepokjin());
    }
    /// <summary>
    /// 대폭진 행동을 중지합니다.
    /// </summary>
    public void StopDaepokjin()
    {
        DoingDaepokjin = false;
        EndAction();
    }

    /// <summary>
    /// 대폭진 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDaepokjin()
    {
        yield return false;
        RunAction();

        // 
        while (IsAnimatorInState("DaepokjinBeg"))
        {
            yield return false;
        }

        // 
        SoundEffects[6].Play();
        while (IsAnimatorInState("DaepokjinRun"))
        {
            yield return false;
        }

        // 
        float time = 0;
        while (IsAnimatorInState("DaepokjinEnd"))
        {
            yield return false;
            if (time >= TIME_DAEPOKJIN_RUN)
            {
                break;
            }
            time += Time.deltaTime;
        }

        // 
        StopDaepokjin();
        yield break;
    }

    #endregion





    #region "영상뢰화" 행동을 정의합니다.
    /// <summary>
    /// 영상뢰화 행동 중이라면 참입니다.
    /// </summary>
    bool _doingRoihwa;
    /// <summary>
    /// 영상뢰화 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingRoihwa
    {
        get { return _doingRoihwa; }
        private set { _Animator.SetBool("DoingRoihwa", _doingRoihwa = value); }
    }

    /// <summary>
    /// 영상뢰화 코루틴입니다.
    /// </summary>
    Coroutine _coroutineRoihwa;

    /// <summary>
    /// 영상뢰화 행동을 시작합니다.
    /// </summary>
    public void DoRoihwa()
    {
        DoingRoihwa = true;

        // 영상뢰화 코루틴을 시작합니다.
        StartAction();
        _coroutineRoihwa = StartCoroutine(CoroutineRoihwa());
    }
    /// <summary>
    /// 영상뢰화 행동을 중지합니다.
    /// </summary>
    public void StopRoihwa()
    {
        DoingRoihwa = false;
        EndAction();
    }

    /// <summary>
    /// 영상뢰화 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineRoihwa()
    {
        //////////////////////////////////////////////////////////
        // 영상뢰화는 총 다섯 번의 번개를 내리찍는 강력한 전체공격기입니다.
        // 실제 게임에서는 모든 모션 직후에 번개가 내리꽂혔지만,
        // 플랫포머 스타일의 전투에서는 어울리지 않았기 때문에 약간 변경했습니다.
        // 린샹이 팔을 흔들면 번개가 내리치는 지점에 마크를 해주고,
        // 플레이어는 해당 지점을 번개가 내리치는 시점에 피하면 되는 식입니다.
        float time = 0;
        Transform[] attackTransforms = new Transform[5];
        yield return false;
        RunAction();

        // 팔 흔들기 1입니다.
        while (IsAnimatorInState("Roihwa1") == false)
        {
            yield return false;
        }
        SoundEffects[1].Play();

        // 팔 흔들기 2입니다.
        while (IsAnimatorInState("Roihwa4") == false)
        {
            yield return false;
        }
        SoundEffects[1].Play();

        // 번개가 내리칠 지점에 효과를 생성합니다.
        SoundEffects[2].Play();
        attackTransforms[0] = GetAttackTransform(_BattleManager.RoihwaPositions[0]);
        Instantiate(_effects[1], attackTransforms[0].position, attackTransforms[0].rotation);

        // 팔 흔들기 1입니다.
        while (IsAnimatorInState("Roihwa1 0") == false)
        {
            yield return false;
        }
        SoundEffects[1].Play();

        // 팔 흔들기 2입니다.
        while (IsAnimatorInState("Roihwa4 0") == false)
        {
            yield return false;
        }
        SoundEffects[1].Play();

        // 번개가 내리칠 지점에 효과를 생성합니다.
        SoundEffects[2].Play();
        attackTransforms[1] = GetAttackTransform(_BattleManager.RoihwaPositions[1]);
        Instantiate(_effects[1], attackTransforms[1].position, attackTransforms[1].rotation);

        // 발차기 1입니다.
        while (IsAnimatorInState("Roihwa6") == false)
        {
            yield return false;
        }
        SoundEffects[1].Play();

        // 번개가 내리칠 지점에 효과를 생성합니다.
        SoundEffects[2].Play();
        attackTransforms[2] = GetAttackTransform(_BattleManager.RoihwaPositions[2]);
        Instantiate(_effects[1], attackTransforms[2].position, attackTransforms[2].rotation);

        // 발차기 2입니다.
        while (IsAnimatorInState("Roihwa8") == false)
        {
            yield return false;
        }
        SoundEffects[1].Play();

        // 번개가 내리칠 지점에 효과를 생성합니다.
        SoundEffects[2].Play();
        attackTransforms[3] = GetAttackTransform(_BattleManager.RoihwaPositions[3]);
        Instantiate(_effects[1], attackTransforms[3].position, attackTransforms[3].rotation);

        // 발차기 3입니다.
        while (IsAnimatorInState("Roihwa10") == false)
        {
            yield return false;
        }
        SoundEffects[1].Play();

        // 번개가 내리칠 지점에 효과를 생성합니다.
        // 마지막 한 발만큼은 플레이어의 X 좌표를 가리키게 합시다.
        SoundEffects[2].Play();
        Vector3 attackPosition = new Vector3(_StageManager.GetCurrentPlayerPosition().x, _BattleManager.RoihwaPositions[4].y);
        attackTransforms[4] = GetAttackTransform(attackPosition);
        Instantiate(_effects[1], attackTransforms[4].position, attackTransforms[4].rotation);

        // 공중 회전입니다. 본격적인 공격이 시작됩니다.
        while (IsAnimatorInState("Roihwa13") == false)
        {
            yield return false;
        }
        Jump();
        time = 0;
        Velocity = Vector2.zero;
        Vector3 position = transform.position;
        float ay = _Groundable._jumpDecSize;
        float vy0 = _Groundable._jumpSpeed;
        float vy = vy0;
        bool[] lightningOccurred = { false, false, false, false, false };
        while (IsAnimatorInState("Roihwa13"))
        {
            yield return false;
            float dt = Time.deltaTime;

            // 
            if (time < TIME_ROIHWA_INTERVAL[0])
            {
                if (lightningOccurred[0] == false)
                {
                    MakeLightning(attackTransforms[0]);
                    lightningOccurred[0] = true;
                }
            }
            else if (time < TIME_ROIHWA_INTERVAL[1])
            {
                if (lightningOccurred[1] == false)
                {
                    MakeLightning(attackTransforms[1]);
                    lightningOccurred[1] = true;
                }
            }
            else if (time < TIME_ROIHWA_INTERVAL[2])
            {
                if (lightningOccurred[2] == false)
                {
                    MakeLightning(attackTransforms[2]);
                    lightningOccurred[2] = true;
                }
            }
            else if (time < TIME_ROIHWA_INTERVAL[3])
            {
                if (lightningOccurred[3] == false)
                {
                    MakeLightning(attackTransforms[3]);
                    lightningOccurred[3] = true;
                }
            }
            else if (time < TIME_ROIHWA_INTERVAL[4])
            {
                if (lightningOccurred[4] == false)
                {
                    MakeLightning(attackTransforms[4]);
                    lightningOccurred[4] = true;
                }
            }

            // 
            position = transform.position;

            // 
            time += dt;
            float ds = vy0 * dt - 0.5f * ay * dt * dt;
            position.y += ds;
            transform.position = position;

            // 
            vy0 -= ay;

            // 
            if (Landed && vy0 < 0)
            {
                break;
            }
            else if (ds <= 0)
            {
                Fall();
                LandBlocked = false;
            }
        }

        // 
        time = 0;
        while (IsAnimatorInState("Roihwa14"))
        {
            yield return false;
            if (time >= TIME_ROIHWA_END)
            {
                break;
            }
            time += Time.deltaTime;
        }

        // 
        StopRoihwa();
        _coroutineRoihwa = null;
        yield break;
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="startPosition"></param>
    /// <returns></returns>
    Transform GetAttackTransform(Vector3 startPosition)
    {
        Vector2 startPos = new Vector2(startPosition.x, startPosition.y);
        RaycastHit2D ray = Physics2D.Raycast(startPos, Vector2.down, 1000000, 1 << 19); // 19번은 TiledGeometry Layer입니다.

        ///Debug.DrawLine(startPosition, ray.point, Color.red, 10f);

        GameObject gameObject = new GameObject();
        gameObject.transform.position = new Vector3(ray.point.x, ray.point.y);
        return gameObject.transform;
    }
    /// <summary>
    /// 영상뢰화 번개를 생성합니다.
    /// </summary>
    /// <param name="position">영상뢰화 번개를 생성할 위치입니다.</param>
    void MakeLightning(Transform transform)
    {
        Instantiate(_effects[2], transform.position, transform.rotation);
        Destroy(transform.gameObject);
    }

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}