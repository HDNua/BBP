using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 타이틀 화면을 처리합니다.
/// </summary>
public class TitleSceneManager : HDSceneManager
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
    /// 메뉴 인덱스입니다.
    /// </summary>
    public int _menuIndex = 2;
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
        base.Start();

        // 페이드인 효과를 실행합니다.
        FadeManager.Instance.FadeIn();

        // 
        GameManager.Instance.RequestSetTryCount(2);
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
                if (HDInput.IsUpKeyDown()) // (HDInput.IsUpKeyPressed()) // (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ChangeMenuItem(_menuIndex - 1);
                }
                else if (HDInput.IsDownKeyDown()) // (HDInput.IsDownKeyPressed()) // (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ChangeMenuItem(_menuIndex + 1);
                }
                else if (IsSelectKeyPressed())
                {
                    /*
                    switch (_menuIndex)
                    {
                        case 0:
                            _nextLevelName = "CS00_PreviousStory";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 1:
                            _nextLevelName = "01_Intro_X";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 2:
                            _nextLevelName = "01_Intro_Z";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 3:
                            _nextLevelName = "01_Intro_2p";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 4:
                            Application.Quit();
                            break;

                        default:
                            _nextLevelName = null;
                            break;
                    }
                    */

                    switch (_menuIndex)
                    {
                        case 0:
                            _nextLevelName = NAME_NEXT_SCENES[0];
                            GameManager.Instance.Difficulty = 0;
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 1:
                            _nextLevelName = NAME_NEXT_SCENES[1];
                            GameManager.Instance.Difficulty = 1;
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 2:
                            _nextLevelName = NAME_NEXT_SCENES[2];
                            GameManager.Instance.Difficulty = 2;
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 3:
                            Application.Quit();
                            break;

                        default:
                            _nextLevelName = null;
                            break;
                    }
                    AudioSources[1].Play();
                }
            }
            else
            {
                if (HDInput.IsUpKeyPressed()) // (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ChangeMenuItem(_menuIndex - 1);
                }
                else if (HDInput.IsDownKeyPressed()) // (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ChangeMenuItem(_menuIndex + 1);
                }
                else if (IsSelectKeyPressed())
                {
                    /*
                    switch (_menuIndex)
                    {
                        case 0:
                            _nextLevelName = "CS00_PreviousStory";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 1:
                            _nextLevelName = "01_Intro_X";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 2:
                            _nextLevelName = "01_Intro_Z";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 3:
                            _nextLevelName = "01_Intro_2p";
                            _changeSceneRequested = true;
                            fader.FadeOut(1);
                            break;

                        case 4:
                            Application.Quit();
                            break;

                        default:
                            _nextLevelName = null;
                            break;
                    }
                    */

                    switch (_menuIndex)
                    {
                        case 0:
                            _nextLevelName = NAME_NEXT_SCENES[0];
                            GameManager.Instance.Difficulty = 0;
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 1:
                            _nextLevelName = NAME_NEXT_SCENES[1];
                            GameManager.Instance.Difficulty = 1;
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 2:
                            _nextLevelName = NAME_NEXT_SCENES[2];
                            GameManager.Instance.Difficulty = 2;
                            _changeSceneRequested = true;
                            FadeManager.Instance.FadeOut();
                            break;

                        case 3:
                            Application.Quit();
                            break;

                        default:
                            _nextLevelName = null;
                            break;
                    }
                    AudioSources[1].Play();
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
        AudioSources[0].Play();

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