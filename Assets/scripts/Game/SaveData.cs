using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public struct SaveData
{
    SystemData systemData;
    GameData gameData;
    StageData[] stageDatas;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="systemData"></param>
    /// <param name="gameData"></param>
    /// <param name="stageDatas"></param>
    public SaveData(SystemData systemData, GameData gameData, StageData[] stageDatas)
    {
        this.systemData = systemData;
        this.gameData = gameData;
        this.stageDatas = stageDatas;
    }
}
