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

    #endregion
}
