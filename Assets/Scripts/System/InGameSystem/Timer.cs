using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイマー
/// </summary>
public class Timer : MonoBehaviour
{
    [SerializeField] private Text _timerTxt;
    [SerializeField] private int _startTime = 150; // 開始時の残り秒数
    
    /// <summary>
    /// 残り時間
    /// </summary>
    public ReactiveProperty<int> CurrentTimeProp => _currentTimeProp;
    private ReactiveProperty<int> _currentTimeProp = new ReactiveProperty<int>();
    public float CurrentTime => _currentTimeProp.Value;
    
    private IDisposable _timerSubscription;

    private void Start()
    {
        _currentTimeProp.Value = _startTime; // 残り秒数の初期値をセットする

        StartTimer(); // タイマーを開始する
        
        CurrentTimeProp
            .Subscribe(time =>
            {
                UpdateTimerText(time);
                
                if (time == 30)
                {
                    // 30秒以下になったらSEを鳴らすのと、タイマーテキストの色を変更する
                    AudioManager.Instance.PlaySE("timeLimit");
                    TimerColorChange();
                }
            })
            .AddTo(this);
    }

    /// <summary>
    /// タイマーを停止する
    /// </summary>
    public void StopTimer()
    {
        if (_timerSubscription != null)
        {
            _timerSubscription.Dispose();
            _timerSubscription = null;
        }
    }

    /// <summary>
    /// タイマーを開始する
    /// </summary>
    public void StartTimer()
    {
        _timerSubscription = Observable
            .Interval(TimeSpan.FromSeconds(1)) // 1秒おきに
            .Subscribe(_ => _currentTimeProp.Value--) // 残り秒数を1秒減らす
            .AddTo(this);
    }
    
    /// <summary>
    /// タイマーのUIを更新する
    /// </summary>
    private void UpdateTimerText(int time)
    {
        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.FloorToInt(time % 60);
        _timerTxt.text = $"{min:00}:{sec:00}";
    }

    /// <summary>
    /// タイマーのテキストの色を変更する。制限時間が近付いてきた時に呼び出される
    /// </summary>
    private void TimerColorChange()
    {
        _timerTxt.color = new Color(210, 0, 0);
    }
}
