using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 敵のクラス
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    // 常数・読み取り専用・静的な変数などはクラス共通なので、クラス構成の頭に入れておく
    private const string k_playerTag = "Player";
    private static readonly int s_dizzyParameter = Animator.StringToHash("Dizzy");
    private static readonly int s_gallopParameter = Animator.StringToHash("Gallop");
    private static PlayerController s_playerController;
    private static Transform s_playerTransform;
    private static PhysicsMaterial2D s_physicsMaterial2D;

    [SerializeField, ReadOnly]
    private Rigidbody2D _rigidbody2D;

    [SerializeField, ReadOnly]
    private BoxCollider2D _boxCollider2D;

    [Header("関連コンポーネント・オブジェクト")]
    [SerializeField, Required, FormerlySerializedAs("_anim")]
    private Animator _animator;

    [SerializeField]
    private GameObject _stunAnimationObject;

    [SerializeField]
    private GameObject _meatIcon;

    [Header("基本設定")]
    [SerializeField, FormerlySerializedAs("_beast")]
    private EnemyType _enemyType;

    [SerializeField]
    private int _maxHp;

    [SerializeField, FormerlySerializedAs("_attack")]
    private int _attackPoint;

    [SerializeField]
    private float _jumpPower;

    [Header("移動スピード")]
    [SerializeField, FormerlySerializedAs("_speed")]
    private float _normalSpeed;

    [SerializeField, FormerlySerializedAs("_chaseSpeed")]
    private float _speedWhenChasingPlayer;

    [Header("移動能力")]
    [SerializeField, FormerlySerializedAs("_jumpOver")]
    private bool _canJumpOver;

    [SerializeField, FormerlySerializedAs("_goDown")]
    private bool _canJumpDown;

    [SerializeField, FormerlySerializedAs("_canChase")]
    private bool _canChasePlayer;

    [SerializeField, FormerlySerializedAs("_canDamage")]
    private bool _canReceiveDamage;

    [Header("Raycast 判定設定")]
    [SerializeField, FormerlySerializedAs("_ground")]
    private GroundedRay _raycastData;

    [Header("現在の数値（インスペクタに編集しても効果なし）")]
    [SerializeField]
    private int _currentHp; // 現在のHP

    [SerializeField]
    private float _currentSpeed;

    [SerializeField]
    private DirectionType _currentDirection;

    [SerializeField]
    private EnemyStateType _currentState;

    public float CurrentSpeed
    {
        get => _currentSpeed;
    }

    public EnemyStateType State
    {
        get => _currentState;
    }

    private EnemyDamageHandler _damageHandler;
    private EnemyAttackHandler _attackHandler;

    private Coroutine _itemReactionCoroutine = null;
    private Coroutine _slowDownCoroutine = null;

    private SpriteRenderer[] _spriteRenderers;
    private Transform _modelTransform;

    private Vector2 _modelScale;
    private Vector2 _pointToEscapeFrom;
    private Vector2 _meatPosition;
    private float _meatEatingStartTime;
    private float _meatEatingTargetDuration;
    private bool _isEatingMeat;
    private bool _canPlayMoveSE;
    private float _lastAttackingTime;

    #region ライフサイクル関数
    private void OnValidate()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        // コンポーネントと数値を初期化する
        Initialize();

        // 地面の高さに自身の位置を合わせる
        MatchPositionToGround();

        // 少し時間をずらして SE 再生を有効化する
        StartCoroutine(DelayEnableSound(0.2f));

        void Initialize()
        {
            if (s_playerController == null || s_playerTransform == null)
            {
                s_playerController = FindAnyObjectByType<PlayerController>();
                s_playerTransform = s_playerController.transform;
            }

            if (s_physicsMaterial2D == null)
            {
                s_physicsMaterial2D = new PhysicsMaterial2D()
                {
                    friction = 0,
                    bounciness = 0
                };
            }
            _boxCollider2D.sharedMaterial = s_physicsMaterial2D;

            _modelTransform = _animator.transform;
            _modelScale = _modelTransform.localScale;
            _modelScale.x = Mathf.Abs(_modelScale.x);

            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            _currentHp = _maxHp;
            _currentSpeed = _normalSpeed;
            _currentState = EnemyStateType.Normal;

            _damageHandler = new EnemyDamageHandler(_maxHp, _canReceiveDamage, _spriteRenderers, this);
            _attackHandler = new EnemyAttackHandler(_attackPoint, ref _lastAttackingTime);
        }

        void MatchPositionToGround()
        {
            var hit = Physics2D.BoxCast(transform.position, _boxCollider2D.size, 0, Vector2.down, 1000, _raycastData.RaycastGroundMask);
            if (hit && hit.collider)
            {
                Vector2 newPosition = transform.position;
                newPosition.y = hit.point.y + _boxCollider2D.size.y / 2;
                transform.position = newPosition;
            }
        }

        IEnumerator DelayEnableSound(float delayDuration)
        {
            _canPlayMoveSE = false;
            yield return new WaitForSeconds(delayDuration);
            _canPlayMoveSE = true;
        }
    }

    private void OnEnable()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        // SE を鳴らす
        if (_canPlayMoveSE)
        {
            AudioManager.Instance.PlaySE(_enemyType switch
            {
                EnemyType.StrayDog => "strayDog",
                EnemyType.Wolf_Normal or EnemyType.Wolf_Gray => "wolf",
                EnemyType.Bear => "bare",
                _ => "strayDog"
            });
            _canPlayMoveSE = false;
        }
    }

    private void OnDisable()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        // 移動
        var modelLocalScale = _modelScale;
        modelLocalScale.x *= _currentDirection switch
        {
            DirectionType.Right => -1,
            DirectionType.Left => 1,
            _ => Mathf.Sign(_modelTransform.localScale.x)
        };
        _modelTransform.localScale = modelLocalScale;

        if (_stunAnimationObject)
        {
            _stunAnimationObject.gameObject.SetActive(_currentState == EnemyStateType.Faint);
        }

        // 状態に応じた処理
        switch (_currentState)
        {
            case EnemyStateType.Faint:
                UpdateFaintState();
                break;

            case EnemyStateType.EatingMeat:
                UpdateEatingMeatState();
                break;

            case EnemyStateType.Escape:
                UpdateEscapeState();
                break;

            case EnemyStateType.ChasingPlayer:
                UpdateChasingPlayerState();
                break;

            case EnemyStateType.Normal:
            default:
                TryChangeDirection();
                UpdateHorizontalMovement();
                SearchForPlayer();
                _animator.SetBool(s_gallopParameter, false);
                _animator.SetBool(s_dizzyParameter, false);
                break;
        }

        // ゲームオーバー、もしくはゲームクリア時には敵を非表示にする
        if (GameManager.Instance.CurrentState == GameStateType.GameOver
            || GameManager.Instance.CurrentState == GameStateType.StageClear)
        {
            SetEnemyEnabledState(false);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        switch (_currentState)
        {
            case EnemyStateType.Faint:
            case EnemyStateType.EatingMeat:
            case EnemyStateType.Escape:
            case EnemyStateType.ChasingPlayer:
                return;
        }

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            CollisionReturn(collision.GetContact(i).normal, collision.GetContact(i).point);
        }

        // TODO: What is this?
        void CollisionReturn(Vector2 normal, Vector2 point)
        {
            float x = point.x - transform.position.x;
            bool isLeft = x < 0;
            bool isTrue = _currentDirection switch
            {
                DirectionType.Right => !isLeft,
                DirectionType.Left => isLeft,
                _ => false
            };
            if (Mathf.Abs(normal.y) <= 0.2f && isTrue)
            {
                _currentDirection = _currentDirection == DirectionType.Right ? DirectionType.Left : DirectionType.Right;
            }
        }
    }

    #endregion

    /// <summary>
    /// エネミーコンポネントを有効化・無効化する
    /// </summary>
    public void SetEnemyEnabledState(bool shouldBeEnabled)
    {
        this.enabled = shouldBeEnabled;
    }

    /// <summary>
    /// スピードを上書きする
    /// </summary>
    public void SetCurrentSpeed(float newCurrentSpeed)
    {
        _currentSpeed = newCurrentSpeed;
    }

    /// <summary>
    /// HPを減らすメソッド
    /// </summary>
    /// <param name="negativeValue">ダメージを受ける場合は負の数を渡す</param>
    public void LifeFluctuation(int negativeValue)
    {
        _currentHp = _damageHandler.LifeFluctuation(negativeValue);
    }

    /// <summary>
    /// 死亡時自身を破棄する
    /// </summary>
    public void Die()
    {
        Destroy(gameObject);
    }

    // TODO: Rename to "ApplyStoneItemEffect"?
    /// <summary>
    /// 石の効果を与える
    /// </summary>
    /// <param name="stunTime"></param>
    public void ReactionStone(float stunTime)
    {
        SetStunStateForDuration(stunTime);
    }

    // TODO: Rename to "ApplyBottleItemEffect"?
    /// <summary>
    /// 空き缶の効果を与える
    /// </summary>
    public void ReactionBottle(Vector3 bottlePosition, float effectTime)
    {
        SetEscapeStateForDuration(bottlePosition, effectTime);
    }

    // TODO: Rename to "ApplyMeatItemEffect"?
    /// <summary>
    /// 肉の効果を与える
    /// </summary>
    public void ReactionMeat(Vector3 meatPosition, float effectTime)
    {
        if (_currentState == EnemyStateType.EatingMeat || _currentState == EnemyStateType.Faint)
        {
            return;
        }

        _currentState = EnemyStateType.EatingMeat;
        _isEatingMeat = false;
        _meatPosition = meatPosition;
        _meatEatingStartTime = Time.time;
        _meatEatingTargetDuration = effectTime;
    }

    // TODO: Rename to "ApplySwampItemEffect"?
    /// <summary>
    /// 沼の効果を与える
    /// </summary>
    public void SlowDownScale(float speedSlowDownScale, float duration)
    {
        SetSlowDownStateForDuration(speedSlowDownScale, duration);
    }

    /// <summary>
    /// ツタの効果を与える
    /// </summary>
    public void ApplyIvyItemEffect(float stunTime)
    {
        // SE を再生する
        AudioManager.Instance.PlaySE("damage_enemy");

        // エネミーをスタンする
        SetStunStateForDuration(stunTime);
    }

    #region 効果処理

    /// <summary>
    /// 一定時間、スタン効果を与える
    /// </summary>
    private void SetStunStateForDuration(float stunTime)
    {
        if (_currentState == EnemyStateType.Escape || _currentState == EnemyStateType.Faint)
        {
            return;
        }

        if (_itemReactionCoroutine != null)
        {
            StopCoroutine(_itemReactionCoroutine);
        }

        _rigidbody2D.linearVelocity = Vector2.zero;
        _itemReactionCoroutine = StartCoroutine(StunCoroutine(stunTime));

        IEnumerator StunCoroutine(float stunTime)
        {
            _currentState = EnemyStateType.Faint;
            yield return new WaitForSeconds(stunTime);
            _currentState = EnemyStateType.Normal;
        }
    }

    /// <summary>
    /// ポイントを設定し、一定時間このポイントから逃げる状態に設定
    /// </summary>
    private void SetEscapeStateForDuration(Vector3 pointToEscapeFrom, float effectTime)
    {
        if (_currentState == EnemyStateType.Faint)
        {
            return;
        }

        if (_itemReactionCoroutine != null)
        {
            StopCoroutine(_itemReactionCoroutine);
        }

        _itemReactionCoroutine = StartCoroutine(SetEscapeStateCoroutine(pointToEscapeFrom, effectTime));

        IEnumerator SetEscapeStateCoroutine(Vector3 pointToEscapeFrom, float effectTime)
        {
            _currentState = EnemyStateType.Escape;
            _pointToEscapeFrom = pointToEscapeFrom;
            yield return new WaitForSeconds(effectTime);
            _currentState = EnemyStateType.Normal;
        }
    }

    /// <summary>
    /// 一定時間、スピードを遅くする状態に設定
    /// </summary>
    private void SetSlowDownStateForDuration(float speedSlowDownScale, float duration)
    {
        if (_slowDownCoroutine != null)
        {
            StopCoroutine(_slowDownCoroutine);
        }

        _slowDownCoroutine = StartCoroutine(SlowDown(speedSlowDownScale, duration));

        IEnumerator SlowDown(float speedSlowDownScale, float duration)
        {
            float startTime = Time.time;

            _currentSpeed = _normalSpeed * speedSlowDownScale;
            while (Time.time <= startTime + duration)
            {
                yield return new WaitForSeconds(1);
                LifeFluctuation(-1);
            }

            _currentSpeed = _normalSpeed;
        }
    }

    #endregion

    #region 移動と攻撃処理

    /// <summary>
    /// 進行方向を更新する
    /// </summary>
    private void UpdateHorizontalMovement()
    {
        Vector2 velocity = _rigidbody2D.linearVelocity;
        velocity.x = _currentSpeed * _currentDirection switch
        {
            DirectionType.Right => 1,
            DirectionType.Left => -1,
            _ => 0
        };
        _rigidbody2D.linearVelocity = velocity;
    }

    /// <summary>
    /// 距離を測って、方向転換の必要があれば転換する
    /// </summary>
    private void TryChangeDirection()
    {
        if (IsFrontGrounded(out bool isRightDirectionGrounded))
        {
            // もし目の前が崖なら移動方向を反転させる
            _currentDirection = isRightDirectionGrounded ? DirectionType.Right : DirectionType.Left;
        }

        if (IsTouchedFromSide(out bool playerHit))
        {
            // もし壁などに触れたら移動方向を反転させる
            _currentDirection = _currentDirection == DirectionType.Right ? DirectionType.Left : DirectionType.Right;
            if (playerHit)
            {
                // プレイヤーに当たっていたら攻撃を行う
                AttackPlayer();
            }
        }
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    private void Jump()
    {
        Vector2 velocity = _rigidbody2D.linearVelocity;
        velocity.y = _jumpPower;
        _rigidbody2D.linearVelocity = velocity;
    }

    /// <summary>
    /// プレイヤーを探す処理
    /// </summary>
    private void SearchForPlayer()
    {
        if (!_canChasePlayer)
        {
            return;
        }

        var hit = Physics2D.Linecast(transform.position, s_playerTransform.position, _raycastData.RaycastGroundMask);
        _currentState = hit ? EnemyStateType.Normal : EnemyStateType.ChasingPlayer;
    }

    /// <summary>
    /// プレイヤーへ攻撃する
    /// </summary>
    private void AttackPlayer()
    {
        _attackHandler.Attack(s_playerController);
    }

    #endregion

    #region 判定処理

    /// <summary>
    /// 前の方向が接地しているか？
    /// </summary>
    private bool IsFrontGrounded(out bool isRightDirectionGrounded)
    {
        Vector2 rRayPos = transform.position + (Vector3)_raycastData.RightRayOriginOffset;
        Vector2 lRayPos = transform.position + (Vector3)_raycastData.LeftRayOriginOffset;
        bool isHitR = Physics2D.Raycast(rRayPos, Vector2.down, _raycastData.GroundCheckRayDistance, _raycastData.RaycastGroundMask);
        bool isHitL = Physics2D.Raycast(lRayPos, Vector2.down, _raycastData.GroundCheckRayDistance, _raycastData.RaycastGroundMask);
        isRightDirectionGrounded = isHitR;
        return isHitR ^ isHitL;
    }

    /// <summary>
    /// 接地しているか？
    /// </summary>
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position, _boxCollider2D.size - new Vector2(0.1f, 0.1f),
            0, Vector2.down, 0.2f, _raycastData.RaycastGroundMask);
    }

    /// <summary>
    /// 左右いずれかの側面から他の物体に触れられているか？
    /// </summary>
    private bool IsTouchedFromSide(out bool isPlayerHit)
    {
        Vector2 rayDirection = _currentDirection switch
        {
            DirectionType.Left => Vector2.left,
            DirectionType.Right => Vector2.right,
            _ => Vector2.zero
        };

        var hit = Physics2D.Raycast(transform.position, rayDirection, _raycastData.SideCheckRayDistance, _raycastData.RaycastSideMask);
        isPlayerHit = false;
        if (hit && hit.collider)
        {
            isPlayerHit = hit.collider.CompareTag(k_playerTag);
        }
        return hit;
    }

    /// <summary>
    /// 壁との水平距離が短くて乗り越えられるか？
    /// </summary>
    private bool IsCloseEnoughToJumpOnWall()
    {
        Vector2 direction = _currentDirection switch
        {
            DirectionType.Left => Vector2.left,
            DirectionType.Right => Vector2.right,
            _ => Vector2.zero
        };
        return Physics2D.Raycast(transform.position, direction, _raycastData.MaxJumpDistanceFromWall, _raycastData.RaycastGroundMask);
    }

    #endregion

    #region 状態更新

    private void UpdateEscapeState()
    {
        float deltaPositionX = _pointToEscapeFrom.x - transform.position.x;

        // プレイヤーとは逆の方向に逃げ出す
        _currentDirection = deltaPositionX >= 0 ? DirectionType.Left : DirectionType.Right;

        // 逃げ出すアニメーションを再生する
        _animator.SetBool(s_gallopParameter, true);

        UpdateHorizontalMovement();
    }

    private void UpdateEatingMeatState()
    {
        // 肉がある方へ方向をセットする
        var deltaPositionX = _meatPosition.x - transform.position.x;
        _currentDirection = deltaPositionX <= 0 ? DirectionType.Left : DirectionType.Right;

        // もし肉に近づいたら...
        if (Mathf.Abs(deltaPositionX) <= 0.2f)
        {
            // ...エネミーを止めて
            _currentDirection = DirectionType.None;

            // ...肉アイコンをつけて
            if (_meatIcon)
            {
                _meatIcon.SetActive(true);
            }

            // ...食う終了時間をセットして
            if (!_isEatingMeat)
            {
                _meatEatingStartTime = Time.time;
            }

            // ...食い始める
            _isEatingMeat = true;
        }

        // もし食い終わったら、状態を戻す
        if (Time.time >= _meatEatingStartTime + _meatEatingTargetDuration)
        {
            _currentState = EnemyStateType.Normal;
            if (_meatIcon)
            {
                _meatIcon.SetActive(false);
            }
            _currentDirection = Mathf.Sign(_modelTransform.localScale.x) switch
            {
                1 => DirectionType.Left,
                -1 => DirectionType.Right,
                _ => DirectionType.None
            };
        }

        if (IsCloseEnoughToJumpOnWall() && IsGrounded() && _canJumpOver)
        {
            Jump();
        }

        UpdateHorizontalMovement();
    }

    private void UpdateChasingPlayerState()
    {
        float x = s_playerTransform.position.x - transform.position.x;
        float y = s_playerTransform.position.y - transform.position.y;
        _currentDirection = x <= 0 ? DirectionType.Left : DirectionType.Right;
        if (Mathf.Abs(x) <= 0.1f)
        {
            _currentDirection = DirectionType.None;
        }

        if (!_canJumpDown)
        {
            if (IsFrontGrounded(out bool isRightDirectionGrounded))
            {
                switch (_currentDirection)
                {
                    case DirectionType.Left:
                        if (isRightDirectionGrounded)
                        {
                            _currentDirection = DirectionType.None;
                        }
                        break;
                    case DirectionType.Right:
                        if (!isRightDirectionGrounded)
                        {
                            _currentDirection = DirectionType.None;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        if (IsCloseEnoughToJumpOnWall() && IsGrounded() && _canJumpOver)
        {
            Jump();
        }

        if (Time.time >= _lastAttackingTime + 0.1f)
        {
            if (IsTouchedFromSide(out bool isPlayerHit))
            {
                if (isPlayerHit)
                {
                    AttackPlayer();
                }
            }
        }

        UpdateHorizontalMovement();
        SearchForPlayer();
    }

    private void UpdateFaintState()
    {
        var velocity = _rigidbody2D.linearVelocity;
        velocity.x = 0;
        _rigidbody2D.linearVelocity = velocity;
        _animator.SetBool(s_dizzyParameter, true);
    }

    #endregion

#if UNITY_EDITOR
    #region ギズモの描画処理

    [Header("デバッグのギズモを描くか")]
    [SerializeField]
    private bool _alwaysDebug;

    private void OnDrawGizmos()
    {
        if (_alwaysDebug)
        {
            DebugRendering();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_alwaysDebug)
        {
            DebugRendering();
        }
    }

    void DebugRendering()
    {
        var cacheColor = Gizmos.color;
        Gizmos.color = Color.yellow;
        Vector3 dirPos = transform.position + Vector3.up;
        Gizmos.DrawLine(dirPos, dirPos + (_currentDirection == DirectionType.Right ? Vector3.right : Vector3.left));
        Vector2 rRayPos = transform.position + (Vector3)_raycastData.RightRayOriginOffset;
        Vector2 lRayPos = transform.position + (Vector3)_raycastData.LeftRayOriginOffset;

        var hit = Physics2D.Raycast(rRayPos, Vector2.down, _raycastData.GroundCheckRayDistance, _raycastData.RaycastGroundMask);
        if (hit)
        {
            Gizmos.DrawLine(rRayPos, hit.point);
        }
        else
        {
            Gizmos.DrawLine(rRayPos, rRayPos + Vector2.down * _raycastData.GroundCheckRayDistance);
        }

        hit = Physics2D.Raycast(lRayPos, Vector2.down, _raycastData.GroundCheckRayDistance, _raycastData.RaycastGroundMask);
        if (hit)
        {
            Gizmos.DrawLine(lRayPos, hit.point);
        }
        else
        {
            Gizmos.DrawLine(lRayPos, lRayPos + Vector2.down * _raycastData.GroundCheckRayDistance);
        }

        Vector2 dir = _currentDirection switch
        {
            DirectionType.Left => Vector2.left,
            DirectionType.Right =>
            Vector2.right, _ =>
            Vector2.zero
        };
        if (_currentState == EnemyStateType.ChasingPlayer)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(dir * _raycastData.MaxJumpDistanceFromWall));
        Gizmos.color = cacheColor;
    }

    #endregion
#endif
}
