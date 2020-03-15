﻿using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 스테이지 선택 화면 관리자입니다.
/// </summary>
public class StageSelectSceneManager : HDSceneManager
{
    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public GameObject _earth;
    /// <summary>
    /// 
    /// </summary>
    public GameObject _cursor;
    /// <summary>
    /// 
    /// </summary>
    public GameObject[] _stagePoints;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    int _cursorRow = 0;
    /// <summary>
    /// 
    /// </summary>
    int _cursorCol = 0;

    /// <summary>
    /// 
    /// </summary>
    bool _loading = false;
    /// <summary>
    /// 
    /// </summary>
    string _nextLevelName;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 
        ChangeItem(0, 0);

        // 
        FadeManager.Instance.FadeIn();
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    protected override void Update()
    {
        if (_loading)
        {
            if (FadeManager.Instance.FadeOutEnded)
            {
                LoadingSceneManager.LoadLevel(_nextLevelName);
            }
            return;
        }

        // 
        if (Input.anyKeyDown)
        {
            if (Input.GetButton("Attack"))
            {
                Load();
                return;
            }
            else if (Input.GetButton("Jump"))
            {
                return;
            }
            else if (Input.GetButton("Dash"))
            {
                Load("Title");
                return;
            }

            // 
            if (Input.GetKey(KeyCode.UpArrow))
            {
                ChangeItem(-1, 0);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                ChangeItem(1, 0);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                ChangeItem(0, -1);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                ChangeItem(0, 1);
            }
        }

        //
        foreach (GameObject stagePoint in _stagePoints)
        {
            Debug.DrawRay(_earth.transform.position, stagePoint.transform.position);
        }
    }

    #endregion





    #region 메서드를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="difR"></param>
    /// <param name="difC"></param>
    void ChangeItem(int difR, int difC)
    {
        // 행/열 위치를 먼저 계산합니다.
        _cursorRow += difR;
        if (_cursorRow < 0) _cursorRow = 2;
        else if (_cursorRow >= 3) _cursorRow = 0;
        _cursorCol += difC;
        if (_cursorCol < 0) _cursorCol = 2;
        else if (_cursorCol >= 3) _cursorCol = 0;


        // 새 인덱스로 업데이트합니다.
        int index = _cursorRow * 3 + _cursorCol;
        if (index < 0 || index >= 9)
        {
            Debug.Log("Error: [" + _cursorRow + "/" + _cursorCol + "]");
        }
        else
        {
            // 커서의 위치를 변경합니다.
            _cursor.transform.position = _stagePoints[index].transform.position;
            _nextLevelName = "02_CommanderYammark"; // _stagePoints[index].name;

            // 회전 테스트
        }
        AudioSources[0].Play();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    int GetStageIndex()
    {
        return _cursorRow * 3 + _cursorCol;
    }

    /// <summary>
    /// Scene을 불러옵니다.
    /// </summary>
    void Load()
    {
        _loading = true;
        AudioSources[1].Play();

        // 페이드 아웃을 진행합니다.
        FadeManager.Instance.FadeOut();
    }
    /// <summary>
    /// Scene을 불러옵니다.
    /// </summary>
    /// <param name="levelName">장면 이름입니다.</param>
    void Load(string levelName)
    {
        _loading = true;
        AudioSources[1].Play();

        _nextLevelName = levelName;

        // 페이드 아웃을 진행합니다.
        FadeManager.Instance.FadeOut();
    }

    #endregion
}
