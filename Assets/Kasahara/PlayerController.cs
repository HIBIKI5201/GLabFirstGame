using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�L������HP�B���ꂪ0�ɂȂ�ƃQ�[���I�[�o�[</summary>
    [SerializeField] int _maxHP;
    /// <summary>�v���C���[�L�������U�����s�������ɁA�G�l�~�[�ɗ^����_���[�W�B</summary>
    [SerializeField] int _attack;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�����߂�l�B���l�������قǍő呬�x�������Ȃ�</summary>
    [SerializeField] float _speed;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�̉����x�����߂�l�B���l�������قǍő呬�x�܂ł̉������Ԃ��Z��</summary>
    [SerializeField] float _movePower;
    /// <summary>�v���C���[�L�����N�^�[��Jump���ɏ�����Ɋ|����́B���l�������قǁA�����W�����v���s����</summary>
    [SerializeField] float _jumpPower;
    /// <summary>�G�l�~�[����_���[�W���󂯂��ہA��莞�Ԃ̓_���[�W���󂯂Ȃ��悤�ɂ��鎞�ԁB�����l�œ��͂���B1=1�b</summary>
    [SerializeField] int _damageCool;
    Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        _rb.AddForce(new Vector2(x * _movePower, 0), ForceMode2D.Force);
        if (_rb.velocity.x > _speed)
        {
            _rb.velocity = new Vector2(_speed, _rb.velocity.y);
        }
        else if (_rb.velocity.x < - _speed)
        {
            _rb.velocity = new Vector2(- _speed, _rb.velocity.y);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
        }
    }
}
