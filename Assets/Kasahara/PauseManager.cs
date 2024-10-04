using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    /// <summary>true �̎��͈ꎞ��~�Ƃ���</summary>
    bool _pauseFlg = false;
    /// <summary>�f���Q�[�g�����Ă����ϐ�</summary>
    Action<bool> _onPauseResume;
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
        if (Input.GetButtonDown("Cancel"))
        {
            PauseResume();
        }
    }

    /// <summary>
    /// �ꎞ��~�E�ĊJ��؂�ւ���
    /// </summary>
    void PauseResume()
    {
        _pauseFlg = !_pauseFlg;
        _onPauseResume(_pauseFlg);  // ����ŕϐ��ɑ�������֐����i�S�āj�Ăяo����
        if (_pauseFlg)
            PauseAllCoroutine();
        else
            ResumeAllCoroutine();
    }
    /// <summary>
    /// �|�[�Y���ė~�����R���[�`���������B(WaitForSeconds���g��Ȃ����Ƃ𐄏�)
    /// �R���[�`�����n�܂�������ɕK��GetCoroutine()���Ăяo����IEnumerator�^�̕ϐ��ɕۑ�����
    /// �R���[�`�����I�������K��OnComplete(�ۑ�����IEnumerator�^�̕ϐ�)���Ăяo������
    /// </summary>
    /// <param name="enumerator"></param>
    public void BeginCoroutine(IEnumerator enumerator)
    {
        _coroutines.Add(enumerator);
        StartCoroutine(enumerator);
    }
    public IEnumerator GetCoroutine()
    {
        return _coroutines[_coroutines.Count-1];
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
    public void OnComplete(IEnumerator enumerator)
    {
        _coroutines.Remove(enumerator);
    }
}
