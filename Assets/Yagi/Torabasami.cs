using UnityEngine;


public class Torabasami : MonoBehaviour
{
    [Header("ê∂ê¨Ç∑ÇÈì˜"),SerializeField] GameObject _meat;
    [SerializeField] GameObject _player;
    PlayerController _controller;
    float _timer = 0;
    public bool _isTrap;
    void Start()
    {
        _controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player.transform.position = transform.position;
            _controller.FluctuationLife(-1);
            _timer = 0;
            if (_timer < 1) _isTrap = true;
            else _isTrap = false;           
        }

        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            if (_meat)Instantiate(_meat);
        }
    }
}
