using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 적 보스 유닛입니다.
/// </summary>
public class EnemyBossUnit : EnemyUnit
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.

    #endregion





    #region Unity에서 접근 가능한 공용 객체를 정의합니다.
    /// <summary>
    /// 탄환 개체입니다.
    /// </summary>
    public EnemyBulletScript[] _bullets;
    /// <summary>
    /// 탄환 발사 지점입니다.
    /// </summary>
    public Transform[] _shotPosition;

    /// <summary>
    /// 캐릭터와 충돌했을 때 플레이어가 입을 기본 대미지입니다.
    /// </summary>
    public int _defaultDamage;

    /// <summary>
    /// 보스 페이즈 변수입니다.
    /// </summary>
    public int _phase = 0;

    #endregion





    #region 캐릭터의 운동 상태 필드를 정의합니다.


    #endregion





    #region 캐릭터의 상태 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 플레이어의 최대 체력을 확인합니다.
    /// </summary>
    public int _maxHealth = 40;
    /// <summary>
    /// 최대 체력입니다.
    /// </summary>
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    /// <summary>
    /// 플레이어의 체력이 가득 찼는지 확인합니다.
    /// </summary>
    /// <returns>체력이 가득 찼다면 true입니다.</returns>
    public bool IsHealthFull()
    {
        return (Health == MaxHealth);
    }



    /// <summary>
    /// 행동을 시작했다면 참입니다.
    /// </summary>
    bool _isActionStarted = false;
    /// <summary>
    /// 행동을 진행중이라면 참입니다.
    /// </summary>
    bool _isActionRunning = false;
    /// <summary>
    /// 행동이 종료되었다면 참입니다.
    /// </summary>
    bool _isActionEnded = false;
    /// <summary>
    /// 행동을 시작했다면 참입니다.
    /// </summary>
    public bool IsActionStarted
    {
        get { return _isActionStarted; }
        protected set { _isActionStarted = value; }
    }
    /// <summary>
    /// 행동을 진행중이라면 참입니다.
    /// </summary>
    public bool IsActionRunning
    {
        get { return _isActionRunning; }
        protected set { _isActionRunning = value; }
    }
    /// <summary>
    /// 행동이 종료되었다면 참입니다.
    /// </summary>
    public bool IsActionEnded
    {
        get { return _isActionEnded; }
        protected set { _isActionEnded = value; }
    }
    /// <summary>
    /// 행동을 시작합니다. 모든 행동 함수에서 코루틴 시작 직전에 호출하십시오.
    /// </summary>
    protected void StartAction()
    {
        IsActionStarted = true;
        IsActionRunning = false;
        IsActionEnded = false;
    }
    /// <summary>
    /// 행동을 진행 상태로 변경합니다.
    /// 가장 먼저 발견되는 IEnumerator의 직후에 호출하십시오.
    /// </summary>
    protected void RunAction()
    {
        IsActionStarted = false;
        IsActionRunning = true;
        IsActionEnded = false;
    }
    /// <summary>
    /// 행동을 종료합니다. 모든 행동 코루틴의 종료 직전에 호출하십시오.
    /// </summary>
    protected void EndAction()
    {
        IsActionStarted = false;
        IsActionRunning = false;
        IsActionEnded = true;
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();
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
        base.FixedUpdate();
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





    #region Collider2D의 기본 메서드를 재정의합니다.
    /// <summary>
    /// 충돌체가 트리거 내부로 진입했습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected virtual void OnTriggerEnter2D(Collider2D other)
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
    /// <summary>
    /// 충돌체가 여전히 트리거 내부에 있습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected virtual void OnTriggerStay2D(Collider2D other)
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
    /// <summary>
    /// 충돌체가 트리거 내부에서 나옵니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected virtual void OnTriggerExit2D(Collider2D other)
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
        BattleManager _battleManager = BattleManager.Instance;
        Transform enemyParent = _StageManager._enemyParent.transform;

        // 
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

        // 개체 대신 놓일 그림을 활성화합니다.
        Vector3 position = transform.position;

        // 
        BossDeadEffectScript effect;
        if (isEveryBossesDead)
        {
            effect = _battleManager._bossDeadEffects[0];
        }
        else
        {
            effect = _battleManager._bossDeadEffects[1];
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
        Destroy(gameObject);
    }

    #endregion





    #region EnemyUnit의 메서드를 오버라이드합니다.
    /// <summary>
    /// 플레이어가 체력을 회복합니다.
    /// </summary>
    public void Heal(int healStep)
    {
        Health += healStep;
        if (Health > MaxHealth)
            Health = MaxHealth;
    }

    #endregion





    #region 행동 메서드를 정의합니다.
    /// <summary>
    /// 플레이어를 공격할 수 있는 상태로 전환합니다.
    /// </summary>
    public void MakeAttackable()
    {
        _damage = _defaultDamage;
    }
    /// <summary>
    /// 플레이어를 공격할 수 없는 상태로 전환합니다.
    /// </summary>
    public void MakeUnattackable()
    {
        _damage = 0;
    }

    #endregion





    #region 보조 메서드를 정의합니다.

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}
