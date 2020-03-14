using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 전투 디버거입니다.
/// </summary>
public class BattleDebugger : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static string GetDirectionString(Direction direction)
    {
        switch (direction)
        {
            case Direction.LU:
                return "LU";
            case Direction.U:
                return "U";
            case Direction.RU:
                return "RU";
            case Direction.R:
                return "R";
            case Direction.RD:
                return "RD";
            case Direction.D:
                return "D";
            case Direction.LD:
                return "LD";
            case Direction.L:
                return "L";
            case Direction.M:
                return "M";
            default:
                return "X";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="phase"></param>
    /// <param name="direction"></param>
    /// <param name="actionName"></param>
    /// <param name="comment"></param>
    public static void Log(Unit unit, int phase, string actionName, string comment = "")
    {
        Handy.Log("Unit={0},Phase={1},Action={2},Comment={3}",
            unit.name, phase, actionName, comment);
    }
}