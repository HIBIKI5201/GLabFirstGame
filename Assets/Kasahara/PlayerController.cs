using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("�v���C���[�̗̑͂̍ő�l")] int _maxHp;
    int _currentHp;
    [SerializeField, Tooltip("�̗͂̃o���̉Ԃт�")] List<GameObject> _rose = new List<GameObject>();
    [SerializeField, Tooltip("�v���C���[�̑��x�̍ő�l")] public float _speed;
    [SerializeField, Tooltip("�v���C���[�̈ړ����x�̉����x")] public float _movePower;
    [SerializeField, Tooltip("�v���C���[�̃W�����v��")] float _jumpPower;
    [SerializeField, Tooltip("�������x")] float _fallSpeed;
    [SerializeField, Tooltip("�v���C���[�̖��G����")] int _damageCool;
    [SerializeField, Tooltip("�ڒn����̈ʒu")] Vector2 _point;
    [SerializeField, Tooltip("�ڒn����̑傫��")] Vector2 _size;
    [SerializeField, Tooltip("�ڒn����̊p�x")] float _angle;
    [SerializeField, Tooltip("<�A�C�e���𓊂���ݒ�>")] Throwsetting _throwsetting;
    [SerializeField, Tooltip("<�A�C�e���̐ݒ�>")] ItemSetting _itemSetting;
    [System.Serializable]
    struct Throwsetting
    {
        [Tooltip("�܂����������鋭��")] public float _throwStraightPower;
        [Tooltip("�����I�ɓ����鋭��")] public float _maxThrowParabolaPower;
        [Tooltip("�����I�ɓ��������")] public Vector2 _throwParabolaDirection;
        [Tooltip("�A�C�e���𓊂���ʒu")] public Vector2 _throwPos;
        [Tooltip("�e���\����")] public LineRenderer _line;
        public int _simulateFrame;
    }
    /// <summary>�A�C�e���̐ݒ�</summary>
    [System.Serializable]
    struct ItemSetting
    {
        [Tooltip("���Ă�΂̍ő�l")] public int _maxRockCount;
        [Tooltip("���Ă�󂫕r�̍ő�l")] public int _maxBottleCount;
        [Tooltip("���Ă���̍ő�l")] public int _maxMeatCount;
        [Tooltip("�΂�UI")] public GameObject _rockUi;
        [Tooltip("�󂫕r��UI")] public GameObject _bottleUi;
        [Tooltip("����UI")] public GameObject _meatUi;
        [Tooltip("�΂̌���\������e�L�X�g")] public Text _rockCountText;
        [Tooltip("�󂫕r�̌���\������e�L�X�g")] public Text _bottleCountText;
        [Tooltip("���̌���\������e�L�X�g")] public Text _meatCountText;
        [Tooltip("�A�C�e���������Ă��Ȃ��Ƃ��̐F")] public Color _zeroItemColor;
        [Tooltip("�΂ɑΉ�����t����")] public GameObject _leafRock;
        [Tooltip("�󂫕r�ɑΉ�����t����")] public GameObject _leafBottle;
        [Tooltip("���ɑΉ�����t����")] public GameObject _leafMeat;
        [Tooltip("�t���ς̊g�嗦")] public float _leafSize;
    }
    /// <summary>�����Ă���A�C�e���̃��X�g</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    /// <summary>�ڒn����</summary>
    bool _isJump;
    /// <summary>�G�𓥂񂾔���</summary>
    bool _isStompEnemy;
    /// <summary>���G���Ԓ����ǂ���</summary>
    bool _isInvincible;
    bool _canAction = true;
    PlayerStatus _playerStatus = PlayerStatus.Normal;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Scene m_simulationScene;
    PhysicsScene2D m_physicsScene;
    float _throwParabolaPower = 0;
    GameObject[] _itemPos = new GameObject[3];
    Vector3[] _afterItemPos0 = new Vector3[3];
    Vector3[] _afterItemPos1 = new Vector3[3];
    Vector3[] _afterItemPos2 = new Vector3[3];
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireCube((Vector2)transform.position + _point, _size);
    }
    void Start()
    {
        CreatePhysicsScene();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHp = _maxHp;
        _itemPos = new GameObject[] { _itemSetting._rockUi, _itemSetting._bottleUi, _itemSetting._meatUi };
        _afterItemPos0 = new Vector3[] { _itemSetting._meatUi.transform.position, _itemSetting._rockUi.transform.position, _itemSetting._bottleUi.transform.position };
        _afterItemPos1 = new Vector3[] { _itemSetting._rockUi.transform.position, _itemSetting._bottleUi.transform.position, _itemSetting._meatUi.transform.position };
        _afterItemPos2 = new Vector3[] { _itemSetting._bottleUi.transform.position, _itemSetting._meatUi.transform.position, _itemSetting._rockUi.transform.position };

    }

    void Update()
    {
        if (_canAction)
        {
            Move();
            Jump();
            ChangeItem();
            UseItem();
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
        if (Mathf.Abs(_rb.velocity.x) > _speed)
        {
            _rb.velocity = new Vector2(_speed * Mathf.Sign(_rb.velocity.x), _rb.velocity.y);
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
                StartCoroutine(GroundingJudge());
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
                StartCoroutine(GroundingJudge());
            }
        }
        else if (_isStompEnemy)
        {
            Debug.Log("�G�𓥂�ŏ��W�����v");
            _rb.AddForce(new Vector2(0, _jumpPower / 1.5f), ForceMode2D.Impulse);
            _isStompEnemy = false;
            StartCoroutine(GroundingJudge());
        }
        else if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.99f);
        }
        //Debug.Log(_isJump);
    }
    /// <summary>
    /// ���n�����m����
    /// </summary>
    /// <returns></returns>
    IEnumerator GroundingJudge()
    {
        while (_rb.velocity.y > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        _rb.gravityScale = _fallSpeed;
        while (_isJump)
        {
            yield return new WaitForEndOfFrame();
            var hit = Physics2D.OverlapBoxAll((Vector2)transform.position + _point, _size, _angle);
            foreach (var obj in hit)
            {
                Debug.Log(obj);
                if (obj.gameObject.CompareTag("Ground"))
                {
                    _isJump = false;
                    _rb.gravityScale = 1;
                }
                else if (obj.gameObject.CompareTag("Enemy"))
                {
                    _isStompEnemy = true;
                    _rb.velocity = new Vector2(_rb.velocity.x, 0);
                    _rb.gravityScale = 1;
                    yield break;
                }
            }
        }
    }
    /// <summary>
    /// �v���C���[���A�C�e������肵���Ƃ��ɌĂԃ��\�b�h
    /// </summary>
    /// <param name="item"></param>
    public void GetItem(ItemBase item)
    {
        if (item as Rock)
        {
            if (_itemList.Where(i => i as Rock).ToList().Count < _itemSetting._maxRockCount)
            {
                _itemList.Add(item);
                _itemSetting._rockCountText.text = _itemList.Where(i => i as Rock).Count().ToString();
                _itemSetting._rockUi.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Bottle)
        {
            if (_itemList.Where(i => i as Bottle).ToList().Count < _itemSetting._maxBottleCount)
            {
                _itemList.Add(item);
                _itemSetting._bottleCountText.text = _itemList.Where(i => i as Bottle).Count().ToString();
                _itemSetting._bottleUi.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Meat)
        {
            if (_itemList.Where(i => i as Meat).ToList().Count < _itemSetting._maxMeatCount)
            {
                _itemList.Add(item);
                _itemSetting._meatCountText.text = _itemList.Where(i => i as Meat).Count().ToString();
                _itemSetting._meatUi.GetComponent<Image>().color = new Color(255, 255, 255, 255);
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
            if (_itemList.Any(i => i as Rock) && _playerStatus != PlayerStatus.Rock)
            {
                _playerStatus = PlayerStatus.Rock;
                _itemSetting._leafRock.transform.localScale *= _itemSetting._leafSize;
                _itemSetting._leafBottle.transform.localScale = Vector3.one;
                _itemSetting._leafMeat.transform.localScale = Vector3.one;
                //for (int i = 0; i < _itemPos.Length; i++)
                //{
                //    _itemPos[i].transform.position = _afterItemPos2[i];
                //}
                Debug.Log("�΂��g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_itemList.Any(i => i as Bottle) && _playerStatus != PlayerStatus.Bottle)
            {
                _playerStatus = PlayerStatus.Bottle;
                _itemSetting._leafRock.transform.localScale = Vector3.one;
                _itemSetting._leafBottle.transform.localScale *= _itemSetting._leafSize;
                _itemSetting._leafMeat.transform.localScale = Vector3.one;
                //for (int i = 0; i < _itemPos.Length; i++)
                //{
                //    _itemPos[i].transform.position = _afterItemPos1[i];
                //}
                Debug.Log("�r���g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_itemList.Any(i => i as Meat) && _playerStatus != PlayerStatus.Meat)
            {
                _playerStatus = PlayerStatus.Meat;
                _itemSetting._leafRock.transform.localScale = Vector3.one;
                _itemSetting._leafBottle.transform.localScale = Vector3.one;
                _itemSetting._leafMeat.transform.localScale *= _itemSetting._leafSize;
                //for (int i = 0; i < _itemPos.Length; i++)
                //{
                //    _itemPos[i].transform.position = _afterItemPos0[i];
                //}
                Debug.Log("�����g��");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _playerStatus = PlayerStatus.Normal;
            _itemSetting._leafRock.transform.localScale = Vector3.one;
            _itemSetting._leafBottle.transform.localScale = Vector3.one;
            _itemSetting._leafMeat.transform.localScale = Vector3.one;
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

        _throwsetting._line.positionCount = _throwsetting._simulateFrame;

        for (int i = 0; i < _throwsetting._simulateFrame; i++)
        {
            m_physicsScene.Simulate(Time.fixedDeltaTime);
            _throwsetting._line.SetPosition(i, ghost.transform.position);
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
                for (int i = 0; i < Mathf.Abs(value) && _rose.Count > 0; i++)
                {
                    Destroy(_rose[0]);
                    _rose.RemoveAt(0);
                }
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
    /// <summary>
    /// �v���C���[���s���ł��Ȃ��Ȃ鏈��
    /// </summary>
    /// <param name="time"></param>
    public void StopAction(float time)
    {
        StartCoroutine(StoppingAction(time));
    }
    IEnumerator StoppingAction(float time)
    {
        _canAction = false;
        yield return new WaitForSeconds(time);
        _canAction = true;
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
            item.transform.position = transform.position + (Vector3)_throwsetting._throwPos;
            rb.gravityScale = 0;
            _throwsetting._throwStraightPower *= transform.localScale.x;
            rb.AddForce(new Vector2(_throwsetting._throwStraightPower, 0), ForceMode2D.Impulse);
        }
        //�����I�ɓ�����
        else
        {
            while (Input.GetKey(KeyCode.Return))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    yield break;
                }
                if (_throwParabolaPower < _throwsetting._maxThrowParabolaPower)
                {
                    _throwParabolaPower += 0.1f;
                }
                ThrowLineSimulate(item.gameObject, transform.position, _throwsetting._throwParabolaDirection.normalized * _throwParabolaPower * transform.localScale);
                yield return new WaitForEndOfFrame();
            }
            _throwsetting._line.positionCount = 0;
            //�A�C�e�����v���C���[�̈ʒu�Ɏ����Ă���
            item.transform.position = transform.position + (Vector3)_throwsetting._throwPos;
            rb.velocity = Vector3.zero;
            rb.AddForce(_throwsetting._throwParabolaDirection.normalized * _throwParabolaPower * transform.localScale, ForceMode2D.Impulse);
            _throwParabolaPower = 0;
        }
        item.Throwing();
        if (item as Rock)
        {
            _itemList.Remove((Rock)item);
            _itemSetting._rockCountText.text = _itemList.Where(i => i as Rock).Count().ToString();
            if (_itemSetting._rockCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
                _itemSetting._rockUi.GetComponent<Image>().color = _itemSetting._zeroItemColor;
                _itemSetting._leafRock.transform.localScale = Vector3.one;
            }
        }
        else if (item as Bottle)
        {
            _itemList.Remove((Bottle)item);
            _itemSetting._bottleCountText.text = _itemList.Where(i => i as Bottle).Count().ToString();
            if (_itemSetting._bottleCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
                _itemSetting._bottleUi.GetComponent<Image>().color = _itemSetting._zeroItemColor;
                _itemSetting._leafBottle.transform.localScale = Vector3.one;
            }
        }
        else
        {
            _itemList.Remove((Meat)item);
            _itemSetting._meatCountText.text = _itemList.Where(i => i as Meat).Count().ToString();
            if (_itemSetting._meatCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
                _itemSetting._meatUi.GetComponent<Image>().color = _itemSetting._zeroItemColor;
                _itemSetting._leafMeat.transform.localScale = Vector3.one;
            }
        }
    }
}