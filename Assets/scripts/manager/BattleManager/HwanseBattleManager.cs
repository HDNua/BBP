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
    #region 전투 개요입니다.
    // 린샹은 아타호가 위험한 상태에 빠진 이후부터 등장하여
    // 전체 공격기와 버프를 통해 아타호를 보조하는 보스입니다.
    // 전투 불능 상태가 되면 수경으로 벽을 생성하고 전투 자세를 가다듬습니다.
    // 린샹은 그대로 놔두면 지속적으로 방해가 되기 때문에,
    // 플레이어가 아타호만을 공략하는 대신 지속적으로 지상에 내려와서
    // 린샹까지 견제하는 플레이를 유도하게 합니다.
    // 이때 지상에 간헐적으로 등장하는 스마슈가 매우 강력하므로, 이를 잘 피해야 합니다.
    // 즉 환세 스테이지의 보스들은 각각 다음과 같은 포지션이 됩니다.
    // 아타호: 메인 보스. 근거리 및 원거리에서 적을 마주하는 탱커.
    // 린샹: 서브 보스. 원거리에서 아타호를 보조하는 서포터.
    // 스마슈: 서브 보스. 근거리에서 빠른 속도로 치고 빠지는 딜러.
    //
    // 린샹이 서포터 역할을 맡게 된 것에는 여러 가지 이유가 있습니다.
    // 1) 아타호, 린샹, 스마슈를 모두 메인 보스로 등장시키기는 어려웠습니다.
    //    환세취호전 게임의 덤프를 따내면 아타호의 스프라이트가 가장 풍부하고,
    //    린샹과 스마슈가 그 다음인데 이들은 보스의 패턴을 구현하기에는 부족한 양입니다.
    //    따라서 환세취호전의 주인공인 아타호를 메인에 내세우고,
    //    나머지 캐릭터를 전투 중간에 게릴라 식으로 등장시키는 것이 적절했습니다.
    // 2) 린샹과 스마슈가 모두 공격적인 기술을 가지고 있지만,
    //    분신술을 이용하여 임의의 위치에서 자유롭게 등장하는 것이 적절한 스마슈와 달리
    //    린샹의 등장을 디자인하는 것은 예상 외로 어려운 일이었습니다.
    //    스마슈와 똑같은 등장을 한다면 캐릭터의 개성을 잃고 전투를 지루하게 만들 수 있었고,
    //    치고 빠지는 다른 동작을 만드는 것은 벽을 뚫고 들어와야 하는데
    //    그것을 보는 플레이어가 린샹의 등장을 적절하게 느끼게 할 방법을 찾지 못했습니다.
    //    그러던 와중 보스 문을 뚫고 들어온 다음 자신이 들어온 문을 막는 방법을 떠올렸고,
    //    그러려면 린샹이 등장 이후에 다시 왔던 길로 되돌아가서는 안 되었습니다.
    //    따라서 린샹은 아타호의 체력이 까인 중후반부터 등장하는 보스가 되었고,
    //    위치 이동을 구현하기가 상대적으로 까다로운 린샹은 거의 고정적인 위치에서
    //    원거리 공격 또는 버프를 제공하는 서포터 캐릭터로 설정되었습니다.

    #endregion



    #region 상수를 정의합니다.
    /// <summary>
    /// 숨고르기 시간입니다.
    /// </summary>
    public float TIME_WAIT = 0.4f;
    /// <summary>
    /// 패턴 1에서 Hop 행동 후 잠깐 쉬는 시간입니다.
    /// </summary>
    public float TIME_WAIT_PATTERN1 = 0.4f;

    /// <summary>
    /// 스마슈의 생존 시간입니다.
    /// </summary>
    public float TIME_LIFE_SMASHU = 3f;

    /// <summary>
    /// 플레이어와 스테이지 위쪽의 거리 차이 threshold입니다.
    /// </summary>
    public float THRESHOLD_HIGH_DIFF = 1f;
    /// <summary>
    /// 플레이어와 바닥의 거리 차이 threshold입니다.
    /// </summary>
    public float THRESHOLD_GROUND_DIFF = 1f;

    /// <summary>
    /// 동쪽 축 피벗입니다.
    /// </summary>
    const float anglePivotR = 0f;
    /// <summary>
    /// 북쪽 축 피벗입니다.
    /// </summary>
    const float anglePivotU = 90f;
    /// <summary>
    /// 서쪽 축 피벗입니다.
    /// </summary>
    const float anglePivotL = 180f;
    /// <summary>
    /// 남쪽 축 피벗입니다.
    /// </summary>
    const float anglePivotD = -90f;

    #endregion



    #region 컨트롤러가 사용할 Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 아타호 적 보스 유닛입니다.
    /// </summary>
    public EnemyBossAtahoUnit _atahoUnit;
    /// <summary>
    /// 린샹 유닛입니다.
    /// </summary>
    public EnemyBossRinshanUnit _rinshanUnit;
    /// <summary>
    /// 스마슈 유닛입니다.
    /// </summary>
    public EnemyBossSmashuUnit _smashuUnit;

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
    /// 아타호를 소환할 위치입니다.
    /// </summary>
    public Transform _atahoSpawnPosition;
    /// <summary>
    /// 린샹을 소환할 위치입니다.
    /// </summary>
    public Transform _rinshanSpawnPosition;
    /// <summary>
    /// 린샹의 등장이 종료되는 위치입니다.
    /// </summary>
    public Transform _rinshanSpawnEndPosition;
    /// <summary>
    /// 이동할 위치 집합입니다.
    /// </summary>
    public Transform[] _positions;

    /// <summary>
    /// 보스 입장 문입니다.
    /// 환세취호전 전투 관리자가 이것을 참조하는 이유는,
    /// 린샹이 문을 부수고 들어오기 때문입니다.
    /// </summary>
    public BossRoomDoorScript _bossRoomDoor;
    /// <summary>
    /// 문을 파괴하는 효과입니다.
    /// </summary>
    public ParticleSpreadScript _doorDestroyEffect;
    /// <summary>
    /// 문이 파괴된 이후에 플레이어를 전투 밖으로 빠져나가지 못하게 막습니다.
    /// </summary>
    public InvisibleWallScript _doorInvisibleWall;

    /// <summary>
    /// 아타호가 쳐다보는 플레이어의 방향입니다.
    /// </summary>
    public Direction _direction;

    /// <summary>
    /// 오른쪽 위 방향 각도 중심입니다.
    /// </summary>
    public float anglePivotRU = 45f;
    /// <summary>
    /// 왼쪽 위 방향 각도 중심입니다.
    /// </summary>
    public float anglePivotLU = 135f;
    /// <summary>
    /// 왼쪽 아래 방향 각도 중심입니다.
    /// </summary>
    public float anglePivotLD = -135f;
    /// <summary>
    /// 오른쪽 아래 방향 각도 중심입니다.
    /// </summary>
    public float anglePivotRD = -45f;

    /// <summary>
    /// R->RU 방향 각도입니다.
    /// </summary>
    public float angleRtoRU = 22.5f;
    /// <summary>
    /// R->RD 방향 각도입니다.
    /// </summary>
    public float angleRtoRD = 22.5f;
    /// <summary>
    /// U->RU 방향 각도입니다.
    /// </summary>
    public float angleUtoRU = 22.5f;
    /// <summary>
    /// U->LU 방향 각도입니다.
    /// </summary>
    public float angleUtoLU = 22.5f;
    /// <summary>
    /// L->LU 방향 각도입니다.
    /// </summary>
    public float angleLtoLU = 22.5f;
    /// <summary>
    /// L->LD 방향 각도입니다.
    /// </summary>
    public float angleLtoLD = 22.5f;
    /// <summary>
    /// D->LD 방향 각도입니다.
    /// </summary>
    public float angleDtoLD = 22.5f;
    /// <summary>
    /// D->RD 방향 각도입니다.
    /// </summary>
    public float angleDtoRD = 22.5f;

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

        /*
        if (StageManager.Instance.MainPlayer.IsDead)
        {
            ///_atahoUnit.Ceremony();
            
            if (_rinshanUnit.DoingCeremony == false)
            {
                _rinshanUnit.Ceremony();
            }

            ///_smashuUnit.Ceremony();
        }
        */
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
        _atahoUnit.transform.position = _atahoSpawnPosition.position;

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
        if (_test)
        {
            if (_testHurt)
            {
                _atahoUnit.Hurt(300, null);
                _testHurt = false;
            }
        }

        while (DoesBattleEnd() == false)
        {
            // 아타호 패턴을 수행합니다.
            if (_coroutineAtahoPattern == null)
            {
                // 전투 관리자의 페이즈가 아타호 유닛의 페이즈와 같지 않다면 갱신합니다.
                // _phaseChanged는 페이즈가 변했음을 알립니다.
                if (_phase != _atahoUnit._phase)
                {
                    _phaseChanged = true;
                    _phase = _atahoUnit._phase;
                }

                // 페이즈에 따라 패턴 분기합니다.
                switch (_phase)
                {
                    case 0:
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern1());
                        break;
                    case 1:
                        //////////////////////////////////////////////////////////
                        // 페이즈가 전환된 직후에는 반드시 새로 추가되는 패턴을 플레이어에게 보여주어야 합니다.
                        // 이것을 보여주지 않으면 플레이어는 아타호의 EXP가 모두 차는 것이
                        // 단순히 마나 회복으로만 이어진다고 오해할 수 있습니다.
                        // 이 때 패턴의 가장 처음에 호출 동작을 제시하고 스마슈를 호출하면,
                        // 플레이어는 EXP가 모두 차는 것이 새로운 패턴이 등장하는 것이라고까지 이해하게 됩니다.
                        // 스마슈는 대타격-쾌진격 연계를 통해 치고 빠지는 근거리 딜러 타입으로,
                        // 첫 페이즈 전투에서 호포권을 피하며 공격을 하기 위해 보통 지상에 있지 않기 때문에
                        // 스마슈의 공격을 피하는 것이 어렵지 않을 것입니다.
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern2());
                        break;
                    case 2:
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern3());
                        break;
                    default:
                        _coroutineAtahoPattern = StartCoroutine(CoroutineAtahoPattern3());
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

            // 린샹 유닛이 존재한다면 린샹에 대한 패턴도 수행합니다.
            if (_rinshanUnit)
            {
                if (_rinshanUnit.Appearing)
                {

                }
                else if (_coroutineRinshanPattern == null)
                {
                    _coroutineRinshanPattern = StartCoroutine(CoroutineRinshanPhase3());
                }
            }
            else if (_coroutineRinshanPattern != null)
            {
                StopCoroutine(_coroutineRinshanPattern);
                _coroutineRinshanPattern = null;
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
                        case 1:
                            _coroutineSmashuPattern = StartCoroutine(CoroutineSmashuPhase2());
                            break;
                        case 2:
                            _coroutineSmashuPattern = StartCoroutine(CoroutineSmashuPhase3());
                            break;
                        default:
                            _coroutineSmashuPattern = StartCoroutine(CoroutineSmashuPhase3());
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
            _rinshanUnit = (EnemyBossRinshanUnit)unit;
        }
        else
        {
            _smashuUnit = (EnemyBossSmashuUnit)unit;
        }
    }
    /// <summary>
    /// 문 파괴를 요청합니다.
    /// </summary>
    public void RequestDestroyDoor()
    {
        Instantiate(DataBase.Instance.MultipleExplosionEffect, 
            _bossRoomDoor.transform.position, 
            _bossRoomDoor.transform.rotation);
        Instantiate(_doorDestroyEffect,
            _bossRoomDoor.transform.position,
            _bossRoomDoor.transform.rotation
            ).gameObject.SetActive(true);
        _doorInvisibleWall.gameObject.SetActive(true);
        Destroy(_bossRoomDoor.gameObject);
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
    /// 페이즈가 변했다면 참입니다.
    /// </summary>
    bool _phaseChanged = false;

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
                PerformAtahoPattern1ActionLU(_atahoUnit, player);
                break;
            case Direction.U:
                PerformAtahoPattern1ActionU(_atahoUnit, player);
                break;
            case Direction.RU:
                PerformAtahoPattern1ActionRU(_atahoUnit, player);
                break;
            case Direction.L:
                PerformAtahoPattern1ActionL(_atahoUnit, player);
                break;
            case Direction.R:
                PerformAtahoPattern1ActionR(_atahoUnit, player);
                break;
            case Direction.LD:
                PerformAtahoPattern1ActionLD(_atahoUnit, player);
                break;
            case Direction.D:
                PerformAtahoPattern1ActionD(_atahoUnit, player);
                break;
            case Direction.RD:
                PerformAtahoPattern1ActionRD(_atahoUnit, player);
                break;
            default:
                PerformAtahoPattern1ActionM(_atahoUnit, player);
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
        while (_atahoUnit.IsAnimatorInState("Idle") == false)
        {
            yield return false;
        }
        _coroutineAtahoPattern = null;
        yield break;
    }
    /// <summary>
    /// 2번 패턴입니다.
    /// </summary>
    IEnumerator CoroutineAtahoPattern2()
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
                PerformAtahoPattern2ActionLU(_atahoUnit, player);
                break;
            case Direction.U:
                PerformAtahoPattern2ActionU(_atahoUnit, player);
                break;
            case Direction.RU:
                PerformAtahoPattern2ActionRU(_atahoUnit, player);
                break;
            case Direction.L:
                PerformAtahoPattern2ActionL(_atahoUnit, player);
                break;
            case Direction.R:
                PerformAtahoPattern2ActionR(_atahoUnit, player);
                break;
            case Direction.LD:
                PerformAtahoPattern2ActionLD(_atahoUnit, player);
                break;
            case Direction.D:
                PerformAtahoPattern2ActionD(_atahoUnit, player);
                break;
            case Direction.RD:
                PerformAtahoPattern2ActionRD(_atahoUnit, player);
                break;
            default:
                PerformAtahoPattern2ActionM(_atahoUnit, player);
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
        while (_atahoUnit.IsAnimatorInState("Idle") == false)
        {
            yield return false;
        }
        _coroutineAtahoPattern = null;
        yield break;
    }
    /// <summary>
    /// 3번 패턴입니다.
    /// </summary>
    IEnumerator CoroutineAtahoPattern3()
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

        if (_test)
        {
            if (_rinshanUnit)
            {
                yield break;
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
                PerformAtahoPattern3ActionLU(_atahoUnit, player);
                break;
            case Direction.U:
                PerformAtahoPattern3ActionU(_atahoUnit, player);
                break;
            case Direction.RU:
                PerformAtahoPattern3ActionRU(_atahoUnit, player);
                break;
            case Direction.L:
                PerformAtahoPattern3ActionL(_atahoUnit, player);
                break;
            case Direction.R:
                PerformAtahoPattern3ActionR(_atahoUnit, player);
                break;
            case Direction.LD:
                PerformAtahoPattern3ActionLD(_atahoUnit, player);
                break;
            case Direction.D:
                PerformAtahoPattern3ActionD(_atahoUnit, player);
                break;
            case Direction.RD:
                PerformAtahoPattern3ActionRD(_atahoUnit, player);
                break;
            default:
                PerformAtahoPattern3ActionM(_atahoUnit, player);
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
        while (_atahoUnit.IsAnimatorInState("Idle") == false)
        {
            yield return false;
        }
        _coroutineAtahoPattern = null;
        yield break;
    }

    #endregion





    #region 아타호 유닛 패턴 1 행동 메서드를 정의합니다.
    /// <summary>
    /// 왼쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionLU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // H
        if (IsTargetOnHigh(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NLU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NLU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NLU");
            }
        }
        // FLU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FLU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FLU");
            }
        }
        // DEFAULT
        else
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // H
        if (IsTargetOnHigh(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "NU");
        }
        // FU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "FU");
        }
        // DEFAULT
        else
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionRU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // H
        if (IsTargetOnHigh(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NRU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NRU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NRU");
            }
        }
        // FRU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FRU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FRU");
            }
        }
        // DEFAULT
        else
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 왼쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionL(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // H
        if (IsTargetOnHigh(player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NL
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NL");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NL");
            }
        }
        // FL
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FL");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FL");
            }
        }
        // DEFAULT
        else
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
            }
            else
            {
                atahoUnit.Guard();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
            }
        }
    }
    /// <summary>
    /// 가운데 방향 전략입니다. 실제로 사용되지는 않을 것입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionM(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // H
        if (IsTargetOnHigh(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NM
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "NM");
        }
        // FM
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            _atahoUnit.DrinkMana();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "FM");
        }
        // DEFAULT
        else
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionR(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // H
        if (IsTargetOnHigh(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NR
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NR");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NR");
            }
        }
        // FR
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FR");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FR");
            }
        }
        // DEFAULT
        else
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "X");
            }
            else
            {
                atahoUnit.Guard();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
            }
        }
    }
    /// <summary>
    /// 왼쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionLD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // G
        if (IsTargetOnGround(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NLD
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (IsTargetOnHigh(atahoUnit.transform))
            {
                if (atahoUnit.IsHopokwonAvailable())
                {
                    atahoUnit.DoHopokwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "NLD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NLD");
                }
            }
            else
            {
                if (atahoUnit.IsHokyukkwonAvailable())
                {
                    atahoUnit.DoHokyukkwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NLD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NLD");
                }
            }
        }
        // FLD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (IsTargetOnHigh(atahoUnit.transform))
            {
                if (atahoUnit.IsHopokwonAvailable())
                {
                    atahoUnit.DoHopokwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FLD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FLD");
                }
            }
            else
            {
                if (atahoUnit.IsHokyukkwonAvailable())
                {
                    atahoUnit.DoHokyukkwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FLD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FLD");
                }
            }
        }
        // DEFAULT
        else
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // G
        if (IsTargetOnGround(player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // ND
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "ND");
        }
        // FD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "FD");
        }
        // DEFAULT
        else
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern1ActionRD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // G
        if (IsTargetOnGround(player.transform))
        {
            if (atahoUnit._mana >= atahoUnit.MANA_HOPOKWON)
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NRD
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (IsTargetOnHigh(atahoUnit.transform))
            {
                if (atahoUnit.IsHopokwonAvailable())
                {
                    atahoUnit.DoHopokwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "NRD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NRD");
                }
            }
            else
            {
                if (atahoUnit.IsHokyukkwonAvailable())
                {
                    atahoUnit.DoHokyukkwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NRD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NRD");
                }
            }
        }
        // FRD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (IsTargetOnHigh(atahoUnit.transform))
            {
                if (atahoUnit.IsHopokwonAvailable())
                {
                    atahoUnit.DoHopokwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FRD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FRD");
                }
            }
            else
            {
                if (atahoUnit.IsHokyukkwonAvailable())
                {
                    atahoUnit.DoHokyukkwon();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FRD");
                }
                else
                {
                    atahoUnit.DrinkMana();
                    HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FRD");
                }
            }
        }
        // DEFAULT
        else
        {
            _atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }

    #endregion





    #region 아타호 유닛 패턴 2 행동 메서드를 정의합니다.
    /// <summary>
    /// 왼쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionLU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NLU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NLU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NLU");
            }
        }
        // FLU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FLU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FLU");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 2 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NU");
        }
        // FU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FU");
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionRU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 2 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NRU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NRU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NRU");
            }
        }
        // FRU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FRU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FRU");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 왼쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionL(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 2 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NL
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NL");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NL");
            }
        }
        // FL
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsGwangpachamAvailable())
            {
                atahoUnit.DoGwangpacham();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoGwangpacham", "FL");
            }
            else if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FL");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FL");
            }
        }
        // DEFAULT
        else
        {
            if (atahoUnit.IsGwangpachamAvailable())
            {
                atahoUnit.DoGwangpacham();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoGwangpacham", "X");
            }
            else if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "X");
            }
            else
            {
                atahoUnit.Guard();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
            }
        }
    }
    /// <summary>
    /// 가운데 방향 전략입니다. 실제로 사용되지는 않을 것입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionM(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 2 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NM
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "NM");
        }
        // FM
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FM");
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionR(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 2 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            // 마나를 회복합니다.
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            // 자신이 마나를 소모하여 원거리의 적을 공격합니다.
            else if (atahoUnit._mana >= atahoUnit.MANA_HOPOKWON)
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            // 마나를 회복합니다.
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NR
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NR");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NR");
            }
        }
        // FR
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsGwangpachamAvailable())
            {
                atahoUnit.DoGwangpacham();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoGwangpacham", "FR");
            }
            else if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FR");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FR");
            }
        }
        // DEFAULT
        else
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "X");
            }
            else
            {
                atahoUnit.Guard();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
            }
        }
    }
    /// <summary>
    /// 왼쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionLD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NLD
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NLD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NLD");
            }
        }
        // FLD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FLD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FLD");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "G");
            }
            else if (atahoUnit._mana >= atahoUnit.MANA_HOPOKWON)
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "G");
            }
        }
        // ND
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "ND");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "ND");
            }
        }
        // FD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FD");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern2ActionRD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            atahoUnit.CallSmashu(_positions[8]);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit._mana >= atahoUnit.MANA_HOPOKWON)
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NRD
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "NRD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NRD");
            }
        }
        // FRD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FRD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FRD");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }

    #endregion





    #region 아타호 유닛 패턴 3 행동 메서드를 정의합니다.
    /// <summary>
    /// 왼쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionLU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NLU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NLU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NLU");
            }
        }
        // FLU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FLU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FLU");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 3 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NU");
        }
        // FU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FU");
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 위 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionRU(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 3 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // NRU
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NRU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NRU");
            }
        }
        // FRU
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FRU");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FRU");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 왼쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionL(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 3 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NL
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NL");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NL");
            }
        }
        // FL
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsGwangpachamAvailable())
            {
                atahoUnit.DoGwangpacham();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoGwangpacham", "FL");
            }
            else if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FL");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FL");
            }
        }
        // DEFAULT
        else
        {
            if (atahoUnit.IsGwangpachamAvailable())
            {
                atahoUnit.DoGwangpacham();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoGwangpacham", "X");
            }
            else if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "X");
            }
            else
            {
                atahoUnit.Guard();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
            }
        }
    }
    /// <summary>
    /// 가운데 방향 전략입니다. 실제로 사용되지는 않을 것입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionM(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 3 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 스마슈를 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (_smashuUnit == null)
            {
                // 아래 방향에 대한 전략이므로 상대적으로 아래에 소환하는 것이 좋아 보입니다.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NM
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "NM");
        }
        // FM
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            atahoUnit.DrinkMana();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FM");
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionR(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // H
        else if (IsTargetOnHigh(player.transform))
        {
            if (IsSmashuAvailable())
            {
                // 적이 맨 위에 있을 때 스마슈를 호출하는 것은... 논란의 여지가 있네요.
                // 그러나 이 조건이 없으면 플레이어는 최초 1회를 빼곤 스마슈를 보지도 못할 것입니다.
                // 실제 플레이를 통해 어떤지 파악해봅시다.
                // 3 페이즈에서는 H/G 모두 스마슈를 호출하지만,
                // 3 페이즈에서는 H를 린샹이 대응할 수 있으므로 G에서만 스마슈를 사용한다던가요.
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "H");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "H");
            }
            // 마나를 회복합니다.
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "H");
            }
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            // 자신이 마나를 소모하여 원거리의 적을 공격합니다.
            else if (atahoUnit._mana >= atahoUnit.MANA_HOPOKWON)
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            // 마나를 회복합니다.
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NR
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NR");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NR");
            }
        }
        // FR
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsGwangpachamAvailable())
            {
                atahoUnit.DoGwangpacham();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoGwangpacham", "FR");
            }
            else if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "FR");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FR");
            }
        }
        // DEFAULT
        else
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "X");
            }
            else
            {
                atahoUnit.Guard();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
            }
        }
    }
    /// <summary>
    /// 왼쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionLD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NLD
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHokyukkwonAvailable())
            {
                atahoUnit.DoHokyukkwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHokyukkwon", "NLD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NLD");
            }
        }
        // FLD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FLD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FLD");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "G");
            }
            else if (atahoUnit._mana >= atahoUnit.MANA_HOPOKWON)
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "G");
            }
        }
        // ND
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "ND");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "ND");
            }
        }
        // FD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FD");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }
    /// <summary>
    /// 오른쪽 아래 방향 전략입니다.
    /// </summary>
    /// <param name="atahoUnit">아타호입니다.</param>
    /// <param name="player">플레이어입니다.</param>
    void PerformAtahoPattern3ActionRD(EnemyBossAtahoUnit atahoUnit, PlayerController player)
    {
        // 페이즈 변화 시엔 가장 먼저 발동해야 합니다.
        if (_phaseChanged)
        {
            // 패턴 3 시작 시에 린샹을 반드시 호출합니다.
            _atahoUnit.CallRinshan(_rinshanSpawnPosition);
            HwanseBattleDebugger.Log(atahoUnit, _phase, "CallRinshan", "PC");

            // 페이즈 변화 플래그를 해제합니다.
            _phaseChanged = false;
        }
        // G
        else if (IsTargetOnGround(player.transform))
        {
            // 대상이 바닥에 있다면 항상 아타호보다는 밑에 있게 됩니다.
            // 아래에 있어서 대상을 공격할 수 없을 경우 아타호에게 효율적인 전략은,
            // 팀원을 재빠르게 호출하여 공격하게 하고 자신은 다른 위치로 이동하는 것입니다.
            if (IsSmashuAvailable())
            {
                atahoUnit.CallSmashu(_positions[8]);
                HwanseBattleDebugger.Log(atahoUnit, _phase, "CallSmashu", "G");
            }
            else if (atahoUnit._mana >= atahoUnit.MANA_HOPOKWON)
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "G");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "G");
            }
        }
        // NRD
        else if (IsNear(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "NRD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "NRD");
            }
        }
        // FRD
        else if (IsFar(atahoUnit.transform, player.transform))
        {
            if (atahoUnit.IsHopokwonAvailable())
            {
                atahoUnit.DoHopokwon();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DoHopokwon", "FRD");
            }
            else
            {
                atahoUnit.DrinkMana();
                HwanseBattleDebugger.Log(atahoUnit, _phase, "DrinkMana", "FRD");
            }
        }
        // DEFAULT
        else
        {
            atahoUnit.Guard();
            HwanseBattleDebugger.Log(atahoUnit, _phase, "Guard", "X");
        }
    }

    #endregion





    #region 아타호 유닛 패턴에 사용되는 보조 메서드를 정의합니다.
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

    /// <summary>
    /// 스마슈 유닛을 호출할 수 있는지 확인합니다.
    /// </summary>
    /// <returns>호출할 수 있다면 참입니다.</returns>
    bool IsSmashuAvailable()
    {
        return _smashuUnit == null;
    }

    #endregion





    #region 린샹 전략을 정의합니다.
    /// <summary>
    /// 린샹이 참조할 영상뢰화 위치입니다.
    /// </summary>
    Vector3[] _roihwaPositions;
    /// <summary>
    /// 린샹이 참조할 영상뢰화 위치입니다.
    /// </summary>
    public Vector3[] RoihwaPositions
    {
        get { return _roihwaPositions; }
    }

    /// <summary>
    /// 패턴 코루틴입니다.
    /// </summary>
    Coroutine _coroutineRinshanPattern;

    /// <summary>
    /// 1번 패턴입니다.
    /// </summary>
    IEnumerator CoroutineRinshanPhase3()
    {
        // 영상뢰화 또는 대폭진을 수행합니다.
        PlayerController player = _stageManager.MainPlayer;
        if (player.IsAlive() == false)
        {
            // 전투 승리 시에 세레모니를 합니다.
            // 웃기기도 하고 빡치기도 할 것입니다.
            _rinshanUnit.Ceremony();
        }
        else if (true)
        {
            // 플레이어가 상대적으로 맵의 왼쪽에 있다면 왼쪽부터,
            // 오른쪽에 있다면 오른쪽부터 시작합니다.
            // _roihwaPositions로 위치를 지정해주고,
            // 마지막 위치는 내리찍는 시점에 린샹이 계산합니다.
            UpdateRoihwaPositions(player);

            // 업데이트된 영상뢰화 위치를 사용하여 영상뢰화를 수행하게 합니다.
            _rinshanUnit.DoRoihwa();
        }
        else if (IsTargetOnHigh(player.transform))
        {
            // 플레이어가 상대적으로 맵의 왼쪽에 있다면 왼쪽부터,
            // 오른쪽에 있다면 오른쪽부터 시작합니다.
            // _roihwaPositions로 위치를 지정해주고,
            // 마지막 위치는 내리찍는 시점에 린샹이 계산합니다.
            UpdateRoihwaPositions(player);

            // 업데이트된 영상뢰화 위치를 사용하여 영상뢰화를 수행하게 합니다.
            _rinshanUnit.DoRoihwa();
        }
        else if (IsTargetOnGround(player.transform))
        {
            _rinshanUnit.DoDaepokjin();
        }
        else
        {
            _rinshanUnit.DoSukyeong();
        }

        // 행동이 종료될 때까지 대기합니다.
        while (_rinshanUnit.IsActionStarted == false)
        {
            yield return false;
            if (_rinshanUnit == null)
            {
                yield break;
            }
        }
        while (_rinshanUnit.IsActionRunning)
        {
            yield return false;
            if (_rinshanUnit == null)
            {
                yield break;
            }
        }
        while (_rinshanUnit.IsActionEnded == false)
        {
            yield return false;
            if (_rinshanUnit == null)
            {
                yield break;
            }
        }

        // 대기 모션으로 애니메이터가 전환될 때까지 코루틴을 유지합니다.
        while (_rinshanUnit.IsAnimatorInState("Idle") == false)
        {
            yield return false;
        }
        yield return new WaitForSeconds(TIME_WAIT_PATTERN1);

        // 코루틴을 종료합니다.
        _coroutineRinshanPattern = null;
        yield break;
    }
    /// <summary>
    /// 영상뢰화를 생성할 위치를 업데이트 합니다.
    /// </summary>
    /// <param name="player">플레이어의 위치입니다. 화면의 중심 기준으로 왼쪽과 오른쪽에 따라 생성 위치가 바뀝니다.</param>
    void UpdateRoihwaPositions(PlayerController player)
    {
        float playerPosX = player.transform.position.x;
        float centerPosX = _positions[3].position.x;
        float lightningPosY = _positions[10].position.y;

        // 화면을 중심으로 플레이어가 왼쪽에 있는 경우, 왼쪽부터 4개를 생성합니다.
        // 마지막 한 발은 린샹이 자체적으로 플레이어의 위치를 계산합니다.
        if (playerPosX <= centerPosX)
        {
            _roihwaPositions = new Vector3[]
            {
                    new Vector3(_positions[6].position.x, lightningPosY),
                    new Vector3(_positions[4].position.x, lightningPosY),
                    new Vector3(_positions[3].position.x, lightningPosY),
                    new Vector3(_positions[2].position.x, lightningPosY),
                    new Vector3(_positions[10].position.x, lightningPosY)
            };
        }
        // 화면을 중심으로 플레이어가 오른쪽에 있는 경우, 오른쪽부터 4개를 생성합니다.
        // 마지막 한 발은 린샹이 자체적으로 플레이어의 위치를 계산합니다.
        else
        {
            _roihwaPositions = new Vector3[]
            {
                    new Vector3(_positions[0].position.x, lightningPosY),
                    new Vector3(_positions[2].position.x, lightningPosY),
                    new Vector3(_positions[3].position.x, lightningPosY),
                    new Vector3(_positions[4].position.x, lightningPosY),
                    new Vector3(_positions[10].position.x, lightningPosY)
            };
        }

    }

    #endregion





    #region 스마슈 전략을 정의합니다.
    /// <summary>
    /// 패턴 코루틴입니다.
    /// </summary>
    Coroutine _coroutineSmashuPattern;

    /// <summary>
    /// 스마슈 개체를 파괴합니다.
    /// </summary>
    public void RequestDestroySmashu()
    {
        if (_smashuUnit)
        {
            Destroy(_smashuUnit.gameObject);
            _smashuUnit = null;
        }
    }

    /// <summary>
    /// 1번 패턴입니다.
    /// </summary>
    IEnumerator CoroutineSmashuPhase2()
    {
        // 대타격을 수행합니다.
        PlayerController player = _stageManager.MainPlayer;
        if (player.IsDead)
        {
            _smashuUnit.Ceremony();
        }
        else
        {
            _smashuUnit.DoDaetakyuk();
        }

        // 행동이 종료될 때까지 대기합니다.
        while (_smashuUnit.IsActionStarted == false)
        {
            yield return false;

            // 모든 단위 행동의 다음에는 해당 개체가 살아있는지 확인해야 에러가 없습니다.
            // 어쩌면... try/catch를 사용하여 그냥 넘겨버릴 수도 있겠지요.
            // 다음 커밋에서 그렇게 구현합시다.
            if (_smashuUnit == null)
            {
                // Enumerator에 try-catch를 못 쓴대요..
                yield break;
            }
        }
        while (_smashuUnit.IsActionRunning)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }
        while (_smashuUnit.IsActionEnded == false)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }

        // 
        if (player.Damaged || player.BigDamaged || player.Invencible)
        {
            // 사라집니다.
            _smashuUnit.Disappear();
        }
        else
        {
            _smashuUnit.DoKwaejinkyuk();
            Invoke("RequestDestroySmashu", TIME_LIFE_SMASHU);
        }

        // 행동이 종료될 때까지 대기합니다.
        while (_smashuUnit.IsActionStarted == false)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }
        while (_smashuUnit.IsActionRunning)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }
        while (_smashuUnit.IsActionEnded == false)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }

        // 
        _coroutineSmashuPattern = null;
        yield break;
    }
    /// <summary>
    /// 2번 패턴입니다.
    /// </summary>
    IEnumerator CoroutineSmashuPhase3()
    {
        // 대타격을 수행합니다.
        PlayerController player = _stageManager.MainPlayer;
        if (player.IsDead)
        {
            _smashuUnit.Ceremony();
        }
        else
        {
            _smashuUnit.DoDaetakyuk();
        }

        // 행동이 종료될 때까지 대기합니다.
        while (_smashuUnit.IsActionStarted == false)
        {
            yield return false;

            // 모든 단위 행동의 다음에는 해당 개체가 살아있는지 확인해야 에러가 없습니다.
            // 어쩌면... try/catch를 사용하여 그냥 넘겨버릴 수도 있겠지요.
            // 다음 커밋에서 그렇게 구현합시다.
            if (_smashuUnit == null)
            {
                // Enumerator에 try-catch를 못 쓴대요..
                yield break;
            }
        }
        while (_smashuUnit.IsActionRunning)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }
        while (_smashuUnit.IsActionEnded == false)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }

        // 
        if (player.Damaged || player.BigDamaged || player.Invencible)
        {
            // 사라집니다.
            _smashuUnit.Disappear();
        }
        else
        {
            _smashuUnit.DoKwaejinkyuk();
            Invoke("RequestDestroySmashu", TIME_LIFE_SMASHU);
        }

        // 행동이 종료될 때까지 대기합니다.
        while (_smashuUnit.IsActionStarted == false)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }
        while (_smashuUnit.IsActionRunning)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
            }
        }
        while (_smashuUnit.IsActionEnded == false)
        {
            yield return false;
            if (_smashuUnit == null)
            {
                yield break;
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

        // 

        // 
        Vector2 ray0 = dir(anglePivotR + angleRtoRU) * _debugRayLength;
        Vector2 ray1 = dir(anglePivotU - angleUtoRU) * _debugRayLength;
        Vector2 ray2 = dir(anglePivotU + angleUtoLU) * _debugRayLength;
        Vector2 ray3 = dir(anglePivotL - angleLtoLU) * _debugRayLength;
        Vector2 ray4 = dir(-anglePivotL + angleLtoLD) * _debugRayLength;
        Vector2 ray5 = dir(anglePivotD - angleDtoLD) * _debugRayLength;
        Vector2 ray6 = dir(anglePivotD + angleDtoRD) * _debugRayLength;
        Vector2 ray7 = dir(anglePivotR - angleRtoRD) * _debugRayLength;
        Debug.DrawRay(st.position, ray0, Color.green);
        Debug.DrawRay(st.position, ray1, Color.green);
        Debug.DrawRay(st.position, ray2, Color.green);
        Debug.DrawRay(st.position, ray3, Color.green);
        Debug.DrawRay(st.position, ray4, Color.green);
        Debug.DrawRay(st.position, ray5, Color.green);
        Debug.DrawRay(st.position, ray6, Color.green);
        Debug.DrawRay(st.position, ray7, Color.green);

        //
        _angle = Vector3.SignedAngle(Vector3.right, _dv.normalized, Vector3.forward);
        if (anglePivotR - angleRtoRD <= _angle && _angle < anglePivotR + angleRtoRU)
        {
            direction = Direction.R;
        }
        else if (anglePivotR + angleRtoRU <= _angle && _angle < anglePivotU - angleUtoRU)
        {
            direction = Direction.RU;
        }
        else if (anglePivotU - angleUtoRU <= _angle && _angle < anglePivotU + angleUtoLU)
        {
            direction = Direction.U;
        }
        else if (anglePivotU + angleUtoLU <= _angle && _angle < anglePivotL - angleLtoLU)
        {
            direction = Direction.LU;
        }
        else if (anglePivotL - angleLtoLU <= _angle || _angle < -anglePivotL + angleLtoLD)
        {
            direction = Direction.L;
        }
        else if (-anglePivotL + angleLtoLD <= _angle && _angle < anglePivotD - angleDtoLD)
        {
            direction = Direction.LD;
        }
        else if (anglePivotD - angleDtoLD <= _angle && _angle < anglePivotD + angleDtoRD)
        {
            direction = Direction.D;
        }
        else if (anglePivotD + angleDtoRD <= _angle && _angle < anglePivotR - angleRtoRD)
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
    /// 타겟이 스테이지의 위쪽에 있는지를 확인합니다.
    /// </summary>
    /// <param name="targetTransform">타겟 Transform 개체입니다. 보통 플레이어가 됩니다.</param>
    /// <returns>타겟이 스테이지의 위쪽에 있다면 참입니다.</returns>
    bool IsTargetOnHigh(Transform targetTransform)
    {
        // 
        float playerPosY = targetTransform.position.y;
        float highPosY = _positions[2].position.y;
        float dy = Mathf.Abs(highPosY - playerPosY);
        return (dy < THRESHOLD_HIGH_DIFF) || (highPosY < playerPosY);
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

    /// <summary>
    /// 주어진 각도에 맞는 방향 벡터를 가져옵니다.
    /// </summary>
    /// <param name="deg">360도 각도입니다.</param>
    /// <returns>주어진 각도에 맞는 방향 벡터를 가져옵니다.</returns>
    public Vector2 dir(float deg)
    {
        return new Vector2(cos(deg), sin(deg));
    }
    /// <summary>
    /// 360도 각도에 대한 코사인 값을 반환합니다.
    /// </summary>
    /// <param name="deg">360도 각도입니다.</param>
    /// <returns>360도 각도에 대한 코사인 값을 반환합니다.</returns>
    public static float cos(float deg)
    {
        return Mathf.Cos(deg * Mathf.Deg2Rad);
    }
    /// <summary>
    /// 360도 각도에 대한 사인 값을 반환합니다.
    /// </summary>
    /// <param name="deg">360도 각도입니다.</param>
    /// <returns>360도 각도에 대한 사인 값을 반환합니다.</returns>
    public static float sin(float deg)
    {
        return Mathf.Sin(deg * Mathf.Deg2Rad);
    }

    #endregion





    #region 디버깅 필드와 메서드를 정의합니다.
    /// <summary>
    /// 테스트 플래그입니다.
    /// </summary>
    public bool _test = true;
    /// <summary>
    /// 테스트 대미지 플래그입니다.
    /// </summary>
    public bool _testHurt = true;

    #endregion





    #region 구형 정의를 보관합니다.

    [Obsolete("아무래도 안 쓸 것 같습니다.")]
    /// <summary>
    /// 행동 코루틴입니다.
    /// </summary>
    Coroutine _subcoroutineSmashuAction;
    [Obsolete("아무래도 안 쓸 것 같습니다.")]
    /// <summary>
    /// 행동 코루틴입니다.
    /// </summary>
    Coroutine _subcoroutineRinshanAction;
    [Obsolete("린샹은 파괴되지 않습니다. 다음 커밋에서 발견하는 즉시 삭제하십시오.")]
    /// <summary>
    /// 린샹 개체를 파괴합니다.
    /// </summary>
    public void RequestDestroyRinshan()
    {
        if (_rinshanUnit)
        {
            Destroy(_rinshanUnit.gameObject);
            _rinshanUnit = null;
        }
    }

    #endregion
}