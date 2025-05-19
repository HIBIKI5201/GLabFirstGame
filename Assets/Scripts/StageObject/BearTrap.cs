using UnityEngine;
using System.Collections;

/// <summary>
/// トラバサミオブジェクトを管理するクラス
/// </summary>
public class BearTrap : MonoBehaviour
{
    [SerializeField] private GameObject _meat;
    [SerializeField, Tooltip("トラバサミが閉じているときの画像")] private Sprite _closedTorabasami;
    [SerializeField] float _stopTime;
    
    private SpriteRenderer _bearTrapSprite;
    private PlayerController _playerController;
    private bool _isTriggered;
    private float _timer;
    
    private void Start()
    {
        _bearTrapSprite = GetComponent<SpriteRenderer>();
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_isTriggered)
        {
            _bearTrapSprite.sprite = _closedTorabasami;
            
            //プレイヤーの位置をトラバサミの座標でロックする
            _playerController.transform.position = new Vector2(transform.position.x, _playerController.transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>();
            StartCoroutine(TrapTime(_stopTime));
            
            _playerController.FluctuationLife(-1); // プレイヤーにダメージを与える
            _playerController.StopAction(_stopTime); // 動けなくする
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySE("damage_enemy");
            Destroy(collision.gameObject);
            
            if (_meat)
            {
                Instantiate(_meat); // トラバサミにかかって肉を落とす敵だった場合、肉を生成する
            }
        }
    }
    
    private IEnumerator TrapTime(float time)
    {
        _isTriggered = true;
        
        yield return new WaitForSeconds(time);
        
        _isTriggered = false;
        Destroy(gameObject);
    }
}

