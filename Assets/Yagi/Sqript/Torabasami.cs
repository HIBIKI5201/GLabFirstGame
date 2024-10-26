using UnityEngine;
using System.Collections;



public class Torabasami : MonoBehaviour
{
    [SerializeField, Header("生成する肉")] GameObject _meat;
    [SerializeField, Header("閉じたトラバサミのイラスト")] Sprite _closedTorabasami;
    SpriteRenderer _torabasamiSprite;
    [SerializeField] float _stopTime;
    PlayerController _controller;
    bool _isTrap;
    float _timer;
    void Start()
    {
        _torabasamiSprite = GetComponent<SpriteRenderer>();
        _controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_isTrap)
        {
            _torabasamiSprite.sprite = _closedTorabasami;
            _controller.transform.position = new Vector2(transform.position.x, _controller.transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>();
            StartCoroutine(TrapTime(_stopTime));
            _controller.FluctuationLife(-1);
            _controller.StopAction(_stopTime);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySE("damage_enemy");
            Destroy(collision.gameObject);
            if (_meat)
            {
                Instantiate(_meat);
            }
        }

        IEnumerator TrapTime(float time)
        {
            _isTrap = true;
            yield return new WaitForSeconds(time);
            _isTrap = false;
            Destroy(this.gameObject);
        }
    }
}

