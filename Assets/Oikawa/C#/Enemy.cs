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
    [SerializeField] int _maxHp;
    [SerializeField] int _currentHp;

    [Space]
    public float _speed;
    public float _currentSpeed;

    [Space]
    [SerializeField] int _attack;

    [Space]
    [SerializeField] State _state;
    State _State {  get => _state;  set{ Debug.Log($"ìG{_state}Ç©ÇÁ{value}Ç…à⁄çs");_state = value; } }

    [Header("îÚÇ—âzÇ¶ÇÈ")]
    [SerializeField] bool _jumpOver;
    [SerializeField] float _jumpRayLong;

    [Header("ínñ Ç‚è·äQï®ÇÃê›íË")]
    [SerializeField] GroundedRay _ground;

    [Header("êiçsï˚å¸")]
    [SerializeField] Direction _dir;

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
        public LayerMask _mask;
        public Vector2 _rightRayPos;
        public Vector2 _leftRayPos;
        public float _rayLong;
    }
    enum State
    {
        Normal,//í èÌ
        Stun,//ãCê‚
        Meat,//ì˜Ç…ãCÇ√Ç¢ÇΩ
        Bottle,//É{ÉgÉãÇÃâπÇ…ãCÇ√Ç¢ÇΩ
        Chase,
    }

    void Start()
    {
        _currentHp = _maxHp;
        _currentSpeed = _speed;
        _state = State.Normal;

        _myTra = transform;
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        if(_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_playerTra == null)
            _playerTra = GameObject.FindAnyObjectByType<PlayerController>().transform;
    }
    void Update()
    {
        if (_currentHp <= 0)
        {
            Debug.Log("ìGéÄñS");
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
        UpdateVelocity();
    }
    void UpdateChase()
    {
        float x = _playerTra.position.x - _myTra.position.x;
        _dir = x <= 0 ? Direction.left : Direction.right;
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
    bool IsJump()
    {
        Vector2 rayPos = _myTra.position;
        Vector2 dir = _dir switch { Direction.right => Vector2.right, Direction.left => Vector2.left, _ => Vector2.zero };
        return Physics2D.Raycast(rayPos, dir, _jumpRayLong, _ground._mask);
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (_State)
        {
            case State.Stun:
            case State.Meat:
            case State.Bottle:
                return;
        }
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Enemy Hit Player");
            collision.transform.GetComponent<PlayerController>().FluctuationLife(-1);
            return;
        }
        CollisionReturn(collision.GetContact(0).normal, collision.GetContact(0).point);
        void CollisionReturn(Vector2 normal,Vector2 point)
        {
            float x = point.x - _myTra.position.x;
            bool isLeft = x < 0;
            bool isTrue = _dir switch { Direction.right => !isLeft, Direction.left => isLeft, _ => false };
            if (Mathf.Abs(normal.y) <= 0.2f)
                if (isTrue)
                    if (_jumpOver && _State == State.Chase)
                    {
                        VelocityJump();
                    }
                    else
                    {
                        _dir = (_dir == Direction.right) ? Direction.left : Direction.right;
                    }
        }
    }
    public void LifeFluctuation(int value)
    {
        _currentHp += value;
        Debug.Log($"ìGHP{value}ëùå∏  écÇË:{_currentHp}");
    }
    void OnDisable()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = Vector3.zero;
    }
    private void OnDrawGizmosSelected()
    {
        _myTra = transform;
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
    }

    [ContextMenu("TestSlowDown")]
    void TestSlowDown() => SlowDownScale(0.5f, 10);

    [ContextMenu("TestReactionStone")]
    void TestReactionStone() => ReactionStone(5);
    [ContextMenu("TestReactionBottle")]
    void TestReactionBottle() => ReactionBottle(Vector2.zero);
    [ContextMenu("TestReactionMeat")]
    void TestReactionMeat() => ReactionMeat(Vector2.zero);
    [ContextMenu("InitializeGroundedRay")]
    void InitializeGroundedRay()
    {
        if (!TryGetComponent<BoxCollider2D>(out BoxCollider2D col)) 
        { 
            Debug.Log("BoxCollider2DÇ™Ç›Ç¬Ç©ÇÁÇ»Ç¢"); 
            return;
        }

        Vector2 size = col.size * transform.localScale;
        _ground._rightRayPos.x = size.x / 2f;
        _ground._leftRayPos.x = -size.x / 2f;
        _ground._rayLong = (size.y / 2f) + 0.1f;
    }
}
