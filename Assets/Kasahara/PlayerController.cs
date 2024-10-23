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
    [SerializeField, Tooltip("プレイヤーの体力の最大値")] int _maxHp;
    public int CurrentHp { get; private set; }
    [SerializeField, Tooltip("体力のバラの花びら")] List<GameObject> _rose = new List<GameObject>();
    [SerializeField, Tooltip("プレイヤーの速度の最大値")] public float _maxSpeed;
    [SerializeField, Tooltip("プレイヤーの移動速度の加速度")] public float _movePower;
    [SerializeField, Tooltip("入力がない時の減速度")] public float _deceleration;
    [SerializeField, Tooltip("着地時に慣性を適用する")] bool _landingInertia;
    [SerializeField, Tooltip("プレイヤーのジャンプ力")] float _jumpPower;
    [SerializeField, Tooltip("落下速度")] float _fallSpeed;
    [SerializeField, Tooltip("プレイヤーの無敵時間")] int _damageCool;
    [SerializeField, Tooltip("接地判定の位置")] Vector2 _point;
    [SerializeField, Tooltip("接地判定の大きさ")] Vector2 _size;
    [SerializeField, Tooltip("接地判定の角度")] float _angle;
    [SerializeField, Tooltip("ダッシュの音")] AudioClip _dash;
    [SerializeField, Tooltip("歩いてる音")] AudioClip _walk;
    [SerializeField, Tooltip("<アイテムを投げる設定>")] Throwsetting _throwsetting;
    [SerializeField, Tooltip("<アイテムの設定>")] ItemSetting _itemSetting;
    [System.Serializable]
    struct Throwsetting
    {
        [Tooltip("まっすぐ投げる強さ")] public float ThrowStraightPower;
        [Tooltip("放物的に投げる強さ")] public float MaxThrowParabolaPower;
        [Tooltip("前後に動くやつの増加率")] public float ThrowRate;
        [Tooltip("放物的に投げる方向")] public Vector2 ThrowParabolaDirection;
        [Tooltip("アイテムを投げる位置")] public Vector2 ThrowPos;
        [Tooltip("弾道予測線")] public LineRenderer BulletSimulationLine;
        [Tooltip("弾道予測線の長さ")] public int SimulateFrame;
        [Tooltip("床のオブジェクトをまとめたやつ")] public GameObject Platform;
    }
    /// <summary>アイテムの設定</summary>
    [System.Serializable]
    struct ItemSetting
    {
        [Tooltip("持てる石の最大値")] public int MaxRockCount;
        [Tooltip("持てる空き瓶の最大値")] public int MaxBottleCount;
        [Tooltip("持てる肉の最大値")] public int MaxMeatCount;
        [Tooltip("石のUI")] public GameObject RockUi;
        [Tooltip("空き瓶のUI")] public GameObject BottleUi;
        [Tooltip("肉のUI")] public GameObject MeatUi;
        [Tooltip("石の個数を表示するテキスト")] public Text RockCountText;
        [Tooltip("空き瓶の個数を表示するテキスト")] public Text BottleCountText;
        [Tooltip("肉の個数を表示するテキスト")] public Text MeatCountText;
        [Tooltip("アイテムを持っていないときの色")] public Color ZeroItemColor;
        [Tooltip("石に対応する葉っぱ")] public GameObject LeafRock;
        [Tooltip("空き瓶に対応する葉っぱ")] public GameObject LeafBottle;
        [Tooltip("肉に対応する葉っぱ")] public GameObject LeafMeat;
        [Tooltip("葉っぱの拡大率")] public float LeafSize;
    }
    /// <summary>持っているアイテムのリスト</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    /// <summary>接地判定</summary>
    [SerializeField] bool _isJump;
    /// <summary>敵を踏んだ判定</summary>
    [SerializeField] bool _isStompEnemy;
    /// <summary>無敵時間中かどうか</summary>
    bool _isInvincible;
    [SerializeField] bool _canAction = true;
    [HideInInspector] public PlayerStatus _playerStatus = PlayerStatus.Normal;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Scene m_simulationScene;
    PhysicsScene2D m_physicsScene;
    float _throwParabolaPower = 0;
    GameObject[] _itemPos = new GameObject[3];
    Vector3[] _afterItemPos0 = new Vector3[3];
    Vector3[] _afterItemPos1 = new Vector3[3];
    Vector3[] _afterItemPos2 = new Vector3[3];
    float _horiInput = 0;
    DamageCamera _damageCamera;
    PauseManager _pauseManager;
    AudioManager _audioManager;
    AudioSource _audioSource;
    Animator _animator;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireCube((Vector2)transform.position + _point, _size);
    }
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _audioManager = FindAnyObjectByType<AudioManager>();
        _pauseManager = FindAnyObjectByType<PauseManager>();
        if (_pauseManager != null)
            _pauseManager.OnPauseResume += PauseAction;
        else
            Debug.LogError("このシーンにPauseManagerが存在しません");
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animatorがありません");
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
            SceneManager.MoveGameObjectToScene(platform, m_simulationScene);
        }
        else
        {
            Debug.LogError("_throwsetting.Platformに地面のオブジェクトをセットしてください");
        }
        _damageCamera = FindAnyObjectByType<DamageCamera>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSourseがありません");
        }
        else
        {
            Debug.Log(_audioSource);
        }
        if (_damageCamera == null)
        {
            Debug.LogError("DamageCameraがありません");
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
        if (_canAction)
        {
            Move();
            //NewMove();
            Jump();
            ChangeItem();
            UseItem();
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
        //問題点:
        //1:地面についてるとき横方向への移動速度が不安定
        //2:プレイヤーを吹っ飛ばす場合に横入力によってあまり吹っ飛ばせないかも
        //加速度的に減る
        _horiInput = Input.GetAxisRaw("Horizontal");
        if (_horiInput == 0)
        {
            if (!_isJump)
            {
                float x = 0;
                if(_rb.velocity.x != 0)
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
                //Debug.Log("ジャンプした");
                AudioManager.Instance.PlaySE("jump");
                _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
                _isJump = true;
                _jumpEnumerator = GroundingJudge(_jumpEnumerator);
                //Debug.Log("StartCoroutine");
                StartCoroutine(_jumpEnumerator);
            }
            else
            {
                //Debug.Log("ジャンプできなかった");
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
            //Debug.Log("敵を踏んで小ジャンプ");
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
    /// <summary>
    /// 着地を検知する
    /// </summary>
    /// <returns></returns>
    IEnumerator GroundingJudge(IEnumerator enumerator)
    {
        _animator.SetBool("isJump", true);
        _audioSource.Stop();
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
                if (obj.gameObject.CompareTag("Ground"))
                {
                    _isJump = false;
                    _rb.gravityScale = 1;
                    AudioManager.Instance.PlaySE("jump_landing");
                    _animator.SetBool("isJump", false);
                    if (_landingInertia && _horiInput == 0)
                    {
                        _rb.velocity = new Vector2(0, _rb.velocity.y);
                    }
                    //コルーチンを連続で起動させないために待つ
                    yield return new WaitForSeconds(0.5f);
                    _jumpEnumerator = null;
                    yield break;
                }
                else if (obj.gameObject.CompareTag("Enemy"))
                {
                    FluctuationLife(-1);
                    _isStompEnemy = true;
                    _rb.gravityScale = 1;
                }
            }
        }
        _jumpEnumerator = null;
    }
    /// <summary>
    /// プレイヤーがアイテムを入手したときに呼ぶメソッド
    /// </summary>
    /// <param name="item"></param>
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
    /// <summary>
    /// Enterが押された時アイテムを投げるためのコルーチンをスタートします
    /// </summary>
    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _pauseManager.BeginCoroutine(ThrowItem());
            //StartCoroutine(ThrowItem());
        }
    }
    /// <summary>
    /// プレイヤーが持っているアイテムに応じて_itemListからアイテムを取ってきます。
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
                _itemSetting.LeafRock.transform.localScale *= _itemSetting.LeafSize;
                _itemSetting.LeafBottle.transform.localScale = Vector3.one;
                _itemSetting.LeafMeat.transform.localScale = Vector3.one;
                //for (int i = 0; i < _itemPos.Length; i++)
                //{
                //    _itemPos[i].transform.position = _afterItemPos2[i];
                //}
                Debug.Log("石を使う");
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
                //for (int i = 0; i < _itemPos.Length; i++)
                //{
                //    _itemPos[i].transform.position = _afterItemPos1[i];
                //}
                Debug.Log("瓶を使う");
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
                //for (int i = 0; i < _itemPos.Length; i++)
                //{
                //    _itemPos[i].transform.position = _afterItemPos0[i];
                //}
                Debug.Log("肉を使う");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _playerStatus = PlayerStatus.Normal;
            _itemSetting.LeafRock.transform.localScale = Vector3.one;
            _itemSetting.LeafBottle.transform.localScale = Vector3.one;
            _itemSetting.LeafMeat.transform.localScale = Vector3.one;
            Debug.Log("アイテムを持たない");
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
    /// <summary>
    /// プレイヤーの体力を引数に渡した数値分増減させます。
    /// </summary>
    /// <param name="value"></param>
    public void FluctuationLife(int value)
    {
        if (value < 0)
        {
            if (!_isInvincible)
            {
                CurrentHp += value;
                for (int i = 0; i < Mathf.Abs(value) && _rose.Count > 0; i++)
                {
                    Destroy(_rose[0]);
                    _rose.RemoveAt(0);
                }
                StartCoroutine(Invincible());
                AudioManager.Instance.PlaySE("damaged");

                if (_damageCamera != null)
                {
                    _damageCamera.Shake();
                }
            }
            if (CurrentHp <= 0)
            {
                //Debug.Log(_playerStatus);
                _playerStatus = PlayerStatus.Death;
            }
            else
            {
                //ダメージを受けた時の処理、一旦保留
                //DOTween.To(() => new Color(), s => )
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
        //Debug.Log($"Playerの体力:{CurrentHp}");
    }
    IEnumerator Invincible()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_damageCool);
        _isInvincible = false;
    }
    /// <summary>
    /// プレイヤーの速度を調整する
    /// </summary>
    /// <param name="multi">倍率</param>
    /// <param name="slowtime">継続時間</param>
    public void Slow(float multi, float slowtime)
    {
        StartCoroutine(Slowing(multi, slowtime));
    }
    /// <summary>
    /// プレイヤーが行動できなくなる処理
    /// </summary>
    /// <param name="time"></param>
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
        //投げるアイテムにRigidbodyがついてなかったらつける
        if (!item.TryGetComponent(out Rigidbody2D rb))
        {
            rb = item.AddComponent<Rigidbody2D>();
        }
        item.gameObject.GetComponent<Collider2D>().isTrigger = false;
        //まっすぐ投げる
        if (item.Throw == ItemBase.ThrowType.Straight)
        {
            //アイテムをプレイヤーの位置に持ってくる
            item.transform.position = transform.position + (Vector3)_throwsetting.ThrowPos;
            rb.gravityScale = 0;
            rb.AddForce(new Vector2(_throwsetting.ThrowStraightPower * transform.localScale.x, 0), ForceMode2D.Impulse);
        }
        //放物的に投げる
        else
        {
            if (_throwsetting.BulletSimulationLine == null)
            {
                Debug.LogError("LineRendererがnullで投げれません");
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
            //アイテムをプレイヤーの位置に持ってくる
            item.transform.position = transform.position + (Vector3)_throwsetting.ThrowPos;
            rb.velocity = Vector3.zero;
            rb.AddForce(_throwsetting.ThrowParabolaDirection.normalized * throwParabolaPower * transform.localScale, ForceMode2D.Impulse);
            _throwParabolaPower = 0;
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
}