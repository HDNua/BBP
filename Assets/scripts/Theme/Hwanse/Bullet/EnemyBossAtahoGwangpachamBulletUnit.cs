using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 아타호 맹호광파참 탄환입니다.
/// </summary>
public class EnemyBossAtahoGwangpachamBulletUnit : EnemyBulletUnit
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 맹호광파참이 플레이어에게 대미지를 입히는 시간입니다.
    /// </summary>
    public float TIME_GWANGPACHAM_DANGEROUS = 2f;
    /// <summary>
    /// 맹호광파참의 크기가 줄어들기 시작하는 시간입니다.
    /// </summary>
    public float TIME_GWANGPACHAM_SHRINK = 1.2f;

    #endregion



    #region 컨트롤러가 사용할 Unity 객체를 정의합니다.

    #endregion





    #region Unity에서 접근 가능한 공용 객체를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public GameObject _bulletTail;
    /// <summary>
    /// 
    /// </summary>
    public GameObject _bulletBody;
    /// <summary>
    /// 
    /// </summary>
    public GameObject _bulletHead;

    /// <summary>
    /// 
    /// </summary>
    public Transform _headBound;
    /// <summary>
    /// 
    /// </summary>
    public Transform _tailBound;

    /// <summary>
    /// 
    /// </summary>
    public float _defaultScaleY = 6f;

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
        float scaleY = _defaultScaleY;
        if (_time >= TIME_GWANGPACHAM_DANGEROUS)
        {
            gameObject.SetActive(false);
            _damage = 0;
        }
        else if (_time >= TIME_GWANGPACHAM_SHRINK)
        {
            float ratio = (_time - TIME_GWANGPACHAM_SHRINK) / (TIME_GWANGPACHAM_DANGEROUS - TIME_GWANGPACHAM_SHRINK);
            scaleY = Mathf.Clamp(1 - ratio, 0, 1) * _defaultScaleY;
        }

        // 
        _time += Time.deltaTime;

        // 
        float prevWidth = Vector3.Distance(_headBound.position, _tailBound.position);
        float ds = (FacingRight ? _movingSpeed : -_movingSpeed) * Time.deltaTime;
        Vector3 newHeadPosition = _bulletHead.transform.position;
        newHeadPosition.x += ds;
        _bulletHead.transform.position = newHeadPosition;

        Vector3 newBodyPosition = _bulletBody.transform.position;
        newBodyPosition.x += ds / 2;
        _bulletBody.transform.position = newBodyPosition;

        // 
        BoxCollider2D bodyCollider = _bulletBody.GetComponent<BoxCollider2D>();
        float prevScaleX = _bulletBody.transform.localScale.x;
        float mainWidth = bodyCollider.size.x;
        float newWidth = Vector3.Distance(_headBound.position, _tailBound.position);

        ///Handy.Log("prevWidth - newWidth = {0} - {1}", mainWidth, newWidth);

        // 
        Vector3 newBodyScale = _bulletBody.transform.localScale;
        newBodyScale.x = newWidth / mainWidth / 4;
        _bulletBody.transform.localScale = newBodyScale;

        // 
        _bulletBody.SetActive(true);

        // 
        transform.localScale = new Vector3(transform.localScale.x, scaleY);
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

    }
    /// <summary>
    /// 충돌체가 트리거 내부로 진입했습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    public override void RequestOnTriggerEnter2D(Collider2D other)
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

            // 맹호광파참은 플레이어를 때려도 사라지지 않는 공격입니다.
            ///Dead();
        }
    }
    /// <summary>
    /// 충돌체가 여전히 트리거 내부에 있습니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    public override void RequestOnTriggerStay2D(Collider2D other)
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

            // 맹호광파참은 플레이어를 때려도 사라지지 않는 공격입니다.
            ///Dead();
        }
    }
    /// <summary>
    /// 충돌체가 트리거 내부에서 나옵니다.
    /// </summary>
    /// <param name="other">자신이 아닌 충돌체 개체입니다.</param>
    public override void RequestOnTriggerExit2D(Collider2D other)
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

            // 맹호광파참은 플레이어를 때려도 사라지지 않는 공격입니다.
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
