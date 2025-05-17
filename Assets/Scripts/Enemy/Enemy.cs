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

    AudioManager _audio;
    Rigidbody2D _rb;
    Transform _playerTra;
    Transform _modelT;
    SpriteRenderer[] spriteRenderers;
    BoxCollider2D _boxCollider;
    PlayerController _player;
    GameObject _meatIcon;
    GameObject _stunAnimeObj;
    
    Transform _myTra;
    Vector2 _bottlePosi;
    Vector2 _modelScale;
    ContactPoint2D[] V;
    
    Coroutine _reactionCoro = null;
    Coroutine damageCoro = null;
    Coroutine coroutine = null;
    
    // 肉アイテム関連
    Vector2 _meatPosi;
    float _meatTimer;
    float _meatTime;
    bool _meatEat;

    bool _canMoveSE;
    bool _canReset;

    float _attackedTimer;
    
    private void Awake()
    {
        _canReset = true;
        ResetStatus();
        CacheComponents();
        _meatIcon = transform.GetChild(1).gameObject;
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

        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        
        _rb.isKinematic = false;
        
        if (_canMoveSE)
        {
            _audio.PlaySE(_beast switch
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
        _audio = AudioManager.Instance;

        _rb = (_rb != null) ? _rb : GetComponent<Rigidbody2D>();

        if (_modelT == null)
            _modelT = (_modelT != null) ? _modelT : GetComponentInChildren<Animator>().transform;
        _modelScale = _modelT.localScale;
        _modelScale.x = MathF.Abs(_modelScale.x);
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        _playerTra = (_playerTra != null) ? _playerTra : GameObject.FindAnyObjectByType<PlayerController>().transform;

        _boxCollider = (_boxCollider != null) ? _boxCollider : GetComponent<BoxCollider2D>();
    }
    
    /// <summary>
    /// 接地地点
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
    
    void Update()
    {
        _rb.isKinematic = false;
        if (_currentHp <= 0)
        {
            Destroy(gameObject); // HPがゼロになったら自分を破棄する
        }
        var vec = _modelScale;
        vec.x *= _dir switch { Direction.Right => -1, Direction.Left => 1, _ => Mathf.Sign(_modelT.localScale.x) };
        _modelT.localScale = vec;

        // _stunAnimeObj.SetActive(State == EnemyStateType.Faint); // TODO: 素材を確認

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
        
        if(GameManager.instance.StateType == GameStateType.GameOver ||
        GameManager.instance.StateType == GameStateType.StageClear)
        {
            // ゲームオーバー、もしくはゲームクリア時には敵を非表示にする
            enabled = false;
            gameObject.SetActive(false);
        }

        void UpdateReturn()
        {
            if (IsFrontGrounded(out bool isRightDir))
            {
                _dir = isRightDir ? Direction.Right : Direction.Left;
            }
            if(IsSideTouch(out bool playerHit))
            {
                _dir = (_dir == Direction.Right) ? Direction.Left : Direction.Right;
                if (playerHit)
                    AttackToPlayer();
            }
        }
        
        void UpdateBottle()
        {
            float x = _bottlePosi.x - _myTra.position.x;
            _dir = x >= 0 ? Direction.Left : Direction.Right;
            _anim.SetBool("Gallop", true);

            UpdateVelocity();
        }
        
        void UpdateMeat()
        {
            _meatIcon.SetActive(true);
            float x = _meatPosi.x - _myTra.position.x;
            _dir = x <= 0 ? Direction.Left : Direction.Right;

            if (Mathf.Abs(x) <= 0.2f) 
            {
                _dir = Direction.None;
                if (!_meatEat)
                    _meatTimer = Time.time;
                _meatEat = true;
            }
            
            if (Time.time >= _meatTimer + _meatTime)
            {
                State = EnemyStateType.Normal;
                _meatIcon.SetActive(false);
                _dir = Mathf.Sign(_modelT.localScale.x) switch { 1 => Direction.Left, -1 => Direction.Right, _ => Direction.None };
            }

            if (IsJump() && IsGrounded() && _jumpOver)
            {
                VelocityJump();
            }
                
            UpdateVelocity();
        }
        
        void UpdateChase()
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
                            if(right)
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
        
        void UpdateVelocity()
        {
            Vector2 velo = _rb.velocity;
            //if (!_jumpOver||!_canChase)
                //velo.y = velo.y > 0.01f ? 0.01f : velo.y;

            velo.x = _currentSpeed * _dir switch { Direction.Right => 1, Direction.Left => -1, _ => 0 };
            _rb.velocity = velo;
        }
        
        void VelocityJump()
        {
            Vector2 velo = _rb.velocity;
            velo.y = _jumpPower;
            _rb.velocity = velo;
        }
        
        void Search()
        {
            if (!_canChase)
                return;

            RaycastHit2D hit = Physics2D.Linecast(_myTra.position, _playerTra.position, _ground._mask);
            State = hit ? EnemyStateType.Normal : EnemyStateType.Chase;
        }
        
        void AttackToPlayer()
        {
            _player = _player != null ? _player : FindAnyObjectByType<PlayerController>();
            if (Time.time <= _attackedTimer + 0.1f)
                return;
            _attackedTimer = Time.time;
            _player.FluctuationLife(-_attack);
        }
        
        bool IsFrontGrounded(out bool isRightDir)
        {

            Vector2 rRayPos = _myTra.position + (Vector3)_ground._rightRayPos;
            Vector2 lRayPos = _myTra.position + (Vector3)_ground._leftRayPos;
            bool isHitR = Physics2D.Raycast(rRayPos, Vector2.down, _ground._rayLong, _ground._mask);
            bool isHitL = Physics2D.Raycast(lRayPos, Vector2.down, _ground._rayLong, _ground._mask);
            isRightDir = isHitR;
            return isHitR ^ isHitL;
            /*Vector2 rayPos = (Vector2)_myTra.position + _dir switch
            {
                Direction.Right => _ground._rightRayPos,
                Direction.Left => _ground._leftRayPos,
                _ => Vector2.zero
            };*/
            //return Physics2D.Raycast(rayPos, Vector2.down, _ground._rayLong, _ground._mask);
        }
        
        bool IsGrounded()
        {
            return Physics2D.BoxCast(_myTra.position, _boxCollider.size - new Vector2(0.1f,0.1f), 0, Vector2.down, 0.2f, _ground._mask);
        }
        
        bool IsSideTouch(out bool playerHit)
        {
            Vector2 dir = _dir switch { Direction.Left => Vector2.left, Direction.Right => Vector2.right, _ => Vector2.zero };

            RaycastHit2D hit = Physics2D.Raycast(_myTra.position, dir, _ground._sideRayLong, _ground._sideMask);
            playerHit = false;
            if (hit)
                playerHit = hit.transform.CompareTag("Player");
            return hit;
        }
        
        bool IsJump()
        {
            Vector2 dir = _dir switch { Direction.Left => Vector2.left, Direction.Right => Vector2.right, _ => Vector2.zero };
            return Physics2D.Raycast(_myTra.position, dir, _ground._jumpRayLong, _ground._mask);
        }
    }
    
    
    public void ReactionStone(float stunTime)
    {
        if (State == EnemyStateType.Escape || State == EnemyStateType.Faint)
            return;

        if (_reactionCoro != null)
            StopCoroutine(_reactionCoro);
        _rb.velocity = Vector2.zero;
        _reactionCoro = StartCoroutine(Stun());
        IEnumerator Stun()
        {
            State = EnemyStateType.Faint;
            yield return new WaitForSeconds(stunTime);
            State = EnemyStateType.Normal;
        }
    }
    
    public void ReactionBottle(Vector3 bottlePosi,float effectTime)
    {
        if (State == EnemyStateType.Faint)
            return;

        if(_reactionCoro != null)
            StopCoroutine(_reactionCoro);
        _reactionCoro = StartCoroutine(Bottle());
        IEnumerator Bottle()
        {
            State = EnemyStateType.Escape;
            _bottlePosi = bottlePosi;
            yield return new WaitForSeconds(effectTime);
            State = EnemyStateType.Normal;
        }
    }
    
    public void ReactionMeat(Vector3 meatPosi, float effectTime)
    {
        if (State == EnemyStateType.Bite|| State == EnemyStateType.Faint)
            return;

        State = EnemyStateType.Bite;
        _meatEat = false;
        _meatPosi = meatPosi;
        _meatTimer = Time.time;
        _meatTime = effectTime;
    }
    
    public void SlowDownScale(float scale, float time)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(SlowDown());
        IEnumerator SlowDown()
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
    }

    
    void DamageEffect()
    {
        if (damageCoro != null)
            StopCoroutine(Damage());
        damageCoro = StartCoroutine(Damage());
        IEnumerator Damage()
        {
            _canDamage = false;
            float time = Time.time;
            while(Time.time <= time + 0.5f)
            {
                spriteRenderers.ToList().ForEach(x => x.color = new Color(0.5f, 0.5f, 0.5f));
                yield return new WaitForSeconds(0.1f);
                spriteRenderers.ToList().ForEach(x => x.color = Color.white);
                yield return new WaitForSeconds(0.1f);
            }
            _canDamage = true;
        }
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

        void CollisionReturn(Vector2 normal, Vector2 point)
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
    }
    
    public void LifeFluctuation(int value)
    {
        if (!_canDamage && value < 0)
        {
            Debug.Log("");
            return;
        }
        
        _currentHp += value;
        Debug.Log($"{value}  現在のHP:{_currentHp}");
        if (value < 0)
        {
            DamageEffect();
        }
    }
    
    void OnDisable()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true;
        _rb.velocity = Vector2.zero;
    }

    #region ギズモの描画処理

    private void OnDrawGizmos(){ if(_alwaysDebug) DebugRendering(); }
    private void OnDrawGizmosSelected() { if (!_alwaysDebug) DebugRendering(); }
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

        Vector2 dir = _dir switch { Direction.Left => Vector2.left, Direction.Right => Vector2.right, _ => Vector2.zero };
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
    void TestReactionMeat() => ReactionMeat(Vector2.zero,5);
    
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
