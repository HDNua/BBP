using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 린샹의 수경 탄환 유닛입니다.
/// </summary>
public class EnemyRinshanSukyeongBulletUnit : EnemyBulletUnit
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 영상뢰화가 플레이어에게 대미지를 입히는 시간입니다.
    /// </summary>
    public int MANA_SUKYEONG_FILL = 20;

    /// <summary>
    /// 
    /// </summary>
    public float TIME_BLINK = TIME_30FPS * 12;
    /// <summary>
    /// 
    /// </summary>
    public float TIME_BLINK_INTERVAL = TIME_30FPS;

    #endregion



    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public EnemyBossAtahoUnit _atahoUnit;

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
        _atahoUnit = HwanseBattleManager.Instance._atahoUnit;
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // 
        if (_time >= TIME_ALIVE)
        {
            Dead();
        }
        else if (_time >= TIME_BLINK)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = ((int)(_time / TIME_BLINK_INTERVAL) % 2) == 1 ? Color.white : Color.clear;
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
        if (other.CompareTag("PlayerAttack"))
        {
            GameObject pObject = other.gameObject;
            AttackScript playerAttack = pObject.GetComponent<AttackScript>();

            //
            Hurt(playerAttack.damage, playerAttack.transform);
            if (IsAlive() == false)
            {
                Dead();
                _atahoUnit.FillMana(MANA_SUKYEONG_FILL);
            }
        }
    }

    #endregion





    #region 기타 메서드를 재정의합니다.

    #endregion
}
