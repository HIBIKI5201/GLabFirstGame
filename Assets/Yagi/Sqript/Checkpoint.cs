using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] GameObject _player;

    [Header("変更される画像"), SerializeField] Sprite _changeSprite;

    SpriteRenderer _spriteRenderer;

    CapsuleCollider2D _capsuleCollider;

    static Vector2 _startPlayerPos;

    public static Vector2 _checkpoint { get; set; }

    public bool _isCheck = false;

    private void Start()
    {
        if (_startPlayerPos == Vector2.zero)_startPlayerPos = _player.transform.position;
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (_gameManager.State == GameManager.GameState.StageClear)
        {
            Debug.Log("チェックポイントをリセット");
            _checkpoint = _startPlayerPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("チェックポイントを通過");
            _isCheck = true;
            _checkpoint = _player.transform.position;
            _capsuleCollider.enabled = false;
            if (_changeSprite) _spriteRenderer.sprite = _changeSprite;
        }
    }
}
