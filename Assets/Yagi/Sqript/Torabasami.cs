using UnityEngine;
using System.Collections;



public class Torabasami : MonoBehaviour
{
    [SerializeField, Header("ê∂ê¨Ç∑ÇÈì˜")] GameObject _meat;
    [SerializeField] GameObject _player;
    [SerializeField] float _stopTime;
    PlayerController _controller;
    bool _isTrap;
    float _timer;
    void Start()
    {
        _controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_isTrap)
        {
            _player.transform.position = new Vector2(transform.position.x, _player.transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(TrapTime(_stopTime));
            _controller.FluctuationLife(-1);
            _controller.StopAction(_stopTime);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
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
        }
    }
}

