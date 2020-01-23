using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
/// <summary>
/// 
/// </summary>
public class Unit : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 캐릭터가 오른쪽을 보고 있다면 참입니다.
    /// </summary>
    public bool _facingRight;

    #endregion



    #region 캐릭터의 상태 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 캐릭터가 오른쪽을 보고 있다면 참입니다.
    /// </summary>
    public bool FacingRight
    {
        get { return _facingRight; }
        set { if (_facingRight != value) Flip(); }
    }

    #endregion



    #region 캐릭터의 운동 상태 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// X 좌표 값입니다.
    /// </summary>
    public float PosX
    {
        get { return transform.position.x; }
        set { transform.position = new Vector3(value, transform.position.y, transform.position.z); }
    }
    /// <summary>
    /// Y 좌표 값입니다.
    /// </summary>
    public float PosY
    {
        get { return transform.position.y; }
        set { transform.position = new Vector3(transform.position.x, value, transform.position.z); }
    }
    /// <summary>
    /// Z 좌표 값입니다.
    /// </summary>
    public float PosZ
    {
        get { return transform.position.z; }
        set { transform.position = new Vector3(transform.position.x, transform.position.y, value); }
    }

    #endregion



    #region 행동 메서드를 정의합니다.
    /// <summary>
    /// 방향을 바꿉니다.
    /// </summary>
    public void Flip()
    {
        if (_facingRight)
        {
            transform.localScale = new Vector3
                (-transform.localScale.x, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector3
                (-transform.localScale.x, transform.localScale.y);
        }
        _facingRight = !_facingRight;
    }

    #endregion
}