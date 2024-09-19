using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーキャラのHP。これが0になるとゲームオーバー</summary>
    [Header("体力の最大値")]
    [SerializeField][Tooltip("プレイヤーの体力の最大値")] int _maxHp;
    int _currentHp;
    /// <summary>アイテムをまっすぐ投げる強さ</summary>
    [Header("まっすぐ投げる強さ")]
    [SerializeField][Tooltip("まっすぐ投げる強さ")] float _throwStraightPower = 5;
    /// <summary>アイテムを放物的に投げる強さ</summary>
    [Header("放物的に投げる強さ")]
    [SerializeField][Tooltip("放物的に投げる強さ")] float _throwParabolaPower = 5;
    /// <summary>アイテムを放物的に投げる方向</summary>
    [Header("放物的に投げる方向")]
    [SerializeField][Tooltip("放物的に投げる方向")] Vector2 _throwParabolaDirection = new Vector2(1, 1);
    /// <summary>プレイヤーキャラクターの移動速度を決める値。数値が高いほど最大速度が高くなる</summary>
    [Header("移動速度の最大値")]
    [SerializeField][Tooltip("プレイヤーの速度の最大値")] float _speed;
    /// <summary>プレイヤーキャラクターの移動速度の加速度を決める値。数値が高いほど最大速度までの加速時間が短い</summary>
    [Header("移動速度の加速度")]
    [SerializeField][Tooltip("プレイヤーの移動速度の加速度")] float _movePower;
    /// <summary>プレイヤーキャラクターのJump時に上方向に掛ける力。</summary>
    [Header("ジャンプ力")]
    [SerializeField][Tooltip("プレイヤーのジャンプ力")] float _jumpPower;
    /// <summary>エネミーからダメージを受けた際、一定時間はダメージを受けないようにする時間。整数値で入力する。1=1秒</summary>
    [Header("無敵時間")]
    [SerializeField][Tooltip("プレイヤーの無敵時間")] int _damageCool;
    /// <summary>接地判定</summary>
  　bool _isJump;
    /// <summary>敵を踏んだ判定</summary>
    bool _isStompEnemy;
    /// <summary>持っているアイテムのリスト</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    /// <summary>持っている武器</summary>
    (bool _isWoodStick, bool _isAxe, int _rocks) _weapons;
    PlayerWeaponStatus _weaponStatus = PlayerWeaponStatus.Normal;
    Rigidbody2D _rb;
    float _jumpTimer = 0;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHp = _maxHp;
    }

    void Update()
    {
        Move();
        Jump();
        UseItem();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ぶつかった");
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isJump = false;
            _jumpTimer = 0;
            Debug.Log("着地！");
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _isStompEnemy = true;
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            Debug.Log("踏んだ");
        }
    }
    enum PlayerWeaponStatus
    {
        WoodStick,
        Axe,
        Rock,
        Normal,
    }
    private void Move()
    {
        //移動処理
        var x = Input.GetAxisRaw("Horizontal");
        if (_isJump)
        {
            x /= 5;
        }
        else
        {
            if(x < 0)
            {
                transform.localScale = new Vector2(-1,1);
            }
            else if(x > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
        }
        _rb.AddForce(new Vector2(x, 0) * _movePower, ForceMode2D.Force);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isJump)
            {
                Debug.Log("ジャンプした");
                _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
                _isJump = true;
            }
            else
            {
                Debug.Log("ジャンプできなかった");
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (_isStompEnemy)
            {
                _isStompEnemy = false;
                _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            }
        }
        else if (_isStompEnemy)
        {
            _rb.AddForce(new Vector2(0, _jumpPower / 1.5f), ForceMode2D.Impulse);
            _isStompEnemy = false;
        }
        else if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.99f);
        }
        //Debug.Log(_isJump);
    }
    public void GetItem(ItemBase item)
    {
        _itemList.Add(item);
        if(item.TryGetComponent<HealItem>(out HealItem a))
        {
            Debug.Log(a);
        }
    }
    void UseItem()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(_itemList.Count > 0)
            {
                ItemBase item = _itemList[0];
                _itemList.RemoveAt(0);
                item.transform.position = transform.position;
                //投げるアイテムにRigidbodyがついてなかったらつける
                if(!item.TryGetComponent(out Rigidbody2D rb))
                {
                    rb = item.AddComponent<Rigidbody2D>();
                }
                //まっすぐ投げる
                if(item.Throw == ItemBase.ThrowType.Straight)
                {
                    _throwStraightPower *= transform.localScale.x;
                    rb.AddForce(new Vector2(_throwStraightPower, 0), ForceMode2D.Impulse);
                }
                //放物的に投げる
                else
                {
                    _throwParabolaPower *= transform.localScale.x;
                    rb.AddForce(_throwParabolaDirection.normalized * _throwParabolaPower,ForceMode2D.Impulse);
                }
                //Destroy(item.gameObject);
            }
        }
    }
    void ChangeWeapon()
    {

    }

    /// <summary>
    /// プレイヤーの体力を引数に渡した数値分増減させます。
    /// </summary>
    /// <param name="value"></param>
    public void FluctuationLife(int value)
    {
        _currentHp += value;
        if (_currentHp > _maxHp)
        {
            _currentHp = _maxHp;
        }
        Debug.Log($"Playerの体力:{_currentHp}");
    }
}
