using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 보스 HUD 스크립트입니다.
/// </summary>
public class BossHUDScript : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 체력 바입니다.
    /// </summary>
    public GameObject _healthBar;
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

    #endregion



    #region Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 
    /// </summary>
    BattleManager _battleManager;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    void Awake()
    {
        ///_bossBattleManager = BossBattleManager.Instance;
        _battleManager = BattleManager.Instance;
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    void Start()
    {

    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    void Update()
    {
        if (_unit != null)
        {
            // 체력을 업데이트 합니다.
            Vector3 healthScale = _healthBar.transform.localScale;
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
        }
    }

    #endregion





    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    EnemyBossUnit _unit;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public void RequestSetUnit(EnemyBossUnit unit)
    {
        _unit = unit;
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("BattleManager로 대체되었습니다.")]
    /// <summary>
    /// 보스 전투 관리자입니다.
    /// </summary>
    BossBattleManager _bossBattleManager;

    [Obsolete("EnemyBossUnit으로 대체되었습니다.")]
    /// <summary>
    /// HUD를 표현할 보스 캐릭터입니다.
    /// </summary>
    public EnemyBossScript _boss;

    #endregion
}
