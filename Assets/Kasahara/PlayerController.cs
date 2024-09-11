using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] int _maxHP;
    [SerializeField] int _attack;
    [SerializeField] float _speed;
    [SerializeField] float _movePower;
    [SerializeField] float _jumpPower;
    [SerializeField] float _damageCool;
    Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        _rb.AddForce(new Vector2(x, 0), ForceMode2D.Force);
    }
}
