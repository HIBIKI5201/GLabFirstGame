using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class StartImage : MonoBehaviour
{
    [SerializeField] Image _startImage;
    [SerializeField, Header("一枚目のイメージ")] Image _firstImage;
    [SerializeField, Header("二枚目のイメージ")] Image _secondImage;
    [SerializeField] float _fadeinTime;
    [SerializeField] float _fadeoutTime;
    [SerializeField, Header("表示時間")] float _indicationTime;
    private bool _isSkip = true;

    private void Awake()
    {
        //_firstImageと_secondImageのアルファ値を０にする
        var fc = _firstImage.color;
        _firstImage.color = new Color(fc.r, fc.g, fc.b, 0);
        var sc = _secondImage.color;
        _secondImage.color = new Color(sc.r, sc.g, sc.b, 0);
    }
    void Start()
    {
        //フェード開始
        StartCoroutine(FadeInterval(_firstImage, _secondImage, _indicationTime, _fadeinTime));
    }
    private void Update()
    {
        if (_isSkip)
        {
            //キーやマウスボタンを押したとき演出をスキップする
            if (Input.anyKeyDown) Skip();
        }
    }

    IEnumerator FadeInterval(Image image, Image image2, float time, float time2)
    {
        image.DOFade(1f, _fadeinTime);
        yield return new WaitForSeconds(time);
        image.DOFade(0f, _fadeoutTime);
        yield return new WaitForSeconds(time2);
        image2.DOFade(1f, _fadeinTime);
        yield return new WaitForSeconds(time);
        image2.DOFade(0f, _fadeoutTime);
        yield return new WaitForSeconds(_fadeoutTime);
        TitleFade();
    }

    private void TitleFade()
    {
        _startImage.DOFade(0, _fadeinTime);
    }

    private void Skip()
    {
        StopAllCoroutines();
        _startImage.enabled = false;
        _firstImage.enabled = false;
        var sc = _secondImage.color;
        _secondImage.color = new Color(sc.r, sc.g, sc.b, 1);
        _secondImage.DOFade(0, _fadeinTime);
        _isSkip = false;
    }
}