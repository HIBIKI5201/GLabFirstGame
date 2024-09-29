using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    Rigidbody2D _rb;
    Transform _playerTra;
    SpriteRenderer _spriteRenderer;

    [Tooltip("Å‘åHP")]
    [SerializeField] int _maxHp;
    [Tooltip("Œ»İHP")]
    [SerializeField] int _currentHp;

    [Space, Tooltip("‰Šú‚Ì‘¬“x")]
    public float _speed;
    [Tooltip("Œ»İ‚Ì‘¬“x")]
    public float _currentSpeed;

    [Space, Tooltip("UŒ‚—Í")]
    [SerializeField] int _attack;

    [Space, Tooltip("ó‘Ô")]
    [SerializeField] State _state;
    State _State { get => _state; set { Debug.Log($"“G{_state}‚©‚ç{value}‚ÉˆÚs"); _state = value; } }

    [Tooltip("”ò‚Ñ‰z‚¦‚é")]
    [SerializeField] bool _jumpOver;

    [Tooltip("’n–Ê‚âáŠQ•¨‚Ìİ’è")]
    [SerializeField] GroundedRay _ground;

    [Tooltip("is•ûŒü")]
    [SerializeField] Direction _dir;

    [Tooltip("í‚ÉƒfƒoƒbƒO‚ğ•\¦")]
    [SerializeField] bool _alwaysDebug;

    Transform _myTra;
    Vector2 _bottlePosi;
    Vector2 _meatPosi;
    float _meatTimer;
    bool _meatEat;
    enum Direction
    {
        right, left, none
    }
    [System.Serializable]
    struct GroundedRay
    {
        [Tooltip("Ray‚ª”½‰‚·‚éLayer")]
        public LayerMask _mask;

        [Tooltip("‰E‘¤‚ÌŠR‚ğ”»’f‚·‚éRay‚Ì’†S")]
        public Vector2 _rightRayPos;

        [Tooltip("¶‘¤‚ÌŠR‚ğ”»’f‚·‚éRay‚Ì’†S")]
        public Vector2 _leftRayPos;

        [Space, Tooltip("ŠR‚ğ”»’f‚·‚éRay‚Ì’·‚³")]
        public float _rayLong;

        [Space, Tooltip("”ò‚Ô‚Ì‚ğ”»’f‚·‚éRay‚Ì’·‚³")]
        public float _jumpRayLong;
    }
    enum State
    {
        Normal,//’Êí
        Stun,//‹Câ
        Meat,//“÷‚É‹C‚Ã‚¢‚½
        Bottle,//ƒ{ƒgƒ‹‚Ì‰¹‚É‹C‚Ã‚¢‚½
        Chase,
    }

    void Start()
    {
        PhysicsMaterial2D physicsMaterial2D = new PhysicsMaterial2D();
        physicsMaterial2D.friction = 0;
        physicsMaterial2D.bounciness = 0;
        GetComponent<Collider2D>().sharedMaterial = physicsMaterial2D;
        _currentHp = _maxHp;
        _currentSpeed = _speed;
        _state = State.Normal;

        _myTra = transform;
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_playerTra == null)
            _playerTra = GameObject.FindAnyObjectByType<PlayerController>().transform;
    }
    void Update()
    {
        if (_currentHp <= 0)
        {
            Debug.Log("“G€–S");
            Destroy(this.gameObject);
        }
        _spriteRenderer.flipX = _dir switch { Direction.right => true, Direction.left => false, _ => _spriteRenderer.flipX };
        switch (_State)
        {
            case State.Normal:
                UpdateReturn();
                break;
            case State.Stun:
                _dir = Direction.none;
                UpdateVelocity();
                break;
            case State.Meat:
                UpdateMeat();
                break;
            case State.Bottle:
                UpdateBottle();
                break;
            case State.Chase:
                UpdateChase();
                break;
            default:
                UpdateReturn();
                break;
        }
        void UpdateReturn()
        {
            IsGrounded(out bool isHitRGround, out bool isHitLGround);
            if (isHitRGround ^ isHitLGround)
            {
                _dir = isHitRGround ? Direction.right : Direction.left;
            }
            UpdateVelocity();
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
            if (Mathf.Abs(x) <= 0.05f)
            {
                _dir = Direction.none;
                if (_meatEat)
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
            _dir = x <= 0 ? Direction.left : Direction.right;
            if (IsJump() && IsGrounded(out _, out _) && _jumpOver)
                VelocityJump();
            UpdateVelocity();
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
        bool IsJump()
        {
            Vector2 dir = _dir switch { Direction.left => Vector2.left, Direction.right => Vector2.right, _ => Vector2.zero };
            return Physics2D.Raycast(_myTra.position, dir, _ground._jumpRayLong, _ground._mask);
        }
    }

    public void ReactionStone(float stunTime)
    {
        _rb.velocity = Vector2.zero;
        StartCoroutine(Stun());
        IEnumerator Stun()
        {
            _State = State.Stun;
            yield return new WaitForSeconds(stunTime);
            _State = State.Chase;
        }
    }
    public void ReactionBottle(Vector3 bottlePosi)
    {
        StartCoroutine(Bottle());
        IEnumerator Bottle()
        {
            _State = State.Bottle;
            _bottlePosi = bottlePosi;
            yield return new WaitForSeconds(5);
            _State = State.Normal;
        }
    }
    public void ReactionMeat(Vector3 meatPosi)
    {
        _State = State.Meat;
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
                _currentHp--;
            }
            _currentSpeed = _speed;
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        switch (_State)
        {
            case State.Stun:
            case State.Meat:
            case State.Bottle:
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
        _currentHp += value;
        Debug.Log($"“GHP{value}‘Œ¸  c‚è:{_currentHp}");
    }
    void OnDisable()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = Vector3.zero;
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
        if (V.Length <= 0 || V == null)
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
    [ContextMenu("InitializeGroundedRay(©“®İ’è)")]
    void InitializeGroundedRay()
    {
        if (!TryGetComponent<BoxCollider2D>(out BoxCollider2D col)) 
        { 
            Debug.Log("BoxCollider2D‚ª‚İ‚Â‚©‚ç‚È‚¢"); 
            return;
        }

        Vector2 size = col.size * transform.localScale;
        _ground._mask = Convert.ToInt32("10000000", 2);
        _ground._rightRayPos.x = size.x / 2f;
        _ground._leftRayPos.x = -size.x / 2f;
        _ground._rayLong = (size.y / 2f) + 0.1f;
        _ground._jumpRayLong = (size.x / 2f) + 0.2f;
    }
}
