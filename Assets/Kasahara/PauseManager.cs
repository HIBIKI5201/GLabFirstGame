using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    /// <summary>true の時は一時停止とする</summary>
    bool _pauseFlg = false;
    /// <summary>デリゲートを入れておく変数</summary>
    Action<bool> _onPauseResume;
    List<IEnumerator> _coroutines = new List<IEnumerator>();
    /// <summary>
    /// 一時停止・再開を入れるデリゲートプロパティ
    /// </summary>
    public Action<bool> OnPauseResume
    {
        get { return _onPauseResume; }
        set { _onPauseResume = value; }
    }

    void Update()
    {
        // ESC キーが押されたら一時停止・再開を切り替える
        if (Input.GetButtonDown("Cancel"))
        {
            PauseResume();
        }
    }

    /// <summary>
    /// 一時停止・再開を切り替える
    /// </summary>
    void PauseResume()
    {
        _pauseFlg = !_pauseFlg;
        _onPauseResume(_pauseFlg);  // これで変数に代入した関数を（全て）呼び出せる
        if (_pauseFlg)
            PauseAllCoroutine();
        else
            ResumeAllCoroutine();
    }
    /// <summary>
    /// ポーズして欲しいコルーチンをいれる。(WaitForSecondsを使わないことを推奨)
    /// コルーチンが始まった直後に必ずGetCoroutine()を呼び出してIEnumerator型の変数に保存して
    /// コルーチンが終わったら必ずOnComplete(保存したIEnumerator型の変数)を呼び出すこと
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
