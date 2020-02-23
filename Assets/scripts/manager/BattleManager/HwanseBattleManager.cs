using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 
/// </summary>
public class HwanseBattleManager : BattleManager
{
    #region 컨트롤러가 사용할 Unity 개체에 대한 참조를 보관합니다.
    /// <summary>
    /// 
    /// </summary>
    EnemyBossAtahoUnit _atahoUnit;

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
        while (_atahoUnit.AppearEnded == false)
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
        ActivateBossHUD();

        // 보스 체력 재생을 요청합니다.
        RequestFillHealth();
        while (_atahoUnit.Health < _atahoUnit.MaxHealth)
        {
            yield return false;
        }

        // 전투를 시작합니다.
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

    /// <summary>
    /// 보스 캐릭터 체력 바를 표시합니다.
    /// </summary>
    protected override void ActivateBossHUD()
    {
        base.ActivateBossHUD();
    }
    /// <summary>
    /// 보스 체력 재생을 요청합니다.
    /// </summary>
    protected override void RequestFillHealth()
    {
        base.RequestFillHealth();
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
    /// 
    /// </summary>
    Coroutine _coroutinePattern;
    /// <summary>
    /// 
    /// </summary>
    Coroutine _subcoroutineAction;

    /// <summary>
    /// 
    /// </summary>
    IEnumerator CoroutinePattern1()
    {
        _subcoroutineAction = StartCoroutine(SubcoroutineHop());
        while (_subcoroutineAction != null)
        {
            yield return false;
        }
        while (_atahoUnit.IsAnimatorInState("FallEnd"))
        {
            yield return false;
        }

        // 
        yield return new WaitForSeconds(1f);

        // 
        _coroutinePattern = null;
        yield break;
    }
    /// <summary>
    /// 
    /// </summary>
    IEnumerator SubcoroutineHop()
    {
        // 
        int[] nextHopPositionArray = GetNextHopPositionArray();
        int newPositionIndex = nextHopPositionArray
            [Random.Range(0, nextHopPositionArray.Length)];

        // 
        ///_previousPositionIndex = _currentPositionIndex;
        ///_currentPositionIndex = newPositionIndex;

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


    #endregion
}