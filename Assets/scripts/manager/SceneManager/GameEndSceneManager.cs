﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 게임 종료 장면 관리자입니다.
/// </summary>
public class GameEndSceneManager : HDSceneManager
{
    #region Unity에서 접근 가능한 공용 객체를 정의합니다.
    /// <summary>
    /// 
    /// </summary>
    public GameObject[] menuItems;
    /// <summary>
    /// 
    /// </summary>
    public Sprite[] sprites;
    /// <summary>
    /// 
    /// </summary>
    public AudioClip[] soundEffects;

    /// <summary>
    /// 
    /// </summary>
    public GameObject _pointer;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 효과음 집합입니다.
    /// </summary>
    AudioSource[] _seSources;

    /// <summary>
    /// 메뉴 인덱스입니다.
    /// </summary>
    int _menuIndex = 0;
    /// <summary>
    /// 장면 변화 요청이 들어왔습니다.
    /// </summary>
    bool _changeSceneRequested = false;
    /// <summary>
    /// 다음 장면의 이름입니다.
    /// </summary>
    string _nextLevelName = null;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        Time.timeScale = 1;

        // 효과음 리스트를 초기화 합니다.
        _seSources = new AudioSource[soundEffects.Length];
        for (int i = 0, len = _seSources.Length; i < len; ++i)
        {
            _seSources[i] = gameObject.AddComponent<AudioSource>();
            _seSources[i].clip = soundEffects[i];
        }

        // 페이드인 효과를 실행합니다.
        FadeManager.Instance.FadeIn();
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트 합니다.
    /// </summary>
    protected override void Update()
    {
        // 장면 전환 요청을 확인한 경우의 처리입니다.
        if (_changeSceneRequested)
        {
            if (FadeManager.Instance.FadeOutEnded)
            {
                LoadingSceneManager.LoadLevel(_nextLevelName);
            }
            return;
        }

        // 키 입력에 대한 처리입니다.
        if (HDInput.IsAnyKeyDown())
        {
            if (Input.anyKeyDown)
            {
                if (HDInput.IsUpKeyDown())
                {
                    ChangeMenuItem(_menuIndex - 1);
                }
                else if (HDInput.IsDownKeyDown())
                {
                    ChangeMenuItem(_menuIndex + 1);
                }
                else if (IsSelectKeyPressed())
                {
                    switch (_menuIndex)
                    {
                        case 0:
                            Application.OpenURL("https://forms.gle/CRCJj7oyEuksKhoU7");
                            break;

                        case 1:
                            _nextLevelName = "BBPTitle";
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 2:
                            Application.Quit();
                            break;

                        default:
                            _nextLevelName = null;
                            break;
                    }
                    _seSources[1].Play();
                }
            }
            else
            {
                if (HDInput.IsUpKeyPressed())
                {
                    ChangeMenuItem(_menuIndex - 1);
                }
                else if (HDInput.IsDownKeyPressed())
                {
                    ChangeMenuItem(_menuIndex + 1);
                }
                else if (IsSelectKeyPressed())
                {
                    switch (_menuIndex)
                    {
                        case 0:
                            Application.OpenURL("https://forms.gle/kH71RHEu9ZRNvVW37");
                            break;

                        case 1:
                            _nextLevelName = "BBPTitle";
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 2:
                            Application.Quit();
                            break;

                        default:
                            _nextLevelName = null;
                            break;
                    }
                    _seSources[1].Play();
                }
            }
        }
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 메뉴 아이템 선택을 변경합니다.
    /// </summary>
    /// <param name="index">선택할 메뉴 아이템의 인덱스입니다.</param>
    void ChangeMenuItem(int index)
    {
        if (index < 0)
        {
            index = menuItems.Length - 1;
        }
        else if (menuItems.Length <= index)
        {
            index = 0;
        }

        GameObject prevItem = menuItems[_menuIndex];
        GameObject nextItem = menuItems[index];
        prevItem.GetComponent<SpriteRenderer>().sprite = sprites[2 * _menuIndex + 1];
        nextItem.GetComponent<SpriteRenderer>().sprite = sprites[2 * index];
        _menuIndex = index;
        _seSources[0].Play();

        // 
        _pointer.transform.position = new Vector3(_pointer.transform.position.x, nextItem.transform.position.y);
    }

    /// <summary>
    /// 선택 키가 눌렸는지 확인합니다.
    /// </summary>
    /// <returns>선택 키가 눌렸다면 참입니다.</returns>
    bool IsSelectKeyPressed()
    {
        return (Input.GetKeyDown(KeyCode.Return)
            || Input.GetButton("Attack")
            || Input.GetKey(KeyCode.Space));
    }

    #endregion





    #region 구형 정의를 보관합니다.

    #endregion
}