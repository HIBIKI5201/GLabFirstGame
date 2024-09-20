using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�L������HP�B���ꂪ0�ɂȂ�ƃQ�[���I�[�o�[</summary>
    [Header("�̗͂̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̗̑͂̍ő�l")] int _maxHp;
    int _currentHp;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�����߂�l�B���l�������قǍő呬�x�������Ȃ�</summary>
    [Header("�ړ����x�̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̑��x�̍ő�l")]�@public float _speed;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�̉����x�����߂�l�B���l�������قǍő呬�x�܂ł̉������Ԃ��Z��</summary>
    [Header("�ړ����x�̉����x")]
    [SerializeField][Tooltip("�v���C���[�̈ړ����x�̉����x")] public float _movePower;
    /// <summary>�v���C���[�L�����N�^�[��Jump���ɏ�����Ɋ|����́B</summary>
    [Header("�W�����v��")]
    [SerializeField][Tooltip("�v���C���[�̃W�����v��")] float _jumpPower;
    /// <summary>�G�l�~�[����_���[�W���󂯂��ہA��莞�Ԃ̓_���[�W���󂯂Ȃ��悤�ɂ��鎞�ԁB�����l�œ��͂���B1=1�b</summary>
    [Header("���G����")]
    [SerializeField][Tooltip("�v���C���[�̖��G����")] int _damageCool;
    /// <summary>�A�C�e�����܂����������鋭��</summary>
    [Header("�܂����������鋭��")]
    [SerializeField][Tooltip("�܂����������鋭��")] float _throwStraightPower = 5;
    /// <summary>�A�C�e��������I�ɓ����鋭��</summary>
    [Header("�����I�ɓ����鋭��")]
    [SerializeField][Tooltip("�����I�ɓ����鋭��")] float _throwParabolaPower = 5;
    /// <summary>�A�C�e��������I�ɓ��������</summary>
    [Header("�����I�ɓ��������")]
    [SerializeField][Tooltip("�����I�ɓ��������")] Vector2 _throwParabolaDirection = new Vector2(1, 1);
    [Header("�A�C�e���𓊂���ʒu")]
    [SerializeField][Tooltip("�A�C�e���𓊂���ʒu")] Vector2 _throwPos;
    /// <summary>�A�C�e���̐�</summary>
    [SerializeField, Header("���Ă�΂̍ő�l")] int _maxRockCount;
    int _currentRockCount;
    [SerializeField, Header("���Ă�󂫕r�̍ő�l")] int _maxBottleCount;
    int _currentBottleCount;
    [SerializeField, Header("���Ă���̍ő�l")] int _maxMeatCount;
    int _currentMeatCount;
    /// <summary>�����Ă���A�C�e���̃��X�g</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    List<Rock> _rockList = new List<Rock>();
    List<Bottle> _bottleList = new List<Bottle>();
    List<Meat> _meatList = new List<Meat>();
    /// <summary>�ڒn����</summary>
    bool _isJump;
    /// <summary>�G�𓥂񂾔���</summary>
    bool _isStompEnemy;
    /// <summary>���G���Ԓ����ǂ���</summary>
    bool _isInvincible;
    PlayerStatus _playerStatus = PlayerStatus.Normal;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    float _jumpTimer = 0;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHp = _maxHp;
    }

    void Update()
    {
        Move();
        Jump();
        ChangeItem();
        UseItem();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("�Ԃ�����");
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isJump = false;
            _jumpTimer = 0;
            Debug.Log("���n�I");
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _isStompEnemy = true;
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            Debug.Log("����");
        }
    }
    enum PlayerStatus
    {
        Rock,
        Bottle,
        Meat,
        Normal,
        Damage,
        Death
    }
    private void Move()
    {
        //�ړ�����
        var x = Input.GetAxisRaw("Horizontal");
        if (_isJump)
        {
            x /= 5;
        }
        else
        {
            if (x < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (x > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
        }
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isJump)
            {
                Debug.Log("�W�����v����");
                _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
                _isJump = true;
            }
            else
            {
                Debug.Log("�W�����v�ł��Ȃ�����");
            }
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
        //_itemList.Add(item);
        if (item.TryGetComponent(out Rock a))
        {
            if (_currentRockCount <= _maxRockCount)
            {
                _currentRockCount++;
                _rockList.Add(a);
            }
        }
        else if (item.TryGetComponent(out Bottle b))
        {
            if (_currentBottleCount <= _maxBottleCount)
            {
                _currentBottleCount++;
                _bottleList.Add(b);
            }
        }
        else if (item.TryGetComponent(out Meat c))
        {
            if (_currentMeatCount <= _maxMeatCount)
            {
                _currentMeatCount++;
                _meatList.Add(c);
            }
        }
    }
    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ItemBase item = null;//= _itemList[0];
            switch (_playerStatus)
            {
                case PlayerStatus.Rock:
                    item = _rockList[0];
                    _rockList.RemoveAt(0);
                    _currentRockCount--;
                    break;
                case PlayerStatus.Bottle:
                    item = _bottleList[0];
                    _bottleList.RemoveAt(0);
                    _currentBottleCount--;
                    break;
                case PlayerStatus.Meat:
                    item = _meatList[0];
                    _meatList.RemoveAt(0);
                    _currentMeatCount--;
                    break;
                case PlayerStatus.Normal:
                    return;
            }
            item.transform.position = transform.position + (Vector3)_throwPos;
            Debug.Log(item + "�𓊂���");
            //������A�C�e����Rigidbody�����ĂȂ����������
            if (!item.TryGetComponent(out Rigidbody2D rb))
            {
                rb = item.AddComponent<Rigidbody2D>();
            }
            item.gameObject.GetComponent<Collider2D>().isTrigger = false;
            item.Throwing();
            //�܂�����������
            if (item.Throw == ItemBase.ThrowType.Straight)
            {
                _throwStraightPower *= transform.localScale.x;
                rb.gravityScale = 0;
                rb.AddForce(new Vector2(_throwStraightPower, 0), ForceMode2D.Impulse);
            }
            //�����I�ɓ�����
            else
            {
                _throwParabolaPower *= transform.localScale.x;
                rb.AddForce(_throwParabolaDirection.normalized * _throwParabolaPower, ForceMode2D.Impulse);
            }
            //Destroy(item.gameObject);
        }
    }
    void ChangeItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_currentRockCount > 0)
            {
                _playerStatus = PlayerStatus.Rock;
                Debug.Log("�΂��g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_currentBottleCount > 0)
            {
                _playerStatus = PlayerStatus.Bottle;
                Debug.Log("�r���g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_currentMeatCount > 0)
            {
                _playerStatus = PlayerStatus.Meat;
                Debug.Log("�����g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _playerStatus = PlayerStatus.Normal;
            Debug.Log("�A�C�e���������Ȃ�");
        }
    }

    /// <summary>
    /// �v���C���[�̗̑͂������ɓn�������l�����������܂��B
    /// </summary>
    /// <param name="value"></param>
    public void FluctuationLife(int value)
    {
        if (value < 0)
        {
            if (!_isInvincible)
            {
                _currentHp += value;
            }
            if (_currentHp <= 0)
            {
                _playerStatus = PlayerStatus.Death;
            }
            else
            {
                //�_���[�W���󂯂����̏����A��U�ۗ�
                //DOTween.To(() => new Color(), s => )
            }
        }
        else
        {
            _currentHp += value;
        }
        if (_currentHp > _maxHp)
        {
            _currentHp = _maxHp;
        }
        Debug.Log($"Player�̗̑�:{_currentHp}");
    }
    /// <summary>
    /// �v���C���[�̑��x�𒲐�����
    /// </summary>
    /// <param name="multi">�{��</param>
    /// <param name="slowtime">�p������</param>
    public void Slow(float multi, float slowtime)
    {
        StartCoroutine(Slowing(multi, slowtime));
    }
    IEnumerator Slowing(float multi, float slowtime)
    {
        float defaultMovePower = _movePower;
        float defaultMaxSpeed = _speed;
        _movePower *= multi;
        _speed *= multi;
        yield return new WaitForSeconds(slowtime);
        _movePower = defaultMovePower;
        _speed = defaultMaxSpeed;
    }
}
