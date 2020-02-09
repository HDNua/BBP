using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 적 유닛입니다.
/// </summary>
public class EnemyUnit : Unit
{
    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        if (_alwaysInvencible)
        {
            Invencible = true;
        }
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

    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    protected override void LateUpdate()
    {
        // 색상표를 사용하는 개체인 경우 이 메서드를 오버라이드하고 다음 문장을 호출합니다.
        // UpdateColor();
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



    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 캐릭터를 소환합니다.
    /// </summary>
    public override void Appear()
    {

    }
    /// <summary>
    /// 캐릭터를 사라지게 합니다.
    /// </summary>
    public override void Disappear()
    {

    }

    /// <summary>
    /// 캐릭터에게 대미지를 입힙니다.
    /// </summary>
    /// <param name="damage">입힐 대미지의 양입니다.</param>
    public virtual void Hurt(int damage)
    {
        Health -= damage;
        IsDamaged = true;

        // 무적 상태 코루틴을 시작합니다.
        if (_coroutineInvencible != null)
            StopCoroutine(_coroutineInvencible);
        _coroutineInvencible = StartCoroutine(CoroutineInvencible(INVENCIBLE_TIME_LENGTH));
    }

    /// <summary>
    /// 자신의 위치에 아이템을 생성합니다.
    /// </summary>
    /// <param name="item">생성할 아이템입니다.</param>
    /// <returns>생성된 아이템을 반환합니다.</returns>
    protected ItemScript CreateItem(ItemScript item)
    {
        if (Random.Range(0, 100) < item.Probability)
        {
            // 아이템 객체를 생성합니다.
            ItemScript ret = Instantiate
                (item, transform.position, transform.rotation);

            // 속성을 업데이트합니다.
            ret.IsDropped = true;

            // 아이템을 반환합니다.
            return ret;
        }
        return null;
    }

    #endregion
}
