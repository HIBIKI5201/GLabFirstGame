using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�L������HP�B���ꂪ0�ɂȂ�ƃQ�[���I�[�o�[</summary>
    [Header("�̗͂̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̗̑͂̍ő�l")] int _maxHP;
    int _currentHP;
    /// <summary>�v���C���[�L�������U�����s�������ɁA�G�l�~�[�ɗ^����_���[�W�B</summary>
    [Header("�U����")]
    [SerializeField][Tooltip("�v���C���[�̍U����")] int _attack;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�����߂�l�B���l�������قǍő呬�x�������Ȃ�</summary>
    [Header("�ړ����x�̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̑��x�̍ő�l")] float _speed;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�̉����x�����߂�l�B���l�������قǍő呬�x�܂ł̉������Ԃ��Z��</summary>
    [Header("�ړ����x�̉����x")]
    [SerializeField][Tooltip("�v���C���[�̈ړ����x�̉����x")] float _movePower;
    /// <summary>�v���C���[�L�����N�^�[��Jump���ɏ�����Ɋ|����́B���l�������قǁA�����W�����v���s����</summary>
    [Header("�W�����v��")]
    [SerializeField][Tooltip("�v���C���[�̃W�����v��")] float _jumpPower;
    /// <summary>�G�l�~�[����_���[�W���󂯂��ہA��莞�Ԃ̓_���[�W���󂯂Ȃ��悤�ɂ��鎞�ԁB�����l�œ��͂���B1=1�b</summary>
    [Header("���G����")]
    [SerializeField][Tooltip("�v���C���[�̖��G����")] int _damageCool;
    /// <summary>�ڒn����</summary>
   �@bool _isGround;
    /// <summary>�����Ă���A�C�e���̃��X�g</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        Move(new Vector2(x, 0));
        Jump();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGround = true;
        }
    }
    private void Move(Vector2 horiMove)
    {
        //�ړ�����
        var x = Input.GetAxisRaw("Horizontal");
        _rb.AddForce(horiMove * _movePower, ForceMode2D.Force);
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
        if (_isGround && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            _isGround = false;
        }
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
        _currentHP += value;
        if(_currentHP > _maxHP)
        {
            _currentHP = _maxHP;
        }
    }
}
