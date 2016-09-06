﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// CameraZoneScript의 부모입니다.
/// </summary>
public class CameraZoneParent : MonoBehaviour
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 장면 관리자입니다.
    /// </summary>
    public StageManager _stageManager;
    /// <summary>
    /// 데이터베이스입니다.
    /// </summary>
    public DataBase _database;


    #endregion










    #region 필드를 정의합니다.
    /// <summary>
    /// CameraFollow 스크립트입니다.
    /// </summary>
    CameraFollowScript _cameraFollow;


    #endregion









    #region 프로퍼티를 정의합니다.
    /// <summary>
    /// 현재 행동중인 플레이어를 가져옵니다.
    /// </summary>
    public PlayerController Player
    {
        get { return _stageManager._player; }
    }
    /// <summary>
    /// CameraFollow 객체입니다.
    /// </summary>
    public CameraFollowScript CameraFollow
    {
        get { return _cameraFollow; }
    }


    #endregion










    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    void Start()
    {
        // 예외 메시지 리스트를 생성합니다.
        List<string> exceptionList = new List<string>();

        // 빈 필드가 존재하는 경우 예외 메시지를 추가합니다.
        if (_stageManager == null)
            exceptionList.Add("CameraZoneParent.StageManager == null");
        if (_database == null)
            exceptionList.Add("CameraZoneParent.DataBase == null");

        // 예외 메시지가 하나 이상 존재하는 경우 예외를 발생하고 중지합니다.
        if (exceptionList.Count > 0)
        {
            foreach (string msg in exceptionList)
            {
                Handy.Log("CameraZoneParent Error: {0}", msg);
            }
            throw new Exception("데이터베이스 필드 정의 부족");
        }


        // 필드를 초기화합니다.
        _cameraFollow = _database.CameraFollow;
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    void Update()
    {
        
    }


    #endregion










    #region 메서드를 정의합니다.


    #endregion










    #region 구형 정의를 보관합니다.


    #endregion
}
