using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    AudioManager _audio;
    Rigidbody2D _rb;
    Transform _playerTra;
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider;

    [Tooltip("動物の種類")]
    [SerializeField] Beast _beast;
    enum Beast
    {
        /// <summary>
        /// 野犬
        /// </summary>
        StrayDog,

        /// <summary>
        /// 狼
        /// </summary>
        Wolf_Normal,

        /// <summary>
        /// 狼(灰)
        /// </summary>
        Wolf_Gray,

        /// <summary>
        /// 熊
        /// </summary>
        Bear,

        /// <summary>
        /// ステージ4のボスの狼
        /// </summary>
        Boss_Wolf
    }
    [Tooltip("最大HP"), Space]
    [SerializeField] int _maxHp;
    [Tooltip("現在HP")]
    [SerializeField] int _currentHp;

    [Space, Tooltip("初期の速度")]
    public float _speed;
    public float _chaseSpeed;
    [Tooltip("現在の速度")]
    public float _currentSpeed;

    [Space, Tooltip("攻撃力")]
    [SerializeField] int _attack;

    [Space, Tooltip("状態")]
    [SerializeField] State _state;
    State _State
    {
        get => _state;
        set
        {
            if (value == _state)
                return;
            Debug.Log($"敵{_state}から{value}に移行");
            _state = value;
        }
    }

    [Tooltip("飛び越える")]
    [SerializeField] bool _jumpOver;

    [Tooltip("追いかけられる")]
    [SerializeField] bool _canChase;

    [Tooltip("ダメージ受けれる")]
    [SerializeField] bool _canDamage;

    [Tooltip("地面や障害物の設定")]
    [SerializeField] GroundedRay _ground;

    [Tooltip("進行方向")]
    [SerializeField] Direction _dir;

    [Tooltip("常にデバッグを表示")]
    [SerializeField] bool _alwaysDebug;

    Transform _myTra;
    Vector2 _bottlePosi;
    Vector2 _meatPosi;
    float _meatTimer;
    bool _meatEat;
    bool _canMoveSE;
    enum Direction
    {
        right, left, none
    }
    [System.Serializable]
    struct GroundedRay
    {
        [Tooltip("Rayが反応するLayer")]
        public LayerMask _mask;

        [Tooltip("右側の崖を判断するRayの中心")]
        public Vector2 _rightRayPos;

        [Tooltip("左側の崖を判断するRayの中心")]
        public Vector2 _leftRayPos;

        [Space, Tooltip("崖を判断するRayの長さ")]
        public float _rayLong;

        [Tooltip("壁を判断するRayの長さ")]
        public float _sideRayLong;

        [Space, Tooltip("飛ぶのを判断するRayの長さ")]
        public float _jumpRayLong;
    }
    enum State
    {
        /// <summary>
        /// 通常
        /// </summary>
        Normal,

        /// <summary>
        /// 石で気絶
        /// </summary>
        Faint,

        /// <summary>
        /// 肉に気づいた
        /// </summary>
        Bite,

        /// <summary>
        /// ボトルの音に気づいた
        /// </summary>
        Escape,

        /// <summary>
        /// プレイヤーを見つけた
        /// </summary>
        Chase,
    }
    void Start() => Awake();
    void Awake()
    {
        ResetStatus();
        CacheComponents();
        MatchGround();
    }
    void ResetStatus()
    {
        PhysicsMaterial2D physicsMaterial2D = new PhysicsMaterial2D();
        physicsMaterial2D.friction = 0;
        physicsMaterial2D.bounciness = 0;
        GetComponent<Collider2D>().sharedMaterial = physicsMaterial2D;

        _currentHp = _maxHp;
        _currentSpeed = _speed;
        _state = State.Normal;
        _canDamage = true;

        StartCoroutine(WaitAudio());
        IEnumerator WaitAudio()
        {
            _canMoveSE = false;
            yield return new WaitForSeconds(0.2f);
            _canMoveSE = true;
        }
    }
    void CacheComponents()
    {
        _myTra = transform;
        _audio = AudioManager.Instance;
        _rb = _rb ?? GetComponent<Rigidbody2D>();

        _spriteRenderer = _spriteRenderer ?? GetComponent<SpriteRenderer>();

        _playerTra = _playerTra ?? GameObject.FindAnyObjectByType<PlayerController>().transform;
        _boxCollider = _boxCollider ?? GetComponent<BoxCollider2D>();
        Debug.Log(_spriteRenderer == null);
    }
    void MatchGround()
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
        if (_currentHp <= 0)
        {
            Debug.Log("敵死亡");
            Destroy(this.gameObject);
        }
        _spriteRenderer.flipX = _dir switch { Direction.right => true, Direction.left => false, _ => _spriteRenderer.flipX };
        switch (_State)
        {
            case State.Faint:
                _dir = Direction.none;
                UpdateVelocity();
                break;
            case State.Bite:
                UpdateMeat();
                break;
            case State.Escape:
                UpdateBottle();
                break;
            case State.Chase:
                UpdateChase();
                break;

            case State.Normal:
            default:
                UpdateReturn();
                UpdateVelocity();
                Search();
                break;
        }
        void UpdateReturn()
        {
            IsGrounded(out bool isHitRGround, out bool isHitLGround);
            if (isHitRGround ^ isHitLGround)
            {
                _dir = isHitRGround ? Direction.right : Direction.left;
            }
            if (IsSideTouch())
            {
                _dir = (_dir == Direction.right) ? Direction.left : Direction.right;
            }
        }
        void UpdateBottle()
        {
            float x = _bottlePosi.x - _myTra.position.x;
            _dir = x >= 0 ? Direction.left : Direction.right;
            UpdateVelocity();
        }
        void UpdateMeat()
        {
            float x = _meatPosi.x - _myTra.position.x;
            _dir = x <= 0 ? Direction.left : Direction.right;
            if (Mathf.Abs(x) <= 0.2f) 
            {
                _dir = Direction.none;
                if (!_meatEat)
                    _meatTimer = Time.time;
                _meatEat = true;
            }
            if (Time.time >= _meatTimer + 5)
            {
                _State = State.Normal;
            }
            if (IsJump() && IsGrounded(out _, out _) && _jumpOver)
                VelocityJump();
            UpdateVelocity();
        }
        void UpdateChase()
        {
            float x = _playerTra.position.x - _myTra.position.x;
            float y = _playerTra.position.y - _myTra.position.y;
            _dir = x <= 0 ? Direction.left : Direction.right;
            if (Mathf.Abs(x) <= Mathf.Min(0.1f, y))
            {
                _dir = Direction.none;
            }
            if (IsJump() && IsGrounded(out _, out _) && _jumpOver)
                VelocityJump();
            UpdateVelocity();
            Search();
        }
        void UpdateVelocity()
        {
            Vector2 velo = _rb.velocity;
            velo.x = _currentSpeed * _dir switch { Direction.right => 1, Direction.left => -1, _ => 0 };
            _rb.velocity = velo;
        }
        void VelocityJump()
        {
            Vector2 velo = _rb.velocity;
            velo.y = 5;
            _rb.velocity = velo;
        }
        void Search()
        {
            if (!_canChase)
                return;

            RaycastHit2D hit = Physics2D.Linecast(_myTra.position, _playerTra.position, _ground._mask);
            _State = hit ? State.Normal : State.Chase;
        }
        bool IsGrounded(out bool IsHitR, out bool IsHitL)
        {
            Vector2 rRayPos = _myTra.position + (Vector3)_ground._rightRayPos;
            Vector2 lRayPos = _myTra.position + (Vector3)_ground._leftRayPos;
            bool isHitR = Physics2D.Raycast(rRayPos, Vector2.down, _ground._rayLong, _ground._mask);
            bool isHitL = Physics2D.Raycast(lRayPos, Vector2.down, _ground._rayLong, _ground._mask);

            IsHitR = isHitR;
            IsHitL = isHitL;
            return isHitR || isHitL;
        }
        bool IsSideTouch()
        {
            Vector2 dir = _dir switch { Direction.left => Vector2.left, Direction.right => Vector2.right, _ => Vector2.zero };
            return Physics2D.Raycast(_myTra.position, dir, _ground._sideRayLong, _ground._mask);
        }
        bool IsJump()
        {
            Vector2 dir = _dir switch { Direction.left => Vector2.left, Direction.right => Vector2.right, _ => Vector2.zero };
            return Physics2D.Raycast(_myTra.position, dir, _ground._jumpRayLong, _ground._mask);
        }
    }
    Coroutine _reactionCoro = null;
    public void ReactionStone(float stunTime)
    {
        if (_State == State.Escape || _State == State.Faint)
            return;

        if (_reactionCoro != null)
            StopCoroutine(_reactionCoro);
        _rb.velocity = Vector2.zero;
        _reactionCoro = StartCoroutine(Stun());
        IEnumerator Stun()
        {
            _State = State.Faint;
            yield return new WaitForSeconds(stunTime);
            _State = State.Chase;
        }
    }
    public void ReactionBottle(Vector3 bottlePosi)
    {
        if (_State == State.Faint)
            return;

        if(_reactionCoro != null)
            StopCoroutine(_reactionCoro);
        _reactionCoro = StartCoroutine(Bottle());
        IEnumerator Bottle()
        {
            _State = State.Escape;
            _bottlePosi = bottlePosi;
            yield return new WaitForSeconds(5);
            _State = State.Normal;
        }
    }
    public void ReactionMeat(Vector3 meatPosi)
    {
        if (_State == State.Bite|| _State == State.Faint)
            return;

        _State = State.Bite;
        _meatEat = false;
        _meatPosi = meatPosi;
        _meatTimer = Time.time;
    }

    Coroutine coroutine = null;
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

    Coroutine damageCoro = null;
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
                _spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
                yield return new WaitForSeconds(0.1f);
                _spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
            _canDamage = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        switch (_State)
        {
            case State.Faint:
            case State.Bite:
            case State.Escape:
                return;
        }
        if (col.transform.CompareTag("Player"))
        {
            for (int i = 0; i < col.contacts.Length; i++)
                CollisionPlayer(col.GetContact(i).normal, col.GetContact(i).point);
            return;
        }
        for (int i = 0; i < col.contacts.Length; i++)
            CollisionReturn(col.GetContact(i).normal, col.GetContact(i).point);

        void CollisionReturn(Vector2 normal, Vector2 point)
        {
            float x = point.x - _myTra.position.x;
            bool isLeft = x < 0;
            bool isTrue = _dir switch { Direction.right => !isLeft, Direction.left => isLeft, _ => false };
            if (Mathf.Abs(normal.y) <= 0.2f)
                if (isTrue)
                {
                    _dir = (_dir == Direction.right) ? Direction.left : Direction.right;
                }
        }
        void CollisionPlayer(Vector2 normal, Vector2 point)
        {
            Debug.Log("Enemy Hit Player");
            Vector2 localPosi = point - (Vector2)_myTra.position;
            bool isSteppedOn = (normal.y <= 0.5f) && (localPosi.y >= 0.2f);
            if (!isSteppedOn)
                col.transform.GetComponent<PlayerController>().FluctuationLife(-_attack);
        }
    }
    ContactPoint2D[] V;
    private void OnCollisionStay2D(Collision2D collision) => V = collision.contacts;
    public void LifeFluctuation(int value)
    {
        if (!_canDamage&&value < 0)
        {
            Debug.Log("敵無敵時間のためダメージ無し");
            return;
        }
        _currentHp += value;
        Debug.Log($"敵HP{value}増減  残り:{_currentHp}");
        if (value < 0)
            DamageEffect();
    }
    void OnDisable()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true;
        _rb.velocity = Vector2.zero;
    }
    void OnEnable()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = false;
        if (_canMoveSE)
        {
            _audio.PlaySE(_beast switch
            {
                Beast.StrayDog => "strayDog",
                Beast.Wolf_Normal or Beast.Wolf_Gray => "wolf",
                Beast.Bear => "bare",
                _ => ""
            });
            _canMoveSE = false;
        } 
    }
    private void OnDrawGizmos(){if(_alwaysDebug) DebugRendering(); }
    private void OnDrawGizmosSelected() { if (!_alwaysDebug) DebugRendering(); }
    void DebugRendering()
    {
        _myTra = transform;
        Gizmos.color = Color.yellow;
        Vector3 dirPos = _myTra.position + Vector3.up;
        Gizmos.DrawLine(dirPos, dirPos + (_dir == Direction.right ? Vector3.right : Vector3.left));
        Vector2 rayPos = _myTra.position;
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

        Vector2 dir = _dir switch { Direction.left => Vector2.left, Direction.right => Vector2.right, _ => Vector2.zero };
        if (_State == State.Chase)
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

    [ContextMenu("TestSlowDown")]
    void TestSlowDown() => SlowDownScale(0.5f, 10);

    [ContextMenu("TestReactionStone")]
    void TestReactionStone() => ReactionStone(5);
    [ContextMenu("TestReactionBottle")]
    void TestReactionBottle() => ReactionBottle(Vector2.zero);
    [ContextMenu("TestReactionMeat")]
    void TestReactionMeat() => ReactionMeat(Vector2.zero);
    [ContextMenu("InitializeGroundedRay(自動設定)")]
    void InitializeGroundedRay()
    {
        if (!TryGetComponent<BoxCollider2D>(out _boxCollider)) 
        { 
            Debug.Log("BoxCollider2Dがみつからない"); 
            return;
        }

        Vector2 size = _boxCollider.size * transform.localScale;
        _ground._mask = Convert.ToInt32("10000000", 2);
        _ground._rightRayPos.x = size.x / 2f;
        _ground._leftRayPos.x = -size.x / 2f;
        _ground._rayLong = (size.y / 2f) + 0.1f;
        _ground._sideRayLong = (size.x / 2f) + 0.1f;
        _ground._jumpRayLong = (size.x / 2f) + 0.2f;
    }

    [ContextMenu("SettingStatus(自動設定)")]
    void SettingStatus()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        float pSpeed = player._speed;

        switch (_beast)
        {
            case Beast.StrayDog:
                _maxHp = 1;
                _attack = 1;
                _speed = pSpeed * 0.7f;
                _chaseSpeed = pSpeed * 0.7f;
                _jumpOver = false;
                _canChase = false;
                break;
            case Beast.Wolf_Normal:
                _maxHp = 3;
                _attack = 1;
                _speed = pSpeed * 0.9f;
                _chaseSpeed = pSpeed * 1.1f;
                _jumpOver = false;
                _canChase = true;
                break;
            case Beast.Wolf_Gray:
                _maxHp = 3;
                _attack = 1;
                _speed = pSpeed * 0.9f;
                _chaseSpeed = pSpeed * 1.1f;
                _jumpOver = true;
                _canChase = true;
                break;
            case Beast.Bear:
                _maxHp = 4;
                _attack = 2;
                _speed = pSpeed * 0.5f;
                _chaseSpeed = pSpeed * 1.1f;
                _jumpOver = false;
                _canChase = false;
                break;
            case Beast.Boss_Wolf:
                _maxHp = 6;
                _attack = 2;
                _speed = pSpeed * 1.2f;
                _chaseSpeed = pSpeed * 1.2f;
                _jumpOver = true;
                _canChase = true;
                break;
            default:
                Debug.LogError("存在しないBeastを参照している");
                break;
        }
    }
}
