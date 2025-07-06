using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 花びらのゲージ
/// </summary>
public class Flower_gauge : MonoBehaviour
{
    [SerializeField] private Image petal_1;
    [SerializeField] private Image petal_2;
    [SerializeField] private Image petal_3;
    [SerializeField] private Image petal_4;
        
    [SerializeField, Range(0, 255)]
    public float transparency;//最初の透明度

    public int petalsToHeal = 4;//４枚で回復
    private int currentPetals = 0;//現在の花びら数

    void Start()
    {
        ColorReset();
    }

    void ColorReset()//透明度の初期化
    {
        float alpha = transparency / 255f;
        Color newColor = new (1f,1f,1f,alpha);
        petal_1.color = newColor;
        petal_2.color = newColor;
        petal_3.color = newColor;
        petal_4.color = newColor;
    }
    void Update()
    {
        if (currentPetals == 0)
        {   
            ColorReset();
        }
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Petal"))//花びらの取得した時
        {
            CollectPetal();
            Destroy(other.gameObject);
        }
    }
    
    private void CollectPetal()
    {
        currentPetals++;
        Debug.Log("collect");
        switch (currentPetals)//取得した枚数に応じて、透明度をかえる
        {
            case 1://動作確認用のコードでアニメーションの変化は未実装
                Debug.Log("一枚目");
                petal_1.color = new Color(1f, 1f, 1f, 1f);
                break;
            case 2:
                petal_2.color = new Color(1f, 1f, 1f, 1f);
                break;
            case 3:
                petal_3.color = new Color(1f, 1f, 1f, 1f);
                break;
        }
        if (currentPetals >= petalsToHeal)//花びらを４枚あつめる
        {
            currentPetals = 0;//ゲージのリセット
        }
    }
}
