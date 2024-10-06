using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class StartImage : MonoBehaviour
{
    [SerializeField] Image _startImage;
    [Header("一枚目のイメージ"), SerializeField] Image _firstImage;
    [Header("二枚目のイメージ"), SerializeField] Image _secondImage;
    [SerializeField] float _fadeinTime;
    [SerializeField] float _fadeoutTime;
    [Header("表示時間"), SerializeField] float _indicationTime;

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
        //一つ目のフェード
        StartCoroutine(FadeInterval(_firstImage, _indicationTime));
        //二つ目のフェード
        Invoke(nameof(Second), _fadeinTime + _indicationTime + _fadeoutTime);
    }
    private void Update()
    {
        if (Input.anyKeyDown) Skip();
    }
    private void Second()
    {
        StartCoroutine(FadeInterval(_secondImage, _indicationTime));
        Invoke(nameof(TitleFade), (_fadeinTime + _indicationTime + _fadeoutTime));
    }

    IEnumerator FadeInterval(Image image, float time)
    {
        image.DOFade(1f, _fadeinTime);
        yield return new WaitForSeconds(time);
        image.DOFade(0f, _fadeoutTime);
    }

    private void TitleFade()
    {
        _startImage.DOFade(0,_fadeinTime);
    }

    private void Skip()
    {
        StopAllCoroutines();
        Destroy(_startImage);
        Destroy(_firstImage);
        Destroy(_secondImage);
    }
}
