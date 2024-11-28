using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField,Header("�|�[�Y���ɗL��������UI"),Tooltip("�|�[�Y���ɗL��������UI")] GameObject[] PauseUI;
    [Header("���^�C�A�������̃t�F�[�h�A�E�g�̐ݒ�")]
    [SerializeField] SceneLoader _loader;
    [SerializeField] GameObject _fadePanel;
    [SerializeField] Color _color;
    [SerializeField] float _fadeTime;
    [SerializeField] Checkpoint _checkpoint;
    /// <summary>true �̎��͈ꎞ��~�Ƃ���</summary>
    bool _pauseFlg = false;
    /// <summary>�f���Q�[�g�����Ă����ϐ�</summary>
    Action<bool> _onPauseResume;
    /// <summary>�R���[�`��������郊�X�g</summary>
    List<IEnumerator> _coroutines = new List<IEnumerator>();
    /// <summary>
    /// �ꎞ��~�E�ĊJ������f���Q�[�g�v���p�e�B
    /// </summary>
    public Action<bool> OnPauseResume
    {
        get { return _onPauseResume; }
        set { _onPauseResume = value; }
    }

    void Update()
    {
        // ESC �L�[�������ꂽ��ꎞ��~�E�ĊJ��؂�ւ���
        if (Input.GetButtonDown("Cancel") && !SceneLoader.IsFading)
        {
            PauseResume();
        }
    }
    /// <summary>
    /// �{�^���N���b�N�ňꎞ��~�E�ĊJ���邽�߂̃��\�b�h
    /// </summary>
    public void PauseResumeByClick()
    {
        PauseResume();
    }
    /// <summary>
    /// �{�^���N���b�N�Ń��^�C�A���邽�߂̃��\�b�h
    /// </summary>
    public void Surrender()
    {
        _checkpoint.ResetPoint();
        _fadePanel.SetActive(true);
        _loader.FadeAndLoadScene(_fadePanel.GetComponent<Image>(), _color, _fadeTime, SceneLoader.Scenes.SelectStage);
    }
    /// <summary>
    /// �ꎞ��~�E�ĊJ��؂�ւ���
    /// </summary>
    void PauseResume()
    {
        _pauseFlg = !_pauseFlg;
        _onPauseResume(_pauseFlg);  // ����ŕϐ��ɑ�������֐����i�S�āj�Ăяo����
        foreach (var i in PauseUI)
        {
            i.SetActive(_pauseFlg);
        }
        if (_pauseFlg)
        {
            PauseAllCoroutine();
            Time.timeScale = 0;
        }
        else
        {
            ResumeAllCoroutine();
            Time.timeScale = 1;
        }
    }
    /// <summary>
    /// �|�[�Y���ė~�����R���[�`���������B(WaitForSeconds���g��Ȃ����Ƃ𐄏�)
    /// �R���[�`�����n�܂�������ɕK��GetCoroutine()���Ăяo����IEnumerator�^�̕ϐ��ɕۑ�����
    /// �R���[�`�����I�������K��1�t���[���҂��ĕK��OnComplete(�ۑ�����IEnumerator�^�̕ϐ�)���Ăяo������
    /// </summary>
    /// <param name="enumerator"></param>
    public void BeginCoroutine(IEnumerator enumerator)
    {
        _coroutines.Add(enumerator);
        StartCoroutine(enumerator);
    }
    /// <summary>
    /// ���������R���[�`�������҂����o���Ă������\�b�h
    /// </summary>
    public IEnumerator GetCoroutine()
    {
        return _coroutines[_coroutines.Count - 1];
    }
    void PauseAllCoroutine()
    {
        foreach (IEnumerator enumerator in _coroutines)
        {
            Debug.Log(enumerator);
            StopCoroutine(enumerator);
        }
    }
    void ResumeAllCoroutine()
    {
        foreach (IEnumerator enumerator in _coroutines)
        {
            StartCoroutine(enumerator);
        }
    }
    /// <summary>
    /// �R���[�`�����I��������Ă�
    /// �ĂԑO��yield return new WaitForEndOfFrame()����������
    /// </summary>
    /// <param name="enumerator"></param>
    public void OnComplete(IEnumerator enumerator)
    {
        _coroutines.Remove(enumerator);
    }
}
