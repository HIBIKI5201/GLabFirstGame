using UnityEngine;

public class Petal : ItemBase
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private float _angle;
    private bool _effected;
    private Rigidbody2D _rb;
    private string _seNameGet = "getPetal"; // アイテム取得時に再生するSEの名称

    protected override void Activate()
    {
        // 取得のみのアイテムなので、何もしない
        return;
    }
    protected override void PlaySE()
    {
        AudioManager.Instance.PlaySE(_seNameGet);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
