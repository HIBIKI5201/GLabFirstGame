using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutBgm : MonoBehaviour
{
    float _fadeSpeed;
    float _fadeTime = 2f;
    float _startVolume;
    private bool _isFading = false;
    // Start is called before the first frame update
    void Start()
    {
        _startVolume = AudioManager._bgmSource.volume;
        _fadeSpeed = _startVolume / _fadeTime;
    }

    //ボタンにアタッチ用関数＆処理
    public void FadeBGM()
    {
        _isFading = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (_isFading)
        {
            if (AudioManager._bgmSource.volume > 0)
            {
                AudioManager._bgmSource.volume -= _fadeSpeed * Time.deltaTime;

                if (AudioManager._bgmSource.volume <= 0)
                {
                    AudioManager._bgmSource.volume = 0;
                    AudioManager._bgmSource.Stop();
                    _isFading = false;
                    AudioManager._bgmSource.volume = _startVolume;
                    Debug.Log(AudioManager._bgmSource.volume);
                }
            }
        }
    }

}
