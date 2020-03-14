using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 환세 전투 디버거입니다.
/// </summary>
public class HwanseBattleDebugger : BattleDebugger
{
    public static void Log(Unit unit, int phase, string actionName, string direction, string comment = "")
    {
        //BattleDebugger.Log(unit, phase, actionName, comment);
        Handy.Log("Unit={0},Phase={1},Action={2},Direction={3},Comment={4}",
            unit.name, phase, actionName, direction, comment);
    }
}