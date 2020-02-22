﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#region 형식을 정의합니다.
/// <summary>
/// 
/// </summary>
public delegate void BattleAction();

#endregion


/// <summary>
/// 
/// </summary>
public class HwanseBossBattlePattern : MonoBehaviour
{
    #region 상수 및 형식을 정의합니다.
    

    

    #endregion



    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    public int _phase;
    public int _close;

    #endregion





    #region 필드를 정의합니다.

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (생성될 때마다)
    /// </summary>
    void Start()
    {

    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    void Update()
    {

    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    void FixedUpdate()
    {

    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    void LateUpdate()
    {

    }

    #endregion





    #region 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    void Test()
    {

    }

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}
