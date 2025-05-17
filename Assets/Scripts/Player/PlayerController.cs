using System;
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
    [SerializeField] int _maxHp;
    public int CurrentHp { get; private set; }
    [SerializeField] public List<GameObject> _rose = new List<GameObject>();
    [SerializeField] public float _maxSpeed;
    [SerializeField] public float _movePower;
    [SerializeField] public float _deceleration;
    [SerializeField] bool _landingInertia;
    [SerializeField] float _jumpPower;
    [SerializeField] float _fallSpeed;
    [SerializeField] int _damageCool;
    [SerializeField] Vector2 _point;
    [SerializeField] Vector2 _size;
    [SerializeField] float _angle;
    [SerializeField] AudioClip _dash;
    [SerializeField] AudioClip _walk;
    [SerializeField] Throwsetting _throwsetting;
    [SerializeField] ItemSetting _itemSetting;
    
    [System.Serializable]
    struct Throwsetting
    {
        public float ThrowStraightPower;
        public float MaxThrowParabolaPower;
        public float ThrowRate;
        public Vector2 ThrowParabolaDirection;
        public Vector2 ThrowPos;
        public LineRenderer BulletSimulationLine;
        public int SimulateFrame;
        public GameObject Platform;
    }
    /// <summary>�A�C�e���̐ݒ�</summary>
    [System.Serializable]
    struct ItemSetting
    {
        public int MaxRockCount;
        public int MaxBottleCount;
        public int MaxMeatCount;
        public GameObject RockUi;
        public GameObject BottleUi;
        public GameObject MeatUi;
        public Text RockCountText;
        public Text BottleCountText;
        public Text MeatCountText;
        public Color ZeroItemColor;
        public GameObject LeafRock;
        public GameObject LeafBottle;
        public GameObject LeafMeat;
        public float LeafSize;
    }
    
    List<ItemBase> _itemList = new List<ItemBase>();
    [SerializeField] bool _isJump;
    [SerializeField] bool _isStompEnemy;
    bool _isInvincible;
    [SerializeField] bool _canAction = true;
    [HideInInspector] public PlayerStatus _playerStatus = PlayerStatus.Normal;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Scene m_simulationScene;
    PhysicsScene2D m_physicsScene;
    GameObject[] _itemPos = new GameObject[3];
    Vector3[] _afterItemPos0 = new Vector3[3];
    Vector3[] _afterItemPos1 = new Vector3[3];
    Vector3[] _afterItemPos2 = new Vector3[3];
    float _horiInput = 0;
    CameraShakeController _cameraShakeController;
    DamageEffect _damageEffect;
    PauseManager _pauseManager;
    AudioManager _audioManager;
    AudioSource _audioSource;
    Animator _animator;
    
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _audioManager = FindAnyObjectByType<AudioManager>();
        _pauseManager = FindAnyObjectByType<PauseManager>();
        _damageEffect = GetComponent<DamageEffect>();
        if (_pauseManager != null)
        {
            _pauseManager.OnPauseResume += PauseAction;
        }
        else
        {
            Debug.LogError("PauseManagerが取得できませんでした");
        }
           
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animatorが取得できませんでした");
            }
        }
    }
    void Start()
    {
        CurrentHp = _maxHp;
        _rb = GetComponent<Rigidbody2D>();
        CreatePhysicsScene();
        GameObject platform;
        if (_throwsetting.Platform != null)
        {
            platform = Instantiate(_throwsetting.Platform);
            Array.ForEach(platform.GetComponentsInChildren<Renderer>(), x => x.enabled = false);
            SceneManager.MoveGameObjectToScene(platform, m_simulationScene);
        }
        _cameraShakeController = FindAnyObjectByType<CameraShakeController>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSourceが設定されていません");
        }
        else
        {
            Debug.Log(_audioSource);
        }
        if (_cameraShakeController == null)
        {
            Debug.LogError("CameraShakeControllerが設定されていません");
        }
        _itemSetting.RockUi.GetComponent<Image>().color = _itemSetting.ZeroItemColor;
        _itemSetting.BottleUi.GetComponent<Image>().color = _itemSetting.ZeroItemColor;
        _itemSetting.MeatUi.GetComponent<Image>().color = _itemSetting.ZeroItemColor;
        _itemPos = new GameObject[] { _itemSetting.RockUi, _itemSetting.BottleUi, _itemSetting.MeatUi };
        //_afterItemPos0 = new Vector3[] { _itemSetting._meatUi.transform.position, _itemSetting._rockUi.transform.position, _itemSetting._bottleUi.transform.position };
        //_afterItemPos1 = new Vector3[] { _itemSetting._rockUi.transform.position, _itemSetting._bottleUi.transform.position, _itemSetting._meatUi.transform.position };
        //_afterItemPos2 = new Vector3[] { _itemSetting._bottleUi.transform.position, _itemSetting._meatUi.transform.position, _itemSetting._rockUi.transform.position };
    }

    void Update()
    {
        if (_canAction && _playerStatus != PlayerStatus.Death)
        {
            Move();
            //NewMove();
            Jump();
            ChangeItem();
            UseItem();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "goal")
        {
            _isInvincible = true;
        }
    }
    public enum PlayerStatus
    {
        Rock,
        Bottle,
        Meat,
        Normal,
        Damage,
        Death
    }
    private void PauseAction(bool isPause)
    {
        if (isPause)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    Vector2 _pauseVelocity;
    void Pause()
    {
        Debug.Log("Pause");
        _canAction = false;
        //_pauseVelocity = _rb.velocity;
        //_rb.Sleep();
    }
    void Resume()
    {
        Debug.Log("Resume");
        //_rb.WakeUp();
        _canAction = true;
        //_rb.velocity = _pauseVelocity;
    }
    float _veloX = 0;
    float _acce = 1;
    void NewMove()//Made in Oikawa
    {
        _horiInput = Input.GetAxisRaw("Horizontal");
        float x = _horiInput * Mathf.Round(Mathf.Abs(_horiInput));
        float times = _isJump ? 0.2f : 1;
        switch (x)
        {
            case 1:
                _veloX = Mathf.Lerp(_veloX, _maxSpeed, Time.deltaTime * _acce * times);
                break;
            case 0:
                _veloX = Mathf.Lerp(_veloX, 0, Time.deltaTime * _acce * times);
                break;
            case -1:
                _veloX = Mathf.Lerp(_veloX, -_maxSpeed, Time.deltaTime * _acce * times);
                break;
        }

        Vector2 vector2 = _rb.velocity;
        vector2.x = _veloX;
        _rb.velocity = vector2;

        if (!_isJump)
            if (x < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (x > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
    }
    private void Move()
    {
        _horiInput = Input.GetAxisRaw("Horizontal");
        if (_horiInput == 0)
        {
            if (!_isJump)
            {
                float x = 0;
                if (_rb.velocity.x != 0)
                {
                    x = _rb.velocity.x - (_deceleration + Mathf.Abs(_rb.velocity.x)) * Mathf.Sign(_rb.velocity.x) * Time.deltaTime;
                }
                if (Mathf.Abs(x) < 0.2)
                {
                    x = 0;
                    _audioSource.Stop();
                }
                else if (_walk != null && _audioSource.clip != _walk)
                {
                    _audioSource.clip = _walk;
                    _audioSource.Play();
                }
                _rb.velocity = new Vector2(x, _rb.velocity.y);
                _animator.SetFloat("isWalk", Mathf.Abs(x));
            }
        }
        else
        {
            float x = _rb.velocity.x + _movePower * _horiInput * Time.deltaTime;
            if (Mathf.Abs(x) > _maxSpeed)
            {
                x = _maxSpeed * Mathf.Sign(x);
                if (!_isJump && _dash != null && _audioSource.clip != _dash)
                {
                    _audioSource.clip = _dash;
                    _audioSource.Play();
                }
            }
            else
            {
                if (!_isJump)
                {
                    if (_walk != null && _audioSource.clip != _walk)
                    {
                        _audioSource.clip = _walk;
                        _audioSource.Play();
                    }
                    if (_animator != null)
                    {
                        _animator.SetFloat("isWalk", Mathf.Abs(x));
                    }
                }
            }
            _rb.velocity = new Vector2(x, _rb.velocity.y);
        }
        if (_isJump)
        {
            _horiInput /= 5;
        }
        else
        {
            if (_horiInput < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (_horiInput > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
        }
    }
    IEnumerator _jumpEnumerator;
    private void Jump()
    {
        if (_rb.velocity.y < -1f)
        {
            if (_jumpEnumerator == null)
            {
                _isJump = true;
                _jumpEnumerator = GroundingJudge(_jumpEnumerator);
                //Debug.Log("StartCoroutine");
                StartCoroutine(_jumpEnumerator);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isJump)
            {
                //Debug.Log("�W�����v����");
                AudioManager.Instance.PlaySE("jump");
                _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
                _isJump = true;
                _jumpEnumerator = GroundingJudge(_jumpEnumerator);
                //Debug.Log("StartCoroutine");
                StartCoroutine(_jumpEnumerator);
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (_isStompEnemy)
            {
                _isStompEnemy = false;
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
                _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            }
        }
        else if (_isStompEnemy)
        {
            //Debug.Log("�G�𓥂�ŏ��W�����v");
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(new Vector2(0, _jumpPower / 1.5f), ForceMode2D.Impulse);
            _isStompEnemy = false;
        }
        else if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.99f);
        }
        //Debug.Log(_isJump);
    }
    
    IEnumerator GroundingJudge(IEnumerator enumerator)
    {
        if (_rb.velocity.y > 0)
        {
            _animator.SetBool("isJump", true);
        }
        _audioSource.Stop();
        while (_rb.velocity.y > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        _animator.SetBool("isFall", true);
        _rb.gravityScale = _fallSpeed;
        while (_isJump)
        {
            yield return new WaitForEndOfFrame();
            var hit = Physics2D.OverlapBoxAll((Vector2)transform.position + _point, _size, _angle);
            foreach (var obj in hit)
            {
                if (obj.gameObject.CompareTag("Ground"))
                {
                    _isJump = false;
                    _rb.gravityScale = 1;
                    AudioManager.Instance.PlaySE("jump_landing");
                    _animator.SetBool("isJump", false);
                    _animator.SetBool("isFall", false);
                    if (_landingInertia && _horiInput == 0)
                    {
                        _rb.velocity = new Vector2(0, _rb.velocity.y);
                    }
                    yield return new WaitForSeconds(0.5f);
                    _jumpEnumerator = null;
                    yield break;
                }
                else if (obj.gameObject.CompareTag("Enemy"))
                {
                    if (obj.gameObject.GetComponent<Enemy>().State != EnemyStateType.Faint)
                    {
                        FluctuationLife(-1);
                    }
                    _isStompEnemy = true;
                    _rb.gravityScale = 1;
                }
            }
        }
        _jumpEnumerator = null;
    }
    
    public void GetItem(ItemBase item)
    {
        if (item as Rock)
        {
            if (_itemList.Where(i => i as Rock).ToList().Count < _itemSetting.MaxRockCount)
            {
                _itemList.Add(item);
                _itemSetting.RockCountText.text = _itemList.Where(i => i as Rock).Count().ToString();
                _itemSetting.RockUi.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Bottle)
        {
            if (_itemList.Where(i => i as Bottle).ToList().Count < _itemSetting.MaxBottleCount)
            {
                _itemList.Add(item);
                _itemSetting.BottleCountText.text = _itemList.Where(i => i as Bottle).Count().ToString();
                _itemSetting.BottleUi.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else if (item as Meat)
        {
            if (_itemList.Where(i => i as Meat).ToList().Count < _itemSetting.MaxMeatCount)
            {
                _itemList.Add(item);
                _itemSetting.MeatCountText.text = _itemList.Where(i => i as Meat).Count().ToString();
                _itemSetting.MeatUi.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
    }
    
    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _pauseManager.BeginCoroutine(ThrowItem());
        }
    }
    
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
                _itemSetting.LeafRock.transform.localScale *= _itemSetting.LeafSize;
                _itemSetting.LeafBottle.transform.localScale = Vector3.one;
                _itemSetting.LeafMeat.transform.localScale = Vector3.one;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_itemList.Any(i => i as Bottle) && _playerStatus != PlayerStatus.Bottle)
            {
                _playerStatus = PlayerStatus.Bottle;
                _itemSetting.LeafRock.transform.localScale = Vector3.one;
                _itemSetting.LeafBottle.transform.localScale *= _itemSetting.LeafSize;
                _itemSetting.LeafMeat.transform.localScale = Vector3.one;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_itemList.Any(i => i as Meat) && _playerStatus != PlayerStatus.Meat)
            {
                _playerStatus = PlayerStatus.Meat;
                _itemSetting.LeafRock.transform.localScale = Vector3.one;
                _itemSetting.LeafBottle.transform.localScale = Vector3.one;
                _itemSetting.LeafMeat.transform.localScale *= _itemSetting.LeafSize;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _playerStatus = PlayerStatus.Normal;
            _itemSetting.LeafRock.transform.localScale = Vector3.one;
            _itemSetting.LeafBottle.transform.localScale = Vector3.one;
            _itemSetting.LeafMeat.transform.localScale = Vector3.one;
        }
    }
    void CreatePhysicsScene()
    {
        m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        m_physicsScene = m_simulationScene.GetPhysicsScene2D();
    }
    void ThrowLineSimulate(GameObject ballPrefab, Vector2 pos, Vector2 velocity)
    {
        var ghost = Instantiate(ballPrefab, pos, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);
        ghost.GetComponent<Rigidbody2D>().AddForce(velocity, ForceMode2D.Impulse);
        _throwsetting.BulletSimulationLine.positionCount = _throwsetting.SimulateFrame;
        for (int i = 0; i < _throwsetting.SimulateFrame && !ghost.GetComponent<ItemBase>().Landing; i++)
        {
            m_physicsScene.Simulate(Time.fixedDeltaTime);
            var hit = Physics2D.OverlapCircleAll(ghost.transform.position, .1f);
            _throwsetting.BulletSimulationLine.SetPosition(i, ghost.transform.position);
            foreach (var item in hit)
            {
                if (item.CompareTag("Ground"))
                {
                    _throwsetting.BulletSimulationLine.positionCount = i;
                    goto ColliderHit;
                }
            }
        }
    ColliderHit:
        Destroy(ghost.gameObject);
    }
    
    public void FluctuationLife(int value)
    {
        if (value < 0)
        {
            if (!_isInvincible)
            {
                CurrentHp += value;
                for (int i = 0; i < Mathf.Abs(value) && _rose.Count > 0; i++)
                {
                    _damageEffect.PlayDamageEffect();
                    _rose.RemoveAt(0);
                }
                StartCoroutine(Invincible());
                AudioManager.Instance.PlaySE("damaged");

                if (_cameraShakeController != null)
                {
                    _cameraShakeController.TriggerShake();
                }
            }
            if (CurrentHp <= 0)
            {
                _playerStatus = PlayerStatus.Death;
            }
        }
        else
        {
            CurrentHp += value;
        }
        if (CurrentHp > _maxHp)
        {
            CurrentHp = _maxHp;
        }
    }
    IEnumerator Invincible()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_damageCool);
        _isInvincible = false;
    }
    
    public void Slow(float multi, float slowtime)
    {
        StartCoroutine(Slowing(multi, slowtime));
    }
    
    public void StopAction(float time)
    {
        //StartCoroutine(StoppingAction(time));
        _audioSource.Stop();
        _animator.SetFloat("isWalk", 0);
        _pauseManager.BeginCoroutine(StoppingAction(time));
    }
    IEnumerator StoppingAction(float time)
    {
        IEnumerator enumerator = _pauseManager.GetCoroutine();
        _canAction = false;
        float timer = 0;
        while (timer < time)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        _canAction = true;
        yield return new WaitForEndOfFrame();
        _pauseManager.OnComplete(enumerator);
    }
    
    IEnumerator Slowing(float multi, float slowtime)
    {
        float defaultMovePower = _movePower;
        float defaultMaxSpeed = _maxSpeed;
        _movePower *= multi;
        _maxSpeed *= multi;
        yield return new WaitForSeconds(slowtime);
        _movePower = defaultMovePower;
        _maxSpeed = defaultMaxSpeed;
    }
    
    IEnumerator ThrowItem()
    {
        IEnumerator enumerator = _pauseManager.GetCoroutine();
        if (!Item(out ItemBase item))
        {
            goto EndCoroutine;
        }
        if (!item.TryGetComponent(out Rigidbody2D rb))
        {
            rb = item.AddComponent<Rigidbody2D>();
        }
        item.gameObject.GetComponent<Collider2D>().isTrigger = false;
        //�܂�����������
        if (item.Throw == ThrowType.Straight)
        {
            item.transform.position = transform.position + (Vector3)_throwsetting.ThrowPos;
            rb.gravityScale = 0;
            rb.AddForce(new Vector2(_throwsetting.ThrowStraightPower * transform.localScale.x, 0), ForceMode2D.Impulse);
        }
        else
        {
            if (_throwsetting.BulletSimulationLine == null)
            {
                Debug.LogError("");
                goto EndCoroutine;
            }
            float t = 0;
            float throwParabolaPower = 0;
            while (Input.GetKey(KeyCode.Return))
            {
                if (Input.GetKey(KeyCode.E))//||Input.GetButton("Cancel"))
                {
                    Debug.Log("ThrowCancel");
                    _throwsetting.BulletSimulationLine.positionCount = 0;
                    goto EndCoroutine;
                }
                t += _throwsetting.ThrowRate * Time.deltaTime;
                throwParabolaPower = (Mathf.Sin(t) + 1) * _throwsetting.MaxThrowParabolaPower;
                ThrowLineSimulate(item.gameObject, transform.position, _throwsetting.ThrowParabolaDirection.normalized * throwParabolaPower * transform.localScale);
                yield return new WaitForEndOfFrame();
            }
            _throwsetting.BulletSimulationLine.positionCount = 0;
            item.transform.position = transform.position + (Vector3)_throwsetting.ThrowPos;
            rb.velocity = Vector3.zero;
            rb.AddForce(_throwsetting.ThrowParabolaDirection.normalized * throwParabolaPower * transform.localScale, ForceMode2D.Impulse);
        }
        item.Throwing();
        if (item as Rock)
        {
            _itemList.Remove((Rock)item);
            _itemSetting.RockCountText.text = _itemList.Where(i => i as Rock).Count().ToString();
            if (_itemSetting.RockCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
                _itemSetting.RockUi.GetComponent<Image>().color = _itemSetting.ZeroItemColor;
                _itemSetting.LeafRock.transform.localScale = Vector3.one;
            }
        }
        else if (item as Bottle)
        {
            _itemList.Remove((Bottle)item);
            _itemSetting.BottleCountText.text = _itemList.Where(i => i as Bottle).Count().ToString();
            if (_itemSetting.BottleCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
                _itemSetting.BottleUi.GetComponent<Image>().color = _itemSetting.ZeroItemColor;
                _itemSetting.LeafBottle.transform.localScale = Vector3.one;
            }
        }
        else
        {
            _itemList.Remove((Meat)item);
            _itemSetting.MeatCountText.text = _itemList.Where(i => i as Meat).Count().ToString();
            if (_itemSetting.MeatCountText.text == "0")
            {
                _playerStatus = PlayerStatus.Normal;
                _itemSetting.MeatUi.GetComponent<Image>().color = _itemSetting.ZeroItemColor;
                _itemSetting.LeafMeat.transform.localScale = Vector3.one;
            }
        }
        AudioManager.Instance.PlaySE("throw");
    EndCoroutine:
        yield return new WaitForEndOfFrame();
        _pauseManager.OnComplete(enumerator);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireCube((Vector2)transform.position + _point, _size);
    }
}