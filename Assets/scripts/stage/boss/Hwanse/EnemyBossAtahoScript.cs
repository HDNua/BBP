using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 아타호를 정의합니다.
/// </summary>
public class EnemyBossAtahoScript : EnemyBossScript
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.

    #endregion





    #region Unity에서 접근 가능한 공용 객체를 정의합니다.
    /// <summary>
    /// 팀원 리스트입니다. 아타호의 경우 린샹과 스마슈가 됩니다.
    /// </summary>
    public EnemyScript[] _team;

    /// <summary>
    /// 탄환 개체입니다.
    /// </summary>
    public EnemyBulletScript[] _bullets;
    /// <summary>
    /// 탄환 발사 지점입니다.
    /// </summary>
    public Transform[] _shotPosition;

    /// <summary>
    /// 이동할 위치 집합입니다.
    /// </summary>
    public Transform[] _positions;

    /// <summary>
    /// X축 속력입니다.
    /// </summary>
    public float _movingSpeedX = 1;
    /// <summary>
    /// Y축 속력입니다.
    /// </summary>
    public float _movingSpeedY = 2;
    /// <summary>
    /// 궁극기 시전 시 X축 속력입니다.
    /// </summary>
    public float _ultimateSpeedX1 = 5;
    /// <summary>
    /// 궁극기 시전 시 Y축 속력입니다.
    /// </summary>
    public float _ultimateSpeedY1 = 5;

    /// <summary>
    /// 방패를 들어 막는 시간입니다.
    /// </summary>
    public float _guardTime = 0.5f;
    /// <summary>
    /// 추적 시간입니다.
    /// </summary>
    public float _followTime = 5f;
    /// <summary>
    /// 샷 간격입니다.
    /// </summary>
    public float _shotInterval = 2f;

    /// <summary>
    /// 궁극기 1의 샷 발사 회수 1입니다.
    /// </summary>
    public int _shotCount_1_1 = 4;
    /// <summary>
    /// 궁극기 1의 샷 발사 회수 2입니다.
    /// </summary>
    public int _shotCount_1_2 = 8;
    /// <summary>
    /// 궁극기 2의 샷 발사 회수 1입니다.
    /// </summary>
    public int _shotCount_2_1 = 4;
    /// <summary>
    /// 궁극기 1의 샷 발사 간격입니다.
    /// </summary>
    public float _ultimateInterval1 = 0.3f;
    /// <summary>
    /// 궁극기 2의 샷 발사 간격입니다.
    /// </summary>
    public float _ultimateInterval2 = 0.3f;

    /// <summary>
    /// 보스 페이즈 변수입니다.
    /// </summary>
    public int _phase = 0;

    #endregion





    #region 캐릭터의 운동 상태 필드를 정의합니다.
    /// <summary>
    /// 캐릭터가 공격 중이라면 참입니다.
    /// </summary>
    bool _attacking = false;
    /// <summary>
    /// 방패를 들어 막는 중이라면 참입니다.
    /// </summary>
    bool _guarding = false;
    /// <summary>
    /// 위치를 변경하는 중이라면 참입니다.
    /// </summary>
    bool _hopping = false;
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


    #endregion





    #region 캐릭터의 상태 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 캐릭터가 공격 중이라면 참입니다.
    /// </summary>
    bool Attacking
    {
        get { return _attacking; }
        set { _Animator.SetBool("Attacking", _attacking = value); }
    }
    /// <summary>
    /// 방패를 들어 막는 중이라면 참입니다.
    /// </summary>
    bool Guarding
    {
        get { return _guarding; }
        set { _Animator.SetBool("Guarding", _guarding = value); }
    }
    /// <summary>
    /// 위치를 전환하는 중이라면 참입니다.
    /// </summary>
    bool Hopping
    {
        get { return _hopping; }
        set { _Animator.SetBool("Hopping", _hopping = value); }
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
    /// 궁극기가 활성화되었다면 참입니다.
    /// </summary>
    bool UltimateEnabled { get; set; }
    /// <summary>
    /// 착지가 불가능하다면 참입니다.
    /// </summary>
    bool LandBlocked
    {
        get { return _landBlocked; }
        set { _landBlocked = value; }
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 컬러 팔레트를 설정합니다.
        DefaultPalette = EnemyColorPalette.BossAtahoPalette;

        // 떨어지는 상태로 변경합니다.
        Landed = false;
        Fall();

        // 아래로 내려오는 것으로 시작합니다.
        // MoveDown();
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // 등장이 끝나지 않았다면
        if (AppearEnded == false)
        {
            // 다음 문장을 수행하지 않습니다.
            return;
        }
        // 전투 중인 상태가 아니라면 
        else if (Fighting == false)
        {
            // 다음 문장을 수행하지 않습니다.
            return;
        }

        // 
        if (Attacking)
        {

        }

        /*
        if (UltimateEnabled == false && Health <= _dangerHealth)
        {
            // 
            StopMoving();
            StopAttack();
            StopGuarding();

            // 
            if (_coroutineAttack != null)
            {
                StopCoroutine(_coroutineAttack);
                _coroutineAttack = null;
            }
            if (_coroutineGuard != null)
            {
                StopCoroutine(_coroutineGuard);
                _coroutineGuard = null;
            }
            if (_coroutineFollow != null)
            {
                StopCoroutine(_coroutineFollow);
                _coroutineFollow = null;
            }

            // 
            UltimateEnabled = true;
            ReadyUltimate();
        }
        */
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
            else if (_Velocity.y <= 0)
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
        base.LateUpdate();

        // 색상을 업데이트합니다.
        UpdateColor();
    }

    #endregion





    #region Collider2D의 기본 메서드를 재정의합니다.
    /// <summary>
    /// 충돌체가 여전히 트리거 내부에 있습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        // 트리거가 발동한 상대 충돌체가 플레이어라면 대미지를 입힙니다.
        if (other.CompareTag("Player"))
        {
            GameObject pObject = other.gameObject;
            PlayerController player = pObject.GetComponent<PlayerController>();


            // 플레이어가 무적 상태이거나 죽었다면
            if (player.Invencible || player.IsDead)
            {
                // 아무 것도 하지 않습니다.

            }
            // 그 외의 경우
            else
            {
                // 플레이어에게 대미지를 입힙니다.
                player.Hurt(Damage);
            }
        }
    }

    #endregion





    #region EnemyScript의 메서드를 오버라이드합니다.
    /// <summary>
    /// 캐릭터가 사망합니다.
    /// </summary>
    public override void Dead()
    {
        BossBattleManager _bossBattleManager = BossBattleManager.Instance;
        Transform enemyParent = _StageManager._enemyParent.transform;

        // 
        bool isEveryBossesDead = _bossBattleManager.IsEveryBossesDead();

        // 모든 탄환을 제거합니다.
        if (isEveryBossesDead)
        {
            _StageManager.RequestDisableAllEnemy();
        }
        else
        {
            transform.SetParent(enemyParent);
        }

        // 개체 대신 놓일 그림을 활성화합니다.
        Vector3 position = transform.position;

        // 
        /// BossDeadEffectScript effect = _bossBattleManager._bossDeadEffect;
        BossDeadEffectScript effect;
        if (isEveryBossesDead)
        {
            effect = _bossBattleManager._lastBossDeadEffect;
        }
        else
        {
            effect = _bossBattleManager._bossDeadEffect;
        }
        // 
        Instantiate(effect, position, transform.rotation)
            .gameObject.SetActive(true);
        effect.transform.position = position;
        if (FacingRight)
            effect.transform.localScale = new Vector3
                (-effect.transform.localScale.x, effect.transform.localScale.x);

        effect.gameObject.SetActive(true);

        // 플레이어의 움직임을 막습니다.
        if (isEveryBossesDead)
        {
            _StageManager.RequestBlockMoving();
        }

        // 개체 자신을 제거합니다.
        RequestDestroy();
    }

    #endregion





    #region EnemyBossScript의 메서드를 오버라이드합니다.
    /// <summary>
    /// 지상에 착륙합니다.
    /// </summary>
    protected override void Land()
    {
        base.Land();
        SoundEffects[0].Play();
    }
    /// <summary>
    /// 등장 액션입니다.
    /// </summary>
    public override void Appear()
    {
        Fall();

        // 
        StartCoroutine(CoroutineAppear());
    }
    /// <summary>
    /// 점프하게 합니다.
    /// </summary>
    protected override void Jump()
    {
        base.Jump();
        SoundEffects[2].Play();
    }
    /// <summary>
    /// 낙하합니다.
    /// </summary>
    protected override void Fall()
    {
        base.Fall();
    }

    #endregion





    #region 행동 메서드를 정의합니다.
    /// <summary>
    /// 왼쪽으로 이동합니다.
    /// </summary>
    protected override void MoveLeft()
    {
        if (FacingRight)
            Flip();

        Moving = true;
        _Rigidbody.velocity = new Vector2(-_movingSpeedX, _Rigidbody.velocity.y);
    }
    /// <summary>
    /// 오른쪽으로 이동합니다.
    /// </summary>
    protected override void MoveRight()
    {
        if (FacingRight == false)
            Flip();

        Moving = true;
        _Rigidbody.velocity = new Vector2(_movingSpeedX, _Rigidbody.velocity.y);
    }
    /// <summary>
    /// 위쪽으로 이동합니다.
    /// </summary>
    protected void MoveUp()
    {
        Moving = true;

        _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _movingSpeedY);
    }
    /// <summary>
    /// 아래쪽으로 이동합니다.
    /// </summary>
    protected void MoveDown()
    {
        Moving = true;

        _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, -_movingSpeedY);
    }
    /// <summary>
    /// 이동을 중지합니다.
    /// </summary>
    protected override void StopMoving()
    {
        _Velocity = new Vector2(0, 0);

        // 
        Moving = false;
    }

    /// <summary>
    /// 공격합니다.
    /// </summary>
    void Attack()
    {
        Attacking = true;

        // 공격 코루틴을 시작합니다.
        _coroutineAttack = StartCoroutine(CoroutineAttack());
    }
    /// <summary>
    /// 공격을 중지합니다.
    /// </summary>
    void StopAttack()
    {
        Attacking = false;
    }

    /// <summary>
    /// 막기 1 행동입니다.
    /// </summary>
    void Guard1()
    {
        Guarding = true;

        // 막기 코루틴을 시작합니다.
        _coroutineGuard = StartCoroutine(CoroutineGuard1());
    }
    /// <summary>
    /// 막기 2 행동입니다.
    /// </summary>
    void Guard2()
    {
        Guarding = true;

        // 막기 코루틴을 시작합니다.
        _coroutineGuard = StartCoroutine(CoroutineGuard2());
    }
    /// <summary>
    /// 막기를 중지합니다.
    /// </summary>
    void StopGuarding()
    {
        Guarding = false;
    }


    /// <summary>
    /// 현재 위치 좌표입니다.
    /// </summary>
    Vector3 _absHopStartPoint;
    /// <summary>
    /// 이동할 좌표 위치입니다.
    /// </summary>
    Vector3 _absHopEndPoint;
    /// <summary>
    /// 위치를 바꿉니다.
    /// </summary>
    /// <param name="newPosition">새 위치입니다.</param>
    void HopTo(Transform newPosition)
    {
        Hopping = true;

        // 
        _absHopStartPoint = transform.position + transform.parent.transform.position;
        _absHopEndPoint = newPosition.position + newPosition.parent.transform.position;

        // 위치 전환 코루틴을 시작합니다.
        _coroutineHop = StartCoroutine(CoroutineHop());
    }
    /// <summary>
    /// 위치 전환을 중지합니다.
    /// </summary>
    void StopHopping()
    {
        // 
        Hopping = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_enemy"></param>
    void Call(EnemyScript _enemy)
    {
        Calling = true;

        // 
        _coroutineCall = StartCoroutine(CoroutineCall());
    }
    /// <summary>
    /// 
    /// </summary>
    void StopCalling()
    {
        Calling = false;
    }

    /// <summary>
    /// 플레이어를 추적합니다.
    /// </summary>
    void Follow()
    {
        Guarding = false;
        Attacking = false;

        // 추적 코루틴을 시작합니다.
        _coroutineFollow = StartCoroutine(CoroutineFollow());
    }
    /// <summary>
    /// 추적을 중지합니다.
    /// </summary>
    void StopFollowing()
    {
        StopMoving();
    }
    /// <summary>
    /// 방어 상태로 플레이어를 추적합니다.
    /// </summary>
    void GuardFollow()
    {
        Guarding = true;
        Attacking = false;

        // 가드를 활성화합니다.
        // _guard.gameObject.SetActive(true);

        // 추적 코루틴을 시작합니다.
        _coroutineFollow = StartCoroutine(CoroutineFollow());
    }
    /// <summary>
    /// 추적을 중지합니다.
    /// </summary>
    void StopGuardFollowing()
    {
        StopGuarding();
        StopMoving();
    }

    /// <summary>
    /// 궁극기를 준비합니다.
    /// </summary>
    void ReadyUltimate()
    {
        StartCoroutine(CoroutineReadyUltimate());
    }
    /// <summary>
    /// 궁극기 1을 시전합니다.
    /// </summary>
    void Ultimate1()
    {
        Guarding = false;

        StartCoroutine(CoroutineUltimate1());
    }
    /// <summary>
    /// 궁극기 2를 시전합니다.
    /// </summary>
    void Ultimate2()
    {
        StartCoroutine(CoroutineUltimate2());
    }

    /// <summary>
    /// 전투 시작 액션입니다.
    /// </summary>
    public override void Fight()
    {
        base.Fight();

        // 
        Attack();
    }

    #endregion





    #region 보조 메서드를 정의합니다.
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
    /// <summary>
    /// 탄환을 발사합니다.
    /// </summary>
    /// <param name="shotPosition">탄환을 발사할 위치입니다.</param>
    public void Shot(Transform shotPosition)
    {
        SoundEffects[1].Play();
        GameObject effect = Instantiate(effects[1], shotPosition.position, shotPosition.rotation);
        if (FacingRight)
        {
            Vector3 scale = effect.transform.localScale;
            effect.transform.localScale = new Vector3(-scale.x, scale.y);
        }

        // 
        EnemyBulletScript bullet = Instantiate
            (_bullets[0], shotPosition.position, shotPosition.rotation)
            as EnemyBulletScript;
        bullet.transform.parent = _StageManager._enemyParent.transform;

        // 
        bullet.FacingRight = FacingRight;
        bullet.MoveTo(_StageManager.GetCurrentPlayerPosition());
    }
    /// <summary>
    /// 탄환을 발사합니다.
    /// </summary>
    /// <param name="shotPosition">탄환을 발사할 위치입니다.</param>
    /// <param name="destination">탄환의 목적지입니다.</param>
    public void Shot(Transform shotPosition, Vector3 destination)
    {
        SoundEffects[1].Play();
        GameObject effect = Instantiate(effects[1], shotPosition.position, shotPosition.rotation);
        if (FacingRight)
        {
            Vector3 scale = effect.transform.localScale;
            effect.transform.localScale = new Vector3(-scale.x, scale.y);
        }

        // 
        EnemyBulletScript bullet = Instantiate
            (_bullets[0], shotPosition.position, shotPosition.rotation)
            as EnemyBulletScript;
        bullet.transform.parent = _StageManager._enemyParent.transform;

        // 
        bullet.FacingRight = FacingRight;
        bullet.MoveTo(destination);
    }

    /// <summary>
    /// 플레이어를 향해 탄환을 발사합니다.
    /// </summary>
    public void ShotToPlayer()
    {
        // 사용할 변수를 선언합니다.
        Vector3 playerPos = _StageManager.GetCurrentPlayerPosition();
        Vector2 relativePos = playerPos - transform.position;

        // 플레이어를 향해 수평 방향 전환합니다.
        if (relativePos.x < 0 && FacingRight)
        {
            /// MoveLeft();
            Flip();
        }
        else if (relativePos.x > 0 && FacingRight == false)
        {
            /// MoveRight();
            Flip();
        }

        // 탄환을 발사합니다.
        Shot(_shotPosition[1]);
    }



    /// <summary>
    /// 다음 뛸 지점의 집합을 반환합니다.
    /// </summary>
    /// <returns>다음 뛸 지점의 집합을 반환합니다.</returns>
    int[] GetHopPositionArray()
    {
        float[] diffs =
        {
            Vector3.Distance(transform.position, _positions[0].position),
            Vector3.Distance(transform.position, _positions[1].position),
            Vector3.Distance(transform.position, _positions[2].position),
            Vector3.Distance(transform.position, _positions[3].position),
            Vector3.Distance(transform.position, _positions[4].position),
            Vector3.Distance(transform.position, _positions[5].position),
            Vector3.Distance(transform.position, _positions[6].position),
        };

        // 
        float minDist = Mathf.Min(diffs);

        // 
        int[] hopPositionArray;
        if (minDist == diffs[0])
        {
            hopPositionArray = new int[] { 1, 2 };
        }
        else if (minDist == diffs[1])
        {
            hopPositionArray = new int[] { 0, 3 };
        }
        else if (minDist == diffs[2])
        {
            hopPositionArray = new int[] { 0, 3, 4 };
        }
        else if (minDist == diffs[3])
        {
            hopPositionArray = new int[] { 1, 2, 4, 5 };
        }
        else if (minDist == diffs[4])
        {
            hopPositionArray = new int[] { 2, 3, 6 };
        }
        else if (minDist == diffs[5])
        {
            hopPositionArray = new int[] { 3, 6 };
        }
        else if (minDist == diffs[6])
        {
            hopPositionArray = new int[] { 4, 5 };
        }
        else
        {
            hopPositionArray = new int[] { 1, 2 };
        }

        // 
        return hopPositionArray;
    }



    /// <summary>
    /// 막기1 다음 액션을 수행합니다.
    /// </summary>
    void PerformActionAfterGuard1()
    {
        // 
        int[] hopPositionArray = GetHopPositionArray();
        int newPositionIndex = hopPositionArray[Random.Range(0, hopPositionArray.Length)];
        Transform newPosition = _positions[newPositionIndex];
        HopTo(newPosition);
    }
    /// <summary>
    /// 공격 다음 액션을 수행합니다.
    /// </summary>
    void PerformActionAfterAttack()
    {
        Guard1();
    }
    /// <summary>
    /// 위치 전환 다음 액션을 수행합니다.
    /// </summary>
    void PerformActionAfterHop()
    {
        ///Attack();
        if (_phase == 0)
        {
            EnemyScript enemy = _team[Random.Range(0, 2)];
            Call(enemy);
        }
        else if (_phase == 1)
        {
            EnemyScript enemy = _team[Random.Range(2, 4)];
            Call(enemy);
        }
        else
        {
            EnemyScript enemy = _team[Random.Range(4, 6)];
            Call(enemy);
        }
    }
    /// <summary>
    /// 팀 호출 다음 액션을 수행합니다.
    /// </summary>
    void PerformActionAfterCall()
    {
        Guard2();
    }
    /// <summary>
    /// 막기2 행동 다음 액션을 수행합니다.
    /// </summary>
    void PerformActionAfterGuard2()
    {
        Attack();
    }

    #endregion





    #region 코루틴 메서드를 정의합니다.
    /// <summary>
    /// 공격 코루틴입니다.
    /// </summary>
    Coroutine _coroutineAttack;
    /// <summary>
    /// 방어 코루틴입니다.
    /// </summary>
    Coroutine _coroutineGuard;
    /// <summary>
    /// 위치 전환 코루틴입니다.
    /// </summary>
    Coroutine _coroutineHop;
    /// <summary>
    /// 추적 코루틴입니다.
    /// </summary>
    Coroutine _coroutineFollow;
    /// <summary>
    /// 호출 코루틴입니다.
    /// </summary>
    Coroutine _coroutineCall;



    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineAppear()
    {
        // 지상에 떨어질 때까지 대기합니다.
        while (Landed == false)
        {
            yield return false;
        }
        StopMoving();

        // 등장을 마칩니다.
        AppearEnded = true;
        yield break;
    }
    /// <summary>
    /// 공격 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineAttack()
    {
        // 움직임을 멈춥니다.
        StopMoving();

        // 탄환을 발사합니다.
        if (_phase == 0)
        {
            while (IsAnimatorInState("P1_02_HokyukkwonRun") == false)
            {
                yield return false;
            }
            ShotToPlayer();
        }
        else if (_phase == 1)
        {
            while (IsAnimatorInState("P2_02_HopokwonRun") == false)
            {
                yield return false;
            }
            ShotToPlayer();
        }
        else
        {
            while (IsAnimatorInState("BossAtahoShot") == false)
            {
                yield return false;
            }
            ShotToPlayer();
        }

        // 공격을 끝냅니다.
        Attacking = false;
        PerformActionAfterAttack();
        _coroutineAttack = null;
        yield break;
    }
    /// <summary>
    /// 막기1 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineGuard1()
    {
        float time = 0;
        while (time < _guardTime)
        {
            time += Time.deltaTime;
            yield return false;
        }

        // 막기 상태를 끝냅니다.
        StopGuarding();
        PerformActionAfterGuard1();
        _coroutineGuard = null;
        yield break;
    }
    /// <summary>
    /// 막기2 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineGuard2()
    {
        float time = 0;
        while (time < _guardTime)
        {
            yield return false;
            time += Time.deltaTime;
        }

        // 막기 상태를 끝냅니다.
        StopGuarding();
        PerformActionAfterGuard2();
        _coroutineGuard = null;
        yield break;
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

        // 
        float posY = transform.position.y;

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

            // 
            float diffX = nextX - transform.position.x;
            float diffY = baseY + arc - transform.position.y;

            // Rotate to face the next position, and then move there
            ///_Velocity = new Vector2(diffX / deltaTime, diffY / deltaTime);
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
        StopFalling();
        StopHopping();
        PerformActionAfterHop();
        _coroutineHop = null;
        yield break;
    }
    /// <summary>
    /// 팀원 호출 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineCall()
    {
        float time = 0;
        while (time < _followTime)
        {
            time += Time.deltaTime;
            yield return false;
        }

        // 팀원 호출 상태를 끝냅니다.
        StopCalling();
        PerformActionAfterCall();
        _coroutineCall = null;
        yield break;
    }


    /// <summary>
    /// 추적 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineFollow()
    {
        float time = 0;
        while (time < _followTime)
        {
            MoveToPlayer();
            time += Time.deltaTime;
            yield return false;
        }

        // 막기 상태를 끝냅니다.
        StopFollowing();
        PerformActionAfterFollow();
        _coroutineFollow = null;
        yield break;
    }

    /// <summary>
    /// 궁극기를 준비합니다.
    /// </summary>
    IEnumerator CoroutineReadyUltimate()
    {
        float originalSpeedX = _movingSpeedX;
        float originalSpeedY = _movingSpeedY;

        _movingSpeedX = _ultimateSpeedX1;
        _movingSpeedY = _ultimateSpeedY1;

        // 
        int newPositionIndex = Random.Range(0, _positions.Length);
        Transform newPosition = _positions[newPositionIndex];

        // 
        MoveTo(newPosition);
        while (true)
        {
            // 
            Vector3 newPos = transform.position;
            Vector3 dstPos = newPosition.position;

            // 
            if (newPos.x > dstPos.x)
                newPos.x = dstPos.x;
            if (newPos.y < dstPos.y)
                newPos.y = dstPos.y;
            transform.position = newPos;

            // 
            if (Vector3.Distance(newPos, dstPos) < 0.1f)
            {
                break;
            }

            // 
            yield return false;
        }
        StopMoving();
        StopGuarding();

        // 
        _movingSpeedX = originalSpeedX;
        _movingSpeedY = originalSpeedY;
        Ultimate1();
        yield break;
    }
    /// <summary>
    /// 궁극기 1을 시전합니다.
    /// </summary>
    private IEnumerator CoroutineUltimate1()
    {
        float originalSpeedX = _movingSpeedX;
        float originalSpeedY = _movingSpeedY;

        // 
        _movingSpeedX = _ultimateSpeedX1;
        _movingSpeedY = _ultimateSpeedY1;
        if (FacingRight)
            Flip();

        // 
        MoveUp();
        for (int i = 0; i < _shotCount_1_1; ++i)
        {
            Shot(_shotPosition[1], transform.position - new Vector3(100, 100));
            yield return new WaitForSeconds(_ultimateInterval1);
        }
        StopMoving();

        // 
        MoveLeft();
        for (int i = 0; i < _shotCount_1_2; ++i)
        {
            Shot(_shotPosition[1], transform.position - new Vector3(100, 100));
            yield return new WaitForSeconds(_ultimateInterval1);
        }
        StopMoving();

        // 궁극기를 끝냅니다.
        _movingSpeedX = originalSpeedX;
        _movingSpeedY = originalSpeedY;
        Ultimate2();
        yield break;
    }
    /// <summary>
    /// 궁극기 2를 시전합니다.
    /// </summary>
    private IEnumerator CoroutineUltimate2()
    {
        float originalSpeedX = _movingSpeedX;
        float originalSpeedY = _movingSpeedY;

        // 
        _movingSpeedX = _ultimateSpeedX1;
        _movingSpeedY = _ultimateSpeedY1;

        Flip();
        MoveDown();
        for (int i = 0; i < _shotCount_2_1; ++i)
        {
            Shot(_shotPosition[1], transform.position + new Vector3(100, 0));
            yield return new WaitForSeconds(_ultimateInterval1);
        }
        StopMoving();

        // 궁극기를 끝냅니다.
        _movingSpeedX = originalSpeedX;
        _movingSpeedY = originalSpeedY;
        PerformActionAfterUltimate();
        yield break;
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("RXPB 인트로 보스 만들 때 쓰던 거라 여기랑 안 맞습니다.")]
    /// <summary>
    /// 방패 개체입니다.
    /// </summary>
    public EnemyScript _guard;

    [Obsolete("RXPB 인트로 보스 만들 때 쓰던 거라 여기랑 안 맞습니다.")]
    /// <summary>
    /// 추적 다음 액션을 수행합니다.
    /// </summary>
    void PerformActionAfterFollow()
    {
        if (UltimateEnabled)
        {
            ReadyUltimate();
        }
        else
        {
            Guard1();
        }
    }
    [Obsolete("RXPB 인트로 보스 만들 때 쓰던 거라 여기랑 안 맞습니다.")]
    /// <summary>
    /// 궁극기를 시전한 다음 액션을 수행합니다.
    /// </summary>
    void PerformActionAfterUltimate()
    {
        Guard1();
    }

    #endregion
}
