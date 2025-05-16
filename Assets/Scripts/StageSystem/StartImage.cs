using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StartImage : MonoBehaviour
{
    [SerializeField] Image _startImage;
    [SerializeField, Header("�ꖇ�ڂ̃C���[�W")] Image _firstImage;
    [SerializeField, Header("�񖇖ڂ̃C���[�W")] Image _secondImage;
    [SerializeField] float _fadeinTime;
    [SerializeField] float _fadeoutTime;
    [SerializeField, Header("������")] float _indicationTime;
    [SerializeField, Header("�S�Ẵ{�^��������")] GameObject[] _button;
    [SerializeField] Volume _volume;
    private ColorAdjustments _colorAdjustments;
    private bool _isSkip = true;

    private void Awake()
    {
        //_firstImage��_secondImage�̃A���t�@�l���O�ɂ���
        var fc = _firstImage.color;
        _firstImage.color = new Color(fc.r, fc.g, fc.b, 0);
        var sc = _secondImage.color;
        _secondImage.color = new Color(sc.r, sc.g, sc.b, 0);
        _volume.profile.TryGet(out _colorAdjustments);
        _colorAdjustments.postExposure.Override(0);
        foreach(var b in _button)
        {
            b.SetActive(false);
        }
    }
    void Start()
    {
        //�t�F�[�h�J�n
        StartCoroutine(FadeInterval(_firstImage, _secondImage, _indicationTime, _fadeinTime));
    }
    private void Update()
    {
        if (_isSkip)
        {
            //�L�[��}�E�X�{�^�����������Ƃ����o���X�L�b�v����
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
        _isSkip = false;
        _startImage.DOFade(0, _fadeinTime);
        _colorAdjustments.postExposure.Override(0.99f);
        foreach (var b in _button)
        {
            b.SetActive(true);
        }
    }

    private void Skip()
    {
        StopAllCoroutines();
        _startImage.enabled = false;
        _firstImage.enabled = false;
        var sc = _secondImage.color;
        _secondImage.color = new Color(sc.r, sc.g, sc.b, 1);
        _secondImage.DOFade(0, _fadeinTime);
        _colorAdjustments.postExposure.Override(0.99f);
        _isSkip = false;
        foreach (var b in _button)
        {
            b.SetActive(true);
        }
    }
}
