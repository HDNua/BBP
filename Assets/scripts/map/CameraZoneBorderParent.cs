﻿using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 카메라 존 경계의 부모입니다.
/// </summary>
public class CameraZoneBorderParent : MonoBehaviour
{
    #region 필드를 정의합니다.

    #endregion



    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 카메라 존 경계의 부모 개체에 대한 참조를 반환합니다.
    /// </summary>
    public static CameraZoneBorderParent Instance
    {
        get
        {
            return GameObject.FindGameObjectWithTag("CameraZoneBorderParent")
                .GetComponent<CameraZoneBorderParent>();
        }
    }

    /// <summary>
    /// 1인 스테이지 장면 관리자입니다.
    /// </summary>
    public StageManager1P StageManager
    {
        get { return StageManager1P.Instance; }
    }

    #endregion



    #region 구형 정의를 보관합니다.

    #endregion
}
