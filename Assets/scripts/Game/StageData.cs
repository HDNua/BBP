using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public struct StageData
{
    /// <summary>
    /// 스테이지 이름입니다.
    /// </summary>
    public string name;

    /// <summary>
    /// 맵을 클리어했다면 참입니다.
    /// </summary>
    public bool cleared;

    /// <summary>
    /// '라이프 업' 아이템을 획득했습니다.
    /// </summary>
    public bool itemLifeUpFound;
    /// <summary>
    /// '웨폰 업' 아이템을 획득했습니다.
    /// </summary>
    public bool itemWeaponUpFound;

    /// <summary>
    /// '라이프 서브탱크' 아이템을 획득했습니다.
    /// </summary>
    public bool itemECanFound;
    /// <summary>
    /// '웨폰 서브탱크' 아이템을 획득했습니다.
    /// </summary>
    public bool itemWCanFound;
    /// <summary>
    /// '트라이 서브탱크' 아이템을 획득했습니다.
    /// </summary>
    public bool itemXCanFound;

    /// <summary>
    /// 헤드 파츠를 획득했습니다.
    /// </summary>
    public bool armorHeadFound;
    /// <summary>
    /// 바디 파츠를 획득했습니다.
    /// </summary>
    public bool armorBodyFound;
    /// <summary>
    /// 암 파츠를 획득했습니다.
    /// </summary>
    public bool armorArmFound;
    /// <summary>
    /// 풋 파츠를 획득했습니다.
    /// </summary>
    public bool armorFootFound;
}