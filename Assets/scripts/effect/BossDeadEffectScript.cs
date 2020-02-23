using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 보스 사망 효과 스크립트입니다.
/// </summary>
public class BossDeadEffectScript : EffectScript
{
    #region 컨트롤러가 사용할 Unity 개체를 정의합니다.
    /// <summary>
    /// 스테이지 관리자입니다.
    /// </summary>
    StageManager _stageManager;
    /// <summary>
    /// 전투 관리자입니다.
    /// </summary>
    BattleManager _battleManager;

    /// <summary>
    /// 팔레트 사용자입니다.
    /// </summary>
    PaletteUser _paletteUser;

    #endregion





    #region 필드를 정의합니다.
    /// <summary>
    /// 원본 텍스쳐입니다.
    /// </summary>
    public Texture2D _originalTexture;
    /// <summary>
    /// 피격 텍스쳐입니다.
    /// </summary>
    public Texture2D _blinkingTexture;

    /// <summary>
    /// 폭발 개체입니다.
    /// </summary>
    public EffectBossExplosionScript _explosion;

    /// <summary>
    /// 불이 켜진 상태라면 참입니다.
    /// </summary>
    bool _highlighted = false;

    /// <summary>
    /// 
    /// </summary>
    public float _time = 0f;

    /// <summary>
    /// 
    /// </summary>
    public int _blinkCount1 = 8;
    /// <summary>
    /// 
    /// </summary>
    public float _blinkInterval1 = 0.2f;
    /// <summary>
    /// 
    /// </summary>
    public int _blinkCount2 = 20;
    /// <summary>
    /// 
    /// </summary>
    public float _blinkInterval2 = 0.1f;

    /// <summary>
    /// 폭발 효과가 지속된 시간입니다.
    /// </summary>
    float _explosionTime = 0f;

    /// <summary>
    /// 
    /// </summary>
    public float _explosionEndTime = 2f;

    /// <summary>
    /// 보스 캐릭터입니다.
    /// </summary>
    public EnemyBossUnit _boss;

    #endregion





    #region MonoBehaviour 기본 메서드를 재정의합니다.
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다. (최초 1회 수행)
    /// </summary>
    void Awake()
    {
        _battleManager = BattleManager.Instance;
        _stageManager = StageManager.Instance;
        _paletteUser = GetComponent<PaletteUser>();
    }
    /// <summary>
    /// MonoBehaviour 개체를 초기화합니다.
    /// </summary>
    void Start()
    {
        if (_battleManager.DoesBattleEnd())
        {
            StartCoroutine(CoroutineLastDead());
        }
        else
        {
            StartCoroutine(CoroutineDead());
        }

        /*
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Sprite sprite = renderer.sprite;
        _originalTexture = sprite.texture;
        _blinkingTexture = GetColorUpdatedTexture(
            sprite.texture,
            _boss.DefaultPalette,
            EnemyColorPalette.InvenciblePalette);
        */

        /*
        _defaultBossPalette = _boss.DefaultPalette;

        // 
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Sprite sprite = renderer.sprite;
        _originalTexture = sprite.texture;
        _blinkingTexture = GetColorUpdatedTexture(
            sprite.texture,
            _boss.DefaultPalette,
            EnemyColorPalette.InvenciblePalette);
        */
    }
    /// <summary>
    /// 프레임이 갱신될 때 MonoBehaviour 개체 정보를 업데이트합니다.
    /// </summary>
    void Update()
    {
        _time += Time.deltaTime;
    }
    /// <summary>
    /// 모든 Update 함수가 호출된 후 마지막으로 호출됩니다.
    /// 주로 오브젝트를 따라가게 설정한 카메라는 LastUpdate를 사용합니다.
    /// </summary>
    void LateUpdate()
    {
        _paletteUser.UpdateColor();
        //_boss.UpdatePaletteIndex(_highlighted ? 0 : 1);

        /*
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Sprite sprite = renderer.sprite;
        Texture2D texture = _highlighted ? _originalTexture : _blinkingTexture;

        // 새 텍스쳐를 렌더러에 반영합니다.
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", texture);
        renderer.SetPropertyBlock(block);
        */
    }

    #endregion





    #region 코루틴 메서드를 정의합니다.
    /// <summary>
    /// 사망 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineDead()
    {
        // 
        for (int i = 0; i < _blinkCount1; ++i)
        {
            if (_battleManager.DoesBattleEnd())
            {
                transform.SetParent(_stageManager._enemyParent.transform);
                yield break;
            }

            ToggleHighlighted();
            yield return new WaitForSeconds(_blinkInterval1);
        }

        // 
        EffectBossExplosionScript explosion = Instantiate
            (_explosion, transform.position, transform.rotation);
        explosion.gameObject.SetActive(true);

        // 
        _explosionTime = 0f;
        while (ExplosionEnd() == false)
        {
            if (_battleManager.DoesBattleEnd())
            {
                transform.SetParent(_stageManager._enemyParent.transform);
                explosion.gameObject.SetActive(false);
                yield break;
            }

            ToggleHighlighted();
            _explosionTime += Time.deltaTime + _blinkInterval2;
            yield return new WaitForSeconds(_blinkInterval2);
        }

        // 
        AudioSource se = _stageManager.BossClearExplosionSoundEffect;
        se.Play();

        // 
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = null;
        explosion.gameObject.SetActive(false);

        // 
        yield break;
    }
    /// <summary>
    /// 최종 사망 코루틴입니다.
    /// </summary>
    IEnumerator CoroutineLastDead()
    {
        // 
        for (int i = 0; i < _blinkCount1; ++i)
        {
            ToggleHighlighted();
            yield return new WaitForSeconds(_blinkInterval1);
        }

        // 
        StageManager stageManager = StageManager.Instance;
        stageManager.RequestStopBackgroundMusic();

        // 
        EffectBossExplosionScript explosion = Instantiate
            (_explosion, transform.position, transform.rotation);
        explosion.gameObject.SetActive(true);

        // 
        ScreenFader fader = stageManager._fader;
        fader.fadeSpeed = 0.2f;
        fader.ChangeFadeTextureColor(1);
        fader.ChangeFadeTextureColor(new Color(1, 1, 1, 0), Color.white, 0.5f);
        fader.FadeOut();

        // 
        while (ExplosionEnd() == false)
        {
            ToggleHighlighted();
            yield return new WaitForSeconds(_blinkInterval2);
        }

        // 
        AudioSource se = stageManager.BossClearExplosionSoundEffect;
        se.Play();

        // 
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = null;
        explosion.gameObject.SetActive(false);

        // 
        fader.fadeSpeed = 1f;
        fader.FadeIn();
        while (fader.FadeInEnded == false || se.isPlaying)
        {
            yield return false;
        }

        // 사망이 끝났습니다.
        fader.fadeSpeed = 3f;
        fader.ChangeFadeTextureColor(0);
        fader.ChangeFadeTextureColor(new Color(0, 0, 0, 0), Color.black, 0.95f);
        stageManager.RequestClearStage();
        yield break;
    }

    #endregion





    #region 보조 메서드를 정의합니다.
    /// <summary>
    /// 하이라이트 상태를 전환합니다.
    /// </summary>
    void ToggleHighlighted()
    {
        if (_highlighted)
        {
            _paletteUser.UpdatePaletteIndex(0);
        }
        else
        {
            _paletteUser.UpdatePaletteIndex(1);
        }
        _highlighted = !_highlighted;
    }

    /// <summary>
    /// 폭발이 끝났는지 확인합니다.
    /// </summary>
    /// <returns>폭발이 끝났다면 참입니다.</returns>
    bool ExplosionEnd()
    {
        if (_battleManager.DoesBattleEnd())
        {
            return ScreenFader.Instance.FadeOutEnded;
        }
        else
        {
            //Debug.Log(_explosionTime);
            return (_explosionTime >= _explosionEndTime);
        }
    }

    #endregion





    #region 구형 정의를 보관합니다.
    [Obsolete("EnemyBossUnit으로 대체되었습니다.")]
    /// <summary>
    /// 보스 캐릭터입니다.
    /// </summary>
    public EnemyBossScript _bossScript;
    [Obsolete("PaletteUser로 대체되었습니다.")]
    /// <summary>
    /// 
    /// </summary>
    public Color[] _defaultBossPalette;

    #endregion
}
