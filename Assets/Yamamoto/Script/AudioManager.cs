using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string _name;
        public AudioClip _clip;
        [Range(0f, 1f)]
        public float _volume = 1.0f;
        [Range(0f, 1f)]
        public float _pitch = 1.0f;
    }

    public List<Sound> _bgmSounds;
    public List<Sound> _seSounds;

    private AudioSource _bgmSource;
    private AudioSource _seSource;


    float _fadeSpeed;
    float _fadeTime = 2f;
    float _startVolume;
    //AwakeでInstanceに保存＆一生壊されない処理
    void Awake()
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

        
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _seSource = gameObject.AddComponent<AudioSource>();
    }

    //以下管理用メソッド。使うときは「Audiomanager.Instance.○○○()」
    public void PlayBGM(string name)
    {
        Sound s = _bgmSounds.Find(sound => sound._name == name);
        if (s != null)
        {
            _bgmSource.clip = s._clip;
            _bgmSource.volume = s._volume;
            _bgmSource.pitch = s._pitch;
            _bgmSource.loop = true; 
            _bgmSource.Play();
        }
    }
    public void PlaySE(string name)
    {
        Sound s = _seSounds.Find(sound => sound._name == name);
        if (s != null)
        {
            _seSource.PlayOneShot(s._clip, s._volume);
        }
    }
    public IEnumerator FadeBGM(float _fadeTime)
    {
        Debug.Log("hatudou");
        float _startVolume = _bgmSource.volume;

        for(float t = 0; t < _fadeTime; t += Time.deltaTime)
        {
            _bgmSource.volume = Mathf.Lerp(_startVolume,0,t/_fadeTime);
            yield return null;
        }
        _bgmSource.volume = _startVolume;

    }

    public void FadeOutBGM()
    {
        Debug.Log("発動");
        StartCoroutine(FadeBGM(2f));
    }
}

