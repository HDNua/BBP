using System;
using UnityEngine;
using System.Collections;



/// <summary>
/// 캡콤 로고 장면 관리자입니다.
/// </summary>
public class CapcomLogoSceneManager : HDSceneManager
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 장면 생존 시간입니다.
    /// </summary>
    public float TIME_SCENE = 7f;

    #endregion



    #region 필드를 정의합니다.
    /// <summary>
    /// 종료가 요청되었다면 참입니다.
    /// </summary>
    private bool _endRequested = false;

    #endregion



    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        FadeManager.Instance.FadeIn();

        StartCoroutine(SceneCoroutine());
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        if (_endRequested)
        {
            if (FadeManager.Instance.FadeOutEnded)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(NAME_NEXT_SCENES[0]);
            }
            return;
        }
        if (Input.anyKeyDown)
        {
            RequestEnd();
        }
    }

    #endregion



    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 종료를 요청합니다.
    /// </summary>
    private void RequestEnd()
    {
        _endRequested = true;
        FadeManager.Instance.FadeOut();
    }
    /// <summary>
    /// 장면 코루틴입니다.
    /// </summary>
    IEnumerator SceneCoroutine()
    {
        yield return new WaitForSeconds(TIME_SCENE);

        RequestEnd();
        yield break;
    }

    #endregion
}
