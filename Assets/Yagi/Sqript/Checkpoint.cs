using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] GameObject _player;

    [Header("変更される画像"), SerializeField] Sprite _changeSprite;

    SpriteRenderer _spriteRenderer;

    CapsuleCollider2D _capsuleCollider;

    static Vector2 _startPlayerPos;

    [SerializeField] int _nowStage;

    static Vector2 _stage3Start;

    public static Vector2 _checkpoint { get; set; }

    public bool _isCheck = false;

    private void Awake()
    {
        if (_startPlayerPos == Vector2.zero) _startPlayerPos = _player.transform.position;
        
        if (_nowStage == 2)
        {
            _stage3Start = new Vector2(_startPlayerPos.x, _startPlayerPos.y + 2.85f);
            _startPlayerPos = _stage3Start;
        }
        else
        {
            if (_startPlayerPos == _stage3Start)
            {
                _startPlayerPos = new Vector2(_startPlayerPos.x, _startPlayerPos.y - 2.85f);
            }
        }
    }
    private void Start()
    {        
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.StageClear)
        {
            ResetPoint();
        }
    }

    /// <summary>
    ///Debug.Log("チェックポイントをリセット");
    /// </summary>
    public void ResetPoint()
    {
        _checkpoint = _startPlayerPos;
        Debug.Log(_checkpoint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("チェックポイントを通過");
            AudioManager.Instance.PlaySE("checkpoint");
            _isCheck = true;
            if(_nowStage == 3) _checkpoint = new Vector2(_player.transform.position.x,_player.transform.position.y - 2.85f);
            else _checkpoint = _player.transform.position;
            _capsuleCollider.enabled = false;
            if (_changeSprite) _spriteRenderer.sprite = _changeSprite;
        }
    }
}
