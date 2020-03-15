using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// 탄환 자식 유닛입니다.
/// </summary>
public class BulletChildUnit : MonoBehaviour
{
    #region Collider2D의 기본 메서드를 재정의합니다.
    /// <summary>
    /// 충돌체가 트리거 내부로 진입했습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBulletUnit bulletUnit = transform.parent.gameObject.GetComponent<EnemyBulletUnit>();
        bulletUnit.RequestOnTriggerEnter2D(other);
    }
    /// <summary>
    /// 충돌체가 여전히 트리거 내부에 있습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected void OnTriggerStay2D(Collider2D other)
    {
        EnemyBulletUnit bulletUnit = transform.parent.gameObject.GetComponent<EnemyBulletUnit>();
        bulletUnit.RequestOnTriggerStay2D(other);
    }
    /// <summary>
    /// 충돌체가 트리거 내부에서 나옵니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    protected void OnTriggerExit2D(Collider2D other)
    {
        EnemyBulletUnit bulletUnit = transform.parent.gameObject.GetComponent<EnemyBulletUnit>();
        bulletUnit.RequestOnTriggerExit2D(other);
    }

    #endregion
}