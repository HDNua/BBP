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
    #region 상수를 정의합니다.
    /// <summary>
    /// 패턴 1에서 Hop 행동 후 잠깐 쉬는 시간입니다.
    /// </summary>
    public float TIME_WAIT_PATTERN1 = 0.4f;

    #endregion



    #region 컨트롤러가 사용할 Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 아타호 적 보스 유닛입니다.
    /// </summary>
    public EnemyBossAtahoUnit _atahoUnit;
    /// <summary>
    /// 린샹 유닛입니다.
    /// </summary>
    public EnemyRinshanUnit _rinshanUnit;
    /// <summary>
    /// 스마슈 유닛입니다.
    /// </summary>
    public EnemySmashuUnit _smashuUnit;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 아타호가 이전에 있던 위치의 인덱스입니다.
    /// </summary>
    public int _previousPositionIndex = 0;
    /// <summary>
    /// 아타호가 현재 있는 위치의 인덱스입니다.
    /// </summary>
    public int _currentPositionIndex = 0;

    /// <summary>
    /// 아타호를 생성할 위치입니다.
    /// </summary>
    public Transform _spawnPosition;

    /// <summary>
    /// 아타호가 쳐다보는 플레이어의 방향입니다.
    /// </summary>
    public Direction _direction;

    /// <summary>
    /// 거리 검사를 위한 타원의 가로 길이입니다.
    /// </summary>
    public float _a = 7;
    /// <summary>
    /// 거리 검사를 위한 타원의 세로 길이입니다.
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
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
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





    #region BattleManager의 Instance가 반드시 정의해야 하는 코루틴 메서드를 정의합니다.
    /// <summary>
    /// 등장 코루틴입니다.
    /// </summary>
    protected override IEnumerator CoroutineAppear()
    {
        // 
        _atahoUnit.transform.position = _spawnPosition.position;

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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="unitIndex"></param>
    public void UpdateTeam(EnemyUnit unit, int unitIndex)
    {
        if (unitIndex == 0)
        {
            _rinshanUnit = (EnemyRinshanUnit)unit;
        }
        else
        {
            _smashuUnit = (EnemySmashuUnit)unit;
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
        return _atahoUnit ? !_atahoUnit.IsAlive() : true;
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
        // 아타호의 남은 마력이 없다면
        // 기술 사용을 위해 마나 회복을 최우선으로 진행해야 합니다.
        // 게임 시작 시에 반드시 아타호의 마력이 0인 상태이므로
        // 플레이어는 아타호의 마력 회복 행동을 통해
        // 아타호가 마력이 부족할 때 마나 회복을 할 것이라고 예상할 수 있습니다.
        if (_atahoUnit._mana == 0)
        {
            _atahoUnit.DrinkMana();

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
        }

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

        // 약간 기다립니다.
        yield return new WaitForSeconds(TIME_WAIT_PATTERN1);

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
            if (atahoUnit._mana >= atahoUnit._maxMana / 3)
            {
                atahoUnit.DoHopokwon();
            }
            else
            {
                atahoUnit.DrinkMana();
            }
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
            if (atahoUnit._mana >= atahoUnit._maxMana / 3)
            {
                atahoUnit.DoHopokwon();
            }
            else
            {
                atahoUnit.DrinkMana();
            }
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
                // 차라리 빨리 도망을 칩시다.
                atahoUnit.SkipAction();
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
                atahoUnit.DrinkMana();
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
            _atahoUnit.DrinkMana();
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
                // 차라리 빨리 도망을 칩시다.
                atahoUnit.SkipAction();
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
                atahoUnit.DrinkMana();
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
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            // 그러나 근거리에 있어 호출하는 것이 별로 이득을 보지 못한다고 판단되면
            // 아타호는 현재 방어를 하는 것으로만 구현되었습니다.
            // 사실 이 부분은 권법가로서의 정신에는 맞지 않기 때문에,
            // 후에 근거리 공격에 적합한 다른 액션을 구현해볼까 합니다.
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            // 거리가 먼 경우에 스마슈를 호출할 수 있으므로, 그대로 진행합니다.
            if (_smashuUnit == null)
            {
                _atahoUnit.CallSmashu(_positions[_previousPositionIndex]);
            }
            // 자신이 마나를 소모하여 원거리의 적을 공격합니다.
            else if (atahoUnit._mana >= atahoUnit._maxMana / 3)
            {
                atahoUnit.DoHopokwon();
            }
            // 마나를 회복합니다.
            else
            {
                atahoUnit.DrinkMana();
            }
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
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            // 그러나 근거리에 있어 호출하는 것이 별로 이득을 보지 못한다고 판단되면
            // 아타호는 현재 방어를 하는 것으로만 구현되었습니다.
            // 사실 이 부분은 권법가로서의 정신에는 맞지 않기 때문에,
            // 후에 근거리 공격에 적합한 다른 액션을 구현해볼까 합니다.
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            // 거리가 먼 경우에 스마슈를 호출할 수 있으므로, 그대로 진행합니다.
            if (_smashuUnit == null)
            {
                _atahoUnit.CallSmashu(_positions[_previousPositionIndex]);
            }
            // 자신이 마나를 소모하여 원거리의 적을 공격합니다.
            else if (atahoUnit._mana >= atahoUnit._maxMana / 3)
            {
                atahoUnit.DoHopokwon();
            }
            // 마나를 회복합니다.
            else
            {
                atahoUnit.DrinkMana();
            }
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
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            // 그러나 근거리에 있어 호출하는 것이 별로 이득을 보지 못한다고 판단되면
            // 아타호는 현재 방어를 하는 것으로만 구현되었습니다.
            // 사실 이 부분은 권법가로서의 정신에는 맞지 않기 때문에,
            // 후에 근거리 공격에 적합한 다른 액션을 구현해볼까 합니다.
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            // 거리가 먼 경우에 스마슈를 호출할 수 있으므로, 그대로 진행합니다.
            if (_smashuUnit == null)
            {
                _atahoUnit.CallSmashu(_positions[_previousPositionIndex]);
            }
            // 자신이 마나를 소모하여 원거리의 적을 공격합니다.
            else if (atahoUnit._mana >= atahoUnit._maxMana / 3)
            {
                atahoUnit.DoHopokwon();
            }
            // 마나를 회복합니다.
            else
            {
                atahoUnit.DrinkMana();
            }
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

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}