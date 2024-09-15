using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�L������HP�B���ꂪ0�ɂȂ�ƃQ�[���I�[�o�[</summary>
    [Header("�̗͂̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̗̑͂̍ő�l")] int _maxHp;
    int _currentHp;
    /// <summary>�v���C���[�L�������U�����s�������ɁA�G�l�~�[�ɗ^����_���[�W�B</summary>
    [Header("�U����")]
    [SerializeField][Tooltip("�v���C���[�̍U����")] int _attack;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�����߂�l�B���l�������قǍő呬�x�������Ȃ�</summary>
    [Header("�ړ����x�̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̑��x�̍ő�l")] float _speed;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�̉����x�����߂�l�B���l�������قǍő呬�x�܂ł̉������Ԃ��Z��</summary>
    [Header("�ړ����x�̉����x")]
    [SerializeField][Tooltip("�v���C���[�̈ړ����x�̉����x")] float _movePower;
    /// <summary>�v���C���[�L�����N�^�[��Jump���ɏ�����Ɋ|����́B</summary>
    [Header("�W�����v��")]
    [SerializeField][Tooltip("�v���C���[�̃W�����v��")] float _jumpPower;
    /// <summary>�G�l�~�[����_���[�W���󂯂��ہA��莞�Ԃ̓_���[�W���󂯂Ȃ��悤�ɂ��鎞�ԁB�����l�œ��͂���B1=1�b</summary>
    [Header("���G����")]
    [SerializeField][Tooltip("�v���C���[�̖��G����")] int _damageCool;
    /// <summary>�ڒn����</summary>
   �@bool _isJump;
    /// <summary>�G�𓥂񂾔���</summary>
    bool _isStompEnemy;
    /// <summary>�����Ă���A�C�e���̃��X�g</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    Rigidbody2D _rb;
    float _jumpTimer = 0;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHp = _maxHp;
    }

    void Update()
    {
        Move();
        Jump();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isJump = false;
            _jumpTimer = 0;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _isStompEnemy = true;
            _rb.velocity = new Vector2(_rb.velocity.x,0);
            Debug.Log("����");
        }
    }
    private void Move()
    {
        //�ړ�����
        var x = Input.GetAxisRaw("Horizontal");
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
        if (!_isJump && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            _isJump = true;
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
        _itemList.Add(item);
    }
    /// <summary>
    /// �v���C���[�̗̑͂������ɓn�������l�����������܂��B
    /// </summary>
    /// <param name="value"></param>
    public void FluctuationLife(int value)
    {
        _currentHp += value;
        if (_currentHp > _maxHp)
        {
            _currentHp = _maxHp;
        }
        Debug.Log($"Player�̗̑�:{_currentHp}");
    }
}
