﻿using System;
using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 사용자 인터페이스 관리자입니다.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 데이터베이스입니다.
    /// </summary>
    public DataBase _database;
    
    /// <summary>
    /// 정지 화면 관리자입니다.
    /// </summary>
    public PauseMenuManager _pauseMenuManager;

    /// <summary>
    /// 주 플레이어 HUD 개체입니다.
    /// </summary>
    public HUDScript _HUD;
    /// <summary>
    /// 부 플레이어 HUD 개체입니다.
    /// </summary>
    public HUDScript _subHUD;

    /// <summary>
    /// 
    /// </summary>
    public BossHUDScript[] _bossHudArray;

    #endregion





    #region Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 사용자 인터페이스 관리자입니다.
    /// </summary>
    public static UIManager Instance
    {
        get
        {
            return GameObject.FindGameObjectWithTag("UIManager")
                .GetComponent<UIManager>();
        }
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    void Awake()
    {

    }

    #endregion





    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// 일시정지 상태를 전환합니다.
    /// </summary>
    public void RequestPauseToggle()
    {
        PauseMenuManager.Instance.RequestPauseToggle();
    }
    /// <summary>
    /// 주 플레이어 HUD를 활성화합니다.
    /// </summary>
    public void ActivateMainPlayerHUD()
    {
        _HUD.UpdateStatusText();
        _HUD.Player = StageManager.Instance.MainPlayer;
        _HUD.gameObject.SetActive(true);
    }
    /// <summary>
    /// 부 플레이어 HUD를 활성화합니다.
    /// </summary>
    public void ActivateSubPlayerHUD()
    {
        _subHUD.UpdateStatusText();
        _subHUD.Player = StageManager2P.Instance.SubPlayer;
        _subHUD.gameObject.SetActive(true);
    }
    /// <summary>
    /// 시도 횟수 텍스트를 업데이트합니다.
    /// </summary>
    public void UpdateTryCountText()
    {
        _HUD.UpdateStatusText();
    }
    /// <summary>
    /// 보스의 체력 잔량을 업데이트합니다.
    /// </summary>
    public void UpdateBossHealthText()
    {
        ///_bossBattleManager.UpdateBossHealthText();

        /*
        EnemyBossScript[] bosses = _bossBattleManager.Bosses;
        for (int i = 0, len = bosses.Length; i < len; ++i)
        {
            EnemyBossScript boss = bosses[i];
            _bossHudArray[i]._healthText.text = boss.Health.ToString();
        }
        */
    }

    /// <summary>
    /// 보스 HUD를 활성화합니다.
    /// </summary>
    public void ActivateBossHUD()
    {
        /// _bossHUD.gameObject.SetActive(true);
        /*
        foreach (BossHUDScript hud in _bossHudArray)
        {
            hud.gameObject.SetActive(true);
        }
        */

        // 
        EnemyBossUnit[] bosses = (from unit in BattleManager.Instance._units
                                  where unit is EnemyBossUnit
                                  select unit as EnemyBossUnit).ToArray();

        //
        for (int i = 0; i < _bossHudArray.Length; ++i)
        {
            EnemyBossUnit boss = bosses[i];
            BossHUDScript hud = _bossHudArray[i];

            // 
            hud.gameObject.SetActive(true);
            hud.RequestSetUnit(boss);
        }

    }
    /// <summary>
    /// 보스 HUD를 비활성화합니다.
    /// </summary>
    public void DeactivateBossHUD()
    {
        /// _bossHUD.gameObject.SetActive(false);
        foreach (BossHUDScript hud in _bossHudArray)
        {
            hud.gameObject.SetActive(false);
        }
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("BattleManager로 대체되었습니다.")]
    /// <summary>
    /// 보스 전투 관리자입니다.
    /// </summary>
    BossBattleManager _bossBattleManager;

    #endregion
}
