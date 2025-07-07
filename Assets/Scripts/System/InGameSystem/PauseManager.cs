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
    [SerializeField] private GameObject[] PauseUI; // ポーズ時に表示・非表示を切り替えるUIオブジェクトの配列
    [SerializeField] private SceneLoader _loader;
    [SerializeField] private GameObject _fadePanel;
    [SerializeField] private Color _color;
    [SerializeField] private float _fadeTime;
    [SerializeField] private Checkpoint _checkpoint;
    [SerializeField] private Timer _timer;

    private bool _isPaused = false; // ポーズ状態かどうかを示すフラグ。_pauseFlgから変更

    private List<IEnumerator> _coroutines = new List<IEnumerator>();

    private Action<bool> _onPauseResume;
    public Action<bool> OnPauseResume
    {
        get { return _onPauseResume; }
        set { _onPauseResume = value; }
    }

    void Update()
    {
        // Cancelボタン（通常はEscape）が押され、かつシーン切り替え中ではない場合
        if (Input.GetButtonDown("Cancel") && !SceneLoader.IsFading)
        {
            // ポーズ状態を切り替える
            SetPauseState(!_isPaused); // 現在の状態を反転させて設定
        }
    }

    /// <summary>
    /// ポーズ解除ボタン用の処理 (UIボタンから呼び出す)
    /// UIボタンは常にポーズを解除する目的で呼ぶため、trueを渡す
    /// </summary>
    public void TogglePauseByUIButton()
    {
        Debug.Log("TogglePauseByUIButtonが呼ばれた - ポーズ解除を試みます。");
        SetPauseState(false); // ボタンが押されたらポーズを解除する (falseを設定)
    }

    /// <summary>
    /// ポーズの状態を設定する
    /// </summary>
    /// <param name="shouldPause">trueならポーズ、falseならポーズ解除</param>
    private void SetPauseState(bool shouldPause)
    {
        // 既に同じ状態なら何もしない
        if (_isPaused == shouldPause)
        {
            Debug.Log($"既にポーズ状態は {_isPaused} です。");
            return;
        }

        _isPaused = shouldPause; // ポーズ状態を更新
        _onPauseResume?.Invoke(_isPaused);

        // UIの表示/非表示を切り替える
        foreach (var uiObject in PauseUI)
        {
            if (uiObject != null) // null参照チェック
            {
                uiObject.SetActive(_isPaused);
            }
        }

        if (_isPaused)
        {
            Debug.Log("ゲームをポーズしました。");
            SuspendAllCoroutines();
            Time.timeScale = 0;
            _timer.StopTimer();
        }
        else
        {
            Debug.Log("ポーズを解除しました。");
            RestartAllCoroutine();
            Time.timeScale = 1;
            _timer.StartTimer();
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
        if (_coroutines.Count > 0)
        {
            return _coroutines[_coroutines.Count - 1];
        }
        return null; // リストが空の場合はnullを返す
    }

    /// <summary>
    /// ポーズ状態になった際に全てのコルーチンを一時停止する
    /// </summary>
    private void SuspendAllCoroutines()
    {
        foreach (IEnumerator enumerator in _coroutines)
        {
            // StopCoroutineの引数は IEnumerator インスタンスそのものを使うのが安全
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
            // RestartAllCoroutineではStartCoroutineを呼ぶ前に一度止める必要はない
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
        Debug.Log("ReturnToStageSelect: 呼ばれました");
        // ゲームをポーズ解除状態に戻してからシーンをロードする
        SetPauseState(false);

        _checkpoint.ResetPoint();
        _fadePanel.SetActive(true);
        _loader.FadeAndLoadScene(_fadePanel.GetComponent<Image>(), _color, _fadeTime, SceneType.SelectStage);
    }
}