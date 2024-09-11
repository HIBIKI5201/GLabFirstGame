using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] int _hp;
    [SerializeField] int _speed;
    [SerializeField] MoveType _moveType;
    [SerializeField] JumpStruct _jump;
    float _jumpTimer;
    [SerializeField] GroundedRay _ground;
    [SerializeField] bool _isFry;
    [SerializeField] Direction _dir;
    Transform _myTra;
    enum Direction
    {
        right, left
    }
    enum MoveType
    {
        Stay,
        Return,
        Jump
    }
    [System.Serializable]
    struct JumpStruct
    {
        public float _jumpCoolTime;
        public float _jumpPower;
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
    void Start()
    {
        _jumpTimer = Time.time;
        _myTra = transform;
        _dir = Direction.left;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        rb.isKinematic = _isFry;
        switch (_moveType)
        {
            case MoveType.Return:
                UpdateReturn();
                break;
            case MoveType.Jump:
                UpdateJump();
                break;
        }
    }
    void UpdateJump()
    {
        if (Time.time - _jumpTimer > _jump._jumpCoolTime)
            if (IsGrounded())
            {
                _jumpTimer = Time.time;
                Vector2 velo = rb.velocity;
                velo.y = _jump._jumpPower;
                rb.velocity = velo;
            }
    }
    void UpdateReturn()
    {
        IsGrounded(out bool isHitRGround, out bool isHitLGround);
        IsSideTouch(out bool isHitRSide, out bool isHitLSide);
        if (isHitRSide ^ isHitLSide)
        {
            _dir = isHitRSide ? Direction.left : _dir;
            _dir = isHitLSide ? Direction.right : _dir;
        }
        if (isHitRGround ^ isHitLGround)
        {
            _dir = isHitRGround ? Direction.right : _dir;
            _dir = isHitLGround ? Direction.left : _dir;
        }

        Vector2 velo = rb.velocity;
        velo.x = _speed * _dir switch { Direction.right => 1, Direction.left => -1, _ => 0 };
        rb.velocity = velo;


        bool IsSideTouch(out bool IsHitR, out bool IsHitL)
        {
            Vector2 rayPos = _myTra.position;
            bool isHitR = Physics2D.Raycast(rayPos, Vector2.right, _ground._sideRayLong, _ground._mask);
            bool isHitL = Physics2D.Raycast(rayPos, Vector2.left, _ground._sideRayLong, _ground._mask);
            Debug.DrawLine(rayPos, rayPos + Vector2.right * _ground._sideRayLong);
            Debug.DrawLine(rayPos, rayPos + Vector2.left * _ground._sideRayLong);

            IsHitR = isHitR;
            IsHitL = isHitL;
            return isHitR || isHitL;
        }
    }
    bool IsGrounded() => IsGrounded(out _, out _);
    bool IsGrounded(out bool IsHitR, out bool IsHitL)
    {
        Vector2 rRayPos = _myTra.position + (Vector3)_ground._rightRayPos;
        Vector2 lRayPos = _myTra.position + (Vector3)_ground._leftRayPos;
        bool isHitR = Physics2D.Raycast(rRayPos, Vector2.down, _ground._rayLong, _ground._mask);
        bool isHitL = Physics2D.Raycast(lRayPos, Vector2.down, _ground._rayLong, _ground._mask);
        Debug.DrawLine(rRayPos, rRayPos + Vector2.down * _ground._rayLong);
        Debug.DrawLine(lRayPos, lRayPos + Vector2.down * _ground._rayLong);

        IsHitR = isHitR;
        IsHitL = isHitL;
        return isHitR || isHitL;
    }
}
