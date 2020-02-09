﻿using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 보스 전투 관리자입니다.
/// </summary>
public class BossBattleManager : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 데이터베이스입니다.
    /// </summary>
    public DataBase _database;

    /// <summary>
    /// 보스 리스트입니다.
    /// </summary>
    public EnemyBossScript[] _bosses;

    /// <summary>
    /// 
    /// </summary>
    public BossBattlePattern _bossBattlePattern;

    /// <summary>
    /// 
    /// </summary>
    public BossDeadEffectScript[] _bossDeadEffects;
    /// <summary>
    /// 
    /// </summary>
    public BossDeadEffectScript[] _lastBossDeadEffects;

    /// <summary>
    /// 
    /// </summary>
    public GameObject _enemyParent;

    #endregion





    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 보스 리스트입니다.
    /// </summary>
    public EnemyBossScript[] Bosses
    {
        get { return _bosses; }
    }

    #endregion





    #region Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 보스 전투 관리자입니다.
    /// </summary>
    public static BossBattleManager Instance
    {
        get
        {
            return GameObject.FindGameObjectWithTag("BossBattleManager")
                .GetComponent<BossBattleManager>();
        }
    }

    /// <summary>
    /// 스테이지 관리자입니다.
    /// </summary>
    StageManager _stageManager;
    /// <summary>
    /// 사용자 인터페이스 관리자입니다.
    /// </summary>
    UIManager _userInterfaceManager;

    #endregion





    #region 필드를 정의합니다.
    // 절차:
    // 1. 경고
    // 2. 등장
    // 3. 대사 (생략 가능)
    // 4. 준비
    // 5. 시작

    /// <summary>
    /// 경고 중이라면 참입니다.
    /// </summary>
    bool _warning = false;
    /// <summary>
    /// 등장 중이라면 참입니다.
    /// </summary>
    bool _appearing = false;
    /// <summary>
    /// 대사 중이라면 참입니다.
    /// </summary>
    bool _scripting = false;
    /// <summary>
    /// 준비 중이라면 참입니다.
    /// </summary>
    bool _readying = false;
    /// <summary>
    /// 전투 중이라면 참입니다.
    /// </summary>
    bool _fighting = false;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    void Awake()
    {
        _stageManager = StageManager.Instance;
        _userInterfaceManager = _database.UIManager;
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    void Start()
    {
        // _stageManager = StageManager.Instance;
        // _userInterfaceManager = _database.UIManager;
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    void Update()
    {
        if (_fighting)
        {

        }
        else if (IsEveryBossesDead())
        {

        }
        else if (_readying)
        {

        }
        else if (_scripting)
        {

        }
        else if (_appearing)
        {

        }
        else if (_warning)
        {

        }
    }

    #endregion

    



    #region 메서드를 정의합니다.
    /// <summary>
    /// 보스 캐릭터 체력 바를 표시합니다.
    /// </summary>
    void ActivateBossHUD()
    {
        _userInterfaceManager.ActivateBossHUD();
    }
    /// <summary>
    /// 모든 보스가 죽었는지를 확인합니다.
    /// </summary>
    /// <returns>모든 보스가 죽었다면 참입니다.</returns>
    public bool IsEveryBossesDead()
    {
        foreach (EnemyBossScript boss in _bosses)
        {
            if (boss.IsAlive())
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    



    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// 보스 전투 시나리오 실행을 요청합니다.
    /// </summary>
    public void RequestBossBattleScenario()
    {
        Warning();
    }
    /// <summary>
    /// 보스 체력 재생을 요청합니다.
    /// </summary>
    public void RequestFillHealth()
    {
        foreach (EnemyBossScript boss in _bosses)
        {
            _stageManager.HealBoss(boss);
        }
    }

    #endregion



    

    #region 시작 메서드를 정의합니다.
    /// <summary>
    /// 경고 화면을 표시합니다.
    /// </summary>
    void Warning()
    {
        _warning = true;

        // 경고를 시작합니다.
        StartCoroutine(CoroutineWarning());
    }
    /// <summary>
    /// 등장을 시작합니다.
    /// </summary>
    void Appear()
    {
        _warning = false;
        _appearing = true;

        // 등장을 진행합니다.
        StartCoroutine(CoroutineAppearing());
    }
    /// <summary>
    /// 대사를 시작합니다.
    /// </summary>
    void Script()
    {
        _appearing = false;
        _scripting = true;

        // 대사를 진행합니다.
        StartCoroutine(CoroutineScripting());
    }
    /// <summary>
    /// 전투 준비를 시작합니다.
    /// </summary>
    void Ready()
    {
        _scripting = false;
        _readying = true;

        // 전투 준비를 진행합니다.
        StartCoroutine(CoroutineReadying());
    }
    /// <summary>
    /// 전투를 시작합니다.
    /// </summary>
    void Fight()
    {
        _readying = false;
        _fighting = true;

        /*
        // 모든 보스에게 전투 시작을 요청합니다.
        foreach (EnemyBossScript boss in _bosses)
        {
            boss.Fight();
        }
        */

        // 
        _bossBattlePattern.Fight();


        // 전투를 위해 입력 요청을 푸는 등의 정리를 합니다.
        _stageManager.RequestUnblockMoving();
        GetComponent<AudioSource>().Play();

        // 전투 코루틴을 수행합니다.
        StartCoroutine(CoroutineFighting());
    }
    /// <summary>
    /// 전투를 끝냅니다.
    /// </summary>
    void EndBattle()
    {
        // 전투 종료 코루틴을 수행합니다.
        StartCoroutine(CoroutineEndBattle());
    }

    /// <summary>
    /// 경고 화면 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineWarning()
    {
        // 보스 전투 전처리를 진행합니다.
        _stageManager.RequestBlockMoving();
        _stageManager.StopBackgroundMusic();

        // 경고 애니메이션을 재생합니다.
        _stageManager.RequestPlayingWarningAnimation();

        // 경고음을 재생합니다.
        AudioClip warningSound = _stageManager._audioClips[8];
        AudioSource warningSource = _stageManager.AudioSources[8];
        float length = warningSound.length * 0.9f;

        warningSource.Play();
        yield return new WaitForSeconds(length);
        warningSource.Play();
        yield return new WaitForSeconds(length);
        warningSource.Play();
        yield return new WaitForSeconds(length);
        warningSource.Play();
        yield return new WaitForSeconds(length);

        // 보스가 등장합니다.
        Appear();
        yield break;
    }
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineAppearing()
    {
        // 모든 보스를 등장시킵니다.
        foreach (EnemyBossScript boss in _bosses)
        {
            boss.gameObject.SetActive(true);
            boss.Appear();
            while (boss.AppearEnded == false)
            {
                yield return false;
            }
        }

        // 인물 간 대사를 출력합니다.
        Script();
        yield break;
    }
    /// <summary>
    /// 대사 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineScripting()
    {
        // TODO: 인물 간 대사 출력은 아직 구현되지 않았습니다.

        // 전투 준비를 시작합니다.
        Ready();
        yield break;
    }
    /// <summary>
    /// 전투 준비 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineReadying()
    {
        // 보스 캐릭터 체력 바를 표시합니다.
        ActivateBossHUD();

        // 보스 체력 재생을 요청합니다.
        RequestFillHealth();

        // 스테이지에 존재하는 모든 보스의 체력이 전부 찰 때까지 루프합니다.
        foreach (EnemyBossScript boss in _bosses)
        {
            while (boss.IsHealthFull() == false)
                yield return false;
        }

        // 전투를 시작합니다.
        Fight();
        yield break;
    }
    /// <summary>
    /// 전투 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineFighting()
    {
        // 모든 보스가 죽었다면 전투를 끝냅니다.
        foreach (EnemyBossScript boss in _bosses)
        {
            if (IsEveryBossesDead())
            {
                EndBattle();
            }
        }
        yield break;
    }
    /// <summary>
    /// 전투 종료 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineEndBattle()
    {
        _stageManager.AudioSources[9].Play();
        yield return new WaitForSeconds(_stageManager._audioClips[9].length);

        // 
        yield break;
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("_boss는 구형 정의입니다. _bosses를 대신 사용하십시오.")]
    /// <summary>
    /// 보스 캐릭터입니다.
    /// </summary>
    public EnemyBossScript _boss;

    [Obsolete("_bossDeadEffects를 대신 사용하십시오.")]
    /// <summary>
    /// 보스 사망 시 효과입니다.
    /// </summary>
    public BossDeadEffectScript _bossDeadEffect;

    [Obsolete("_lastBossDeadEffects를 대신 사용하십시오.")]
    /// <summary>
    /// 보스 사망 시 효과입니다.
    /// </summary>
    public BossDeadEffectScript _lastBossDeadEffect;


    #endregion
}