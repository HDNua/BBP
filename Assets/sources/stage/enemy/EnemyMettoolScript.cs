﻿using UnityEngine;
using System;
using System.Collections;



/// <summary>
/// 멧토 적 캐릭터를 정의합니다.
/// </summary>
public class EnemyMettoolScript : EnemyScript
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    Rigidbody2D _rigidbody;

    #endregion










    #region Unity에서 접근 가능한 공용 객체를 정의합니다.
    /// <summary>
    /// 자신의 밑에 지면이 존재하는지 검사하기 위해 사용합니다.
    /// </summary>
    public Transform groundCheck;
    /// <summary>
    /// 자신이 진행하는 방향에 벽이 존재하는지 검사하기 위해 사용합니다.
    /// </summary>
    public Transform pushCheck;
    /// <summary>
    /// 무엇이 벽인지를 결정합니다. 기본값은 "Wall, MapBlock"입니다.
    /// </summary>
    public LayerMask whatIsWall;


    /// <summary>
    /// 
    /// </summary>
    public bool canJump;




    #endregion










    #region 캐릭터의 상태 필드 및 프로퍼티를 정의합니다.
    public float movingSpeed;

    bool facingRight = false;

    #endregion










    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(WalkAround());
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        RaycastHit2D pushRay = Physics2D.Raycast
            (pushCheck.position, direction, 0.1f, whatIsWall);
        if (pushRay)
        {
            if (facingRight)
            {
                MoveLeft();
            }
            else
            {
                MoveRight();
            }
        }
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










    #region EnemyScript의 메서드를 오버라이드 합니다.
    /// <summary>
    /// 캐릭터가 사망합니다.
    /// </summary>
    public override void Dead()
    {
        SoundEffects[0].Play();
        Instantiate(effects[0], transform.position, transform.rotation);
        base.Dead();
    }


    #endregion










    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 왼쪽으로 이동합니다.
    /// </summary>
    void MoveLeft()
    {
        if (facingRight)
            Flip();
        _rigidbody.velocity = new Vector2(-movingSpeed, 0);
    }
    /// <summary>
    /// 오른쪽으로 이동합니다.
    /// </summary>
    void MoveRight()
    {
        if (facingRight == false)
            Flip();
        _rigidbody.velocity = new Vector2(movingSpeed, 0);
    }
    /// <summary>
    /// 방향을 바꿉니다.
    /// </summary>
    void Flip()
    {
        if (facingRight)
        {
            _rigidbody.transform.localScale = new Vector3
                (-_rigidbody.transform.localScale.x, _rigidbody.transform.localScale.y);
        }
        else
        {
            _rigidbody.transform.localScale = new Vector3
                (-_rigidbody.transform.localScale.x, _rigidbody.transform.localScale.y);
        }
        facingRight = !facingRight;
    }
    /// <summary>
    /// 주변을 방황합니다.
    /// </summary>
    /// <returns>StartCoroutine 호출에 적합한 값을 반환합니다.</returns>
    IEnumerator WalkAround()
    {
        while (_health != 0)
        {
            int random = UnityEngine.Random.Range(0, 2);
            if (random == 1)
            {
                MoveLeft();
            }
            else
            {
                MoveRight();
            }
            yield return new WaitForSeconds(1);
        }
    }


    #endregion










    #region 구형 정의를 보관합니다.
    [Obsolete("OnTriggerStay2D로 이동했습니다.", true)]
    void _OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject pObject = other.gameObject;
            PlayerController player = pObject.GetComponent<PlayerController>();

            if (player.Invencible || player.IsDead)
            {

            }
            else
            {
                player.Hurt(Damage);
            }
        }
    }

    #endregion
}