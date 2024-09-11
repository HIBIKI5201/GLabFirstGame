using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーキャラのHP。これが0になるとゲームオーバー</summary>
    [Header("体力の最大値")]
    [SerializeField][Tooltip("プレイヤーの体力の最大値")] int _maxHP;
    int _currentHP;
    /// <summary>プレイヤーキャラが攻撃を行った時に、エネミーに与えるダメージ。</summary>
    [Header("攻撃力")]
    [SerializeField][Tooltip("プレイヤーの攻撃力")] int _attack;
    /// <summary>プレイヤーキャラクターの移動速度を決める値。数値が高いほど最大速度が高くなる</summary>
    [Header("移動速度の最大値")]
    [SerializeField][Tooltip("プレイヤーの速度の最大値")] float _speed;
    /// <summary>プレイヤーキャラクターの移動速度の加速度を決める値。数値が高いほど最大速度までの加速時間が短い</summary>
    [Header("移動速度の加速度")]
    [SerializeField][Tooltip("プレイヤーの移動速度の加速度")] float _movePower;
    /// <summary>プレイヤーキャラクターのJump時に上方向に掛ける力。数値が高いほど、高くジャンプが行える</summary>
    [Header("ジャンプ力")]
    [SerializeField][Tooltip("プレイヤーのジャンプ力")] float _jumpPower;
    /// <summary>エネミーからダメージを受けた際、一定時間はダメージを受けないようにする時間。整数値で入力する。1=1秒</summary>
    [Header("無敵時間")]
    [SerializeField][Tooltip("プレイヤーの無敵時間")] int _damageCool;
    /// <summary>接地判定</summary>
   　bool _isGround;
    /// <summary>持っているアイテムのリスト</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        Move(new Vector2(x, 0));
        Jump();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGround = true;
        }
    }
    private void Move(Vector2 horiMove)
    {
        //移動処理
        var x = Input.GetAxisRaw("Horizontal");
        _rb.AddForce(horiMove * _movePower, ForceMode2D.Force);
        if (_rb.velocity.x > _speed)
        {
            _rb.velocity = new Vector2(_speed, _rb.velocity.y);
        }
        else if (_rb.velocity.x < -_speed)
        {
            _rb.velocity = new Vector2(-_speed, _rb.velocity.y);
        }
    }
    private void Jump()
    {
        if (_isGround && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            _isGround = false;
        }
    }
    public void GetItem(ItemBase item)
    {
        _itemList.Add(item);
    }
    /// <summary>
    /// プレイヤーの体力を引数に渡した数値分増減させます。
    /// </summary>
    /// <param name="value"></param>
    public void FluctuationLife(int value)
    {
        _currentHP += value;
        if(_currentHP > _maxHP)
        {
            _currentHP = _maxHP;
        }
    }
}
