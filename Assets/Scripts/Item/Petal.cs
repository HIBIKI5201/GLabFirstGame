using UnityEngine;

public class Petal : ItemBase
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private float _angle;
    private bool _effected;
    private Rigidbody2D _rb;
    private string _seNameGet = "getPetal"; // �A�C�e���擾���ɍĐ�����SE�̖���

    protected override void Activate()
    {
        // �擾�݂̂̃A�C�e���Ȃ̂ŁA�������Ȃ�
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
