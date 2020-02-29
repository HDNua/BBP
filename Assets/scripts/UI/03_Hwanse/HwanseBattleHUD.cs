using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 환세 전투 HUD입니다.
/// </summary>
public class HwanseBattleHUD : BattleHUD
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 체력 바입니다.
    /// </summary>
    public GameObject _healthBar;
    /// <summary>
    /// 대미지 바입니다.
    /// </summary>
    public GameObject _damageBar;

    /// <summary>
    /// 마나 바입니다.
    /// </summary>
    public GameObject _manaBar;
    /// <summary>
    /// 마나 대미지 바입니다.
    /// </summary>
    public GameObject _manaDamageBar;

    /// <summary>
    /// 경험치 바입니다.
    /// </summary>
    public GameObject _expBar;
    /// <summary>
    /// 경험치 대미지 바입니다.
    /// </summary>
    public GameObject _expDamageBar;

    /// <summary>
    /// 대미지 바가 체력 바를 따라가는 속도입니다.
    /// </summary>
    public float _hpFollowTime = 0.5f;
    /// <summary>
    /// 대미지 바가 체력 바를 쫓아가기 위해 보관하는 현재 시간입니다.
    /// </summary>
    public float _hpTargetTime = 0;

    /// <summary>
    /// 대미지 바가 체력 바를 따라가는 속도입니다.
    /// </summary>
    public float _mpFollowTime = 0.5f;
    /// <summary>
    /// 대미지 바가 체력 바를 쫓아가기 위해 보관하는 현재 시간입니다.
    /// </summary>
    public float _mpTargetTime = 0;

    /// <summary>
    /// 대미지 바가 체력 바를 따라가는 속도입니다.
    /// </summary>
    public float _expFollowTime = 0.5f;
    /// <summary>
    /// 대미지 바가 체력 바를 쫓아가기 위해 보관하는 현재 시간입니다.
    /// </summary>
    public float _expTargetTime = 0;

    #endregion



    #region Unity 개체에 대한 참조를 보관합니다.

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {

    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    protected override void Update()
    {
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    protected override void LateUpdate()
    {
        UpdateHealthBar();
        UpdateManaBar();
        UpdateExpBar();
    }

    #endregion





    #region 요청 메서드를 정의합니다.

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 체력 바를 업데이트 합니다.
    /// </summary>
    void UpdateHealthBar()
    {
        if (_battleManager.DoesBattleEnd())
        {
            // 체력 바를 업데이트 합니다.
            Vector3 healthScale = _healthBar.transform.localScale;
            healthScale.x = 0;
            _healthBar.transform.localScale = healthScale;

            // 대미지 바를 업데이트 합니다.
            Vector3 damageScale = _damageBar.transform.localScale;
            Vector3 newScale = damageScale;
            float newScaleValue = Mathf.Lerp(healthScale.x, damageScale.x, 1 - _hpTargetTime / _hpFollowTime);
            newScale.x = newScaleValue;
            _damageBar.transform.localScale = newScale;
        }
        else if (_atahoUnit != null)
        {
            // 체력 바를 업데이트 합니다.
            Vector3 healthScale = _healthBar.transform.localScale;
            Vector3 damageScale = _damageBar.transform.localScale;
            float value = (float)_atahoUnit.Health / _atahoUnit.MaxHealth;
            healthScale.x = value;
            _healthBar.transform.localScale = healthScale;

            // 대미지 바가 체력 바를 추적합니다.
            if (_atahoUnit.IsDamaged)
            {
                _hpTargetTime = 0;
            }
            else
            {
                Vector3 newScale = damageScale;
                float healthScaleValue;
                float damageScaleValue;

                // 
                healthScaleValue = healthScale.x;
                damageScaleValue = damageScale.x;

                //
                float newScaleValue = Mathf.Lerp(healthScaleValue, damageScaleValue, 1 - _hpTargetTime / _hpFollowTime);
                newScale.x = newScaleValue;

                // 
                _damageBar.transform.localScale = newScale;
            }
        }
        else
        {
            Vector3 healthScale = _healthBar.transform.localScale;
            healthScale.x = 0;
            _healthBar.transform.localScale = healthScale;
            _damageBar.transform.localScale = healthScale;
        }

        // 
        _hpTargetTime = (_hpTargetTime >= _hpFollowTime) ? (_hpFollowTime) : _hpTargetTime + Time.deltaTime;
    }
    /// <summary>
    /// 마나 바를 업데이트 합니다.
    /// </summary>
    void UpdateManaBar()
    {
        _manaDamageBar.GetComponent<UnityEngine.UI.Image>().color = Color.white;

        // 
        if (_battleManager.DoesBattleEnd())
        {
            // 마나 바를 업데이트 합니다.
            Vector3 manaScale = _manaBar.transform.localScale;
            manaScale.x = 0;
            _manaBar.transform.localScale = manaScale;

            // 대미지 바를 업데이트 합니다.
            Vector3 damageScale = _manaDamageBar.transform.localScale;
            Vector3 newScale = damageScale;
            float newScaleValue = Mathf.Lerp(manaScale.x, damageScale.x, 1 - _hpTargetTime / _hpFollowTime);
            newScale.x = newScaleValue;
            _manaDamageBar.transform.localScale = newScale;
        }
        else if (_atahoUnit != null)
        {
            // 마나를 회복하는 경우의 처리입니다.
            if (_atahoUnit._manaFilling)
            {
                _manaDamageBar.GetComponent<UnityEngine.UI.Image>().color = Color.clear;

                // 다른 바와 달리, 마나 대미지 바를 업데이트 합니다.
                Vector3 manaScale = _manaDamageBar.transform.localScale;
                Vector3 damageScale = _manaBar.transform.localScale;
                float value = (float)_atahoUnit._mana / _atahoUnit._maxExp;
                manaScale.x = value;
                _manaDamageBar.transform.localScale = manaScale;

                // 마나 바가 대미지 바를 추적합니다.
                if (_atahoUnit._manaFillRequest)
                {
                    _mpTargetTime = 0;
                    _atahoUnit.SetManaFillRequest(false);
                }
                else
                {
                    Vector3 newScale = damageScale;
                    float manaScaleValue;
                    float damageScaleValue;

                    // 
                    manaScaleValue = manaScale.x;
                    damageScaleValue = damageScale.x;

                    //
                    float newScaleValue = Mathf.Lerp(manaScaleValue, damageScaleValue, 1 - _mpTargetTime / _mpFollowTime);
                    newScale.x = newScaleValue;

                    // 
                    _manaBar.transform.localScale = newScale;

                    // 회복이 완료되었다면 마나 회복 상태를 종료합니다.
                    if (_mpTargetTime == _mpFollowTime)
                    {
                        _atahoUnit.EndFillMana();
                    }
                }
            }
            // 일반 및 마나를 사용하는 경우의 처리입니다.
            else
            {
                // 마나 바를 업데이트 합니다.
                Vector3 manaScale = _manaBar.transform.localScale;
                Vector3 damageScale = _manaDamageBar.transform.localScale;
                float value = (float)_atahoUnit._mana / _atahoUnit._maxMana;
                manaScale.x = value;
                _manaBar.transform.localScale = manaScale;

                // 대미지 바가 마나 바를 추적합니다.
                if (_atahoUnit._manaUseRequest)
                {
                    _mpTargetTime = 0;
                    _atahoUnit.SetManaUseRequest(false);
                }
                else
                {
                    Vector3 newScale = damageScale;
                    float manaScaleValue;
                    float damageScaleValue;

                    // 
                    manaScaleValue = manaScale.x;
                    damageScaleValue = damageScale.x;

                    //
                    float newScaleValue = Mathf.Lerp(manaScaleValue, damageScaleValue, 1 - _mpTargetTime / _mpFollowTime);
                    newScale.x = newScaleValue;

                    // 
                    _manaDamageBar.transform.localScale = newScale;
                }
            }
        }
        else
        {
            Vector3 manaScale = _manaBar.transform.localScale;
            manaScale.x = 0;
            _manaBar.transform.localScale = manaScale;
            _manaDamageBar.transform.localScale = manaScale;
        }

        // 
        _mpTargetTime = (_mpTargetTime >= _mpFollowTime) ? (_mpFollowTime) : _mpTargetTime + Time.deltaTime;
    }
    /// <summary>
    /// 경험치 바를 업데이트 합니다.
    /// </summary>
    void UpdateExpBar()
    {
        if (_battleManager.DoesBattleEnd())
        {
            // 다른 바와 달리, 경험치 대미지 바를 업데이트 합니다.
            Vector3 expScale = _expDamageBar.transform.localScale;
            expScale.x = 0;
            _expDamageBar.transform.localScale = expScale;

            // 경험치 바를 업데이트 합니다.
            Vector3 damageScale = _expBar.transform.localScale;
            Vector3 newScale = damageScale;
            float newScaleValue = Mathf.Lerp(expScale.x, damageScale.x, 1 - _hpTargetTime / _hpFollowTime);
            newScale.x = newScaleValue;
            _expBar.transform.localScale = newScale;
        }
        else if (_atahoUnit != null)
        {
            // 다른 바와 달리, 경험치 대미지 바를 업데이트 합니다.
            Vector3 expScale = _expDamageBar.transform.localScale;
            Vector3 damageScale = _expBar.transform.localScale;
            float value = (float)(_atahoUnit._exp % _atahoUnit._maxExp) / _atahoUnit._maxExp;
            expScale.x = value;
            _expDamageBar.transform.localScale = expScale;

            // 경험치 바가 대미지 바를 추적합니다.
            if (_atahoUnit._expUpdateRequest)
            {
                _expTargetTime = 0;
                _atahoUnit.SetExpUpdateRequest(false);
            }
            else
            {
                Vector3 newScale = damageScale;
                float expScaleValue;
                float damageScaleValue;

                // 
                expScaleValue = expScale.x;
                damageScaleValue = damageScale.x;

                //
                float newScaleValue = Mathf.Lerp(expScaleValue, damageScaleValue, 1 - _expTargetTime / _expFollowTime);
                newScale.x = newScaleValue;

                // 
                _expBar.transform.localScale = newScale;
            }
        }
        else
        {
            Vector3 expScale = _expDamageBar.transform.localScale;
            expScale.x = 0;
            _expDamageBar.transform.localScale = expScale;
            _expBar.transform.localScale = expScale;
        }

        // 
        _expTargetTime = (_expTargetTime >= _expFollowTime) ? (_expFollowTime) : _expTargetTime + Time.deltaTime;
    }

    #endregion





    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    EnemyBossAtahoUnit _atahoUnit;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public override void RequestSetUnit(EnemyBossUnit unit)
    {
        base.RequestSetUnit(unit);
        _atahoUnit = (EnemyBossAtahoUnit)unit;
    }

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}