using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEditor.Progress;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーキャラのHP。これが0になるとゲームオーバー</summary>
    [Header("体力の最大値")]
    [SerializeField][Tooltip("プレイヤーの体力の最大値")] int _maxHp;
    int _currentHp;
    /// <summary>プレイヤーキャラクターの移動速度を決める値。数値が高いほど最大速度が高くなる</summary>
    [Header("移動速度の最大値")]
    [SerializeField][Tooltip("プレイヤーの速度の最大値")] public float _speed;
    /// <summary>プレイヤーキャラクターの移動速度の加速度を決める値。数値が高いほど最大速度までの加速時間が短い</summary>
    [Header("移動速度の加速度")]
    [SerializeField][Tooltip("プレイヤーの移動速度の加速度")] public float _movePower;
    /// <summary>プレイヤーキャラクターのJump時に上方向に掛ける力。</summary>
    [Header("ジャンプ力")]
    [SerializeField][Tooltip("プレイヤーのジャンプ力")] float _jumpPower;
    /// <summary>エネミーからダメージを受けた際、一定時間はダメージを受けないようにする時間。整数値で入力する。1=1秒</summary>
    [Header("無敵時間")]
    [SerializeField][Tooltip("プレイヤーの無敵時間")] int _damageCool;
    [SerializeField, Header("<アイテムを投げる設定>")] Throwsetting _throwsetting;
    [SerializeField, Header("<アイテムの設定>")] ItemSetting _itemSetting;
    [System.Serializable]
    struct Throwsetting
    {
        [Tooltip("まっすぐ投げる強さ")] public float _throwStraightPower;
        [Tooltip("放物的に投げる強さ")] public float _maxThrowParabolaPower;
        [Tooltip("放物的に投げる方向")] public Vector2 _throwParabolaDirection;
        [Tooltip("アイテムを投げる位置")] public Vector2 _throwPos;
        [Tooltip("弾道予測線")] public LineRenderer _line;
        public int _simulateFrame;
    }
    /// <summary>アイテムの設定</summary>
    [System.Serializable]
    struct ItemSetting
    {
        [Tooltip("持てる石の最大値")] public int _maxRockCount;
        [Tooltip("持てる空き瓶の最大値")] public int _maxBottleCount;
        [Tooltip("持てる肉の最大値")] public int _maxMeatCount;
        [Tooltip("石のUI")] public GameObject _rockUi;
        [Tooltip("空き瓶のUI")] public GameObject _bottleUi;
        [Tooltip("肉のUI")] public GameObject _meatUi;
        [Tooltip("石の個数を表示するテキスト")] public Text _rockCountText;
        [Tooltip("空き瓶の個数を表示するテキスト")] public Text _bottleCountText;
        [Tooltip("肉の個数を表示するテキスト")] public Text _meatCountText;
    }
    /// <summary>持っているアイテムのリスト</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    /// <summary>接地判定</summary>
    bool _isJump;
    /// <summary>敵を踏んだ判定</summary>
    bool _isStompEnemy;
    /// <summary>無敵時間中かどうか</summary>
    bool _isInvincible;
    bool _canAction = true;
    PlayerStatus _playerStatus = PlayerStatus.Normal;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Scene m_simulationScene;
    PhysicsScene2D m_physicsScene;
    float _throwParabolaPower = 0;
    GameObject[] _itemPos = new GameObject[3];
    void Start()
    {
        CreatePhysicsScene();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHp = _maxHp;
        _itemPos = new GameObject[] { _itemSetting._rockUi, _itemSetting._bottleUi, _itemSetting._meatUi };
    }

    void Update()
    {
        if (_canAction)
        {
            Move();
            Jump();
            ChangeItem();
            UseItem();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ぶつかった");
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isJump = false;
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
    /// <summary>
    /// プレイヤーがアイテムを入手したときに呼ぶメソッド
    /// </summary>
    /// <param name="item"></param>
    public void GetItem(ItemBase item)
    {
        if (item as Rock)
        {
            if (_itemList.Where(i => i as Rock).ToList().Count < _itemSetting._maxRockCount)
            {
                _itemList.Add(item);
                _itemSetting._rockCountText.text = _itemList.Where(i => i as Rock).Count().ToString();
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Bottle)
        {
            if (_itemList.Where(i => i as Bottle).ToList().Count < _itemSetting._maxBottleCount)
            {
                _itemList.Add(item);
                _itemSetting._bottleCountText.text = _itemList.Where(i => i as Bottle).Count().ToString();
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Meat)
        {
            if (_itemList.Where(i => i as Meat).ToList().Count < _itemSetting._maxMeatCount)
            {
                _itemList.Add(item);
                _itemSetting._meatCountText.text = _itemList.Where(i => i as Meat).Count().ToString();
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
    }
    /// <summary>
    /// Enterが押された時アイテムを投げるためのコルーチンをスタートします
    /// </summary>
    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(ThrowItem());
        }
    }
    /// <summary>
    /// プレイヤーが持っているアイテムに応じて_itemListからアイテムを取ってきます。
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Item(out ItemBase item)
    {
        switch (_playerStatus)
        {
            case PlayerStatus.Rock:
                item = _itemList.Where(i => i as Rock).ToList().First();
                return true;
            case PlayerStatus.Bottle:
                item = _itemList.Where(i => i as Bottle).ToList().First();
                return true;
            case PlayerStatus.Meat:
                item = _itemList.Where(i => i as Meat).ToList().First();
                return true;
            default:
                item = null;
                return false;
        }
    }
    void ChangeItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_itemList.Any(i => i as Rock))
            {
                _playerStatus = PlayerStatus.Rock;

                Debug.Log("石を使う");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_itemList.Any(i => i as Bottle))
            {
                _playerStatus = PlayerStatus.Bottle;
                Debug.Log("瓶を使う");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_itemList.Any(i => i as Meat))
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
        void RotateItem()
        {

            Vector2 itemPos = _itemPos[(int)_playerStatus].transform.position;
            for (int i = 0; i < _itemPos.Length - 1; i++)
            {
                _itemPos[(int)_playerStatus % _itemPos.Length].transform.position = _itemPos[(int)_playerStatus + 1 % _itemPos.Length].transform.position;
            }
        }
    }
    void CreatePhysicsScene()
    {
        m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        m_physicsScene = m_simulationScene.GetPhysicsScene2D();
    }
    void ThrowLineSimulate(GameObject _ballPrefab, Vector2 _pos, Vector2 _velocity)
    {
        var ghost = Instantiate(_ballPrefab, _pos, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);
        ghost.GetComponent<Rigidbody2D>().AddForce(_velocity, ForceMode2D.Impulse);

        _throwsetting._line.positionCount = _throwsetting._simulateFrame;

        for (int i = 0; i < _throwsetting._simulateFrame; i++)
        {
            m_physicsScene.Simulate(Time.fixedDeltaTime);
            _throwsetting._line.SetPosition(i, ghost.transform.position);
        }
        Destroy(ghost.gameObject);
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
    /// <summary>
    /// プレイヤーが行動できなくなる処理
    /// </summary>
    /// <param name="time"></param>
    public void StopAction(float time)
    {
        StartCoroutine(StoppingAction(time));
    }
    IEnumerator StoppingAction(float time)
    {
        _canAction = false;
        yield return new WaitForSeconds(time);
        _canAction = true;
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
    IEnumerator ThrowItem()
    {
        if (!Item(out ItemBase item)) yield break;
        //投げるアイテムにRigidbodyがついてなかったらつける
        if (!item.TryGetComponent(out Rigidbody2D rb))
        {
            rb = item.AddComponent<Rigidbody2D>();
        }
        item.gameObject.GetComponent<Collider2D>().isTrigger = false;
        //まっすぐ投げる
        if (item.Throw == ItemBase.ThrowType.Straight)
        {
            //アイテムをプレイヤーの位置に持ってくる
            item.transform.position = transform.position + (Vector3)_throwsetting._throwPos;
            rb.gravityScale = 0;
            _throwsetting._throwStraightPower *= transform.localScale.x;
            rb.AddForce(new Vector2(_throwsetting._throwStraightPower, 0), ForceMode2D.Impulse);
        }
        //放物的に投げる
        else
        {
            while (Input.GetKey(KeyCode.Return))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    yield break;
                }
                if (_throwParabolaPower < _throwsetting._maxThrowParabolaPower)
                {
                    _throwParabolaPower += 0.1f;
                }
                ThrowLineSimulate(item.gameObject, transform.position, _throwsetting._throwParabolaDirection.normalized * _throwParabolaPower);
                yield return new WaitForEndOfFrame();
            }
            _throwsetting._line.positionCount = 0;
            //アイテムをプレイヤーの位置に持ってくる
            item.transform.position = transform.position + (Vector3)_throwsetting._throwPos;
            rb.velocity = Vector3.zero;
            _throwParabolaPower *= transform.localScale.x;
            rb.AddForce(_throwsetting._throwParabolaDirection.normalized * _throwParabolaPower, ForceMode2D.Impulse);
            _throwParabolaPower = 0;
        }
        item.Throwing();
        if (item as Rock)
        {
            _itemList.Remove((Rock)item);
            _itemSetting._rockCountText.text = _itemList.Where(i => i as Rock).Count().ToString();
            if (_itemSetting._rockCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
            }
        }
        else if (item as Bottle)
        {
            _itemList.Remove((Bottle)item);
            _itemSetting._bottleCountText.text = _itemList.Where(i => i as Bottle).Count().ToString();
            if (_itemSetting._bottleCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
            }
        }
        else
        {
            _itemList.Remove((Meat)item);
            _itemSetting._meatCountText.text = _itemList.Where(i => i as Meat).Count().ToString();
            if (_itemSetting._meatCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
            }
        }
    }
}