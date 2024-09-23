using UnityEngine;
using System.Collections;



public class Torabasami : MonoBehaviour
{
    [Header("ê∂ê¨Ç∑ÇÈì˜"),SerializeField] GameObject _meat;
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
        if (_isTrap) _player.transform.position = new Vector2(transform.position.x,_player.transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(TrapTime(_stopTime));
            _controller.FluctuationLife(-1);
            _controller.StopAction(_stopTime);
        }

        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            if (_meat)Instantiate(_meat);
        }
    }

    IEnumerator TrapTime(float time)
    {
        _isTrap = true;
        yield return new WaitForSeconds(time);
        _isTrap = false;
    }


}
