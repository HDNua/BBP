using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 린샹 영상뢰화 탄환입니다.
/// </summary>
public class EnemyRinshanRoihwaBulletUnit : EnemyBulletUnit
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 영상뢰화가 플레이어에게 대미지를 입히는 시간입니다.
    /// </summary>
    public float TIME_ROIHWA_DANGEROUS = 0.2f;

    #endregion



    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.

    #endregion





    #region Unity에서 접근 가능한 공용 객체를 정의합니다.

    #endregion





    #region 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 타이머입니다.
    /// </summary>
    float _time = 0;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 
        _time = 0;
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // 
        if (_time >= TIME_ROIHWA_DANGEROUS)
        {
            _damage = 0;
        }

        // 
        _time += Time.deltaTime;
    }

    #endregion





    #region Collider2D의 기본 메서드를 재정의합니다.
    /// <summary>
    /// 충돌체가 트리거 내부로 진입했습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected override void OnTriggerEnter2D(Collider2D other)
    {

    }
    /// <summary>
    /// 충돌체가 여전히 트리거 내부에 있습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected override void OnTriggerStay2D(Collider2D other)
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
                player.Hurt(Damage, transform);
            }

            // 영상뢰화는 플레이어를 때려도 사라지지 않는 공격입니다.
            ///Dead();
        }
    }

    #endregion





    #region EnemyScript의 메서드를 오버라이드합니다.
    /// <summary>
    /// 캐릭터에게 대미지를 입힙니다.
    /// </summary>
    /// <param name="damage">입힐 대미지의 양입니다.</param>
    /// <param name="hitTransform">타격체입니다.</param>
    public override void Hurt(int damage, Transform hitTransform)
    {
        base.Hurt(damage, hitTransform);
    }

    #endregion
}
