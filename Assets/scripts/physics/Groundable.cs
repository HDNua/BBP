using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Unit))]
/// <summary>
/// 지상에 착지할 수 있는 유닛입니다.
/// </summary>
public class Groundable : MonoBehaviour
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// Rigidbody2D 요소를 가져옵니다.
    /// </summary>
    protected Rigidbody2D _Rigidbody
    {
        get { return GetComponent<Rigidbody2D>(); }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Collider2D _Collider
    {
        get { return GetComponent<Collider2D>(); }
    }
    /// <summary>
    /// Animator 요소를 가져옵니다.
    /// </summary>
    protected Animator _Animator
    {
        get { return GetComponent<Animator>(); }
    }
    /// <summary>
    /// SpriteRenderer 요소를 가져옵니다.
    /// </summary>
    protected SpriteRenderer _Renderer
    {
        get { return GetComponent<SpriteRenderer>(); }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Unit _Unit
    {
        get { return GetComponent<Unit>(); }
    }

    #endregion





    #region Unity에서 접근 가능한 공용 객체를 정의합니다.
    /// <summary>
    /// 전방 지형 검사를 위한 위치 객체입니다.
    /// </summary>
    public Transform _groundCheckFront;
    /// <summary>
    /// 후방 지형 검사를 위한 위치 객체입니다.
    /// </summary>
    public Transform _groundCheckBack;
    /// <summary>
    /// 무엇이 땅인지를 결정합니다. 기본값은 "Ground, TiledGeometry"입니다.
    /// </summary>
    public LayerMask _whatIsGround;
    /// <summary>
    /// 지형 검사 범위를 표현하는 실수입니다.
    /// </summary>
    public float _groundCheckRadius = 0.1f;

    /// <summary>
    /// 자신이 진행하는 방향에 벽이 존재하는지 검사하기 위해 사용합니다.
    /// </summary>
    public Transform _pushCheck;
    /// <summary>
    /// 무엇이 벽인지를 결정합니다. 기본값은 "Wall, MapBlock"입니다.
    /// </summary>
    public LayerMask _whatIsWall;

    /// <summary>
    /// 걷는 속도입니다.
    /// </summary>
    public float _walkSpeed = 5;
    /// <summary>
    /// 점프 시작 속도입니다.
    /// </summary>
    public float _jumpSpeed = 16;
    /// <summary>
    /// 점프한 이후로 매 시간 속도가 깎이는 양입니다.
    /// </summary>
    public float _jumpDecSize = 0.8f;
    /// <summary>
    /// 대쉬 속도입니다.
    /// </summary>
    public float _dashSpeed = 12;

    /// <summary>
    /// 
    /// </summary>
    public GameObject _indicatorObject;
    /// <summary>
    /// 
    /// </summary>
    public Vector3 _position;
    /// <summary>
    /// 
    /// </summary>
    public Vector2 _previousRayHit;

    #endregion





    #region 캐릭터의 운동 상태 필드를 정의합니다.
    /// <summary>
    /// 지상에 있다면 true입니다.
    /// </summary>
    bool _landed = false;
    /// <summary>
    /// 지상에서 이동하고 있다면 true입니다.
    /// </summary>
    bool _moving = false;
    /// <summary>
    /// 벽을 밀고 있다면 true입니다.
    /// </summary>
    bool _pushing = false;
    /// <summary>
    /// 점프 상태라면 true입니다.
    /// </summary>
    bool _jumping = false;
    /// <summary>
    /// 떨어지고 있다면 true입니다.
    /// </summary>
    bool _falling = false;

    /// <summary>
    /// 현재 플레이어와 닿아있는 땅 지형의 집합입니다.
    /// </summary>
    HashSet<EdgeCollider2D> _groundEdgeSet = new HashSet<EdgeCollider2D>();

    #endregion





    #region 캐릭터의 상태 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 캐릭터가 움직이는 속도를 정의합니다.
    /// </summary>
    public float _movingSpeed = 1;

    /// <summary>
    /// 지상에 있다면 true입니다.
    /// </summary>
    public bool Landed
    {
        get { return _landed; }
        protected set { _Animator.SetBool("Landed", _landed = value); }
    }
    /// <summary>
    /// 지상에서 이동하고 있다면 true입니다.
    /// </summary>
    public bool Moving
    {
        get { return _moving; }
        set { _Animator.SetBool("Moving", _moving = value); }
    }
    /// <summary>
    /// 벽을 밀고 있다면 true입니다.
    /// </summary>
    public bool Pushing
    {
        get { return _pushing; }
        set { _Animator.SetBool("Pushing", _pushing = value); }
    }
    /// <summary>
    /// 점프 상태라면 true입니다.
    /// </summary>
    public bool Jumping
    {
        get { return _jumping; }
        set { _Animator.SetBool("Jumping", _jumping = value); }
    }
    /// <summary>
    /// 떨어지고 있다면 true입니다.
    /// </summary>
    public bool Falling
    {
        get { return _falling; }
        set { _Animator.SetBool("Falling", _falling = value); }
    }

    /// <summary>
    /// 플레이어의 속도(RigidBody2D.velocity)입니다.
    /// </summary>
    public Vector2 _Velocity
    {
        get { return _Rigidbody.velocity; }
        set
        {
            _Rigidbody.velocity = value;
        }
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected virtual void DefaultStart()
    {
        
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected virtual void DefaultUpdate()
    {
        
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    protected virtual void DefaultFixedUpdate()
    {
        // 점프 중이라면
        if (Jumping)
        {
            if (_Velocity.y <= 0)
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
                Land();
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
    protected virtual void LateUpdate()
    {
        UpdateState();
    }

    #endregion





    #region 행동 메서드를 정의합니다.
    ///////////////////////////////////////////////////////////////////
    // 기본
    /// <summary>
    /// 지상에 착륙합니다.
    /// </summary>
    protected virtual void Land()
    {
        StopJumping();
        StopFalling();
    }
    /// <summary>
    /// 왼쪽으로 이동합니다.
    /// </summary>
    protected virtual void MoveLeft()
    {
        if (_Unit.FacingRight)
            _Unit.Flip();

        Moving = true;
        _Rigidbody.velocity = new Vector2(-_movingSpeed, 0);
    }
    /// <summary>
    /// 오른쪽으로 이동합니다.
    /// </summary>
    protected virtual void MoveRight()
    {
        if (_Unit.FacingRight == false)
            _Unit.Flip();

        Moving = true;
        _Rigidbody.velocity = new Vector2(_movingSpeed, 0);
    }
    /// <summary>
    /// 이동을 중지합니다.
    /// </summary>
    protected virtual void StopMoving()
    {
        _Velocity = new Vector2(0, _Velocity.y);

        // 
        Moving = false;
    }


    ///////////////////////////////////////////////////////////////////
    // 점프 및 낙하
    /// <summary>
    /// 점프하게 합니다.
    /// </summary>
    public virtual void Jump()
    {
        // 개체의 운동 상태를 갱신합니다.
        _Velocity = new Vector2(_Velocity.x, _jumpSpeed);

        // 개체의 운동 상태가 갱신되었음을 알립니다.
        Jumping = true;
    }
    /// <summary>
    /// 점프를 중지합니다.
    /// </summary>
    public virtual void StopJumping()
    {
        // 개체의 운동 상태가 갱신되었음을 알립니다.
        Jumping = false;
    }
    /// <summary>
    /// 낙하합니다.
    /// </summary>
    public virtual void Fall()
    {
        // 개체의 운동 상태를 갱신합니다.
        if (_Velocity.y > 0)
        {
            _Velocity = new Vector2(_Velocity.x, 0);
        }

        // 개체의 운동 상태가 갱신되었음을 알립니다.
        Jumping = false;
        Falling = true;
    }
    /// <summary>
    /// 플레이어의 낙하를 중지합니다.
    /// </summary>
    public virtual void StopFalling()
    {
        // 개체의 운동 상태가 갱신되었음을 알립니다.
        Falling = false;
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 애니메이터가 지정된 문자열의 상태인지 확인합니다.
    /// </summary>
    /// <param name="stateName">재생 중인지 확인하려는 상태의 이름입니다.</param>
    /// <returns>애니메이터가 지정된 문자열의 상태라면 true를 반환합니다.</returns>
    protected bool IsAnimatorInState(string stateName)
    {
        return _Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    /// <summary>
    /// 중력 가속도를 반영하여 종단 속도를 업데이트 합니다.
    /// </summary>
    protected void UpdateVy()
    {
        float vy = _Velocity.y - _jumpDecSize;
        _Velocity = new Vector2(_Velocity.x, vy > -16 ? vy : -16);
    }

    #endregion





    #region 캐릭터의 지상 착륙 상태를 업데이트합니다.
    /// <summary>
    /// 플레이어의 물리 상태를 갱신합니다.
    /// </summary>
    protected void UpdateState()
    {
        UpdateLanding();
    }
    /// <summary>
    /// 충돌이 시작되었습니다.
    /// </summary>
    /// <param name="collision">충돌 객체입니다.</param>
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        UpdatePhysicsState(collision);
    }
    /// <summary>
    /// 충돌이 유지되고 있습니다.
    /// </summary>
    /// <param name="collision">충돌 객체입니다.</param>
    protected void OnCollisionStay2D(Collision2D collision)
    {
        UpdatePhysicsState(collision);
    }
    /// <summary>
    /// 충돌이 끝났습니다.
    /// </summary>
    /// <param name="collision">충돌 객체입니다.</param>
    protected void OnCollisionExit2D(Collision2D collision)
    {
        UpdatePhysicsState(collision);
    }
    /// <summary>
    /// 플레이어가 땅과 접촉했는지에 대한 필드를 갱신합니다.
    /// </summary>
    /// <returns>플레이어가 땅에 닿아있다면 참입니다.</returns>
    bool UpdateLanding()
    {
        RaycastHit2D rayB = Physics2D.Raycast(_groundCheckBack.position, Vector2.down, _groundCheckRadius, _whatIsGround);
        RaycastHit2D rayF = Physics2D.Raycast(_groundCheckFront.position, Vector2.down, _groundCheckRadius, _whatIsGround);

        Debug.DrawRay(_groundCheckBack.position, Vector2.down, Color.red);
        Debug.DrawRay(_groundCheckFront.position, Vector2.down, Color.red);

        if (Handy.DebugPoint) // PlayerController.UpdateLanding
        {
            Handy.Log("PlayerController.UpdateLanding");
        }

        if (OnGround())
        {
            // 절차:
            // 1. 캐릭터에서 수직으로 내린 직선에 맞는 경사면의 법선 벡터를 구한다.
            // 2. 법선 벡터와 이동 방향 벡터가 이루는 각도가 예각이면 내려오는 것
            //    법선 벡터와 이동 방향 벡터가 이루는 각도가 둔각이면 올라가는 것
            /// Handy.Log("OnGround()");

            // 앞 부분 Ray와 뒤 부분 Ray의 경사각이 다른 경우
            if (rayB.normal.normalized != rayF.normal.normalized)
            {
                bool isTouchingSlopeFromB = rayB.normal.x == 0;
                /// Transform pos = isTouchingSlopeFromB ? groundCheckBack : groundCheckFront;
                RaycastHit2D ray = isTouchingSlopeFromB ? rayB : rayF;

                Vector2 from = _Unit.FacingRight ? Vector2.right : Vector2.left;
                float rayAngle = Vector2.Angle(from, ray.normal);
                float rayAngleRad = Mathf.Deg2Rad * rayAngle;

                float sx = _movingSpeed * Mathf.Cos(rayAngleRad);
                float sy = _movingSpeed * Mathf.Sin(rayAngleRad);
                float vx = _Unit.FacingRight ? sx : -sx;

                if (Jumping)
                {
                }
                // 예각이라면 내려갑니다.
                else if (rayAngle < 90)
                {
                    float vy = -sy;
                    _Velocity = new Vector2(vx, vy);
                }
                // 둔각이라면 올라갑니다.
                else if (rayAngle > 90)
                {
                    float vy = sy;
                    _Velocity = new Vector2(vx, vy);
                }
                // 90도라면
                else
                {
                }
            }
            else
            {
            }

            Landed = true;
        }
        else if (rayB || rayF)
        {
            // 가장 가까운 거리에 적중한 Ray를 탐색합니다.
            RaycastHit2D ray;
            if (rayB && !rayF)
            {
                ray = rayB;
            }
            else if (!rayB && rayF)
            {
                ray = rayF;
            }
            else
            {
                ray = rayB.distance < rayF.distance ? rayB : rayF;
            }
            _position = transform.position;
            _previousRayHit = ray.point;

            /// Vector3 pos = transform.position;
            /// pos.y -= difY;

            // 지형과 Y 좌표의 차이가 작으면 추락을 중지합니다.
            float difY = ray.distance / transform.localScale.y;
            if (Mathf.Abs(difY) < _jumpDecSize)
            {
                // transform.position = pos;
                float vy = _Velocity.y > 0 ? _Velocity.y : 0;
                _Velocity = new Vector2(_Velocity.x, vy);

                // 
                BoxCollider2D boxCollider = _Collider as BoxCollider2D;
                float posY =
                    ((boxCollider.size.y / 2) - (boxCollider.offset.y))
                    * transform.localScale.y;
                _Unit.PosY = posY + _previousRayHit.y;

                // 
                Landed = true;
            }
            else
            {
                Landed = false;
            }
        }
        else if (Jumping || Falling)
        {
            Landed = false;
        }
        else
        {
            Landed = false;
        }
        return Landed;
    }
    /// <summary>
    /// 플레이어의 물리 상태를 갱신합니다.
    /// </summary>
    /// <param name="collision">충돌 정보를 담고 있는 객체입니다.</param>
    void UpdatePhysicsState(Collision2D collision)
    {
        int layer = collision.collider.gameObject.layer;

        // 땅과 접촉한 경우의 처리입니다.
        if (IsSameLayer(layer, _whatIsGround))
        {
            EdgeCollider2D groundCollider = collision.collider as EdgeCollider2D;
            if (IsTouchingGround(groundCollider))
            {
                _groundEdgeSet.Add(groundCollider);
            }
            else
            {
                _groundEdgeSet.Remove(groundCollider);
            }
        }

        // 벽과 접촉한 경우의 처리입니다.
        if (IsSameLayer(layer, _whatIsWall))
        {
            bool isTouchingWall = IsTouchingWall(collision);
            Pushing = isTouchingWall;
        }
    }
    /// <summary>
    /// 레이어가 어떤 레이어 마스크에 포함되는지 확인합니다.
    /// </summary>
    /// <param name="layer">확인할 레이어입니다.</param>
    /// <param name="layerMask">레이어 마스크입니다.</param>
    /// <returns>레이어가 인자로 넘어온 레이어 마스크에 포함된다면 true입니다.</returns>
    bool IsSameLayer(int layer, LayerMask layerMask)
    {
        return ((1 << layer) & layerMask) != 0;
    }
    /// <summary>
    /// 땅에 닿았는지 확인합니다. 측면에서 닿은 것은 포함하지 않습니다.
    /// </summary>
    /// <returns>땅과 닿아있다면 true입니다.</returns>
    bool OnGround()
    {
        // 땅과 닿아있는 경우 몇 가지 더 검사합니다.
        if (_Collider.IsTouchingLayers(_whatIsGround))
        {
            float playerBottom = _Collider.bounds.min.y;
            foreach (EdgeCollider2D edge in _groundEdgeSet)
            {
                float groundTop = edge.bounds.max.y;
                float groundBottom = edge.bounds.min.y;

                // 평면인 경우
                if (groundBottom == groundTop)
                {
                    if (playerBottom >= groundTop)
                    {
                        Debug.DrawLine(edge.points[0] * 0.02008f, edge.points[1] * 0.02008f, Color.red);
                        return true;
                    }
                }
                // 경사면인 경우
                else
                {
                    if (groundBottom <= playerBottom && playerBottom <= groundTop)
                    {
                        Debug.DrawLine(edge.points[0] * 0.02008f, edge.points[1] * 0.02008f, Color.blue);
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }
    /// <summary>
    /// 땅에 닿았는지 확인합니다. 측면에서 닿은 것은 포함하지 않습니다.
    /// </summary>
    /// <param name="groundCollider">확인하려는 collider입니다.</param>
    /// <returns>땅에 닿았다면 참입니다.</returns>
    bool IsTouchingGround(EdgeCollider2D groundCollider)
    {
        // 땅과 닿아있는 경우 몇 가지 더 검사합니다.
        if (_Collider.IsTouching(groundCollider))
        {
            Bounds groundBounds = groundCollider.bounds;
            if (groundBounds.min.y == groundBounds.max.y)
            {
                float playerBot = _Collider.bounds.min.y;
                float groundTop = groundBounds.max.y;
                if (playerBot >= groundTop)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                float playerBot = _Collider.bounds.min.y;
                // float groundTop = groundBounds.max.y;
                float groundBottom = groundBounds.min.y;
                if (groundBottom <= playerBot) // && playerBot <= groundTop)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        // 땅과 닿아있지 않다면 거짓입니다.
        return false;
    }
    /// <summary>
    /// 벽에 닿았는지 확인합니다.
    /// </summary>
    /// <param name="collision">충돌 정보를 갖고 있는 객체입니다.</param>
    /// <returns>벽과 닿아있다면 true입니다.</returns>
    bool IsTouchingWall(Collision2D collision)
    {
        // 벽과 닿아있는 경우 몇 가지 더 검사합니다.
        if (_Collider.IsTouchingLayers(_whatIsWall))
        {
            return true;
        }

        // 벽과 닿아있지 않으면 거짓입니다.
        return false;
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("이거 쓰긴 쓰나요?")]
    /// <summary>
    /// 자신의 밑에 지면이 존재하는지 검사하기 위해 사용합니다.
    /// </summary>
    public Transform _groundCheck;

    #endregion
}