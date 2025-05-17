using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムに関する設定
/// </summary>
[Serializable]
public struct ItemSetting
{
    public int MaxRockCount;
    public int MaxBottleCount;
    public int MaxMeatCount;
    public GameObject RockUi;
    public GameObject BottleUi;
    public GameObject MeatUi;
    public Text RockCountText;
    public Text BottleCountText;
    public Text MeatCountText;
    public Color ZeroItemColor;
    public GameObject LeafRock;
    public GameObject LeafBottle;
    public GameObject LeafMeat;
    public float LeafSize;
}