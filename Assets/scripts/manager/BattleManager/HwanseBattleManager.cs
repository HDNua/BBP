using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



/// <summary>
/// 환세 스테이지의 전투 관리자입니다.
/// </summary>
public class HwanseBattleManager : BattleManager
{
    #region 컨트롤러가 사용할 Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 아타호 적 보스 유닛입니다.
    /// </summary>
    EnemyBossAtahoUnit _atahoUnit;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    int _previousPositionIndex = 0;
    /// <summary>
    /// 
    /// </summary>
    int _currentPositionIndex = 0;

    /// <summary>
    /// 
    /// </summary>
    public Direction _direction;

    /// <summary>
    /// 
    /// </summary>
    public float _a = 7;
    /// <summary>
    /// 
    /// </summary>
    public float _b = 4;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회만 수행)
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        // 
        _atahoUnit = (EnemyBossAtahoUnit)_units[0];
    }

    #endregion





    #region BattleManager의 Instance가 반드시 정의해야 하는 코루틴 메서드를 정의합니다.
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineAppear()
    {
        // 모든 보스를 등장시킵니다.
        _atahoUnit.gameObject.SetActive(true);
        _atahoUnit.Appear();
        while (_atahoUnit.IsActionEnded == false)
        {
            yield return false;
        }

        // 인물 간 대사를 출력합니다.
        Script();
        yield break;
    }
    /// <summary>
    /// 대화 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineScript()
    {
        Ready();
        yield break;
    }
    /// <summary>
    /// 전투 준비 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineReady()
    {
        // 전투를 시작합니다.
        SetupBattle();
        yield break;
    }
    /// <summary>
    /// 전투 시작 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineSetupBattle()
    {
        // 보스 캐릭터 체력 바를 표시합니다.
        ActivateBattleHUD();

        // 보스 체력 재생을 요청합니다.
        RequestFillHealth();
        while (_atahoUnit.Health < _atahoUnit.MaxHealth)
        {
            yield return false;
        }

        // 전투를 시작합니다.
        ActivateBattleDamageHUD();
        Fight();
        yield break;
    }
    /// <summary>
    /// 전투 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineFight()
    {
        while (DoesBattleEnd() == false)
        {
            switch (_phase)
            {
                case 0:
                    _coroutinePattern = StartCoroutine(CoroutinePattern1());
                    break;
                case 1:

                    break;
                case 2:

                    break;
                default:

                    break;
            }

            // 
            while (_coroutinePattern != null)
            {
                yield return false;
            }
        }

        // 
        if (_coroutinePattern != null)
        {
            StopCoroutine(_coroutinePattern);
        }
        _coroutinePattern = null;
        EndBattle();
        yield break;
    }
    /// <summary>
    /// 전투 종료 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineEndBattle()
    {
        yield break;
    }

    #endregion





    #region 요청 메서드를 정의합니다.
    /// <summary>
    /// 대미지 바를 활성화 합니다.
    /// </summary>
    public void ActivateBattleDamageHUD()
    {
        foreach (HwanseBattleHUD hud in _uiManager._battleHudArray)
        {
            hud._damageBar.SetActive(true);
            hud._manaDamageBar.SetActive(true);
            hud._expDamageBar.SetActive(true);
        }
    }
    
    #endregion





    #region 기타 재정의할 메서드입니다.
    /// <summary>
    /// 전투 종료 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineWarning()
    {
        return base.CoroutineWarning();
    }
    /// <summary>
    /// 전투가 종료되었는지 확인합니다.
    /// </summary>
    /// <returns>관찰중인 모든 적 유닛이 죽었다면 참입니다.</returns>
    public override bool DoesBattleEnd()
    {
        return base.DoesBattleEnd();
    }

    #endregion





    #region 유닛 행동 메서드를 정의합니다.
    /// <summary>
    /// 이동할 위치 집합입니다.
    /// </summary>
    public Transform[] _positions;

    /// <summary>
    /// 다음 뛸 지점의 집합을 반환합니다.
    /// </summary>
    /// <returns>다음 뛸 지점의 집합을 반환합니다.</returns>\
    int[] GetNextHopPositionArray()
    {
        float[] diffs =
        {
            Vector3.Distance(_atahoUnit.transform.position, _positions[0].position),
            Vector3.Distance(_atahoUnit.transform.position, _positions[1].position),
            Vector3.Distance(_atahoUnit.transform.position, _positions[2].position),
            Vector3.Distance(_atahoUnit.transform.position, _positions[3].position),
            Vector3.Distance(_atahoUnit.transform.position, _positions[4].position),
            Vector3.Distance(_atahoUnit.transform.position, _positions[5].position),
            Vector3.Distance(_atahoUnit.transform.position, _positions[6].position),
        };

        // 
        float minDist = Mathf.Min(diffs);

        // 
        int[] hopPositionArray;
        if (minDist == diffs[0])
        {
            hopPositionArray = new int[] { 1, 2 };
        }
        else if (minDist == diffs[1])
        {
            hopPositionArray = new int[] { 0, 3 };
        }
        else if (minDist == diffs[2])
        {
            hopPositionArray = new int[] { 0, 3, 4 };
        }
        else if (minDist == diffs[3])
        {
            hopPositionArray = new int[] { 1, 2, 4, 5 };
        }
        else if (minDist == diffs[4])
        {
            hopPositionArray = new int[] { 2, 3, 6 };
        }
        else if (minDist == diffs[5])
        {
            hopPositionArray = new int[] { 3, 6 };
        }
        else if (minDist == diffs[6])
        {
            hopPositionArray = new int[] { 4, 5 };
        }
        else
        {
            hopPositionArray = new int[] { 1, 2 };
        }

        // 
        return hopPositionArray;
    }

    #endregion





    #region 유닛 패턴 메서드를 정의합니다.
    /// <summary>
    /// 패턴 코루틴입니다.
    /// </summary>
    Coroutine _coroutinePattern;
    /// <summary>
    /// 행동 코루틴입니다.
    /// </summary>
    Coroutine _subcoroutineAction;

    /// <summary>
    /// 1번 패턴입니다.
    /// </summary>
    IEnumerator CoroutinePattern1()
    {
        //////////////////////////////////////////////////////////
        // 
        _subcoroutineAction = StartCoroutine(SubcoroutineHop());
        while (_subcoroutineAction != null)
        {
            yield return false;
        }
        while (_atahoUnit.IsAnimatorInState("FallEnd"))
        {
            yield return false;
        }

        // 1초 기다립니다.
        yield return new WaitForSeconds(1f);

        //////////////////////////////////////////////////////////
        // 
        PlayerController player = _stageManager.MainPlayer;
        if (player == null || _atahoUnit.IsDead)
        {
            yield break;
        }

        // 
        UpdateCondition(_atahoUnit, player);
        switch (_direction)
        {
            case Direction.LU:
                PerformActionLU(_atahoUnit, player);
                break;
            case Direction.U:
                PerformActionU(_atahoUnit, player);
                break;
            case Direction.RU:
                PerformActionRU(_atahoUnit, player);
                break;
            case Direction.L:
                PerformActionL(_atahoUnit, player);
                break;
            case Direction.R:
                PerformActionR(_atahoUnit, player);
                break;
            case Direction.LD:
                PerformActionLD(_atahoUnit, player);
                break;
            case Direction.D:
                PerformActionD(_atahoUnit, player);
                break;
            case Direction.RD:
                PerformActionRD(_atahoUnit, player);
                break;
            default:
                PerformActionM(_atahoUnit, player);
                break;
        }

        // 행동이 종료될 때까지 대기합니다.
        while (_atahoUnit.IsActionStarted == false)
        {
            yield return false;
        }
        while (_atahoUnit.IsActionRunning)
        {
            yield return false;
        }
        while (_atahoUnit.IsActionEnded == false)
        {
            yield return false;
        }
        _coroutinePattern = null;
        yield break;
    }
    /// <summary>
    /// Hop 서브 코루틴입니다.
    /// </summary>
    IEnumerator SubcoroutineHop()
    {
        // 
        int[] nextHopPositionArray = GetNextHopPositionArray();
        int newPositionIndex = nextHopPositionArray
            [Random.Range(0, nextHopPositionArray.Length)];

        // 
        _previousPositionIndex = _currentPositionIndex;
        _currentPositionIndex = newPositionIndex;

        // 
        Transform newPosition = _positions[newPositionIndex];
        _atahoUnit.HopTo(newPosition);

        //
        while (_atahoUnit.Hopping)
        {
            yield return false;
        }

        // 
        _subcoroutineAction = null;
        yield return true;
    }

    #endregion





    #region 유닛 전략 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionLU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
        }
        else
        {
            _atahoUnit.Guard();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
        }
        else
        {
            _atahoUnit.Guard();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionRU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
        }
        else
        {
            _atahoUnit.Guard();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionL(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit._mana >= atahoUnit.MANA_HOKYOKKWON)
            {
                atahoUnit.DoHokyokkwon();
            }
            else
            {
                atahoUnit.Guard();
            }
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit._mana >= atahoUnit.MANA_HOKYOKKWON)
            {
                atahoUnit.DoHokyokkwon();
            }
            else
            {
                atahoUnit.Guard();
            }
        }
        else
        {
            if (atahoUnit._mana >= atahoUnit.MANA_HOKYOKKWON)
            {
                atahoUnit.DoHokyokkwon();
            }
            else
            {
                atahoUnit.Guard();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionM(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
        }
        else
        {
            _atahoUnit.Guard();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionR(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit._mana >= atahoUnit.MANA_HOKYOKKWON)
            {
                atahoUnit.DoHokyokkwon();
            }
            else
            {
                atahoUnit.Guard();
            }
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit._mana >= atahoUnit.MANA_HOKYOKKWON)
            {
                atahoUnit.DoHokyokkwon();
            }
            else
            {
                atahoUnit.Guard();
            }
        }
        else
        {
            if (atahoUnit._mana >= atahoUnit.MANA_HOKYOKKWON)
            {
                atahoUnit.DoHokyokkwon();
            }
            else
            {
                atahoUnit.Guard();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionLD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
        }
        else
        {
            _atahoUnit.Guard();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
        }
        else
        {
            _atahoUnit.Guard();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="atahoUnit"></param>
    /// <param name="player"></param>
    void PerformActionRD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsNear(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
        }
        else
        {
            _atahoUnit.Guard();
        }
    }

    #endregion





    #region 유닛 전략 구성에 사용되는 행동 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public void CallUnit(EnemyUnit unit)
    {
        // 새 유닛을 생성합니다.
        int newUnitPositionIndex = _previousPositionIndex;
        Transform newUnitPosition = _positions[newUnitPositionIndex];
        Unit newUnit = Instantiate(
            _units[1],
            newUnitPosition.position,
            newUnitPosition.rotation,
            _stageManager._enemyParent.transform
            );
        newUnit.gameObject.SetActive(true);
    }

    #endregion





    #region 유닛 전략 구성을 위한 보조 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public Vector3 _dv;
    /// <summary>
    /// 
    /// </summary>
    public float _angle;
    /// <summary>
    /// 
    /// </summary>
    public float _distance;
    /// <summary>
    /// 
    /// </summary>
    public float _ellipseR;


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Direction GetDirectionToTarget(Transform st, Transform dt)
    {
        _dv = dt.position - st.position;
        float dx = _dv.x;
        float dy = _dv.y;
        Direction direction = Direction.M;

        _angle = Vector3.SignedAngle(Vector3.right, _dv.normalized, Vector3.forward);
        if (0 - 22.5 <= _angle && _angle < 0 + 22.5)
        {
            direction = Direction.R;
        }
        else if (45 - 22.5 <= _angle && _angle < 45 + 22.5)
        {
            direction = Direction.RU;
        }
        else if (90 - 22.5 <= _angle && _angle < 90 + 22.5)
        {
            direction = Direction.U;
        }
        else if (135 - 22.5 <= _angle && _angle < 135 + 22.5)
        {
            direction = Direction.LU;
        }
        else if (180 - 22.5 <= _angle || _angle < -180 + 22.5)
        {
            direction = Direction.L;
        }
        else if (-135 - 22.5 <= _angle && _angle < -135 + 22.5)
        {
            direction = Direction.LD;
        }
        else if (-90 - 22.5 <= _angle && _angle < -90 + 22.5)
        {
            direction = Direction.D;
        }
        else if (-45 - 22.5 <= _angle && _angle < -45 + 22.5)
        {
            direction = Direction.RD;
        }
        else
        {
            direction = Direction.M;
        }

        //
        return direction;
    }
    /// <summary>
    /// 플레이어와의 거리가 가까운지를 확인합니다.
    /// </summary>
    /// <returns>플레이어와의 거리가 가깝다면 참입니다.</returns>
    bool IsNear(Transform st, Transform dt)
    {
        return (_distance < _ellipseR);
    }
    /// <summary>
    /// 플레이어와의 거리가 먼지를 확인합니다.
    /// </summary>
    /// <returns>플레이어와의 거리가 멀다면 참입니다.</returns>
    bool IsFar(Transform st, Transform dt)
    {
        return (_distance >= _ellipseR);
    }


    /// <summary>
    /// 패턴 수행을 위한 조건들을 초기화합니다.
    /// </summary>
    /// <param name="unit">유닛입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void UpdateCondition(Unit unit, PlayerController player)
    {
        _direction = GetDirectionToTarget(unit.transform, player.transform);
        _distance = Vector3.Distance(unit.transform.position, player.transform.position);

        // 
        float a = _a;
        float b = _b;
        float sin_t = Mathf.Sin(_angle * Mathf.Deg2Rad);
        float cos_t = Mathf.Cos(_angle * Mathf.Deg2Rad);
        _ellipseR = Mathf.Sqrt(a * a * cos_t * cos_t + b * b * sin_t * sin_t);
    }
    /// <summary>
    /// 
    /// </summary>
    public override void LateUpdate()
    {
        if (_atahoUnit)
        {
            PlayerController player = _stageManager.MainPlayer;
            UpdateCondition(_atahoUnit, player);

            //
            Transform st = _atahoUnit.transform;
            Transform dt = player.transform;
            Vector3 direction = (dt.position - st.position).normalized;

            Debug.DrawRay(st.position, direction, Color.yellow);

            // 
            Vector3 dstVector = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad), Mathf.Sin(_angle * Mathf.Deg2Rad)) * _ellipseR;
            Debug.DrawRay(st.position, dstVector, Color.red);
        }
    }

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}