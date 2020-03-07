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
    /// 임시; 린샹은 대타격을 쓰지 않죠.
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
    public bool IsAlmostDead
    {
        get { return _Animator.GetBool("IsAlmostDead"); }
        set { _Animator.SetBool("IsAlmostDead", value); }
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
            if (Landed == false)
            {
                Fall();
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
        while (Velocity.x > 0)
        {
            yield return false;

            // 
            //Velocity = new Vector2(v0 + (2 * dist / time) - ((2 * dist) / (time * time)) * passTime, 0);
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





    #region "기절" 행동을 정의합니다.
    /// <summary>
    /// 사망 코루틴입니다. 린샹은 실제로 사망하지 않고 그저 무적 상태로 일정 시간 방어만 합니다.
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





    #region "수경" 행동을 정의합니다.
    /// <summary>
    /// 대타격 행동 중이라면 참입니다.
    /// </summary>
    bool _doingSukyeong;
    /// <summary>
    /// 대타격 행동 중이라면 참입니다.
    /// </summary>
    public bool DoingSukyeong
    {
        get { return _doingSukyeong; }
        private set { _Animator.SetBool("DoingSukyeong", _doingSukyeong = value); }
    }

    /// <summary>
    /// 대타격 코루틴입니다.
    /// </summary>
    Coroutine _coroutineSukyeong;

    /// <summary>
    /// 대타격 행동을 시작합니다.
    /// </summary>
    public void DoSukyeong()
    {
        DoingSukyeong = true;

        // 대타격 코루틴을 시작합니다.
        StartAction();
        _coroutineSukyeong = StartCoroutine(CoroutineSukyeong());
    }
    /// <summary>
    /// 대타격 행동을 중지합니다.
    /// </summary>
    public void StopSukyeong()
    {
        DoingSukyeong = false;
        _damage = _defaultDamage;

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



        // 수경을 종료합니다.
        StopSukyeong();
        StopCoroutine(_coroutineSukyeong);
        _coroutineSukyeong = null;
        yield break;
    }

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

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("린샹이 쓰는 기술은 아닙니다만.. 스마슈에서 복사하느라 임시로 남았습니다.")]
    /// <summary>
    /// 등장 풀때기 효과입니다.
    /// </summary>
    GameObject EffectNinjaGrass
    {
        get { return _effects[0]; }
    }



    /// <summary>
    /// 
    /// </summary>
    bool Moving
    {
        get; set;
    }

    /// <summary>
    /// 왼쪽으로 이동합니다.
    /// </summary>
    protected void MoveLeft()
    {
        if (FacingRight)
            Flip();

        Moving = true;
        _Rigidbody.velocity = new Vector2(-_movingSpeedX, _Rigidbody.velocity.y);
    }
    /// <summary>
    /// 오른쪽으로 이동합니다.
    /// </summary>
    protected void MoveRight()
    {
        if (FacingRight == false)
            Flip();

        // 상태를 정의합니다.
        Moving = true;

        // 내용을 정의합니다.
        _Rigidbody.velocity = new Vector2(_movingSpeedX, _Rigidbody.velocity.y);
    }
    /// <summary>
    /// 위쪽으로 이동합니다.
    /// </summary>
    protected void MoveUp()
    {
        // 상태를 정의합니다.
        Moving = true;

        // 내용을 정의합니다.
        _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _movingSpeedY);
    }
    /// <summary>
    /// 아래쪽으로 이동합니다.
    /// </summary>
    protected void MoveDown()
    {
        // 상태를 정의합니다.
        Moving = true;

        // 내용을 정의합니다.
        _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, -_movingSpeedY);
    }
    /// <summary>
    /// 이동을 중지합니다.
    /// </summary>
    protected void StopMoving()
    {
        Velocity = Vector2.zero;

        // 상태를 정의합니다.
        Moving = false;
    }

    /// <summary>
    /// 특정 지점으로 이동합니다.
    /// </summary>
    /// <param name="t">이동할 지점입니다.</param>
    void MoveTo(Transform t)
    {
        // 사용할 변수를 선언합니다.
        Vector2 relativePos = t.position - transform.position;

        // 플레이어를 향해 수평 방향 전환합니다.
        if (relativePos.x < 0)
        {
            MoveLeft();
        }
        else if (relativePos.x > 0)
        {
            MoveRight();
        }
        // 플레이어를 향해 수직 방향 전환합니다.
        if (relativePos.y > 0)
        {
            MoveUp();
        }
        else if (relativePos.y < 0)
        {
            MoveDown();
        }
    }
    /// <summary>
    /// 플레이어를 향해 이동합니다.
    /// </summary>
    private void MoveToPlayer()
    {
        // 사용할 변수를 선언합니다.
        Vector3 playerPos = _StageManager.GetCurrentPlayerPosition();
        Vector2 relativePos = playerPos - transform.position;

        // 플레이어를 향해 수평 방향 전환합니다.
        if (relativePos.x < 0)
        {
            MoveLeft();
        }
        else if (relativePos.x > 0)
        {
            MoveRight();
        }
        // 플레이어를 향해 수직 방향 전환합니다.
        if (relativePos.y > 0)
        {
            MoveUp();
        }
        else if (relativePos.y < 0)
        {
            MoveDown();
        }
    }

    #endregion
}