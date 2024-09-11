using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーキャラのHP。これが0になるとゲームオーバー</summary>
    [SerializeField] int _maxHP;
    /// <summary>プレイヤーキャラが攻撃を行った時に、エネミーに与えるダメージ。</summary>
    [SerializeField] int _attack;
    /// <summary>プレイヤーキャラクターの移動速度を決める値。数値が高いほど最大速度が高くなる</summary>
    [SerializeField] float _speed;
    /// <summary>プレイヤーキャラクターの移動速度の加速度を決める値。数値が高いほど最大速度までの加速時間が短い</summary>
    [SerializeField] float _movePower;
    /// <summary>プレイヤーキャラクターのJump時に上方向に掛ける力。数値が高いほど、高くジャンプが行える</summary>
    [SerializeField] float _jumpPower;
    /// <summary>エネミーからダメージを受けた際、一定時間はダメージを受けないようにする時間。整数値で入力する。1=1秒</summary>
    [SerializeField] int _damageCool;
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
        _rb.AddForce(new Vector2(x * _movePower, 0), ForceMode2D.Force);
        if (_rb.velocity.x > _speed)
        {
            _rb.velocity = new Vector2(_speed, _rb.velocity.y);
        }
        else if (_rb.velocity.x < - _speed)
        {
            _rb.velocity = new Vector2(- _speed, _rb.velocity.y);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
        }
    }
}
