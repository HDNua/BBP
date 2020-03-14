using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;



[RequireComponent(typeof(Groundable))]
/// <summary>
/// 아타호 적 보스 유닛입니다.
/// </summary>
public class EnemyBossAtahoUnit : EnemyBossUnit
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 호격권 사용에 필요한 마나입니다.
    /// </summary>
    public int MANA_HOKYUKKWON = 10;
    /// <summary>
    /// 호포권 사용에 필요한 마나입니다.
    /// </summary>
    public int MANA_HOPOKWON = 15;
    /// <summary>
    /// 맹호광파참 사용에 필요한 마나입니다.
    /// </summary>
    public int MANA_GWANGPACHAM = 30;

    /// <summary>
    /// 숨고르기 시간입니다.
    /// </summary>
    public float TIME_WAIT = 0.4f;

    /// <summary>
    /// 팔을 한 번 돌릴 때 걸리는 시간입니다.
    /// </summary>
    public float TIME_SWING_ARM = 0.12f;
    /// <summary>
    /// 한 번 마실 때 걸리는 시간입니다.
    /// </summary>
    public float TIME_DRINK = 0.24f;
    /// <summary>
    /// 호격권을 수행하는 시간입니다.
    /// </summary>
    public float TIME_HOKYOKKWON_RUN = 1f;
    /// <summary>
    /// 호포권을 수행하는 시간입니다.
    /// </summary>
    public float TIME_HOPOKWON_RUN = 1f;
    /// <summary>
    /// 맹호광파참을 수행하는 시간입니다.
    /// </summary>
    public float TIME_GWANGPACHAM_RUN = 3f;

    /// <summary>
    /// 방어 행동을 수행하는 시간입니다.
    /// </summary>
    public float TIME_GUARD = 0.5f;

    /// <summary>
    /// 팀원을 호출하는 데 걸리는 시간입니다.
    /// </summary>
    public float TIME_READY_TO_CALL_TEAM = 0.9f;
    /// <summary>
    /// 팀원을 호출하는 데 걸리는 시간입니다.
    /// </summary>
    public float TIME_CALL_TEAM = 1.2f;

    #endregion



    #region 효과음을 정의합니다.
    /// <summary>
    /// 효과음 인덱스입니다.
    /// </summary>
    public enum SoundEffect
    {
        Land,
        TigerCry,
        Jump,
        Whoosh,
        Drink,
        Stairs,
        Recover,
        Jing,
        Runaway,
        Door,
        Stab,
        Anger,
        Panpare,
        _쉬쉭,
        _기절,
        _청천벽력,
        Cancel,
        Snoring,
        Critical,
        Hit1,
        Hit2,
        _팅,
        Accept,
        Miss,
        GigadeathFire,
        Charge,
    }
    /// <summary>
    /// 탄환 타입입니다.
    /// </summary>
    public enum Bullet
    {
        Hokyokkwon,
        Hopokwon,
        Gwangpacham
    }

    #endregion



    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// 착지할 수 있는 유닛입니다.
    /// </summary>
    Groundable _groundable;
    /// <summary>
    /// 환세 전투 관리자입니다.
    /// </summary>
    HwanseBattleManager _battleManager;

    #endregion





    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 팀원 리스트입니다. 아타호의 경우 린샹과 스마슈가 됩니다.
    /// </summary>
    public EnemyUnit[] _team;

    /// <summary>
    /// 캐릭터의 마나입니다.
    /// </summary>
    public int _mana = 40;
    /// <summary>
    /// 캐릭터의 최대 마나입니다.
    /// </summary>
    public int _maxMana = 40;
    /// <summary>
    /// 캐릭터의 레벨업에 따른 최대 마나 증가량입니다.
    /// </summary>
    public int _manaIncreaseStep = 20;
    /// <summary>
    /// 캐릭터의 경험치입니다.
    /// </summary>
    public int _exp = 0;
    /// <summary>
    /// 캐릭터의 최대 경험치입니다.
    /// </summary>
    public int _maxExp = 200;
    /// <summary>
    /// 캐릭터의 레벨업에 따른 최대 경험치 증가량입니다.
    /// </summary>
    public int _expIncreaseStep = 50;

    /// <summary>
    /// 이동할 위치 집합입니다.
    /// </summary>
    public Transform[] _positions;

    #endregion





    #region 캐릭터의 운동 상태 필드를 정의합니다.
    /// <summary>
    /// 방어 중이라면 참입니다.
    /// </summary>
    bool _guarding = false;
    /// <summary>
    /// 위치를 변경하는 중이라면 참입니다.
    /// </summary>
    bool _hopping = false;

    /// <summary>
    /// 호격권 중이라면 참입니다.
    /// </summary>
    bool _doingHokyokkwon = false;
    /// <summary>
    /// 호포권 중이라면 참입니다.
    /// </summary>
    bool _doingHopokwon = false;
    /// <summary>
    /// 맹호광파참 중이라면 참입니다.
    /// </summary>
    bool _doingGwangpacham = false;



    /// <summary>
    /// 마나를 회복하는 중이라면 참입니다.
    /// </summary>
    bool _drinkingMana = false;
    /// <summary>
    /// 팀원을 호출하는 중이라면 참입니다.
    /// </summary>
    bool _calling = false;



    /// <summary>
    /// 위치 전환 시에 얼마나 오랫동안 공중에 있을지를 나타냅니다.
    /// </summary>
    float _hopTime = 0.8f;
    /// <summary>
    /// 위치 전환 시에 얼마나 높이 점프할지를 나타냅니다.
    /// </summary>
    float _arcHeight = 5f;

    /// <summary>
    /// 착지가 막혀있다면 참입니다.
    /// </summary>
    public bool _landBlocked = false;

    /// <summary>
    /// 플레이어의 속도(RigidBody2D.velocity)입니다.
    /// </summary>
    public Vector2 _Velocity
    {
        get { return _groundable.Velocity; }
        set { _groundable.Velocity = value; }
    }

    #endregion





    #region 캐릭터의 상태 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 지상에 있다면 참입니다.
    /// </summary>
    bool Landed
    {
        get { return _groundable.Landed; }
        set { _groundable.Landed = value; }
    }
    /// <summary>
    /// 점프하고 있다면 참입니다.
    /// </summary>
    bool Jumping
    {
        get { return _groundable.Jumping; }
        set { _groundable.Jumping = value; }
    }
    /// <summary>
    /// 떨어지고 있다면 참입니다.
    /// </summary>
    bool Falling
    {
        get { return _groundable.Falling; }
        set { _groundable.Falling = value; }
    }
    /// <summary>
    /// 착지가 불가능하다면 참입니다.
    /// </summary>
    bool LandBlocked
    {
        get { return _landBlocked; }
        set { _landBlocked = value; }
    }

    /// <summary>
    /// 방어 중이라면 참입니다.
    /// </summary>
    bool Guarding
    {
        get { return _guarding; }
        set { _Animator.SetBool("Guarding", _guarding = value); }
    }
    /// <summary>
    /// 위치를 전환하는 중이라면 참입니다.
    /// </summary>
    public bool Hopping
    {
        get { return _hopping; }
        private set { _Animator.SetBool("Hopping", _hopping = value); }
    }

    /// <summary>
    /// 호격권 중이라면 참입니다.
    /// </summary>
    bool DoingHokyukkwon
    {
        get { return _doingHokyokkwon; }
        set { _Animator.SetBool("DoingHokyukkwon", _doingHokyokkwon = value); }
    }
    /// <summary>
    /// 호포권 중이라면 참입니다.
    /// </summary>
    bool DoingHopokwon
    {
        get { return _doingHopokwon; }
        set { _Animator.SetBool("DoingHopokwon", _doingHopokwon = value); }
    }
    /// <summary>
    /// 맹호광파참 중이라면 참입니다.
    /// </summary>
    bool DoingGwangpacham
    {
        get { return _doingGwangpacham; }
        set { _Animator.SetBool("DoingGwangpacham", _doingGwangpacham = value); }
    }

    /// <summary>
    /// 마나를 회복하고 있다면 참입니다.
    /// </summary>
    bool DrinkingMana
    {
        get { return _drinkingMana; }
        set { _Animator.SetBool("DrinkingMana", _drinkingMana = value); }
    }
    /// <summary>
    /// 팀원을 호출하는 중이라면 참입니다.
    /// </summary>
    bool Calling
    {
        get { return _calling; }
        set { _Animator.SetBool("Calling", _calling = value); }
    }

    /// <summary>
    /// 플레이어의 마나가 가득 찼는지 확인합니다.
    /// </summary>
    /// <returns>마나가 가득 찼다면 true입니다.</returns>
    public bool IsManaFull()
    {
        return (_mana == _maxMana);
    }
    /// <summary>
    /// 플레이어의 경험치가 가득 찼는지 확인합니다.
    /// </summary>
    /// <returns>경험치가 가득 찼다면 true입니다.</returns>
    public bool IsExpFull()
    {
        return (_exp == _maxExp);
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // 
        _groundable = GetComponent<Groundable>();
        _battleManager = (HwanseBattleManager)BattleManager.Instance;
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 떨어지는 상태로 변경합니다.
        Landed = false;
        Fall();
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
                    _groundable.UpdateVy();
                }
                else
                {
                    Land();
                }
            }
            else if (_Velocity.y <= 0)
            {
                Fall();
            }
            else
            {
                _groundable.UpdateVy();
            }
        }
        // 떨어지고 있다면
        else if (Falling)
        {
            if (Landed)
            {
                if (LandBlocked)
                {
                    _groundable.UpdateVy();
                }
                else
                {
                    Land();
                }
            }
            else
            {
                _groundable.UpdateVy();
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
        base.LateUpdate();
    }

    #endregion





    #region EnemyUnit의 메서드를 오버라이드합니다.
    /// <summary>
    /// 캐릭터가 사망합니다.
    /// </summary>
    public override void Dead()
    {
        BattleManager _battleManager = BattleManager.Instance;
        Transform enemyParent = _StageManager._enemyParent.transform;

        // 전투 상태를 확인합니다.
        bool isEveryBossesDead = _battleManager.DoesBattleEnd();

        // 모든 탄환을 제거합니다.
        if (isEveryBossesDead)
        {
            _StageManager.RequestDisableAllEnemy();
        }
        else
        {
            transform.SetParent(enemyParent);
        }

        // 효과를 생성하기 전에 방향을 설정합니다.
        LookPlayer();

        // 개체 대신 놓일 그림을 활성화합니다.
        Vector3 position = transform.position;
        BossDeadEffectScript effectPrefab;
        if (isEveryBossesDead)
        {
            effectPrefab = _battleManager._bossDeadEffects[0];
        }
        else
        {
            effectPrefab = _battleManager._bossDeadEffects[1];
        }
        BossDeadEffectScript effect = Instantiate(effectPrefab, position, transform.rotation);
        effect.gameObject.SetActive(true);
        effect.transform.position = position;

        // 
        if (!FacingRight)
        {
            effect.transform.localScale = new Vector3
                    (-effect.transform.localScale.x, effect.transform.localScale.x);
        }
        effect.gameObject.SetActive(true);

        // 플레이어의 움직임을 막습니다.
        if (isEveryBossesDead)
        {
            _StageManager.RequestBlockMoving();
        }

        // 개체 자신을 제거합니다.
        Destroy(gameObject);
    }
    /// <summary>
    /// 캐릭터에게 대미지를 입힙니다.
    /// </summary>
    /// <param name="damage">입힐 대미지의 양입니다.</param>
    /// <param name="hitTransform">타격체입니다.</param>
    public override void Hurt(int damage, Transform hitTransform)
    {
        base.Hurt(damage, hitTransform);

        // 맞은 대미지만큼 경험치를 올립니다.
        UpdateExp(damage);
    }

    #endregion





    #region 공용 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public bool _manaFillRequest = false;
    /// <summary>
    /// 
    /// </summary>
    public bool _manaFilling = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mana"></param>
    public void FillMana(int mana)
    {
        _manaFillRequest = true;
        _manaFilling = true;

        //
        _mana = Mathf.Clamp(_mana + mana, 0, _maxMana);
    }
    /// <summary>
    /// 
    /// </summary>
    public void EndFillMana()
    {
        _manaFilling = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetManaFillRequest(bool value)
    {
        _manaFillRequest = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool _manaUseRequest = false;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mana"></param>
    public void UseMana(int mana)
    {
        _manaUseRequest = true;

        //
        _mana = Mathf.Clamp(_mana - mana, 0, _maxMana);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetManaUseRequest(bool value)
    {
        _manaUseRequest = value;
    }


    /// <summary>
    /// 경험치 업데이트 요청이 들어왔습니다.
    /// </summary>
    public bool _expUpdateRequest = false;
    /// <summary>
    /// 경험치를 업데이트 합니다.
    /// </summary>
    /// <param damage="">피해량이 경험치가 됩니다.</param>
    public void UpdateExp(int damage)
    {
        _expUpdateRequest = true;

        // 페이즈 업데이트 시엔 마나를 모두 회복합니다.
        if (_exp + damage - _phase * _maxExp >= _maxExp)
        {
            // 최대 마나와 최대 경험치를 업데이트 합니다.
            _maxMana += _manaIncreaseStep;
            //_maxExp += _expIncreaseStep;

            // 마나를 모두 회복합니다.
            FillMana(_maxMana);
        }

        // 환세 전투에서 페이즈는 아타호의 경험치와 관계되어 있습니다.
        _exp = _exp + damage;
        _phase = _exp / _maxExp;
    }
    /// <summary>
    /// 경험치 업데이트 상태 플래그를 업데이트 합니다.
    /// </summary>
    /// <param name="value">경험치 상태입니다.</param>
    public void SetExpUpdateRequest(bool value)
    {
        _expUpdateRequest = value;
    }

    #endregion





    #region Groundable을 재정의합니다.
    /// <summary>
    /// 지상에 착륙합니다.
    /// </summary>
    void Land()
    {
        _groundable.Land();
        PlaySoundEffect(SoundEffect.Land);
    }
    /// <summary>
    /// 점프하게 합니다.
    /// </summary>
    void Jump()
    {
        /// _groundable.Jump();
        _groundable.Jumping = true;
    }
    /// <summary>
    /// 낙하합니다.
    /// </summary>
    void Fall()
    {
        _groundable.Fall();
    }
    /// <summary>
    /// 이동을 중지합니다.
    /// </summary>
    void StopMoving()
    {
        _Velocity = new Vector2(0, 0);

        // 상태를 정의합니다.
        _groundable.Moving = false;
    }

    #endregion





    #region "등장" 행동을 정의합니다.
    /// <summary>
    /// 등장 액션입니다.
    /// </summary>
    public override void Appear()
    {
        Fall();

        // 
        StartAction();
        StartCoroutine(CoroutineAppear());
    }
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineAppear()
    {
        // 지상에 떨어질 때까지 대기합니다.
        while (Landed == false)
        {
            yield return false;
            RunAction();
        }
        StopMoving();

        // 
        while (IsAnimatorInState("Ready"))
        {
            yield return false;
        }
        while (IsAnimatorInState("ReadyEnd"))
        {
            yield return false;
        }

        // 등장을 마칩니다.
        EndAction();
        yield break;
    }

    #endregion





    #region 무시 행동을 정의합니다.
    /// <summary>
    /// 무시 코루틴입니다.
    /// </summary>
    Coroutine _coroutineSkip;

    /// <summary>
    /// 할 행동이 없습니다. 행동 상태를 초기화하고 다음 행동을 진행합니다.
    /// </summary>
    public void SkipAction()
    {
        StartAction();
        _coroutineSkip = StartCoroutine(CoroutineSkip());
    }

    /// <summary>
    /// 무시 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineSkip()
    {
        yield return false;
        RunAction();

        // 
        yield return false;

        // 
        EndAction();
        yield break;
    }

    #endregion





    #region "Hop" 행동을 정의합니다.
    /// <summary>
    /// 현재 위치 좌표입니다.
    /// </summary>
    Vector3 _absHopStartPoint;
    /// <summary>
    /// 이동할 좌표 위치입니다.
    /// </summary>
    Vector3 _absHopEndPoint;
    /// <summary>
    /// 위치 전환 코루틴입니다.
    /// </summary>
    Coroutine _coroutineHop;

    /// <summary>
    /// 위치를 바꿉니다.
    /// </summary>
    /// <param name="newPosition">새 위치입니다.</param>
    public void HopTo(Transform newPosition)
    {
        Hopping = true;

        // 포물선 형태로 이동하기 위해 점프의 시작과 끝 지점을 구합니다.
        _absHopStartPoint = transform.position + transform.parent.transform.position;
        _absHopEndPoint = newPosition.position + newPosition.parent.transform.position;

        // 위치 전환 코루틴을 시작합니다.
        StartAction();
        _coroutineHop = StartCoroutine(CoroutineHop());
    }
    /// <summary>
    /// 위치 전환을 중지합니다.
    /// </summary>
    void StopHopping()
    {
        // 상태를 정의합니다.
        Hopping = false;

        // 코루틴 참조를 갱신합니다.
        if (_coroutineHop != null)
        {
            StopCoroutine(_coroutineHop);
            _coroutineHop = null;
        }
        EndAction();
    }

    /// <summary>
    /// 위치 전환 코루틴입니다.
    /// http://luminaryapps.com/blog/arcing-projectiles-in-unity/
    /// </summary>
    IEnumerator CoroutineHop()
    {
        Jump();
        LandBlocked = true;
        float x0 = _absHopStartPoint.x;
        float x1 = _absHopEndPoint.x;
        yield return false;
        RunAction();

        // 방향을 맞춥니다.
        if (x0 < x1)
        {
            if (FacingRight == false)
            {
                Flip();
            }
        }
        else
        {
            if (FacingRight)
            {
                Flip();
            }
        }

        // 
        while (IsAnimatorInState("JumpBeg1"))
        {
            yield return false;
        }
        PlaySoundEffect(SoundEffect.Jump);

        // 점프 하고 있는 중에는 코루틴을 그냥 진행합니다.
        while (Hopping)
        {
            float deltaTime = Time.deltaTime;

            // Compute the next position, with arc added in
            float dist = x1 - x0;
            float vx = Mathf.Abs(dist / _hopTime);

            float nextX = Mathf.MoveTowards(transform.position.x, x1, vx * deltaTime);
            float baseY = Mathf.Lerp(_absHopStartPoint.y, _absHopEndPoint.y, (nextX - x0) / dist);
            float arc = _arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);

            // Rotate to face the next position, and then move there
            if (transform.position.y > nextPos.y)
            {
                Fall();
            }
            transform.position = nextPos;

            // Do something when we reach the target
            if (nextPos == _absHopEndPoint)
            {
                LandBlocked = false;
                Land();
                break;
            }
            else if (nextPos.x == _absHopEndPoint.x)
            {
                LandBlocked = false;
                break;
            }

            //
            yield return false;
        }

        // 
        StopHopping();
        _coroutineHop = null;
        yield break;
    }

    #endregion





    #region "호격권" 행동을 정의합니다.
    /// <summary>
    /// 호격권을 사용합니다.
    /// </summary>
    public void DoHokyukkwon()
    {
        DoingHokyukkwon = true;

        // 공격 코루틴을 시작합니다.
        StartAction();
        _coroutineHokyokkwon = StartCoroutine(CoroutineHokyokkwon());
    }
    /// <summary>
    /// 호격권을 중지합니다.
    /// </summary>
    public void StopDoingHokyokkwon()
    {
        DoingHokyukkwon = false;
        EndAction();
    }
    /// <summary>
    /// 호격권이 사용 가능한지 확인합니다.
    /// </summary>
    /// <returns>사용 가능하다면 참입니다.</returns>
    public bool IsHokyukkwonAvailable()
    {
        return _mana >= MANA_HOPOKWON;
    }

    /// <summary>
    /// 호격권 코루틴입니다.
    /// </summary>
    Coroutine _coroutineHokyokkwon;
    /// <summary>
    /// 호격권 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineHokyokkwon()
    {
        // 움직임을 멈춥니다.
        LookPlayer();
        StopMoving();
        yield return false;
        RunAction();

        // 
        float time = 0;

        // 탄환을 발사합니다.
        while (IsAnimatorInState("HokyukkwonBeg"))
        {
            yield return false;
            time += Time.deltaTime;

            if (time > TIME_SWING_ARM)
            {
                PlaySoundEffect(SoundEffect.Whoosh);
                time = 0;
            }
        }

        // 
        time = 0;
        UseMana(MANA_HOKYUKKWON);

        // 
        Transform shotPosition = _shotPosition[1];
        Vector3 destination = _StageManager.GetCurrentPlayerPosition();
        destination.y = shotPosition.position.y; // transform.position.y;
        Shot(shotPosition, destination, Bullet.Hokyokkwon, 1, SoundEffect.TigerCry);
        while (IsAnimatorInState("HokyukkwonRun"))
        {
            yield return false;
            time += Time.deltaTime;

            if (time >= TIME_HOKYOKKWON_RUN)
            {
                break;
            }
        }

        // 공격을 끝냅니다.
        StopDoingHokyokkwon();
        _coroutineHokyokkwon = null;
        yield break;
    }

    #endregion





    #region "호포권" 행동을 정의합니다.
    /// <summary>
    /// 호포권 코루틴입니다.
    /// </summary>
    Coroutine _coroutineHopokwon;

    /// <summary>
    /// 호포권을 사용합니다.
    /// </summary>
    public void DoHopokwon()
    {
        DoingHopokwon = true;

        // 공격 코루틴을 시작합니다.
        StartAction();
        _coroutineHopokwon = StartCoroutine(CoroutineHopokwon());
    }
    /// <summary>
    /// 호포권을 중지합니다.
    /// </summary>
    public void StopDoingHopokwon()
    {
        DoingHopokwon = false;
        EndAction();
    }
    /// <summary>
    /// 호포권이 사용 가능한지 확인합니다.
    /// </summary>
    /// <returns>사용 가능하다면 참입니다.</returns>
    public bool IsHopokwonAvailable()
    {
        return _mana >= MANA_HOPOKWON;
    }

    /// <summary>
    /// 호포권 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineHopokwon()
    {
        // 움직임을 멈춥니다.
        LookPlayer();
        StopMoving();
        yield return false;
        RunAction();

        // 탄환을 발사하기 위해 기를 모읍니다.
        PlaySoundEffect(SoundEffect.Recover);
        while (IsAnimatorInState("HopokwonBeg"))
        {
            yield return false;
        }

        // 
        float time = 0;
        UseMana(MANA_HOPOKWON);
        Vector3 destination = _StageManager.GetCurrentPlayerPosition();
        Shot(_shotPosition[1], destination, Bullet.Hopokwon, 1, SoundEffect.GigadeathFire);
        while (IsAnimatorInState("HopokwonRun"))
        {
            yield return false;

            // 
            time += Time.deltaTime;
            if (time >= TIME_HOPOKWON_RUN)
            {
                break;
            }
        }

        // 공격을 끝냅니다.
        StopDoingHopokwon();
        _coroutineHopokwon = null;
        yield break;
    }

    #endregion





    #region "맹호광파참" 행동을 정의합니다.
    /// <summary>
    /// 맹호광파참 코루틴입니다.
    /// </summary>
    Coroutine _coroutineGwangpacham;

    /// <summary>
    /// 맹호광파참을 사용합니다.
    /// </summary>
    public void DoGwangpacham()
    {
        DoingGwangpacham = true;

        // 공격 코루틴을 시작합니다.
        StartAction();
        _coroutineGwangpacham = StartCoroutine(CoroutineGwangpacham());
    }
    /// <summary>
    /// 맹호광파참을 중지합니다.
    /// </summary>
    public void StopDoingGwangpacham()
    {
        DoingGwangpacham = false;
        EndAction();
    }
    /// <summary>
    /// 맹호광파참이 사용 가능한지 확인합니다.
    /// </summary>
    /// <returns>사용 가능하다면 참입니다.</returns>
    public bool IsGwangpachamAvailable()
    {
        return _mana >= MANA_GWANGPACHAM;
    }

    /// <summary>
    /// 맹호광파참 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineGwangpacham()
    {
        // 움직임을 멈춥니다.
        LookPlayer();
        StopMoving();
        yield return false;
        RunAction();

        // 탄환을 발사하기 위해 기를 모읍니다.
        PlaySoundEffect(SoundEffect.Charge);
        while (IsAnimatorInState("GwangpachamBeg"))
        {
            yield return false;
        }

        // 탄환을 발사합니다.
        float time = 0;
        UseMana(MANA_GWANGPACHAM);
        Vector3 destination = _StageManager.GetCurrentPlayerPosition();
        Shot(_shotPosition[1], destination, Bullet.Gwangpacham, 1, SoundEffect.GigadeathFire);
        while (IsAnimatorInState("GwangpachamRun"))
        {
            yield return false;

            // 
            time += Time.deltaTime;
            if (time >= TIME_GWANGPACHAM_RUN)
            {
                break;
            }
        }

        // 공격을 끝냅니다.
        StopDoingGwangpacham();
        _coroutineGwangpacham = null;
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





    #region "마나 드링크" 행동을 정의합니다.
    /// <summary>
    /// 마나를 회복합니다.
    /// </summary>
    public void DrinkMana()
    {
        DrinkingMana = true;

        // 행동을 시작합니다.
        StartAction();
        _coroutineDrinkMana = StartCoroutine(CoroutineDrinkMana());
    }
    /// <summary>
    /// 마나 회복을 중지합니다.
    /// </summary>
    public void StopDrinkingMana()
    {
        DrinkingMana = false;

        // 행동을 종료합니다.
        EndAction();
    }

    /// <summary>
    /// 마나 회복 코루틴입니다.
    /// </summary>
    Coroutine _coroutineDrinkMana;
    /// <summary>
    /// 마나 회복 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDrinkMana()
    {
        StopMoving();
        yield return false;
        RunAction();

        // 마나를 회복하는 간격은 최대 마력의 3/5입니다.
        // 한 번에 전체를 다 회복하지 않도록 디자인하였습니다.
        int manaFillStep = _maxMana / 5;

        // 마나 회복을 3회에 걸쳐 진행합니다.
        // 원래는 좀 더 간격이 없었는데, 있게 하도록 하고 싶었습니다.
        yield return new WaitForSeconds(TIME_DRINK);
        PlaySoundEffect(SoundEffect.Drink);
        FillMana(manaFillStep);
        yield return new WaitForSeconds(TIME_DRINK);
        PlaySoundEffect(SoundEffect.Drink);
        FillMana(manaFillStep);
        yield return new WaitForSeconds(TIME_DRINK);
        PlaySoundEffect(SoundEffect.Drink);
        FillMana(manaFillStep);
        yield return new WaitForSeconds(TIME_DRINK);

        //
        StopDrinkingMana();
        _coroutineDrinkMana = null;
        yield break;
    }

    #endregion





    #region "팀원 호출" 행동을 정의합니다.
    /// <summary>
    /// 린샹을 호출합니다.
    /// </summary>
    /// <param name="newUnitPosIndex">린샹 인덱스입니다.</param>
    public void CallRinshan(Transform newUnitPosition)
    {
        ///int rinshanIndex = 2 * _phase;
        int rinshanIndex = 0;
        CallTeamUnit(rinshanIndex, newUnitPosition);
    }
    /// <summary>
    /// 스마슈를 호출합니다.
    /// </summary>
    /// <param name="newUnitPosIndex">스마슈 인덱스입니다.</param>
    public void CallSmashu(Transform newUnitPosition)
    {
        ///int smashuIndex = 2 * _phase + 1;
        int smashuIndex = 1;
        CallTeamUnit(smashuIndex, newUnitPosition);
    }

    /// <summary>
    /// 팀원을 호출합니다.
    /// </summary>
    /// <param name="unitIndex">호출할 유닛의 인덱스입니다.</param>
    /// <param name="newUnitPosition">유닛을 생성할 위치입니다.</param>
    void CallTeamUnit(int unitIndex, Transform newUnitPosition)
    {
        // 상태를 정의합니다.
        Calling = true;

        // 행동을 시작합니다.
        StartAction();
        _coroutineCallTeamUnit = StartCoroutine(CoroutineCallTeamUnit(unitIndex, newUnitPosition));
    }
    /// <summary>
    /// 팀원 호출을 중지합니다.
    /// </summary>
    void StopCallingTeamUnit()
    {
        Calling = false;

        // 행동을 중단합니다.
        EndAction();
    }

    /// <summary>
    /// 팀원 호출 코루틴입니다.
    /// </summary>
    Coroutine _coroutineCallTeamUnit;
    /// <summary>
    /// 팀원 호출 코루틴입니다.
    /// </summary>
    /// <param name="unitIndex">호출할 유닛의 인덱스입니다.</param>
    /// <param name="newUnitPosition">유닛을 생성할 위치입니다.</param>
    IEnumerator CoroutineCallTeamUnit(int unitIndex, Transform newUnitPosition)
    {
        yield return false;
        RunAction();

        // 팀원 호출 AnimatorState로 진입할 수 있게 해주는 강제 대기 시간입니다.
        // (이 시간이 없으면 상태 천이 이전에 새 유닛이 생성될 수 있습니다.)
        float time = 0;
        while (time < TIME_READY_TO_CALL_TEAM)
        {
            time += Time.deltaTime;
            yield return false;
        }

        // 아타호가 팀원을 호출하는 준비 동작입니다.
        while (IsAnimatorInState("CallBeg"))
        {
            yield return false;
        }

        // 
        EnemyUnit newUnit = Instantiate(
            _team[unitIndex],
            newUnitPosition.position,
            newUnitPosition.rotation,
            _StageManager._enemyParent.transform
            );
        newUnit.gameObject.SetActive(true);
        _battleManager.UpdateTeam(newUnit, unitIndex % 2);

        // 아타호가 팀원을 호출하는 동작입니다.
        time = 0;
        while (IsAnimatorInState("CallTeam"))
        {
            yield return false;
            time += Time.deltaTime;
            if (time >= TIME_CALL_TEAM)
            {
                break;
            }
        }

        // 팀원 호출 상태를 끝냅니다.
        StopCallingTeamUnit();
        _coroutineCallTeamUnit = null;
        yield break;
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 효과음을 재생합니다.
    /// </summary>
    /// <param name="seIndex">재생할 효과음의 인덱스입니다.</param>
    public void PlaySoundEffect(SoundEffect seIndex)
    {
        SoundEffects[(int)seIndex].Play();
    }
    /// <summary>
    /// 탄환을 발사합니다.
    /// </summary>
    /// <param name="shotPosition">탄환을 발사할 위치입니다.</param>
    /// <param name="destination">탄환의 목적지입니다.</param>
    /// <param name="bulletIndex">탄환 인덱스입니다.</param>
    /// <param name="effectIndex">탄환 생성 효과 인덱스입니다.</param>
    /// <param name="seIndex">효과음 인덱스입니다.</param>
    void Shot(Transform shotPosition, Vector3 destination, Bullet bulletIndex, int effectIndex, SoundEffect seIndex)
    {
        // 
        LookPlayer();

        //
        SoundEffects[(int)seIndex].Play();
        GameObject effect = Instantiate
            (_effects[effectIndex], shotPosition.position, shotPosition.rotation);
        if (FacingRight)
        {
            Vector3 scale = effect.transform.localScale;
            effect.transform.localScale = new Vector3(-scale.x, scale.y);
        }

        // 
        EnemyBulletScript bullet = Instantiate
            (_bullets[(int)bulletIndex], shotPosition.position, shotPosition.rotation)
            as EnemyBulletScript;
        bullet.transform.parent = _StageManager._enemyParent.transform;

        // 
        bullet.FacingRight = FacingRight;
        bullet.MoveTo(destination);
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("지금은 사용하지 않습니다.")]
    /// <summary>
    /// 궁극기가 활성화되었다면 참입니다.
    /// </summary>
    bool UltimateEnabled { get; set; }

    #endregion
}
