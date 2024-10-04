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
    }
}
