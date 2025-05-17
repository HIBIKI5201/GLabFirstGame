using System;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// 敵のクラス
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyType _beast; // 敵の種類
    [SerializeField] Animator _anim;

    [Header("基本設定")] 
    [SerializeField] int _maxHp; // 最大HP
    [SerializeField] int _currentHp; // 現在のHP
    [SerializeField] int _attack; // 攻撃力
    [SerializeField] float _jumpPower; // ジャンプ力

    [Header("移動スピード")] 
    public float _speed; // 通常時の移動スピード
    public float _chaseSpeed; // プレイヤーを発見したときの移動スピード
    public float _currentSpeed; // 現在の移動スピード

    [SerializeField] EnemyStateType _state; // 状態

    public EnemyStateType State
    {
        get => _state;
        set
        {
            if (value == _state) return; // ステートが変わらなければ処理を行わない
            _state = value;
        }
    }

    [SerializeField] bool _jumpOver; // 段差を乗り越えられるか
    [SerializeField] bool _goDown;
    [SerializeField] bool _canChase;
    [SerializeField] bool _canDamage;
    [SerializeField] GroundedRay _ground;
    [SerializeField] Direction _dir; // どちらの方向に動くか
    [SerializeField] bool _alwaysDebug;

    Rigidbody2D _rb;
    Transform _myTra;
    Transform _playerTra;
    Transform _modelT;
    Vector2 _modelScale;
    SpriteRenderer[] _spriteRenderers;
    BoxCollider2D _boxCollider;
    PlayerController _player;
    
    private EnemyDamageHandler _damageHandler;
    
    GameObject _stunAnimeObj; // スタンエフェクトのオブジェクト
    Vector2 _bottlePosi;
    
    ContactPoint2D[] V;

    // コルーチン
    Coroutine _reactionCoro = null; // アイテム効果
    Coroutine _coroutine = null;

    // 肉アイテム関連
    GameObject _meatIcon;
    Vector2 _meatPosi;
    float _meatTimer;
    float _meatTime;
    bool _meatEat; // 肉アイテムの効果中

    bool _canMoveSE;
    bool _canReset;

    float _attackedTimer; // 攻撃クールタイムを管理するための変数

    #region 初期化処理

    private void Awake()
    {
        _canReset = true;
        ResetStatus();
        CacheComponents();
        
        _damageHandler = new EnemyDamageHandler(_currentHp, _canDamage, _spriteRenderers, this);
    }

    private void OnEnable()
    {
        CacheComponents();

        if (_canReset)
        {
            ResetStatus();
            MatchGround();
            _canReset = false;
        }

        if (_canMoveSE)
        {
            AudioManager.Instance.PlaySE(_beast switch
            {
                EnemyType.StrayDog => "strayDog",
                EnemyType.Wolf_Normal or EnemyType.Wolf_Gray => "wolf",
                EnemyType.Bear => "bare",
                _ => "strayDog"
            });
            _canMoveSE = false;
        }
    }

    /// <summary>
    /// ステータスの初期化
    /// </summary>
    private void ResetStatus()
    {
        PhysicsMaterial2D physicsMaterial2D = new()
        {
            friction = 0,
            bounciness = 0,
        };
        GetComponent<Collider2D>().sharedMaterial = physicsMaterial2D;

        _currentHp = _maxHp;
        _currentSpeed = _speed;
        _state = EnemyStateType.Normal;
        _canDamage = true;

        StartCoroutine(WaitAudio());

        IEnumerator WaitAudio()
        {
            _canMoveSE = false;
            yield return new WaitForSeconds(0.2f); // 開始後0.2秒は歩行時のSEを再生しない
            _canMoveSE = true;
        }
    }

    /// <summary>
    /// コンポーネントの参照を取得する
    /// </summary>
    private void CacheComponents()
    {
        // _stunAnimeObj = GetComponentInChildren<StunAnime>().gameObject; // TODO
        _myTra = transform;

        if (TryGetComponent(out _rb))
        {
            _rb.isKinematic = false;
        }

        _meatIcon = transform.GetChild(1).gameObject;

        if (_modelT == null)
        {
            _modelT = GetComponentInChildren<Animator>().transform;
        }

        _modelScale = _modelT.localScale;
        _modelScale.x = MathF.Abs(_modelScale.x);

        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (_playerTra == null)
        {
            _playerTra = FindAnyObjectByType<PlayerController>().transform;
        }

        if (_boxCollider == null)
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }
    }

    /// <summary>
    /// 地面の高さに自身の位置を合わせる
    /// </summary>
    private void MatchGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(_myTra.position, _boxCollider.size, 0, Vector2.down, 1000, _ground._mask);
        if (hit)
        {
            Vector2 pos = _myTra.position;
            pos.y = hit.point.y + _boxCollider.size.y / 2;
            _myTra.position = pos;
        }
    }

    #endregion

    void Update()
    {
        _rb.isKinematic = false;

        // 移動
        var vec = _modelScale;
        vec.x *= _dir switch { Direction.Right => -1, Direction.Left => 1, _ => Mathf.Sign(_modelT.localScale.x) };
        _modelT.localScale = vec;

        // _stunAnimeObj.SetActive(State == EnemyStateType.Faint); // TODO: 素材を確認

        // 状態に応じた処理
        switch (State)
        {
            case EnemyStateType.Faint:
                Vector2 velo = _rb.velocity;
                velo.x = 0;
                _rb.velocity = velo;
                _anim.SetBool("Dizzy", true);
                break;

            case EnemyStateType.Bite:
                UpdateMeat();
                break;

            case EnemyStateType.Escape:
                UpdateBottle();
                break;

            case EnemyStateType.Chase:
                UpdateChase();
                break;

            case EnemyStateType.Normal:
            default:
                UpdateReturn();
                UpdateVelocity();
                Search();
                _anim.SetBool("Gallop", false);
                _anim.SetBool("Dizzy", false);
                break;
        }

        if (GameManager.instance.StateType == GameStateType.GameOver ||
            GameManager.instance.StateType == GameStateType.StageClear)
        {
            // ゲームオーバー、もしくはゲームクリア時には敵を非表示にする
            enabled = false;
            gameObject.SetActive(false);
        }
    }

    private void UpdateReturn()
    {
        if (IsFrontGrounded(out bool isRightDir))
        {
            // もし目の前が崖なら移動方向を反転させる
            _dir = isRightDir ? Direction.Right : Direction.Left;
        }

        if (IsSideTouch(out bool playerHit))
        {
            // もし壁などに触れたら移動方向を反転させる
            _dir = _dir == Direction.Right ? Direction.Left : Direction.Right;
            if (playerHit)
            {
                AttackToPlayer(); // プレイヤーに当たっていたら攻撃を行う
            }
        }
    }

    /// <summary>
    /// 空き瓶の効果の処理
    /// </summary>
    private void UpdateBottle()
    {
        float x = _bottlePosi.x - _myTra.position.x;
        _dir = x >= 0 ? Direction.Left : Direction.Right; // プレイヤーとは逆の方向に逃げ出す
        _anim.SetBool("Gallop", true); // 逃げ出すアニメーションを再生する

        UpdateVelocity();
    }

    /// <summary>
    /// 肉の効果の処理
    /// </summary>
    private void UpdateMeat()
    {
        _meatIcon.SetActive(true);
        float x = _meatPosi.x - _myTra.position.x;
        _dir = x <= 0 ? Direction.Left : Direction.Right; // 肉がある方へ方向をセットする

        if (Mathf.Abs(x) <= 0.2f)
        {
            _dir = Direction.None;
            if (!_meatEat)
            {
                _meatTimer = Time.time;
            }
            _meatEat = true;
        }

        if (Time.time >= _meatTimer + _meatTime)
        {
            State = EnemyStateType.Normal; // 状態を戻す
            _meatIcon.SetActive(false);
            _dir = Mathf.Sign(_modelT.localScale.x) switch
            {
                1 => Direction.Left, -1 => Direction.Right, _ => Direction.None
            };
        }

        if (IsJump() && IsGrounded() && _jumpOver)
        {
            VelocityJump();
        }

        UpdateVelocity();
    }

    /// <summary>
    /// プレイヤー追跡状態の処理
    /// </summary>
    private void UpdateChase()
    {
        float x = _playerTra.position.x - _myTra.position.x;
        float y = _playerTra.position.y - _myTra.position.y;
        _dir = x <= 0 ? Direction.Left : Direction.Right;
        if (Mathf.Abs(x) <= 0.1f)
        {
            _dir = Direction.None;
        }

        if (!_goDown)
        {
            if (IsFrontGrounded(out bool right))
            {
                switch (_dir)
                {
                    case Direction.Left:
                        if (right)
                            _dir = Direction.None;
                        break;
                    case Direction.Right:
                        if (!right)
                            _dir = Direction.None;
                        break;
                }
            }
        }

        if (IsJump() && IsGrounded() && _jumpOver)
        {
            VelocityJump();
        }

        if (Time.time >= _attackedTimer + 0.1f)
        {
            if (IsSideTouch(out bool playerHit))
            {
                if (playerHit)
                {
                    AttackToPlayer();
                }
            }
        }

        UpdateVelocity();
        Search();
    }

    private void UpdateVelocity()
    {
        Vector2 velo = _rb.velocity;
        velo.x = _currentSpeed * _dir switch { Direction.Right => 1, Direction.Left => -1, _ => 0 };
        _rb.velocity = velo;
    }

    private void VelocityJump()
    {
        Vector2 velo = _rb.velocity;
        velo.y = _jumpPower;
        _rb.velocity = velo;
    }

    /// <summary>
    /// プレイヤーを探す処理
    /// </summary>
    private void Search()
    {
        if (!_canChase) return;

        RaycastHit2D hit = Physics2D.Linecast(_myTra.position, _playerTra.position, _ground._mask);
        State = hit ? EnemyStateType.Normal : EnemyStateType.Chase;
    }

    private void AttackToPlayer()
    {
        // 攻撃のクールタイム中であれば以降の処理は行わない
        if (Time.time <= _attackedTimer + 0.1f) return;
        
        if (_player == null)
        {
            _player = FindAnyObjectByType<PlayerController>();
        }
        
        _attackedTimer = Time.time;
        _player.FluctuationLife(-_attack);
    }

    private bool IsFrontGrounded(out bool isRightDir)
    {
        Vector2 rRayPos = _myTra.position + (Vector3)_ground._rightRayPos;
        Vector2 lRayPos = _myTra.position + (Vector3)_ground._leftRayPos;
        bool isHitR = Physics2D.Raycast(rRayPos, Vector2.down, _ground._rayLong, _ground._mask);
        bool isHitL = Physics2D.Raycast(lRayPos, Vector2.down, _ground._rayLong, _ground._mask);
        isRightDir = isHitR;
        return isHitR ^ isHitL;
    }

    /// <summary>
    /// 接地判定
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(_myTra.position, _boxCollider.size - new Vector2(0.1f, 0.1f),
            0, Vector2.down, 0.2f, _ground._mask);
    }

    /// <summary>
    /// 左右の判定
    /// </summary>
    private bool IsSideTouch(out bool playerHit)
    {
        Vector2 dir = _dir switch
        {
            Direction.Left => Vector2.left, Direction.Right => Vector2.right, _ => Vector2.zero
        };

        RaycastHit2D hit = Physics2D.Raycast(_myTra.position, dir, _ground._sideRayLong, _ground._sideMask);
        playerHit = false;
        if (hit)
            playerHit = hit.transform.CompareTag("Player");
        return hit;
    }

    /// <summary>
    /// ジャンプできるか
    /// </summary>
    /// <returns></returns>
    private bool IsJump()
    {
        Vector2 dir = _dir switch
        {
            Direction.Left => Vector2.left, Direction.Right => Vector2.right, _ => Vector2.zero
        };
        return Physics2D.Raycast(_myTra.position, dir, _ground._jumpRayLong, _ground._mask);
    }

　　/// <summary>
　　/// 石の効果
　　/// </summary>
    public void ReactionStone(float stunTime)
    {
        if (State == EnemyStateType.Escape || State == EnemyStateType.Faint)
            return;

        if (_reactionCoro != null)
            StopCoroutine(_reactionCoro);
        _rb.velocity = Vector2.zero;
        _reactionCoro = StartCoroutine(Stun(stunTime));
    }
  
    /// <summary>
    /// スタンの効果時間を管理するコルーチン
    /// </summary>
    private IEnumerator Stun(float stunTime)
    {
        State = EnemyStateType.Faint;
        yield return new WaitForSeconds(stunTime);
        State = EnemyStateType.Normal;
    }

    public void ReactionBottle(Vector3 bottlePosi, float effectTime)
    {
        if (State == EnemyStateType.Faint) return;

        if (_reactionCoro != null)
        {
            // 再生中のコルーチンがあれば止める
            StopCoroutine(_reactionCoro);
        }
        
        _reactionCoro = StartCoroutine(Bottle(bottlePosi, effectTime));
    }
    
    /// <summary>
    /// 空き瓶の効果持続時間を管理するコルーチン
    /// </summary>
    private IEnumerator Bottle(Vector3 bottlePosi, float effectTime)
    {
        State = EnemyStateType.Escape;
        _bottlePosi = bottlePosi;
        yield return new WaitForSeconds(effectTime);
        State = EnemyStateType.Normal;
    }

    public void ReactionMeat(Vector3 meatPosi, float effectTime)
    {
        if (State == EnemyStateType.Bite || State == EnemyStateType.Faint)
            return;

        State = EnemyStateType.Bite;
        _meatEat = false;
        _meatPosi = meatPosi;
        _meatTimer = Time.time;
        _meatTime = effectTime;
    }

    public void SlowDownScale(float scale, float time)
    {
        if (_coroutine != null)
        {
            // 再生中のコルーチンがあったら止める
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(SlowDown(scale, time));
    }
    
    private IEnumerator SlowDown(float scale, float time)
    {
        float startTime = Time.time;

        _currentSpeed = _speed * scale;
        while (Time.time <= startTime + time)
        {
            yield return new WaitForSeconds(1);
            LifeFluctuation(-1);
        }

        _currentSpeed = _speed;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        switch (State)
        {
            case EnemyStateType.Faint:
            case EnemyStateType.Bite:
            case EnemyStateType.Escape:
            case EnemyStateType.Chase:
                return;
        }

        for (int i = 0; i < col.contacts.Length; i++)
            CollisionReturn(col.GetContact(i).normal, col.GetContact(i).point);
    }
    
    private void CollisionReturn(Vector2 normal, Vector2 point)
    {
        float x = point.x - _myTra.position.x;
        bool isLeft = x < 0;
        bool isTrue = _dir switch { Direction.Right => !isLeft, Direction.Left => isLeft, _ => false };
        if (Mathf.Abs(normal.y) <= 0.2f)
            if (isTrue)
            {
                _dir = (_dir == Direction.Right) ? Direction.Left : Direction.Right;
            }
    }
    
    /// <summary>
    /// HPを減らすメソッド
    /// </summary>
    /// <param name="value">ダメージを受ける場合は負の数を渡す</param>
    public void LifeFluctuation(int value) => _damageHandler.LifeFluctuation(value);

    /// <summary>
    /// 死亡時自身を破棄する
    /// </summary>
    public void Die() => Destroy(gameObject);
    
    private void OnDisable()
    {
        _rb.isKinematic = true;
        _rb.velocity = Vector2.zero;
    }

    #region ギズモの描画処理

    private void OnDrawGizmos()
    {
        if (_alwaysDebug) DebugRendering();
    }

    private void OnDrawGizmosSelected()
    {
        if (!_alwaysDebug) DebugRendering();
    }

    void DebugRendering()
    {
        _myTra = transform;
        Gizmos.color = Color.yellow;
        Vector3 dirPos = _myTra.position + Vector3.up;
        Gizmos.DrawLine(dirPos, dirPos + (_dir == Direction.Right ? Vector3.right : Vector3.left));
        Vector2 rRayPos = _myTra.position + (Vector3)_ground._rightRayPos;
        Vector2 lRayPos = _myTra.position + (Vector3)_ground._leftRayPos;

        RaycastHit2D hit = Physics2D.Raycast(rRayPos, Vector2.down, _ground._rayLong, _ground._mask);
        if (hit)
            Gizmos.DrawLine(rRayPos, hit.point);
        else
            Gizmos.DrawLine(rRayPos, rRayPos + Vector2.down * _ground._rayLong);

        hit = Physics2D.Raycast(lRayPos, Vector2.down, _ground._rayLong, _ground._mask);
        if (hit)
            Gizmos.DrawLine(lRayPos, hit.point);
        else
            Gizmos.DrawLine(lRayPos, lRayPos + Vector2.down * _ground._rayLong);

        Vector2 dir = _dir switch
        {
            Direction.Left => Vector2.left, Direction.Right => Vector2.right, _ => Vector2.zero
        };
        if (State == EnemyStateType.Chase)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue;
        Gizmos.DrawLine(_myTra.position, _myTra.position + (Vector3)(dir * _ground._jumpRayLong));
        if (V == null)
            return;
        if (V.Length <= 0)
            return;
        Gizmos.color = Color.red;
        foreach (ContactPoint2D v in V)
        {
            Gizmos.DrawSphere(v.point, 0.2f);
            Gizmos.DrawLine(v.point, v.point + v.normal);
        }
    }

    #endregion

    #region デバッグ用のメソッド

    [ContextMenu("TestSlowDown")]
    void TestSlowDown() => SlowDownScale(0.5f, 10);

    [ContextMenu("TestReactionStone")]
    void TestReactionStone() => ReactionStone(5);

    [ContextMenu("TestReactionBottle")]
    void TestReactionBottle() => ReactionBottle(Vector2.zero, 5);

    [ContextMenu("TestReactionMeat")]
    void TestReactionMeat() => ReactionMeat(Vector2.zero, 5);

    [ContextMenu("InitializeGroundedRay")]
    void InitializeGroundedRay()
    {
        if (!TryGetComponent<BoxCollider2D>(out _boxCollider))
        {
            Debug.Log("BoxCollider2Dが取得できませんでした");
            return;
        }

        Vector2 size = _boxCollider.size * transform.localScale;
        _ground._mask = Convert.ToInt32("10010000000", 2);
        _ground._sideMask = Convert.ToInt32("110000000", 2);
        _ground._rightRayPos.x = size.x / 2f;
        _ground._leftRayPos.x = -size.x / 2f;
        _ground._rayLong = (size.y / 2f) + 0.2f;
        _ground._sideRayLong = (size.x / 2f) + 0.1f;
        _ground._jumpRayLong = (size.x / 2f) + 0.2f;
    }

    [ContextMenu("SettingStatus")]
    void SettingStatus()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        float pSpeed = player._maxSpeed;

        switch (_beast)
        {
            case EnemyType.StrayDog:
                _maxHp = 1;
                _attack = 1;
                _speed = pSpeed * 0.7f;
                _chaseSpeed = pSpeed * 0.7f;
                _jumpOver = false;
                _canChase = false;
                _goDown = false;
                break;

            case EnemyType.Wolf_Normal:
                _maxHp = 3;
                _attack = 1;
                _speed = pSpeed * 0.9f;
                _chaseSpeed = pSpeed * 1.1f;
                _jumpOver = false;
                _canChase = true;
                _goDown = false;
                break;

            case EnemyType.Wolf_Gray:
                _maxHp = 3;
                _attack = 1;
                _speed = pSpeed * 0.9f;
                _chaseSpeed = pSpeed * 1.1f;
                _jumpOver = true;
                _canChase = true;
                _goDown = true;
                break;

            case EnemyType.Bear:
                _maxHp = 4;
                _attack = 2;
                _speed = pSpeed * 0.5f;
                _chaseSpeed = pSpeed * 1.1f;
                _jumpOver = false;
                _canChase = false;
                _goDown = false;
                break;

            case EnemyType.Boss_Wolf:
                _maxHp = 6;
                _attack = 2;
                _speed = pSpeed * 1.2f;
                _chaseSpeed = pSpeed * 1.2f;
                _jumpOver = true;
                _canChase = true;
                _goDown = true;
                break;

            default:
                Debug.LogError("想定されていない敵の種類です");
                break;
        }
    }

    #endregion
}