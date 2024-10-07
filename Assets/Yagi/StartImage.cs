using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class StartImage : MonoBehaviour
{
    [SerializeField] Image _startImage;
    [SerializeField, Header("�ꖇ�ڂ̃C���[�W")] Image _firstImage;
    [SerializeField, Header("�񖇖ڂ̃C���[�W")] Image _secondImage;
    [SerializeField] float _fadeinTime;
    [SerializeField] float _fadeoutTime;
    [SerializeField, Header("�\������")] float _indicationTime;

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
        //�L�[��}�E�X�{�^�����������Ƃ����o���X�L�b�v����
        if (Input.anyKeyDown) Skip();
    }
    private void Second()
    {
        StartCoroutine(FadeInterval(_secondImage, _indicationTime));
        //�^�C�g���̃t�F�[�h�C��
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
    }
}