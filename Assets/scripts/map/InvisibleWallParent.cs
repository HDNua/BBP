using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 투명한 벽의 부모 개체입니다.
/// </summary>
public class InvisibleWallParent : MonoBehaviour
{
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    void Start()
    {
        PhysicsMaterial2D material = DataBase.Instance.FrictionlessWall;
        Collider2D[] children = GetComponentsInChildren<Collider2D>(includeInactive: true);

        // 모든 자식 개체의 material을 업데이트 합니다.
        foreach (Collider2D child in children)
        {
            child.sharedMaterial = material;
        }
    }
}
