using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class StartImage : MonoBehaviour
{
    [SerializeField] Image _startImage;
    [Header("�ꖇ�ڂ̃C���[�W"), SerializeField] Image _firstImage;
    [Header("�񖇖ڂ̃C���[�W"), SerializeField] Image _secondImage;
    [SerializeField] float _fadeinTime;
    [SerializeField] float _fadeoutTime;
    [Header("�\������"), SerializeField] float _indicationTime;

    private void Awake()
    {
        //_firstImage��_secondImage�̃A���t�@�l���O�ɂ���
        var fc = _firstImage.color;
        _firstImage.color = new Color(fc.r, fc.g, fc.b, 0);
        var sc = _secondImage.color;
        _secondImage.color = new Color(sc.r, sc.g, sc.b, 0);

    }
    void Start()
    {
        //��ڂ̃t�F�[�h
        StartCoroutine(FadeInterval(_firstImage, _indicationTime));
        //��ڂ̃t�F�[�h
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
