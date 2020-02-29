using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region 상수 및 형식을 정의합니다.
/// <summary>
/// 
/// </summary>
public enum Direction
{
    LU, U, RU,
    L, M, R,
    LD, D, RD
}

#endregion



/// <summary>
/// 전투 관리자입니다.
/// </summary>
public abstract class BattleManager : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 전투 유닛입니다.
    /// </summary>
    public Unit[] _units;
    /// <summary>
    /// 전투 환경 설정 시에 프레임 단위로 회복할 양입니다.
    /// </summary>
    public int _healStep = 10;

    /// <summary>
    /// 전투 페이즈입니다.
    /// </summary>
    public int _phase;

    /// <summary>
    /// 보스 사망 효과입니다.
    /// </summary>
    public BossDeadEffectScript[] _bossDeadEffects;

    #endregion





    #region Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 스테이지 관리자입니다.
    /// </summary>
    protected StageManager _stageManager;
    /// <summary>
    /// 사용자 인터페이스 관리자입니다.
    /// </summary>
    protected UIManager _uiManager;

    #endregion





    #region 필드를 정의합니다.
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





    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 전투 관리자입니다.
    /// </summary>
    public static BattleManager Instance
    {
        get
        {
            return GameObject.FindGameObjectWithTag("BattleManager")
                .GetComponent<BattleManager>();
        }
    }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    public virtual void Awake()
    {
        _stageManager = StageManager.Instance;
        _uiManager = UIManager.Instance;
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    public virtual void Start()
    {

    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    public virtual void Update()
    {
        // 전투 중이라면
        if (_fighting)
        {

        }
        // 전투가 종료되었다면
        else if (DoesBattleEnd())
        {

        }
        // 전투 준비중이라면
        else if (_readying)
        {

        }
        // 대화 중이라면
        else if (_scripting)
        {

        }
        // 등장 중이라면
        else if (_appearing)
        {

        }
        // 경고 중이라면
        else if (_warning)
        {

        }
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    public virtual void FixedUpdate()
    {

    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    public virtual void LateUpdate()
    {

    }

    #endregion





    #region 단계 메서드를 정의합니다.
    /// <summary>
    /// 경고 화면을 표시합니다.
    /// </summary>
    protected void Warning()
    {
        _warning = true;

        // 경고를 시작합니다.
        StartCoroutine(CoroutineWarning());
    }
    /// <summary>
    /// 등장을 시작합니다.
    /// </summary>
    protected void Appear()
    {
        _warning = false;
        _appearing = true;

        // 등장을 진행합니다.
        StartCoroutine(CoroutineAppear());
    }
    /// <summary>
    /// 대화를 시작합니다.
    /// </summary>
    protected void Script()
    {
        _appearing = false;
        _scripting = true;

        // 대사를 진행합니다.
        StartCoroutine(CoroutineScript());
    }
    /// <summary>
    /// 전투 준비를 시작합니다.
    /// </summary>
    protected void Ready()
    {
        _scripting = false;
        _readying = true;

        // 전투 준비를 진행합니다.
        StartCoroutine(CoroutineReady());
    }
    /// <summary>
    /// 전투 환경을 설정합니다.
    /// 보스 HUD가 나오고 체력이 채워지는 단계입니다.
    /// </summary>
    protected void SetupBattle()
    {
        // 전투 시작 코루틴을 실행합니다.
        StartCoroutine(CoroutineSetupBattle());
    }
    /// <summary>
    /// 전투를 시작합니다.
    /// </summary>
    protected void Fight()
    {
        _readying = false;
        _fighting = true;

        // 전투를 위해 입력 요청을 푸는 등의 정리를 합니다.
        _stageManager.RequestUnblockMoving();
        GetComponent<AudioSource>().Play();

        // 전투 코루틴을 수행합니다.
        StartCoroutine(CoroutineFight());
    }
    /// <summary>
    /// 전투를 끝냅니다.
    /// </summary>
    protected void EndBattle()
    {
        // 전투 종료 코루틴을 수행합니다.
        StartCoroutine(CoroutineEndBattle());
    }

    /// <summary>
    /// 전투가 종료되었는지 확인합니다.
    /// </summary>
    /// <returns>관찰중인 모든 적 유닛이 죽었다면 참입니다.</returns>
    public virtual bool DoesBattleEnd()
    {
        foreach (Unit unit in _units)
        {
            if (unit.IsAlive())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 경고 화면 코루틴입니다.
    /// </summary>
    protected virtual IEnumerator CoroutineWarning()
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
    /// 전투 환경 설정 코루틴입니다.
    /// </summary>
    protected virtual IEnumerator CoroutineSetupBattle()
    {
        // 전투 HUD를 활성화합니다.
        ActivateBattleHUD();

        // 보스 체력 재생을 요청합니다.
        RequestFillHealth();

        // 전투를 시작합니다.
        Fight();
        yield break;
    }

    #endregion





    #region BattleManager의 Instance가 반드시 정의해야 하는 코루틴 메서드를 정의합니다.
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    protected abstract IEnumerator CoroutineAppear();
    /// <summary>
    /// 대화 코루틴입니다.
    /// </summary>
    protected abstract IEnumerator CoroutineScript();
    /// <summary>
    /// 전투 준비 코루틴입니다.
    /// </summary>
    protected abstract IEnumerator CoroutineReady();
    /// <summary>
    /// 전투 코루틴입니다.
    /// </summary>
    protected abstract IEnumerator CoroutineFight();
    /// <summary>
    /// 전투 종료 코루틴입니다.
    /// </summary>
    protected abstract IEnumerator CoroutineEndBattle();

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 전투 HUD를 활성화합니다.
    /// </summary>
    protected virtual void ActivateBattleHUD()
    {
        UIManager.Instance.ActivateBattleHUD();
    }
    /// <summary>
    /// 보스 체력 재생을 요청합니다.
    /// </summary>
    protected virtual void RequestFillHealth()
    {
        foreach (EnemyBossUnit unit in _units)
        {
            _stageManager.HealBoss(unit, _healStep);
        }
    }

    #endregion





    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// 시나리오를 시작합니다.
    /// </summary>
    public void RequestStart()
    {
        Warning();
    }

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}
