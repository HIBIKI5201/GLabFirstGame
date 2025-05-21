using UnityEngine;

/// <summary>
/// チェックポイントオブジェクトを管理するクラス
/// </summary>
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [Header("チェックポイント通過後のイラスト"), SerializeField] private Sprite _changeSprite;
    [SerializeField] private int nowStage;
    private GameManager _gameManager;   
    private SpriteRenderer _spriteRenderer;
    private CapsuleCollider2D _capsuleCollider;
    private bool _isFirstCheck = false; // 通過済みかどうか

    private void Start()
    {        
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance.StateType == GameStateType.StageClear)
        {
            ResetPoint();
        }
    }
    
    /// <summary>
    /// 現在のステージのリスポーン地点をVector2.zeroの位置にセットしなおす
    /// </summary>
    public void ResetPoint() => CheckPointManager._checkPoint[nowStage - 1] = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Colliderにプレイヤーが入って、かつまだチェックポイント通過前だった場合
        if (collision.gameObject.CompareTag("Player") && !_isFirstCheck)
        {
            AudioManager.Instance.PlaySE("checkpoint");
            CheckPointManager._checkPoint[nowStage - 1] = transform.position; // リスポーン地点を変更する
            _capsuleCollider.enabled = false; // コライダーを無効化
            if (_changeSprite) _spriteRenderer.sprite = _changeSprite; // 画像を変更
            _isFirstCheck = true; // 通過済みとする
        }
    }
}
