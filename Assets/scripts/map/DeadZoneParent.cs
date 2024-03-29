﻿using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 사망 구역의 부모 개체입니다.
/// </summary>
public class DeadZoneParent : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        PhysicsMaterial2D material = DataBase.Instance.FrictionlessWall;
        Collider2D[] children = GetComponentsInChildren<Collider2D>();

        // 
        foreach (Collider2D child in children)
        {
            child.sharedMaterial = material; // _database.FrictionlessWall;
        }
    }
}
