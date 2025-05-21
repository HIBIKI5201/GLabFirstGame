using System;
using System.Collections;
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
    [SerializeField] DirectionType _dir; // どちらの方向に動くか
    [SerializeField] bool _alwaysDebug;

    Rigidbody2D _rb;
    Transform _playerTra;
    Transform _modelT;
    Vector2 _modelScale;
    SpriteRenderer[] _spriteRenderers;
    BoxCollider2D _boxCollider;
    PlayerController _player;
    
    private EnemyDamageHandler _damageHandler;
    private EnemyAttackHandler _attackHandler;
    
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
        _attackHandler = new EnemyAttackHandler(_attack, ref _attackedTimer);
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
        if (TryGetComponent(out _rb)) _rb.isKinematic = false;
        if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider2D>();
        _meatIcon = transform.GetChild(1).gameObject;
        
        if (_modelT == null) _modelT = GetComponentInChildren<Animator>().transform;
        _modelScale = _modelT.localScale;
        _modelScale.x = MathF.Abs(_modelScale.x);

        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (_playerTra == null) _playerTra = FindAnyObjectByType<PlayerController>().transform;
    }

    /// <summary>
    /// 地面の高さに自身の位置を合わせる
    /// </summary>
    private void MatchGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, _boxCollider.size, 0, Vector2.down, 1000, _ground._mask);
        if (hit)
        {
            Vector2 pos = transform.position;
            pos.y = hit.point.y + _boxCollider.size.y / 2;
            transform.position = pos;
        }
    }

    #endregion

    void Update()
    {
        _rb.isKinematic = false;

        // 移動
        var vec = _modelScale;
        vec.x *= _dir switch { DirectionType.Right => -1, DirectionType.Left => 1, _ => Mathf.Sign(_modelT.localScale.x) };
        _modelT.localScale = vec;

        // _stunAnimeObj.SetActive(State == EnemyStateType.Faint); // TODO: 素材を確認

        // 状態に応じた処理
        switch (State)
        {
            case EnemyStateType.Faint:
                UpdateStone();
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
                ChangeDirection();
                UpdateHorizontalMovement();
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

    /// <summary>
    /// 方向転換のためのメソッド
    /// </summary>
    private void ChangeDirection()
    {
        if (IsFrontGrounded(out bool isRightDir))
        {
            // もし目の前が崖なら移動方向を反転させる
            _dir = isRightDir ? DirectionType.Right : DirectionType.Left;
        }

        if (IsSideTouch(out bool playerHit))
        {
            // もし壁などに触れたら移動方向を反転させる
            _dir = _dir == DirectionType.Right ? DirectionType.Left : DirectionType.Right;
            if (playerHit)
            {
                AttackToPlayer(); // プレイヤーに当たっていたら攻撃を行う
            }
        }
    }
    
    /// <summary>
    /// プレイヤー追跡状態の処理
    /// </summary>
    private void UpdateChase()
    {
        float x = _playerTra.position.x - transform.position.x;
        float y = _playerTra.position.y - transform.position.y;
        _dir = x <= 0 ? DirectionType.Left : DirectionType.Right;
        if (Mathf.Abs(x) <= 0.1f)
        {
            _dir = DirectionType.None;
        }

        if (!_goDown)
        {
            if (IsFrontGrounded(out bool right))
            {
                switch (_dir)
                {
                    case DirectionType.Left:
                        if (right)
                            _dir = DirectionType.None;
                        break;
                    case DirectionType.Right:
                        if (!right)
                            _dir = DirectionType.None;
                        break;
                }
            }
        }

        if (IsJump() && IsGrounded() && _jumpOver)
        {
            Jump();
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

        UpdateHorizontalMovement();
        Search();
    }

    /// <summary>
    /// 進行方向を更新する
    /// </summary>
    private void UpdateHorizontalMovement()
    {
        Vector2 velo = _rb.velocity;
        velo.x = _currentSpeed * _dir switch { DirectionType.Right => 1, DirectionType.Left => -1, _ => 0 };
        _rb.velocity = velo;
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    private void Jump()
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

        RaycastHit2D hit = Physics2D.Linecast(transform.position, _playerTra.position, _ground._mask);
        State = hit ? EnemyStateType.Normal : EnemyStateType.Chase;
    }

    /// <summary>
    /// プレイヤーへ攻撃する
    /// </summary>
    private void AttackToPlayer() => _attackHandler.Attack();

    private bool IsFrontGrounded(out bool isRightDir)
    {
        Vector2 rRayPos = transform.position + (Vector3)_ground._rightRayPos;
        Vector2 lRayPos = transform.position + (Vector3)_ground._leftRayPos;
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
        return Physics2D.BoxCast(transform.position, _boxCollider.size - new Vector2(0.1f, 0.1f),
            0, Vector2.down, 0.2f, _ground._mask);
    }

    /// <summary>
    /// 左右の判定
    /// </summary>
    private bool IsSideTouch(out bool playerHit)
    {
        Vector2 dir = _dir switch
        {
            DirectionType.Left => Vector2.left, DirectionType.Right => Vector2.right, _ => Vector2.zero
        };

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, _ground._sideRayLong, _ground._sideMask);
        playerHit = false;
        if (hit)
            playerHit = hit.transform.CompareTag("Player");
        return hit;
    }

    private bool IsJump()
    {
        Vector2 dir = _dir switch
        {
            DirectionType.Left => Vector2.left, DirectionType.Right => Vector2.right, _ => Vector2.zero
        };
        return Physics2D.Raycast(transform.position, dir, _ground._jumpRayLong, _ground._mask);
    }

    #region 石

    private void UpdateStone()
    {
        Vector2 velo = _rb.velocity;
        velo.x = 0;
        _rb.velocity = velo;
        _anim.SetBool("Dizzy", true);
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

    #endregion

    #region 空き瓶

    /// <summary>
    /// 空き瓶の効果の処理
    /// </summary>
    private void UpdateBottle()
    {
        float x = _bottlePosi.x - transform.position.x;
        _dir = x >= 0 ? DirectionType.Left : DirectionType.Right; // プレイヤーとは逆の方向に逃げ出す
        _anim.SetBool("Gallop", true); // 逃げ出すアニメーションを再生する

        UpdateHorizontalMovement();
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

    #endregion

    #region 肉

    /// <summary>
    /// 肉の効果の処理
    /// </summary>
    private void UpdateMeat()
    {
        _meatIcon.SetActive(true);
        float x = _meatPosi.x - transform.position.x;
        _dir = x <= 0 ? DirectionType.Left : DirectionType.Right; // 肉がある方へ方向をセットする

        if (Mathf.Abs(x) <= 0.2f)
        {
            _dir = DirectionType.None;
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
                1 => DirectionType.Left, -1 => DirectionType.Right, _ => DirectionType.None
            };
        }

        if (IsJump() && IsGrounded() && _jumpOver)
        {
            Jump();
        }

        UpdateHorizontalMovement();
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

    #endregion
    
    #region 減速（沼）

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

    #endregion
    
    /// <summary>
    /// 何かに衝突しているときの処理
    /// </summary>
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
        {
            CollisionReturn(col.GetContact(i).normal, col.GetContact(i).point);
        }
        
    }
    
    private void CollisionReturn(Vector2 normal, Vector2 point)
    {
        float x = point.x - transform.position.x;
        bool isLeft = x < 0;
        bool isTrue = _dir switch { DirectionType.Right => !isLeft, DirectionType.Left => isLeft, _ => false };
        if (Mathf.Abs(normal.y) <= 0.2f && isTrue)
        {
            _dir = _dir == DirectionType.Right ? DirectionType.Left : DirectionType.Right;
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
        Gizmos.color = Color.yellow;
        Vector3 dirPos = transform.position + Vector3.up;
        Gizmos.DrawLine(dirPos, dirPos + (_dir == DirectionType.Right ? Vector3.right : Vector3.left));
        Vector2 rRayPos = transform.position + (Vector3)_ground._rightRayPos;
        Vector2 lRayPos = transform.position + (Vector3)_ground._leftRayPos;

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
            DirectionType.Left => Vector2.left, DirectionType.Right => Vector2.right, _ => Vector2.zero
        };
        if (State == EnemyStateType.Chase)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(dir * _ground._jumpRayLong));
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
}