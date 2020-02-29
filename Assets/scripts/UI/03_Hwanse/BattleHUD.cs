using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 전투 HUD(Head Up Display)입니다.
/// </summary>
public class BattleHUD : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /*
    /// <summary>
    /// 체력 바입니다.
    /// </summary>
    public GameObject _healthBar;
    /// <summary>
    /// 대미지 바입니다.
    /// </summary>
    public GameObject _damageBar;

    /// <summary>
    /// 체력 바 보드의 머리 부분입니다.
    /// </summary>
    public GameObject _healthBoardHead;
    /// <summary>
    /// 체력 바 보드의 몸통 부분입니다.
    /// </summary>
    public GameObject _healthBoardBody;

    /// <summary>
    /// 체력 텍스트 보드입니다.
    /// </summary>
    public GameObject _healthTextBoard;
    /// <summary>
    /// 체력 텍스트입니다.
    /// </summary>
    public UnityEngine.UI.Text _healthText;

    /// <summary>
    /// 체력 바가 수평인지를 표시합니다.
    /// </summary>
    public bool _isHorizontal = false;

    /// <summary>
    /// 대미지 바가 체력 바를 따라가는 속도입니다.
    /// </summary>
    public float _followTime = 0.5f;
    /// <summary>
    /// 대미지 바가 체력 바를 쫓아가기 위해 보관하는 현재 시간입니다.
    /// </summary>
    public float _nowTime = 0;
    */

    /// <summary>
    /// HUD 대상입니다.
    /// </summary>
    protected EnemyBossUnit _unit;

    #endregion



    #region Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 전투 관리자입니다.
    /// </summary>
    protected BattleManager _battleManager;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    protected virtual void Awake()
    {
        _battleManager = BattleManager.Instance;
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected virtual void Start()
    {

    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    protected virtual void Update()
    {
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    protected virtual void LateUpdate()
    {
        /*
        if (_unit != null)
        {
            // 체력 바를 업데이트 합니다.
            Vector3 healthScale = _healthBar.transform.localScale;
            Vector3 damageScale = _damageBar.transform.localScale;
            float value = (float)_unit.Health / _unit.MaxHealth;
            if (_isHorizontal)
            {
                healthScale.x = value;
            }
            else
            {
                healthScale.y = value;
            }
            _healthBar.transform.localScale = healthScale;

            // 대미지 바가 체력 바를 추적합니다.
            if (_unit.IsDamaged)
            {
                _nowTime = 0;
            }
            else
            {
                Vector3 newScale = damageScale;
                float healthScaleValue;
                float damageScaleValue;
                if (_isHorizontal)
                {
                    healthScaleValue = healthScale.x;
                    damageScaleValue = damageScale.x;

                    //
                    float newScaleValue = Mathf.Lerp(healthScaleValue, damageScaleValue, 1 - _nowTime / _followTime);
                    newScale.x = newScaleValue;
                }
                else
                {
                    healthScaleValue = healthScale.y;
                    damageScaleValue = damageScale.y;

                    //
                    float newScaleValue = Mathf.Lerp(healthScaleValue, damageScaleValue, _nowTime / _followTime);
                    newScale.y = newScaleValue;
                }

                // 
                _damageBar.transform.localScale = newScale;
            }

            // 
            _nowTime = (_nowTime >= _followTime) ? (_followTime) : _nowTime + Time.deltaTime;
        }
        else
        {
            Vector3 healthScale = _healthBar.transform.localScale;
            healthScale.x = 0;
            _healthBar.transform.localScale = healthScale;
            _damageBar.transform.localScale = healthScale;
        }
        */
    }

    #endregion





    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// HUD 대상 유닛을 설정합니다.
    /// </summary>
    /// <param name="unit">HUD 대상 유닛입니다.</param>
    public virtual void RequestSetUnit(EnemyBossUnit unit)
    {
        _unit = unit;
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("필요한지 잘 모르겠네요.")]
    /// <summary>
    /// 
    /// </summary>
    public void Activate()
    {

    }
    [Obsolete("필요한지 잘 모르겠네요.")]
    /// <summary>
    /// 
    /// </summary>
    public void Deactivate()
    {

    }

    #endregion
}
