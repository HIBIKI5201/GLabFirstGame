using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using static UnityEditor.Progress;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�L������HP�B���ꂪ0�ɂȂ�ƃQ�[���I�[�o�[</summary>
    [Header("�̗͂̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̗̑͂̍ő�l")] int _maxHp;
    int _currentHp;
    /// <summary>�v���C���[�L�����N�^�[�̈ړ����x�����߂�l�B���l�������قǍő呬�x�������Ȃ�</summary>
    [Header("�ړ����x�̍ő�l")]
    [SerializeField][Tooltip("�v���C���[�̑��x�̍ő�l")] public float _speed;
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
    [SerializeField][Tooltip("�����I�ɓ����鋭��")] float _maxThrowParabolaPower;
    float _throwParabolaPower = 0;
    /// <summary>�A�C�e��������I�ɓ��������</summary>
    [Header("�����I�ɓ��������")]
    [SerializeField][Tooltip("�����I�ɓ��������")] Vector2 _throwParabolaDirection = new Vector2(1, 1);
    [Header("�A�C�e���𓊂���ʒu")]
    [SerializeField][Tooltip("�A�C�e���𓊂���ʒu")] Vector2 _throwPos;
    /// <summary>�A�C�e���̐�</summary>
    [SerializeField, Header("���Ă�΂̍ő�l")] int _maxRockCount;
    [SerializeField, Header("���Ă�󂫕r�̍ő�l")] int _maxBottleCount;
    [SerializeField, Header("���Ă���̍ő�l")] int _maxMeatCount;
    [SerializeField, Header("�e���\����")] LineRenderer _line;
    [SerializeField] int _simulateFrame;
    /// <summary>�����Ă���A�C�e���̃��X�g</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    /// <summary>�ڒn����</summary>
    bool _isJump;
    /// <summary>�G�𓥂񂾔���</summary>
    bool _isStompEnemy;
    /// <summary>���G���Ԓ����ǂ���</summary>
    bool _isInvincible;
    PlayerStatus _playerStatus = PlayerStatus.Normal;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Scene m_simulationScene;
    PhysicsScene2D m_physicsScene;
    float _jumpTimer = 0;
    void Start()
    {
        CreatePhysicsScene();
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
    /// <summary>
    /// �v���C���[���A�C�e������肵���Ƃ��ɌĂԃ��\�b�h
    /// </summary>
    /// <param name="item"></param>
    public void GetItem(ItemBase item)
    {

        //_itemList.Add(item);
        if (item as Rock)
        {
            if (_itemList.Where(i => i as Rock).ToList().Count < _maxRockCount)
            {
                _itemList.Add(item);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Bottle)
        {
            if (_itemList.Where(i => i as Bottle).ToList().Count < _maxBottleCount)
            {
                _itemList.Add(item);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Meat)
        {
            if (_itemList.Where(i => i as Meat).ToList().Count < _maxMeatCount)
            {
                _itemList.Add(item);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
    }
    /// <summary>
    /// Enter�������ꂽ���A�C�e���𓊂��邽�߂̃R���[�`�����X�^�[�g���܂�
    /// </summary>
    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(ThrowItem());
        }
    }
    /// <summary>
    /// �v���C���[�������Ă���A�C�e���ɉ�����_itemList����A�C�e��������Ă��܂��B
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Item(out ItemBase item)
    {
        switch (_playerStatus)
        {
            case PlayerStatus.Rock:
                item = _itemList.Where(i => i as Rock).ToList().First();
                return true;
            case PlayerStatus.Bottle:
                item = _itemList.Where(i => i as Bottle).ToList().First();
                return true;
            case PlayerStatus.Meat:
                item = _itemList.Where(i => i as Meat).ToList().First();
                return true;
            default:
                item = null;
                return false;
        }
    }
    void ChangeItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_itemList.Any(i => i as Rock))
            {
                _playerStatus = PlayerStatus.Rock;
                Debug.Log("�΂��g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_itemList.Any(i => i as Bottle))
            {
                _playerStatus = PlayerStatus.Bottle;
                Debug.Log("�r���g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_itemList.Any(i => i as Meat))
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
    void CreatePhysicsScene()
    {
        m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        m_physicsScene = m_simulationScene.GetPhysicsScene2D();
    }
    void ThrowLineSimulate(GameObject _ballPrefab, Vector2 _pos, Vector2 _velocity)
    {
        var ghost = Instantiate(_ballPrefab, _pos, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);
        ghost.GetComponent<Rigidbody2D>().AddForce(_velocity, ForceMode2D.Impulse);

        _line.positionCount = _simulateFrame;

        for (int i = 0; i < _simulateFrame; i++)
        {
            m_physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghost.transform.position);
        }
        Destroy(ghost.gameObject);
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
    IEnumerator ThrowItem()
    {
        if (!Item(out ItemBase item)) yield break;
        //������A�C�e����Rigidbody�����ĂȂ����������
        if (!item.TryGetComponent(out Rigidbody2D rb))
        {
            rb = item.AddComponent<Rigidbody2D>();
        }
        item.gameObject.GetComponent<Collider2D>().isTrigger = false;
        //�܂�����������
        if (item.Throw == ItemBase.ThrowType.Straight)
        {
            //�A�C�e�����v���C���[�̈ʒu�Ɏ����Ă���
            item.transform.position = transform.position + (Vector3)_throwPos;
            rb.gravityScale = 0;
            _throwStraightPower *= transform.localScale.x;
            rb.AddForce(new Vector2(_throwStraightPower, 0), ForceMode2D.Impulse);
        }
        //�����I�ɓ�����
        else
        {
            while (Input.GetKey(KeyCode.Return))
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    GetItem(item);
                    yield break;
                }
                if (_throwParabolaPower < _maxThrowParabolaPower)
                {
                    _throwParabolaPower += 0.1f;
                }
                ThrowLineSimulate(item.gameObject, transform.position, _throwParabolaDirection.normalized * _throwParabolaPower);
                yield return new WaitForEndOfFrame();
            }
            _line.positionCount = 0;
            //�A�C�e�����v���C���[�̈ʒu�Ɏ����Ă���
            item.transform.position = transform.position + (Vector3)_throwPos;
            rb.velocity = Vector3.zero;
            _throwParabolaPower *= transform.localScale.x;
            rb.AddForce(_throwParabolaDirection.normalized * _throwParabolaPower, ForceMode2D.Impulse);
            _throwParabolaPower = 0;
        }
        item.Throwing();
        if(item as Rock)
        {
            _itemList.Remove((Rock)item);

        }
        else if(item as Bottle)
        {
            _itemList.Remove((Bottle)item);

        }
        else
        {
            _itemList.Remove((Meat)item);

        }
    }
}