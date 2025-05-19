using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// タイトル画面の演出シーケンスを管理するクラス
/// </summary>
public class TitleSceneSequence : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _startImage;
    [SerializeField] private Image _firstImage;
    [SerializeField] private Image _secondImage;
    [SerializeField] private GameObject[] _button;
    
    [Header("アニメーションの時間設定")]
    [SerializeField] float _fadeinTime;
    [SerializeField] float _fadeoutTime;
    [SerializeField] float _indicationTime;
    
    [Header("ポストプロセッシング")]
    [SerializeField] private Volume _volume;
    
    private ColorAdjustments _colorAdjustments;
    private bool _isSkip = true;

    private void Awake()
    {
        InitializeUIElements();
        SetupPostProcessing();
    }
    
    private void Start()
    {
        StartCoroutine(PlayIntroSequence(_firstImage, _secondImage, _indicationTime, _fadeinTime));
    }
    
    private void Update()
    {
        if (_isSkip)
        {
            if (Input.anyKeyDown) SkipIntroSequence();
        }
    }
    
    /// <summary>
    /// UI要素の初期状態を設定
    /// </summary>
    private void InitializeUIElements()
    {
        // すべての画像を初期状態で透明に設定
        SetImageAlpha(_firstImage, 0f);
        SetImageAlpha(_secondImage, 0f);
        
        // メニューボタンを非表示に設定
        foreach(var b in _button)
        {
            b.SetActive(false);
        }
    }
    
    /// <summary>
    ///  ポストプロセッシングの設定
    /// </summary>
    private void SetupPostProcessing()
    {
        _volume.profile.TryGet(out _colorAdjustments);
        _colorAdjustments.postExposure.Override(0);
    }
    
    /// <summary>
    /// 画像のアルファ値を設定するヘルパーメソッド
    /// </summary>
    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        image.color = new Color(color.r, color.g, color.b, alpha);
    }

    /// <summary>
    /// シーケンスを再生
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayIntroSequence(Image image, Image image2, float time, float time2)
    {
        image.DOFade(1f, _fadeinTime); // 最初の画像をフェードイン
        
        yield return new WaitForSeconds(time);
        
        image.DOFade(0f, _fadeoutTime); // フェードアウト
        
        yield return new WaitForSeconds(time2);
        
        image2.DOFade(1f, _fadeinTime); // 2枚目の画像をフェードイン
        
        yield return new WaitForSeconds(time);
        
        image2.DOFade(0f, _fadeoutTime); // フェードアウト
        
        yield return new WaitForSeconds(_fadeoutTime);　
        
        _startImage.DOFade(0, _fadeinTime);
        ShowMainTitleScreen();
    }

    /// <summary>
    /// メインのタイトル画面を表示
    /// </summary>
    private void ShowMainTitleScreen()
    {
        _isSkip = false;
        _colorAdjustments.postExposure.Override(0.99f); // 露出値を調整して画面の明るさを変更
        
        // メニューボタンを表示
        foreach (var b in _button)
        {
            b.SetActive(true);
        }
    }

    /// <summary>
    /// イントロシーケンスをスキップしてタイトル画面に移行
    /// </summary>
    private void SkipIntroSequence()
    {
        StopAllCoroutines(); // 実行中のコルーチンを全て停止
        
        _startImage.enabled = false;
        _firstImage.enabled = false;
        
        var sc = _secondImage.color;
        
        SetImageAlpha(_secondImage, 1f);
        _secondImage.DOFade(0, _fadeinTime);
        ShowMainTitleScreen();
    }
}
