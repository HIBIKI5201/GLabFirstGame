using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] GameObject _player;

    [Header("変更するイラスト"), SerializeField] Sprite _changeSprite;

    SpriteRenderer _spriteRenderer;

    CapsuleCollider2D _capsuleCollider;

    [SerializeField] int nowStage;

    private bool _isFirstCheck = false;

    private void Start()
    {        
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.instance.StateType == GameStateType.StageClear)
        {
            ResetPoint();
        }
    }

   
    public void ResetPoint()
    {
        CheckPointManager._checkPoint[nowStage - 1] = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isFirstCheck)
        {
            AudioManager.Instance.PlaySE("checkpoint");
            CheckPointManager._checkPoint[nowStage - 1] = this.transform.position;
            _capsuleCollider.enabled = false;
            if (_changeSprite) _spriteRenderer.sprite = _changeSprite;
            _isFirstCheck = true;
        }
    }
}
