using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PaletteUser))]
/// <summary>
/// 유닛을 정의합니다.
/// </summary>
public class Unit : MonoBehaviour
{
    #region 상수를 정의합니다.
    /// <summary>
    /// 1/30 프레임 간의 시간입니다.
    /// </summary>
    public const float TIME_30FPS = 0.0333333f;
    /// <summary>
    /// 1/60 프레임 간의 시간입니다.
    /// </summary>
    public const float TIME_60FPS = 0.0166667f;
    /// <summary>
    /// 무적 시간입니다.
    /// </summary>
    public float INVENCIBLE_TIME_LENGTH = 1f;

    #endregion



    #region 컨트롤러가 사용할 Unity 개체를 정의합니다.
    /// <summary>
    /// Rigidbody2D 요소를 가져옵니다.
    /// </summary>
    protected Rigidbody2D _Rigidbody
    {
        get { return GetComponent<Rigidbody2D>(); }
    }
    /// <summary>
    /// Collider2D 요소를 가져옵니다.
    /// </summary>
    protected Collider2D _Collider
    {
        get { return GetComponent<Collider2D>(); }
    }
    /// <summary>
    /// Animator 요소를 가져옵니다.
    /// </summary>
    protected Animator _Animator
    {
        get { return GetComponent<Animator>(); }
    }
    /// <summary>
    /// SpriteRenderer 요소를 가져옵니다.
    /// </summary>
    protected SpriteRenderer _Renderer
    {
        get { return GetComponent<SpriteRenderer>(); }
    }
    /// <summary>
    /// PaletteUser 요소를 가져옵니다.
    /// </summary>
    protected PaletteUser _PaletteUser
    {
        get { return GetComponent<PaletteUser>(); }
    }

    /// <summary>
    /// 스테이지 관리자입니다.
    /// </summary>
    protected StageManager _StageManager
    {
        get { return StageManager.Instance; }
    }

    #endregion



    #region Unity에서 접근 가능한 공용 필드를 정의합니다.
    /// <summary>
    /// 캐릭터가 오른쪽을 보고 있다면 참입니다.
    /// </summary>
    public bool _facingRight;

    /// <summary>
    /// 캐릭터의 체력입니다.
    /// </summary>
    public int _health;
    /// <summary>
    /// 캐릭터와 충돌했을 때 플레이어가 입을 대미지입니다.
    /// </summary>
    public int _damage;
    /// <summary>
    /// 항상 무적 상태라면 참입니다.
    /// </summary>
    public bool _alwaysInvencible = false;

    /// <summary>
    /// 탄환을 무시한다면 참입니다.
    /// </summary>
    public bool _hasBulletImmunity = false;


    /// <summary>
    /// 캐릭터가 사용할 효과음 집합입니다.
    /// </summary>
    public AudioClip[] _audioClips;
    /// <summary>
    /// 캐릭터가 사용할 효과 집합입니다.
    /// </summary>
    public GameObject[] _effects;

    /// <summary>
    /// 적이 사망할 때 드롭 가능한 아이템의 목록입니다.
    /// </summary>
    public ItemScript[] _items;

    /// <summary>
    /// 사망 시 효과를 보관하는 개체입니다.
    /// </summary>
    public ParticleSpreadScript _deadParticleSpreadEffect;

    /// <summary>
    /// 피격 텍스쳐 집합입니다.
    /// </summary>
    Dictionary<int, Texture2D> _hitTextures = new Dictionary<int, Texture2D>();


    #endregion



    #region 캐릭터의 물리 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 캐릭터가 오른쪽을 보고 있다면 참입니다.
    /// </summary>
    public bool FacingRight
    {
        get { return _facingRight; }
        set { if (_facingRight != value) Flip(); }
    }

    /// <summary>
    /// X 좌표 값입니다.
    /// </summary>
    public float PosX
    {
        get { return transform.position.x; }
        set { transform.position = new Vector3(value, transform.position.y, transform.position.z); }
    }
    /// <summary>
    /// Y 좌표 값입니다.
    /// </summary>
    public float PosY
    {
        get { return transform.position.y; }
        set { transform.position = new Vector3(transform.position.x, value, transform.position.z); }
    }
    /// <summary>
    /// Z 좌표 값입니다.
    /// </summary>
    public float PosZ
    {
        get { return transform.position.z; }
        set { transform.position = new Vector3(transform.position.x, transform.position.y, value); }
    }

    #endregion



    #region 캐릭터의 상태 필드 및 프로퍼티를 정의합니다.
    /// <summary>
    /// 사망했다면 참입니다.
    /// </summary>
    bool _isDead;
    /// <summary>
    /// 무적 상태라면 참입니다.
    /// </summary>
    bool _isInvencible;
    /// <summary>
    /// 피해를 입었다면 참입니다.
    /// </summary>
    bool _isDamaged;
    /// <summary>
    /// 무적 상태 시간입니다.
    /// </summary>
    float _invencibleTime = 0;

    /// <summary>
    /// 체력을 가져옵니다.
    /// </summary>
    public int Health
    {
        get { return _health; }
        protected set
        {
            _health = value > 0 ? value : 0;
        }
    }
    /// <summary>
    /// 대미지를 가져옵니다.
    /// </summary>
    public int Damage
    {
        get { return _damage; }
    }

    /// <summary>
    /// 대미지를 입었다면 참입니다.
    /// </summary>
    public bool IsDamaged
    {
        get { return _isDamaged; }
        protected set { _isDamaged = value; }
    }
    /// <summary>
    /// 캐릭터가 죽었다면 참입니다.
    /// </summary>
    public bool IsDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    /// <summary>
    /// 캐릭터가 무적 상태라면 참입니다.
    /// </summary>
    public bool Invencible
    {
        get { return _isInvencible; }
        protected set { _isInvencible = value; }
    }
    /// <summary>
    /// 캐릭터가 살아있는지 확인합니다.
    /// </summary>
    /// <returns>캐릭터가 살아있다면 참입니다.</returns>
    public bool IsAlive()
    {
        return (0 < Health);
    }

    /// <summary>
    /// 탄환을 무시한다면 참입니다.
    /// </summary>
    public bool HasBulletImmunity
    {
        get { return _hasBulletImmunity; }
    }

    #endregion





    #region 기타 필드를 정의합니다.
    /// <summary>
    /// 캐릭터가 사용할 효과음을 사용 가능한 형태로 보관합니다.
    /// </summary>
    AudioSource[] _soundEffects;
    /// <summary>
    /// 캐릭터가 사용할 효과음을 사용 가능한 형태로 보관합니다.
    /// </summary>
    public AudioSource[] SoundEffects { get { return _soundEffects; } }

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의 합니다.
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    protected virtual void Awake()
    {
        _soundEffects = new AudioSource[_audioClips.Length];
        for (int i = 0, len = _audioClips.Length; i < len; ++i)
        {
            _soundEffects[i] = gameObject.AddComponent<AudioSource>();
            _soundEffects[i].clip = _audioClips[i];
        }
    }
    /// <summary>
    /// MonoBehaviour 객체를 초기화합니다.
    /// </summary>
    protected virtual void Start()
    {
        if (_alwaysInvencible)
        {
            Invencible = true;
        }
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    protected virtual void Update()
    {
        if (IsAlive() == false)
        {
            Dead();
        }
    }
    /// <summary>
    /// FixedTimestep에 설정된 값에 따라 일정한 간격으로 업데이트 합니다.
    /// 물리 효과가 적용된 오브젝트를 조정할 때 사용됩니다.
    /// (Update는 불규칙한 호출이기 때문에 물리엔진 충돌검사가 제대로 되지 않을 수 있습니다.)
    /// </summary>
    protected virtual void FixedUpdate() { }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    protected virtual void LateUpdate()
    {
        // 색상표를 사용하는 개체인 경우 이 메서드를 오버라이드하고 다음 문장을 호출합니다.
        // UpdateColor();
    }

    #endregion





    #region 행동 메서드를 정의합니다.
    /// <summary>
    /// 방향을 바꿉니다.
    /// </summary>
    public void Flip()
    {
        if (_facingRight)
        {
            transform.localScale = new Vector3
                (-transform.localScale.x, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector3
                (-transform.localScale.x, transform.localScale.y);
        }
        _facingRight = !_facingRight;
    }

    /// <summary>
    /// 유닛을 소환합니다.
    /// </summary>
    public virtual void Appear()
    {

    }
    /// <summary>
    /// 유닛을 사라지게 합니다.
    /// </summary>
    public virtual void Disappear()
    {

    }
    /// <summary>
    /// 사망합니다.
    /// </summary>
    public virtual void Dead()
    {
        gameObject.SetActive(false);

        // 사망 효과가 존재하는 적이라면 호출합니다.
        if (_deadParticleSpreadEffect != null)
        {
            Instantiate
                (_deadParticleSpreadEffect, transform.position, transform.rotation)
                .gameObject.SetActive(true);
        }

        //
        Destroy(gameObject);
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 애니메이터가 지정된 문자열의 상태인지 확인합니다.
    /// </summary>
    /// <param name="stateName">재생 중인지 확인하려는 상태의 이름입니다.</param>
    /// <param name="layerIndex">애니메이터 레이어 인덱스입니다. 기본값은 0입니다.</param>
    /// <returns>애니메이터가 지정된 문자열의 상태라면 true를 반환합니다.</returns>
    protected bool IsAnimatorInState(string stateName, int layerIndex = 0)
    {
        return _Animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
    }

    #endregion




    #region 컬러 팔레트 관련 메서드를 정의합니다.


    #endregion



    #region 코루틴을 정의합니다.
    /// <summary>
    /// 무적 상태 코루틴입니다.
    /// </summary>
    protected Coroutine _coroutineInvencible;

    /// <summary>
    /// 무적 상태에 대한 코루틴입니다.
    /// </summary>
    /// <returns>코루틴 열거자입니다.</returns>
    protected IEnumerator CoroutineInvencible()
    {
        _invencibleTime = 0;
        bool invencibleColorState = false;
        while (_invencibleTime < INVENCIBLE_TIME_LENGTH)
        {
            _invencibleTime += TIME_30FPS + Time.deltaTime;

            // 
            if (invencibleColorState)
            {
                UpdateColorWithInvenciblePalette();
            }
            else
            {
                UpdateColorWithoutInvenciblePalette();
            }
            invencibleColorState = !invencibleColorState;

            // 
            yield return new WaitForSeconds(TIME_30FPS);
        }
        Invencible = false;
        IsDamaged = false;
        UpdateColorEndOfInvencibleTime();
        yield break;
    }

    // 
    int _prevPaletteIndex = 0;
    int _currentPaletteIndex = 0;

    /// <summary>
    /// 팔레트를 업데이트 합니다.
    /// </summary>
    /// <param name="newPaletteIndex">새 팔레트의 인덱스입니다.</param>
    public void UpdatePaletteIndex(int newPaletteIndex)
    {
        _prevPaletteIndex = _currentPaletteIndex;
        _currentPaletteIndex = newPaletteIndex;
        _PaletteUser.UpdatePaletteIndex(_currentPaletteIndex);
    }

    /// <summary>
    /// 무적 상태 팔레트로 색상을 업데이트합니다.
    /// </summary>
    void UpdateColorWithInvenciblePalette()
    {
        ///_currentPalette = EnemyColorPalette.InvenciblePalette;

        // 
        UpdatePaletteIndex(1);
    }
    /// <summary>
    /// 무적 상태가 아닌 팔레트로 색상을 업데이트합니다.
    /// </summary>
    void UpdateColorWithoutInvenciblePalette()
    {
        ///ResetBodyColor();

        //
        UpdatePaletteIndex(_prevPaletteIndex);
    }
    /// <summary>
    /// 무적 상태가 끝난 후의 색상을 업데이트합니다.
    /// </summary>
    void UpdateColorEndOfInvencibleTime()
    {
        ///ResetBodyColor();

        // 
        if (_prevPaletteIndex == 1 && _currentPaletteIndex == 1)
            throw new Exception("Unexpected palette index duplication");
        UpdatePaletteIndex(_prevPaletteIndex == 1 ? _currentPaletteIndex : _prevPaletteIndex);
    }

    #endregion




    #region 구형 정의를 보관합니다.
    [Obsolete("PaletteUser로 대체되었습니다.")]
    /// <summary>
    /// 현재 색상 팔레트입니다.
    /// </summary>
    protected Color[] _currentPalette = null;
    [Obsolete("PaletteUser로 대체되었습니다.")]
    /// <summary>
    /// 기본 색상 팔레트입니다.
    /// </summary>
    Color[] _defaultPalette = null;
    [Obsolete("PaletteUser로 대체되었습니다.")]
    /// <summary>
    /// 기본 색상 팔레트를 설정합니다.
    /// </summary>
    public Color[] DefaultPalette
    {
        get { return _defaultPalette; }
        set { _defaultPalette = value; }
    }


    [Obsolete("PaletteUser로 대체되었습니다.")]
    /// <summary>
    /// 색상을 업데이트합니다.
    /// </summary>
    protected void UpdateColor()
    {
        if (IsDamaged)
        {
            // 바디 색상을 맞춥니다.
            UpdateBodyColor(_currentPalette);
        }
    }
    [Obsolete("PaletteUser로 대체되었습니다.")]
    /// <summary>
    /// 색상을 주어진 팔레트로 업데이트합니다.
    /// </summary>
    /// <param name="_currentPalette">현재 팔레트입니다.</param>
    void UpdateBodyColor(Color[] currentPalette)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Sprite sprite = renderer.sprite;
        Texture2D texture = sprite.texture;
        Texture2D cloneTexture = null;

        // 
        if (currentPalette == null)
        {
            cloneTexture = texture;
        }
        else if (_hitTextures.ContainsKey(sprite.GetInstanceID()))
        {
            cloneTexture = _hitTextures[sprite.GetInstanceID()];
        }
        else
        {
            // !!!!! IMPORTANT !!!!!
            // 1. 텍스쳐 파일은 Read/Write 속성이 Enabled여야 합니다.
            // 2. 반드시 Generate Mip Maps 속성을 켜십시오.
            Color[] colors = texture.GetPixels();
            Color[] pixels = new Color[colors.Length];
            Color[] DefaultPalette = _defaultPalette;

            // 모든 픽셀을 돌면서 색상을 업데이트합니다.
            for (int pixelIndex = 0, pixelCount = colors.Length; pixelIndex < pixelCount; ++pixelIndex)
            {
                Color color = colors[pixelIndex];
                if (color.a == 1)
                {
                    for (int targetIndex = 0, targetPixelCount = DefaultPalette.Length; targetIndex < targetPixelCount; ++targetIndex)
                    {
                        Color colorDst = DefaultPalette[targetIndex];
                        if (Mathf.Approximately(color.r, colorDst.r) &&
                            Mathf.Approximately(color.g, colorDst.g) &&
                            Mathf.Approximately(color.b, colorDst.b) &&
                            Mathf.Approximately(color.a, colorDst.a))
                        {
                            pixels[pixelIndex] = currentPalette[targetIndex];
                            break;
                        }
                    }
                }
                else
                {
                    /// pixels[pixelIndex] = color;
                }
            }

            // 텍스쳐를 복제하고 새 픽셀 팔레트로 덮어씌웁니다.
            cloneTexture = new Texture2D(texture.width, texture.height);
            cloneTexture.filterMode = FilterMode.Point;
            cloneTexture.SetPixels(pixels);
            cloneTexture.Apply();

            // 
            _hitTextures.Add(sprite.GetInstanceID(), cloneTexture);
        }

        // 새 텍스쳐를 렌더러에 반영합니다.
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", cloneTexture);
        renderer.SetPropertyBlock(block);
    }
    [Obsolete("PaletteUser로 대체되었습니다.")]
    /// <summary>
    /// 바디 색상표를 초기화합니다.
    /// </summary>
    void ResetBodyColor()
    {
        _currentPalette = null;
    }

    #endregion
}