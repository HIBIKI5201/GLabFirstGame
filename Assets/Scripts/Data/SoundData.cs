using UnityEngine;
using System;

/// <summary>
/// 音源データを管理するクラス
/// </summary>
[Serializable]
public class SoundData
{
    public string _name; // 音源データの名前
    public AudioClip _clip; // データ
    [Range(0, 1)] public float _volume = 1.0f; // 音量
    [Range(0, 1)] public float _pitch = 1.0f; // ピッチ
}