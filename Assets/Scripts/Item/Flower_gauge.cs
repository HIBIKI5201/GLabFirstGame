using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 花びらのゲージ
/// </summary>
public class Flower_gauge : MonoBehaviour
{
    public Slider petalGauge;//花びらゲージ
    public int petalsToHeal = 5;//５枚で回復
    private int currentPetals = 0;//現在の花びら数

    void start()
    {
        //スライダーの初期設定
        petalGauge.maxValue = petalsToHeal;
        petalGauge.value = currentPetals;
    }

    void Update()
    {
        if ()//花びらの取得
        {
            CollectPetal();
        }
    }
    public void CollectPetal()
    {
        currentPetals++;
        if (currentPetals >= petalsToHeal)//花びらを５枚あつめる
        {
            currentPetals = 0;//ゲージのリセット
        }

        petalGauge.value = currentPetals;//ゲージUIの更新
    }
}
