using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



[Serializable]
/// <summary>
/// 게임 데이터입니다.
/// </summary>
public class GameData
{
    #region 필드를 정의합니다.
    /// <summary>
    /// 엑스의 최대 체력입니다.
    /// </summary>
    int _maxHealthX = 20;
    /// <summary>
    /// 제로의 최대 체력입니다.
    /// </summary>
    int _maxHealthZ = 20;

    /// <summary>
    /// 맵 상태 집합입니다.
    /// </summary>
    StageData[] _stageDataList = new StageData[8];

    /// <summary>
    /// 재시도 가능한 횟수를 정의합니다. 기본값은 2, EX아이템 획득 시에는 4입니다.
    /// </summary>
    int _tryCount = 2;
    /// <summary>
    /// 재시도 가능한 횟수를 정의합니다. 기본값은 2, EX아이템 획득 시에는 4입니다.
    /// </summary>
    public int TryCount
    {
        get { return _tryCount; }
        set { _tryCount = value; }
    }

    /// <summary>
    /// 난이도입니다.
    /// </summary>
    int _difficulty = 1;
    /// <summary>
    /// 난이도입니다.
    /// </summary>
    public int Difficulty
    {
        get { return _difficulty; }
        set { _difficulty = value; }
    }

    #endregion



    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 엑스의 최대 체력입니다.
    /// </summary>
    public int MaxHealthX
    {
        get { return _maxHealthX; }
        set { _maxHealthX = value; }
    }
    /// <summary>
    /// 제로의 최대 체력입니다.
    /// </summary>
    public int MaxHealthZ
    {
        get { return _maxHealthZ; }
        set { _maxHealthZ = value; }
    }

    /// <summary>
    /// 스테이지 데이터 집합입니다.
    /// </summary>
    public StageData[] StageData { get { return _stageDataList; } }

    #endregion



    #region 구형 정의를 보관합니다.
    [Obsolete("StageData로 대체되었습니다.")]
    /// <summary>
    /// 맵 상태 집합입니다.
    /// </summary>
    GameMapStatus[] _mapStatuses = new GameMapStatus[8];
    [Obsolete("StageData로 대체되었습니다.")]
    /// <summary>
    /// 맵 상태 집합입니다.
    /// </summary>
    public GameMapStatus[] MapStatuses { get { return _mapStatuses; } }

    #endregion
}