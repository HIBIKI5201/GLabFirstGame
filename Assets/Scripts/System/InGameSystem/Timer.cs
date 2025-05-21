using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイマー
/// </summary>
public class Timer : MonoBehaviour
{
    [SerializeField] private Text timerTxt;
    [SerializeField] private float _startTime;
    public float _currentTime;
    private bool _sePlayed;

    private void Start()
    {
        _currentTime = _startTime;
        UpdateTimerText();
    }
    
    private void Update()
    {
        if (_currentTime <= 30)
        {
            TimerColorChange();
            if(!_sePlayed) AudioManager.Instance.PlaySE("timeLimit");
            _sePlayed = true;
        }

        if(_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            _currentTime = 0;
            UpdateTimerText();
        }
    }

    /// <summary>
    /// タイマーのUIを更新する
    /// </summary>
    private void UpdateTimerText()
    {
        int min = Mathf.FloorToInt(_currentTime / 60);
        int sec = Mathf.FloorToInt(_currentTime % 60);
        timerTxt.text = $"{min:00}:{sec:00}";
    }

    /// <summary>
    /// タイマーのテキストの色を変更する。制限時間が近付いてきた時に呼び出される
    /// </summary>
    private void TimerColorChange()
    {
        timerTxt.color = new Color(210, 0, 0);
    }
}
