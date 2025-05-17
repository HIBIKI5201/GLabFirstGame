using System;
using UnityEngine;

/// <summary>
/// プレイヤーがアイテムを投げる挙動の設定
/// </summary>
[Serializable]
public struct ThrowSetting
{
    public float ThrowStraightPower;
    public float MaxThrowParabolaPower;
    public float ThrowRate;
    public Vector2 ThrowParabolaDirection;
    public Vector2 ThrowPos;
    public LineRenderer BulletSimulationLine;
    public int SimulateFrame;
    public GameObject Platform;
}