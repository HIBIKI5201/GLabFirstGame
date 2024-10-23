using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Text timerTxt;
    [SerializeField][Header("äJénéûä‘")] float _startTime;
    public float _currentTime;

    // Start is called before the first frame update
    void Start()
    {
        _currentTime = _startTime;
        UpdateTimerText();
    }

    
    // Update is called once per frame
    void Update()
    {
        if(_currentTime <= 30) TimerColorChange();

        if(_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            _currentTime = 0;
            UpdateTimerText();
            //HP0ÇÃéûÇÃèàóù
        }
    }

    void UpdateTimerText()
    {
        int min = Mathf.FloorToInt(_currentTime / 60);
        int sec = Mathf.FloorToInt(_currentTime % 60);
        timerTxt.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    void TimerColorChange()
    {
        timerTxt.color = new Color(210, 0, 0);
    }
}
