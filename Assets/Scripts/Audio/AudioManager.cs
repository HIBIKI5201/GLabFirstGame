using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio再生を管理するクラス
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // シングルトン

    [Header("サウンドデータ設定")]
    public List<SoundData> _bgmSounds;
    public List<SoundData> _seSounds;
    
    public AudioSource BGMSource => _bgmSource; // プロパティ
    private AudioSource _bgmSource;
    
    public AudioSource SESource => _seSource;
    private AudioSource _seSource;

    private float _fadeSpeed;
    private float _fadeTime = 1f;
    private float _startVolume;
    private bool _isFading;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // 自身のゲームオブジェクトにAudioSourceコンポーネントを追加する
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _seSource = gameObject.AddComponent<AudioSource>();
    }
    
    private void Update()
    {
        if (_isFading)
        {
            FadeBGM();
        }
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    public void PlayBGM(string name)
    {
        SoundData data = _bgmSounds.Find(sound => sound._name == name);
        
        if (data != null)
        {
            // データが見つかったら、登録されたデータをBGM用のAudioSourceに適用する
            BGMSource.clip = data._clip;
            BGMSource.volume = data._volume;
            BGMSource.pitch = data._pitch;
            BGMSource.loop = true; 
            BGMSource.Play();
        }
    }
    
    /// <summary>
    /// SEを1回再生する
    /// </summary>
    public void PlaySE(string name)
    {
        SoundData data = _seSounds.Find(sound => sound._name == name);
     
        if (data != null)
        {
            // データが見つかったら、SE用のAudioSourceでSEを再生
            _seSource.PlayOneShot(data._clip, data._volume);
        }
    }
    
    /// <summary>
    /// BGMをフェードアウトさせる
    /// </summary>
    public void FadeBGM()
    {
        _startVolume = BGMSource.volume;
        _fadeSpeed = _startVolume / _fadeTime;
        BGMSource.volume -= _fadeSpeed * Time.deltaTime;
        
        if (BGMSource.volume < 0.14f)
        {
            BGMSource.volume = 0;
            BGMSource.Stop();
            BGMSource.volume = _startVolume;
            _isFading = false;
        }
    }

    /// <summary>
    /// BGMをフェードインさせる
    /// </summary>
    public void FadeInBGM()
    {
        _startVolume = 0f;
        _fadeSpeed = 0.3f;
        PlayBGM("bossFight");
        BGMSource.volume += _fadeSpeed * Time.deltaTime;

        if(BGMSource.volume > 0.6f)
        {
            BGMSource.volume = 0.6f;
        }
    }
    
    /// <summary>
    /// フェードアウトを開始する
    /// </summary>
    public void OnFading() => _isFading = true; // ラムダ式
}

