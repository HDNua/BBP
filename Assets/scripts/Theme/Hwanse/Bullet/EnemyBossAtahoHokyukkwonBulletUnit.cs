﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 아타호 호격권 탄환 유닛입니다.
/// </summary>
public class EnemyBossAtahoHokyukkwonBulletUnit : EnemyBulletUnit
{
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
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (_Collider.IsTouchingLayers(_whatIsWall))
        {
            // Dead();
        }
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

            // 맞는 순간 폭발합니다.
            Dead();
        }
    }

    #endregion





    #region EnemyScript의 메서드를 오버라이드합니다.
    /// <summary>
    /// 캐릭터가 사망합니다.
    /// </summary>
    public override void Dead()
    {
        // 폭발 효과를 생성하고 효과음을 재생합니다.
        CreateExplosion(transform.position);

        // 캐릭터가 사망합니다.
        base.Dead();
    }
    /// <summary>
    /// 탄환 발사 방향을 지정합니다.
    /// </summary>
    /// <param name="destination">현재 조작중인 플레이어의 위치입니다.</param>
    public override void MoveToDst(Vector3 destination)
    {
        Vector3 bulletPos = transform.position;
        Vector3 diff = destination - bulletPos;

        // 플레이어를 향해 발사합니다.
        _Rigidbody.velocity = diff.normalized * _movingSpeed;
    }

    #endregion
}