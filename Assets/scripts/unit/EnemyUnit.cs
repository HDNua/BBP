using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 적 유닛입니다.
/// </summary>
public class EnemyUnit : Unit
{
    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        if (_alwaysInvencible)
        {
            Invencible = true;
        }
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected override void Update()
    {
        base.Update();
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    protected override void FixedUpdate()
    {

    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    protected override void LateUpdate()
    {
        // 색상표를 사용하는 개체인 경우 이 메서드를 오버라이드하고 다음 문장을 호출합니다.
        // UpdateColor();
    }

    #endregion



    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 캐릭터를 소환합니다.
    /// </summary>
    public void Spawn()
    {

    }
    /// <summary>
    /// 캐릭터를 사라지게 합니다.
    /// </summary>
    public void Disappear()
    {

    }

    #endregion
}
