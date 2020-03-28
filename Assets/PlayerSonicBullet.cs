using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 소닉 플레이어 탄환을 정의합니다.
/// </summary>
public class PlayerSonicBullet : AttackScript
{
    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.
    /// <summary>
    /// Collider2D 컴포넌트입니다.
    /// </summary>
    Collider2D _collider;
    /// <summary>
    /// Rigidbody2D 컴포넌트입니다.
    /// </summary>
    Rigidbody2D _rigidbody;

    #endregion





    #region Unity에서 접근 가능한 공용 객체를 정의합니다.
    /// <summary>
    /// 버스터가 통과할 수 없는 지형에 대한 마스크입니다.
    /// </summary>
    public LayerMask _busterUnpassable;
    
    /// <summary>
    /// 
    /// </summary>
    public float _volume = 0.5f;

    /// <summary>
    /// 
    /// </summary>
    public float _vy = 0.1f;

    /// <summary>
    /// 
    /// </summary>
    public float _gravityX = 0.01f;
    /// <summary>
    /// 
    /// </summary>
    public float _gravityY = 0.02f;
    /// <summary>
    /// 
    /// </summary>
    public float _gravityZ = 0.04f;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    bool _disappearing;

    /// <summary>
    /// 
    /// </summary>
    float _direction;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // 필드를 초기화합니다.
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 
        SoundEffects[8].enabled = true;
        SoundEffects[8].volume = _volume;
        SoundEffects[8].Play();
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // 
        if (_disappearing)
        {
            Vector3 newPosition = transform.position;
            newPosition.x += _direction * _gravityX * Time.deltaTime;
            newPosition.y += _vy;
            newPosition.z += _gravityZ * Time.deltaTime;
            transform.position = newPosition;

            // 
            _vy -= _gravityY * Time.deltaTime;
        }
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // 
        Camera mainCamera = Camera.main;
        Vector3 camPos = mainCamera.transform.position;
        Vector3 bulPos = transform.position;
        float dist = Vector3.Distance(bulPos, camPos);

        if (SoundEffects[2].isPlaying || SoundEffects[8].isPlaying)
        {

        }
        else if ((Mathf.Abs(camPos.x - bulPos.x) > 10) || (Mathf.Abs(dist) > 20))
        {
            Destroy(gameObject);
        }
    }

    #endregion





    #region Collider2D의 기본 메서드를 재정의합니다.
    /// <summary>
    /// 충돌체가 트리거 내부로 진입했습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // 사라지는 중엔 무시합니다.
        if (_disappearing)
        {

        }
        // 적과 충돌했습니다.
        else if (other.gameObject.CompareTag("Enemy"))
        {
            GameObject otherObject = other.gameObject;
            EnemyUnit enemyUnit = otherObject.GetComponent<EnemyUnit>();

            if (enemyUnit is EnemyBulletUnit)
            {

            }
            else if (enemyUnit is EnemyBossRinshanUnit)
            {
                EnemyBossRinshanUnit rinshanUnit = (EnemyBossRinshanUnit)enemyUnit;

                // 적이 무적 상태라면
                if (rinshanUnit.Invencible)
                {
                    // 반사 효과를 생성합니다.
                    MakeReflectedParticle(_rigidbody.velocity.x < 0, transform);
                }
                // 
                else if (rinshanUnit.HasBulletImmunity)
                {

                }
                // 그 외의 경우
                else
                {
                    // 대미지를 입힙니다.
                    rinshanUnit.Hurt(damage, transform);
                }

                // 탄환을 필드 밖으로 튕겨냅니다.
                Disappear();
            }
            else if (enemyUnit)
            {
                EnemyUnit enemy = enemyUnit;

                // 적이 무적 상태라면
                if (enemy.Invencible)
                {
                    // 반사 효과를 생성합니다.
                    MakeReflectedParticle(_rigidbody.velocity.x < 0, transform);
                }
                // 
                else if (enemy.HasBulletImmunity)
                {

                }
                // 그 외의 경우
                else
                {
                    // 타격 효과를 생성하고 대미지를 입힙니다.
                    // MakeHitParticle(_rigidbody.velocity.x < 0, transform);
                    enemy.Hurt(damage, transform);
                }

                // 탄환을 필드 밖으로 튕겨냅니다.
                Disappear();
            }
        }
        // X 버스터가 통과할 수 없는 레이어와 충돌했습니다.
        else if (_collider.IsTouchingLayers(_busterUnpassable))
        {
            // 타격 입자를 생성하고 탄환을 제거합니다.
            Disappear();
        }
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public void Disappear()
    {
        _disappearing = true;

        // 
        SoundEffects[2].enabled = true;
        ///SoundEffects[2].Play();

        // 
        _direction = Random.Range(0, 2) == 0 ? -1 : 1;
        _rigidbody.velocity = Vector2.zero;
    }

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}
