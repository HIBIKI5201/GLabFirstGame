using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    Rigidbody2D _rb;
    [SerializeField] int _maxHp;
    [SerializeField] int _currentHp;

    [Space]
    public float _speed;
    public float _currentSpeed;

    [Space]
    [SerializeField] int _attack;

    [Space]
    [SerializeField] State _state;
    State _State {  get => _state;  set{ Debug.Log($"敵{_state}から{value}に移行");_state = value; } }

    [Header("地面や障害物の設定")]
    [SerializeField] GroundedRay _ground;

    [Header("進行方向")]
    [SerializeField] Direction _dir;

    Transform _myTra;
    Vector2 _bottlePosi;
    Vector2 _meatPosi;
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
        public float _sideRayLong;
    }
    enum State
    {
        Normal,//通常
        Stun,//気絶
        Meat,//肉に気づいた
        Bottle//ボトルの音に気づいた
    }

    void Start()
    {
        _currentHp = _maxHp;
        _currentSpeed = _speed;
        _state = State.Normal;

        _myTra = transform;
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
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
            default:
                UpdateReturn();
                break;
        }
    }
    void UpdateReturn()
    {
        IsGrounded(out bool isHitRGround, out bool isHitLGround);
        IsSideTouch(out bool isHitRSide, out bool isHitLSide);
        if (isHitRSide ^ isHitLSide)
        {
            _dir = isHitRSide ? Direction.left : Direction.right;
        }
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
            _dir = Direction.none;
        UpdateVelocity();
    }
    void UpdateVelocity()
    {
        Vector2 velo = _rb.velocity;
        velo.x = _currentSpeed * _dir switch { Direction.right => 1, Direction.left => -1, _ => 0 };
        _rb.velocity = velo;
    }
    bool IsSideTouch(out bool IsHitR, out bool IsHitL)
    {
        Vector2 rayPos = _myTra.position;
        bool isHitR = Physics2D.Raycast(rayPos, Vector2.right, _ground._sideRayLong, _ground._mask);
        bool isHitL = Physics2D.Raycast(rayPos, Vector2.left, _ground._sideRayLong, _ground._mask);

        IsHitR = isHitR;
        IsHitL = isHitL;
        return isHitR || isHitL;
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
            _State = State.Normal;
        }
    }
    public void ReactionBottle(Vector3 bottlePosi)
    {
        _State = State.Bottle;
        _bottlePosi = bottlePosi;
    }
    public void ReactionMeat(Vector3 meatPosi)
    {
        _State = State.Meat;
        _meatPosi = meatPosi;
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
        if (collision.transform.CompareTag("Enemy"))
        {
            _dir = _dir == Direction.right ? Direction.left : Direction.right;
            return;
        }
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Enemy Hit Player");
            return;
        }
    }
    public void LifeFluctuation(int value)
    {
        _currentHp += value;
        Debug.Log($"敵HP{value}増減  残り:{_currentHp}");
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

        hit = Physics2D.Raycast(rayPos, Vector2.right, _ground._sideRayLong, _ground._mask);
        if(hit)
            Gizmos.DrawLine(rayPos, hit.point);
        else
            Gizmos.DrawLine(rayPos, rayPos + Vector2.right * _ground._sideRayLong);

        hit = Physics2D.Raycast(rayPos, Vector2.left, _ground._sideRayLong, _ground._mask);
        if(hit)
            Gizmos.DrawLine(rayPos, hit.point);
        else
            Gizmos.DrawLine(rayPos, rayPos + Vector2.left * _ground._sideRayLong);
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
            Debug.Log("BoxCollider2Dがみつからない"); 
            return;
        }

        Vector2 size = col.size * transform.localScale;
        _ground._rightRayPos.x = size.x / 2f;
        _ground._leftRayPos.x = -size.x / 2f;
        _ground._rayLong = (size.y / 2f) + 0.1f;
        _ground._sideRayLong = (size.x / 2f) + 0.1f;
    }
}
