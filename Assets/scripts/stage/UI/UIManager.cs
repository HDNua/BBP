﻿using System;
using UnityEngine;
using System.Collections;



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
    /// 보스 전투 관리자입니다.
    /// </summary>
    BossBattleManager _bossBattleManager;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    void Awake()
    {
        _bossBattleManager = BossBattleManager.Instance;
    }

    #endregion





    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// 일시정지 상태를 전환합니다.
    /// </summary>
    public void RequestPauseToggle()
    {
        _pauseMenuManager.RequestPauseToggle();
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
        EnemyBossScript[] bosses = _bossBattleManager.Bosses;
        for (int i = 0, len = bosses.Length; i < len; ++i)
        {
            EnemyBossScript boss = bosses[i];
            _bossHudArray[i]._healthText.text = boss.Health.ToString();
        }
    }

    /// <summary>
    /// 보스 HUD를 활성화합니다.
    /// </summary>
    public void ActivateBossHUD()
    {
        /// _bossHUD.gameObject.SetActive(true);
        foreach (BossHUDScript hud in _bossHudArray)
        {
            hud.gameObject.SetActive(true);
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
    [Obsolete("_bossHUD는 더 이상 사용되지 않습니다. _bossHudArray로 대체되었습니다.")]
    /// <summary>
    /// 보스 HUD 개체입니다.
    /// </summary>
    public BossHUDScript _bossHUD;

    #endregion
}
