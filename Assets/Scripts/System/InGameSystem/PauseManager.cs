using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ポーズ機能を管理するクラス
/// </summary>
public class PauseManager : MonoBehaviour
{
    [SerializeField] private  GameObject[] PauseUI;
    [SerializeField] private SceneLoader _loader;
    [SerializeField] private GameObject _fadePanel;
    [SerializeField] private Color _color;
    [SerializeField] private float _fadeTime;
    [SerializeField] private Checkpoint _checkpoint;
    private bool _pauseFlg = false;
    private List<IEnumerator> _coroutines = new List<IEnumerator>();
    
    private Action<bool> _onPauseResume;
    public Action<bool> OnPauseResume
    {
        get { return _onPauseResume; }
        set { _onPauseResume = value; }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !SceneLoader.IsFading)
        {
            TogglePauseState();
        }
    }
    
    /// <summary>
    /// ポーズ解除ボタン用の処理
    /// </summary>
    public void TogglePauseByUIButton() => TogglePauseState();
    
    /// <summary>
    /// ポーズの状態を変更する
    /// </summary>
    private void TogglePauseState()
    {
        _pauseFlg = !_pauseFlg;
        _onPauseResume?.Invoke(_pauseFlg);
        
        foreach (var i in PauseUI)
        {
            i.SetActive(_pauseFlg);
        }
        
        if (_pauseFlg)
        {
            SuspendAllCoroutines();
            Time.timeScale = 0;
        }
        else
        {
            RestartAllCoroutine();
            Time.timeScale = 1;
        }
    }
    
    /// <summary>
    /// 新しいコルーチンを開始し、管理リストに追加する
    /// </summary>
    public void RegisterAndStartCoroutine(IEnumerator routine)
    {
        _coroutines.Add(routine);
        StartCoroutine(routine);
    }
    
    /// <summary>
    /// 最後に追加されたコルーチンを取得する
    /// </summary>
    public IEnumerator GetLatestCoroutine()
    {
        return _coroutines[_coroutines.Count - 1];
    }
    
    /// <summary>
    /// ポーズ状態になった際に全てのコルーチンを一時停止する
    /// </summary>
    private void SuspendAllCoroutines()
    {
        foreach (IEnumerator enumerator in _coroutines)
        {
            Debug.Log(enumerator);
            StopCoroutine(enumerator);
        }
    }
    
    /// <summary>
    /// ポーズ解除時に一時停止中の全てのコルーチンを再開する
    /// </summary>
    private void RestartAllCoroutine()
    {
        foreach (IEnumerator enumerator in _coroutines)
        {
            StartCoroutine(enumerator);
        }
    }
    
    /// <summary>
    /// コルーチンの完了を通知し、管理リストから削除する
    /// </summary>
    public void OnComplete(IEnumerator enumerator)
    {
        _coroutines.Remove(enumerator);
    }
    
    /// <summary>
    /// ゲームを諦めて選択画面に戻る処理
    /// </summary>
    public void ReturnToStageSelect()
    {
        _checkpoint.ResetPoint();
        _fadePanel.SetActive(true);
        _loader.FadeAndLoadScene(_fadePanel.GetComponent<Image>(), _color, _fadeTime, SceneType.SelectStage);
    }
}
