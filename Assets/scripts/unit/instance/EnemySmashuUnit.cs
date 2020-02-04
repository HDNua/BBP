using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Groundable))]
/// <summary>
/// 스마슈를 정의합니다.
/// </summary>
public class EnemySmashuUnit : EnemyUnit
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// 지상에 착지할 수 있는 유닛입니다.
    /// </summary>
    Groundable _Groundable
    {
        get { return GetComponent<Groundable>(); }
    }

    #endregion





    #region Groundable 컴포넌트를 구현합니다.
    /// <summary>
    /// 지상에 있다면 참입니다.
    /// </summary>
    bool Landed
    {
        get { return _Groundable.Landed; }
        set { _Groundable.Landed = value; }
    }
    /// <summary>
    /// 점프 상태라면 참입니다.
    /// </summary>
    bool Jumping
    {
        get { return _Groundable.Jumping; }
        set { _Groundable.Jumping = value; }
    }
    /// <summary>
    /// 낙하 상태라면 참입니다.
    /// </summary>
    bool Falling
    {
        get { return _Groundable.Falling; }
        set { _Groundable.Falling = value; }
    }
    /// <summary>
    /// 개체의 속도 벡터를 구합니다.
    /// </summary>
    Vector2 Velocity
    {
        get { return _Groundable.Velocity; }
        set { _Groundable.Velocity = value; }
    }

    /// <summary>
    /// 지상에 착륙합니다.
    /// </summary>
    void Land()
    {
        _Groundable.Land();
    }
    /// <summary>
    /// 점프합니다.
    /// </summary>
    void Jump()
    {
        _Groundable.Jump();
    }
    /// <summary>
    /// 낙하합니다.
    /// </summary>
    void Fall()
    {
        _Groundable.Fall();
    }

    /// <summary>
    /// 중력 가속도를 반영하여 종단 속도를 업데이트 합니다.
    /// </summary>
    void UpdateVy()
    {
        _Groundable.UpdateVy();
    }

    #endregion




    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 등장 시간입니다.
    /// </summary>
    float _appearTime = 3f;

    #endregion




    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 등장 상태라면 참입니다.
    /// </summary>
    bool Appearing
    {
        get { return _Animator.GetBool("Appearing"); }
        set { _Animator.SetBool("Appearing", value); }
    }

    /// <summary>
    /// Groundable 컴포넌트가 활성화된 상태라면 참입니다.
    /// </summary>
    bool _isGroundableNow = true;

    /// <summary>
    /// 등장 풀때기 효과입니다.
    /// </summary>
    GameObject EffectNinjaGrass
    {
        get { return _effects[0]; }
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
        if (_isGroundableNow)
        {
            // 점프 중이라면
            if (Jumping)
            {
                if (Velocity.y <= 0)
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
    }

    #endregion





    #region 패턴 메서드를 정의합니다.
    /// <summary>
    /// 등장합니다.
    /// </summary>
    void Appear()
    {
        // 상태를 정의합니다.
        Appearing = true;

        // 내용을 정의합니다.
        _coroutineAppear = StartCoroutine(CoroutineAppear());
    }
    /// <summary>
    /// 등장을 중지합니다.
    /// </summary>
    void StopAppearing()
    {
        Appearing = false;
    }
    /// <summary>
    /// 등장이 끝난 이후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterAppear()
    {

    }

    /// <summary>
    /// 쾌진격 기술을 사용합니다.
    /// </summary>
    void DoSkillKwaejinkyuk()
    {

    }
    /// <summary>
    /// 쾌진격 기술의 사용을 중지합니다.
    /// </summary>
    void StopSkillKwaejinkyuk()
    {

    }
    /// <summary>
    /// 쾌진격 기술이 끝난 이후의 행동을 정의합니다.
    /// </summary>
    void PerformActionAfterSkillKwaejinkyuk()
    {

    }

    #endregion





    #region 행동 메서드를 정의합니다.
    /// <summary>
    /// 캐릭터를 죽입니다.
    /// </summary>
    public override void Dead()
    {
        // 폭발 효과를 생성하고 효과음을 재생합니다.
        Instantiate(DataBase.Instance.MultipleExplosionEffect,
            transform.position, transform.rotation);

        // 캐릭터를 죽입니다.
        base.Dead();
    }

    /// <summary>
    /// 폭발 효과를 생성합니다. (주의: 효과 0번은 폭발 개체여야 합니다.)
    /// </summary>
    protected virtual void CreateExplosion(Vector3 position)
    {
        Instantiate(DataBase.Instance.Explosion1Effect, position, transform.rotation)
            .gameObject.SetActive(true);
    }

    #endregion




    #region 코루틴 메서드를 정의합니다.
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    Coroutine _coroutineAppear;

    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineAppear()
    {
        // 닌자 등장 풀때기 효과를 생성합니다.
        Instantiate(EffectNinjaGrass, transform.position, transform.rotation)
            .gameObject.SetActive(true);

        // 
        float time = 0;
        while (time < _appearTime)
        {
            time += Time.deltaTime;
            yield return false;
        }


        // 등장을 끝냅니다.
        StopAppearing();
        PerformActionAfterAppear();
        _coroutineAppear = null;
        yield break;
    }

    #endregion





    #region 보조 메서드를 정의합니다.


    #endregion
}