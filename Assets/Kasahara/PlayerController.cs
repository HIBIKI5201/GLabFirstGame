using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーキャラのHP。これが0になるとゲームオーバー</summary>
    [Header("体力の最大値")]
    [SerializeField][Tooltip("プレイヤーの体力の最大値")] int _maxHp;
    int _currentHp;
    /// <summary>プレイヤーキャラクターの移動速度を決める値。数値が高いほど最大速度が高くなる</summary>
    [Header("移動速度の最大値")]
    [SerializeField][Tooltip("プレイヤーの速度の最大値")]　public float _speed;
    /// <summary>プレイヤーキャラクターの移動速度の加速度を決める値。数値が高いほど最大速度までの加速時間が短い</summary>
    [Header("移動速度の加速度")]
    [SerializeField][Tooltip("プレイヤーの移動速度の加速度")] public float _movePower;
    /// <summary>プレイヤーキャラクターのJump時に上方向に掛ける力。</summary>
    [Header("ジャンプ力")]
    [SerializeField][Tooltip("プレイヤーのジャンプ力")] float _jumpPower;
    /// <summary>エネミーからダメージを受けた際、一定時間はダメージを受けないようにする時間。整数値で入力する。1=1秒</summary>
    [Header("無敵時間")]
    [SerializeField][Tooltip("プレイヤーの無敵時間")] int _damageCool;
    /// <summary>アイテムをまっすぐ投げる強さ</summary>
    [Header("まっすぐ投げる強さ")]
    [SerializeField][Tooltip("まっすぐ投げる強さ")] float _throwStraightPower = 5;
    /// <summary>アイテムを放物的に投げる強さ</summary>
    [Header("放物的に投げる強さ")]
    [SerializeField][Tooltip("放物的に投げる強さ")] float _throwParabolaPower = 5;
    /// <summary>アイテムを放物的に投げる方向</summary>
    [Header("放物的に投げる方向")]
    [SerializeField][Tooltip("放物的に投げる方向")] Vector2 _throwParabolaDirection = new Vector2(1, 1);
    [Header("アイテムを投げる位置")]
    [SerializeField][Tooltip("アイテムを投げる位置")] Vector2 _throwPos;
    /// <summary>アイテムの数</summary>
    [SerializeField, Header("持てる石の最大値")] int _maxRockCount;
    int _currentRockCount;
    [SerializeField, Header("持てる空き瓶の最大値")] int _maxBottleCount;
    int _currentBottleCount;
    [SerializeField, Header("持てる肉の最大値")] int _maxMeatCount;
    int _currentMeatCount;
    /// <summary>持っているアイテムのリスト</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    List<Rock> _rockList = new List<Rock>();
    List<Bottle> _bottleList = new List<Bottle>();
    List<Meat> _meatList = new List<Meat>();
    /// <summary>接地判定</summary>
    bool _isJump;
    /// <summary>敵を踏んだ判定</summary>
    bool _isStompEnemy;
    /// <summary>無敵時間中かどうか</summary>
    bool _isInvincible;
    PlayerStatus _playerStatus = PlayerStatus.Normal;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    float _jumpTimer = 0;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHp = _maxHp;
    }

    void Update()
    {
        Move();
        Jump();
        ChangeItem();
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
    enum PlayerStatus
    {
        Rock,
        Bottle,
        Meat,
        Normal,
        Damage,
        Death
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
            if (x < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (x > 0)
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
        //_itemList.Add(item);
        if (item.TryGetComponent(out Rock a))
        {
            if (_currentRockCount <= _maxRockCount)
            {
                _currentRockCount++;
                _rockList.Add(a);
            }
        }
        else if (item.TryGetComponent(out Bottle b))
        {
            if (_currentBottleCount <= _maxBottleCount)
            {
                _currentBottleCount++;
                _bottleList.Add(b);
            }
        }
        else if (item.TryGetComponent(out Meat c))
        {
            if (_currentMeatCount <= _maxMeatCount)
            {
                _currentMeatCount++;
                _meatList.Add(c);
            }
        }
    }
    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ItemBase item = null;//= _itemList[0];
            switch (_playerStatus)
            {
                case PlayerStatus.Rock:
                    item = _rockList[0];
                    _rockList.RemoveAt(0);
                    _currentRockCount--;
                    break;
                case PlayerStatus.Bottle:
                    item = _bottleList[0];
                    _bottleList.RemoveAt(0);
                    _currentBottleCount--;
                    break;
                case PlayerStatus.Meat:
                    item = _meatList[0];
                    _meatList.RemoveAt(0);
                    _currentMeatCount--;
                    break;
                case PlayerStatus.Normal:
                    return;
            }
            item.transform.position = transform.position + (Vector3)_throwPos;
            Debug.Log(item + "を投げた");
            //投げるアイテムにRigidbodyがついてなかったらつける
            if (!item.TryGetComponent(out Rigidbody2D rb))
            {
                rb = item.AddComponent<Rigidbody2D>();
            }
            item.gameObject.GetComponent<Collider2D>().isTrigger = false;
            item.Throwing();
            //まっすぐ投げる
            if (item.Throw == ItemBase.ThrowType.Straight)
            {
                _throwStraightPower *= transform.localScale.x;
                rb.gravityScale = 0;
                rb.AddForce(new Vector2(_throwStraightPower, 0), ForceMode2D.Impulse);
            }
            //放物的に投げる
            else
            {
                _throwParabolaPower *= transform.localScale.x;
                rb.AddForce(_throwParabolaDirection.normalized * _throwParabolaPower, ForceMode2D.Impulse);
            }
            //Destroy(item.gameObject);
        }
    }
    void ChangeItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_currentRockCount > 0)
            {
                _playerStatus = PlayerStatus.Rock;
                Debug.Log("石を使う");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_currentBottleCount > 0)
            {
                _playerStatus = PlayerStatus.Bottle;
                Debug.Log("瓶を使う");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_currentMeatCount > 0)
            {
                _playerStatus = PlayerStatus.Meat;
                Debug.Log("肉を使う");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _playerStatus = PlayerStatus.Normal;
            Debug.Log("アイテムを持たない");
        }
    }

    /// <summary>
    /// プレイヤーの体力を引数に渡した数値分増減させます。
    /// </summary>
    /// <param name="value"></param>
    public void FluctuationLife(int value)
    {
        if (value < 0)
        {
            if (!_isInvincible)
            {
                _currentHp += value;
            }
            if (_currentHp <= 0)
            {
                _playerStatus = PlayerStatus.Death;
            }
            else
            {
                //ダメージを受けた時の処理、一旦保留
                //DOTween.To(() => new Color(), s => )
            }
        }
        else
        {
            _currentHp += value;
        }
        if (_currentHp > _maxHp)
        {
            _currentHp = _maxHp;
        }
        Debug.Log($"Playerの体力:{_currentHp}");
    }
    /// <summary>
    /// プレイヤーの速度を調整する
    /// </summary>
    /// <param name="multi">倍率</param>
    /// <param name="slowtime">継続時間</param>
    public void Slow(float multi, float slowtime)
    {
        StartCoroutine(Slowing(multi, slowtime));
    }
    IEnumerator Slowing(float multi, float slowtime)
    {
        float defaultMovePower = _movePower;
        float defaultMaxSpeed = _speed;
        _movePower *= multi;
        _speed *= multi;
        yield return new WaitForSeconds(slowtime);
        _movePower = defaultMovePower;
        _speed = defaultMaxSpeed;
    }
}
