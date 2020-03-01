﻿using System;
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

    /// <summary>
    /// 플레이어와 바닥의 거리 차이 threshold입니다.
    /// </summary>
    public float THRESHOLD_GROUND_DIFF = 1f;

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
    /// 이동할 위치 집합입니다.
    /// </summary>
    public Transform[] _positions;

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

    /// <summary>
    /// 원점(아타호)에서 플레이어와의 벡터입니다.
    /// </summary>
    public Vector3 _dv;
    /// <summary>
    /// 원점(아타호)에서 플레이어와의 벡터의 각도입니다.
    /// </summary>
    public float _angle;
    /// <summary>
    /// 원점(아타호)에서 플레이어와의 거리입니다. Near/Far 판단에 사용됩니다.
    /// </summary>
    public float _distance;
    /// <summary>
    /// 원점(아타호)에서 타원의 꼭짓점까지의 거리입니다. Near/Far 판단에 사용됩니다.
    /// </summary>
    public float _ellipseR;

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
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    public override void Update()
    {
        base.Update();
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
            // 아타호 패턴을 수행합니다.
            if (_coroutineAtahoPattern == null)
            {
                switch (_phase)
                {
                    case 0:
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern1());
                        break;
                    case 1:
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern1());
                        break;
                    case 2:
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern1());
                        break;
                    default:
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern1());
                        break;
                }

                /*
                // 패턴이 종료되기 전까지 다음 패턴의 실행을 막습니다.
                while (_coroutineAtahoPattern != null)
                {
                    yield return false;
                }
                */
            }

            // 스마슈 유닛이 존재한다면 스마슈에 대한 패턴도 수행합니다.
            if (_smashuUnit)
            {
                if (_smashuUnit.Appearing)
                {

                }
                else if (_coroutineSmashuPattern == null)
                {
                    switch (_phase)
                    {
                        case 0:
                            _coroutineSmashuPattern = StartCoroutine(CoroutineSmashuPattern1());
                            break;
                        case 1:
                            _coroutineSmashuPattern = StartCoroutine(CoroutineSmashuPattern1());
                            break;
                        case 2:
                            _coroutineSmashuPattern = StartCoroutine(CoroutineSmashuPattern1());
                            break;
                        default:
                            _coroutineSmashuPattern = StartCoroutine(CoroutineSmashuPattern1());
                            break;
                    }
                }
            }
            else if (_coroutineSmashuPattern != null)
            {
                StopCoroutine(_coroutineSmashuPattern);
                _coroutineSmashuPattern = null;
            }

            // 코루틴 단위 수행의 끝입니다.
            yield return false;
        }

        // 패턴이 아직 종료되지 않은 채로 남아있다면 확실하게 중지합니다.
        if (_coroutineAtahoPattern != null)
        {
            StopCoroutine(_coroutineAtahoPattern);
        }
        _coroutineAtahoPattern = null;
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
    /// 팀원 필드를 업데이트 합니다.
    /// </summary>
    /// <param name="unit">업데이트할 유닛입니다.</param>
    /// <param name="unitIndex">유닛 식별자입니다. 0은 린샹, 1은 스마슈입니다.</param>
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
        // 아타호가 생존해있다면 거짓, 죽었다면 참입니다.
        return _atahoUnit ? !_atahoUnit.IsAlive() : true;
    }

    #endregion





    #region 유닛 행동 메서드를 정의합니다.
    #endregion





    #region 아타호 유닛 패턴 메서드를 정의합니다.
    /// <summary>
    /// 패턴 코루틴입니다.
    /// </summary>
    Coroutine _coroutineAtahoPattern;
    /// <summary>
    /// 행동 코루틴입니다.
    /// </summary>
    Coroutine _subcoroutineAtahoAction;

    /// <summary>
    /// 1번 패턴입니다.
    /// </summary>
    IEnumerator CoroutineAtahoPattern1()
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
        _subcoroutineAtahoAction = StartCoroutine(SubcoroutineHop());
        while (_subcoroutineAtahoAction != null)
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
        // 코루틴 도중 둘 중 하나가 끝났다면 코루틴을 중지합니다.
        PlayerController player = _stageManager.MainPlayer;
        if (player == null || _atahoUnit.IsDead)
        {
            yield break;
        }

        // 아타호가 전략을 구상하기 위한 조건들을 초기화합니다.
        UpdateCondition(_atahoUnit, player);

        // 업데이트한 조건을 바탕으로 전략을 수행합니다.
        switch (_direction)
        {
            case Direction.LU:
                PerformAtahoActionLU(_atahoUnit, player);
                break;
            case Direction.U:
                PerformAtahoActionU(_atahoUnit, player);
                break;
            case Direction.RU:
                PerformAtahoActionRU(_atahoUnit, player);
                break;
            case Direction.L:
                PerformAtahoActionL(_atahoUnit, player);
                break;
            case Direction.R:
                PerformAtahoActionR(_atahoUnit, player);
                break;
            case Direction.LD:
                PerformAtahoActionLD(_atahoUnit, player);
                break;
            case Direction.D:
                PerformAtahoActionD(_atahoUnit, player);
                break;
            case Direction.RD:
                PerformAtahoActionRD(_atahoUnit, player);
                break;
            default:
                PerformAtahoActionM(_atahoUnit, player);
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
        _coroutineAtahoPattern = null;
        yield break;
    }
    /// <summary>
    /// Hop 서브 코루틴입니다.
    /// </summary>
    IEnumerator SubcoroutineHop()
    {
        // 다음 점프 위치를 구합니다.
        int[] nextHopPositionArray = GetNextHopPositionArray();
        int newPositionIndex = nextHopPositionArray
            [Random.Range(0, nextHopPositionArray.Length)];

        // 이전에 있던 위치, 현재 위치를 업데이트 합니다.
        _previousPositionIndex = _currentPositionIndex;
        _currentPositionIndex = newPositionIndex;

        // 새 점프할 위치로 실제로 점프하게 합니다.
        Transform newPosition = _positions[newPositionIndex];
        _atahoUnit.HopTo(newPosition);
        while (_atahoUnit.Hopping)
        {
            yield return false;
        }

        // 서브 코루틴을 종료합니다.
        _subcoroutineAtahoAction = null;
        yield return true;
    }

    #endregion





    #region 아타호 유닛 전략 메서드를 정의합니다.
    /// <summary>
    /// 왼쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionLU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
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
    /// 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
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
    /// 오른쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionRU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
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
    /// 왼쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionL(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
        else if (IsNear(atahoUnit.transform, player.transform))
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
    /// 가운데 방향 전략입니다. 실제로 사용되지는 않을 것입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionM(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
        else if (IsNear(atahoUnit.transform, player.transform))
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
    /// 오른쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionR(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
        else if (IsNear(atahoUnit.transform, player.transform))
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
    /// 왼쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionLD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            // 근거리에 있어 호출하는 것이 별로 이득을 보지 못한다고 판단되면
            // 아타호는 현재 방어를 하는 것으로만 구현되었습니다.
            // 사실 이 부분은 권법가로서의 정신과는 약간 다른 듯하여,
            // 후에 근거리 공격에 적합한 다른 액션을 구현해볼까 합니다.
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            // 거리가 먼 경우에 스마슈를 호출할 수 있으므로, 그대로 진행합니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
    /// 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            // 근거리에 있어 호출하는 것이 별로 이득을 보지 못한다고 판단되면
            // 아타호는 현재 방어를 하는 것으로만 구현되었습니다.
            // 사실 이 부분은 권법가로서의 정신과는 약간 다른 듯하여,
            // 후에 근거리 공격에 적합한 다른 액션을 구현해볼까 합니다.
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            // 거리가 먼 경우에 스마슈를 호출할 수 있으므로, 그대로 진행합니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
    /// 오른쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoActionRD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            // 근거리에 있어 호출하는 것이 별로 이득을 보지 못한다고 판단되면
            // 아타호는 현재 방어를 하는 것으로만 구현되었습니다.
            // 사실 이 부분은 권법가로서의 정신과는 약간 다른 듯하여,
            // 후에 근거리 공격에 적합한 다른 액션을 구현해볼까 합니다.
            _atahoUnit.Guard();
        }
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            // 거리가 먼 경우에 스마슈를 호출할 수 있으므로, 그대로 진행합니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                int spawnIndex;
                spawnIndex = 8; // Random.Range(7, 10);
                _atahoUnit.CallSmashu(_positions[spawnIndex]);
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





    #region 아타호 유닛 전략 구성에 사용되는 보조 메서드를 정의합니다.
    /// <summary>
    /// 다음 뛸 지점의 집합을 반환합니다.
    /// </summary>
    /// <returns>다음 뛸 지점의 집합을 반환합니다.</returns>
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
    /// <summary>
    /// 유닛을 소환합니다.
    /// </summary>
    /// <param name="unit">소환할 유닛입니다.</param>
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





    #region 스마슈 전략을 정의합니다.
    /// <summary>
    /// 패턴 코루틴입니다.
    /// </summary>
    Coroutine _coroutineSmashuPattern;
    /// <summary>
    /// 행동 코루틴입니다.
    /// </summary>
    Coroutine _subcoroutineSmashuAction;

    /// <summary>
    /// 1번 패턴입니다.
    /// </summary>
    IEnumerator CoroutineSmashuPattern1()
    {
        if (false)
        {
            /*
            //////////////////////////////////////////////////////////
            // 스마슈의 남은 마력이 없다면
            // 기술 사용을 위해 마나 회복을 최우선으로 진행해야 합니다.
            // 게임 시작 시에 반드시 스마슈의 마력이 0인 상태이므로
            // 플레이어는 스마슈의 마력 회복 행동을 통해
            // 스마슈가 마력이 부족할 때 마나 회복을 할 것이라고 예상할 수 있습니다.
            if (_smashuUnit._mana == 0)
            {
                _smashuUnit.DrinkMana();

                // 행동이 종료될 때까지 대기합니다.
                while (_smashuUnit.IsActionStarted == false)
                {
                    yield return false;
                }
                while (_smashuUnit.IsActionRunning)
                {
                    yield return false;
                }
                while (_smashuUnit.IsActionEnded == false)
                {
                    yield return false;
                }
            }

            //////////////////////////////////////////////////////////
            // 
            _subcoroutineSmashuAction = StartCoroutine(SubcoroutineHop());
            while (_subcoroutineSmashuAction != null)
            {
                yield return false;
            }
            while (_smashuUnit.IsAnimatorInState("FallEnd"))
            {
                yield return false;
            }

            // 약간 기다립니다.
            yield return new WaitForSeconds(TIME_WAIT_PATTERN1);

            //////////////////////////////////////////////////////////
            // 코루틴 도중 둘 중 하나가 끝났다면 코루틴을 중지합니다.
            PlayerController player = _stageManager.MainPlayer;
            if (player == null || _smashuUnit.IsDead)
            {
                yield break;
            }

            // 스마슈가 전략을 구상하기 위한 조건들을 초기화합니다.
            UpdateCondition(_smashuUnit, player);

            // 업데이트한 조건을 바탕으로 전략을 수행합니다.
            switch (_direction)
            {
                case Direction.LU:
                    PerformSmashuActionLU(_smashuUnit, player);
                    break;
                case Direction.U:
                    PerformSmashuActionU(_smashuUnit, player);
                    break;
                case Direction.RU:
                    PerformSmashuActionRU(_smashuUnit, player);
                    break;
                case Direction.L:
                    PerformSmashuActionL(_smashuUnit, player);
                    break;
                case Direction.R:
                    PerformSmashuActionR(_smashuUnit, player);
                    break;
                case Direction.LD:
                    PerformSmashuActionLD(_smashuUnit, player);
                    break;
                case Direction.D:
                    PerformSmashuActionD(_smashuUnit, player);
                    break;
                case Direction.RD:
                    PerformSmashuActionRD(_smashuUnit, player);
                    break;
                default:
                    PerformSmashuActionM(_smashuUnit, player);
                    break;
            }
            */
        }

        // 대타격을 수행합니다.
        _smashuUnit.DoDaetakyok();

        // 행동이 종료될 때까지 대기합니다.
        while (_smashuUnit.IsActionStarted == false)
        {
            yield return false;
        }
        while (_smashuUnit.IsActionRunning)
        {
            yield return false;
        }
        while (_smashuUnit.IsActionEnded == false)
        {
            yield return false;
        }

        // 사라집니다.
        _smashuUnit.Disappear();

        // 행동이 종료될 때까지 대기합니다.
        while (_smashuUnit.IsActionStarted == false)
        {
            yield return false;
        }
        while (_smashuUnit.IsActionRunning)
        {
            yield return false;
        }
        while (_smashuUnit.IsActionEnded == false)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                break;
            }
        }

        // 
        _coroutineSmashuPattern = null;
        yield break;
    }

    #endregion





    #region 유닛 전략 구성을 위한 보조 메서드를 정의합니다.
    /// <summary>
    /// 소스에서 타겟을 바라보는 방향을 업데이트 합니다.
    /// </summary>
    /// <param name="st">유닛의 위치입니다. 보통 아타호가 됩니다.</param>
    /// <param name="dt">타겟의 위치입니다. 보통 플레이어가 됩니다.</param>
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
    /// 타겟이 지상에 있는지를 확인합니다.
    /// </summary>
    /// <param name="targetTransform">타겟 Transform 개체입니다. 보통 플레이어가 됩니다.</param>
    /// <returns>타겟이 지상에 있다면 참입니다.</returns>
    bool IsTargetOnGround(Transform targetTransform)
    {
        // 
        float playerPosY = targetTransform.position.y;
        float groundPosY = _positions[7].position.y;
        float dy = Mathf.Abs(groundPosY - playerPosY);
        return (dy < THRESHOLD_GROUND_DIFF);
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