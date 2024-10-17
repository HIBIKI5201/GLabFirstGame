using UnityEngine;

/// <summary>
/// �A�C�e���𐧌䂷����N���X
/// �A�C�e���̋��ʋ@�\����������
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>�A�C�e�����ǂ������邩/summary>
    [Tooltip("Straight �܂������AParabola �����I")]
    [SerializeField] ThrowType _throwType = ThrowType.Straight;
    /// <summary>�A�C�e���������̌��ʔ͈�/summary>
    [Tooltip("�A�C�e���������̌��ʔ͈�")]
    [SerializeField] float _effectRange = 1f;
    [Tooltip("�A�C�e���̌��ʎ���")]
    [SerializeField] static float _activateTime = 5;
    public float ActivatetTime => _activateTime;
    public float EffectRange => _effectRange;
    public ThrowType Throw => _throwType;
    bool _isThrowing;
    public bool IsThrowing => _isThrowing;
    public bool Landing { get; set; }
    public GameObject Player { get; private set; }
    PauseManager _pauseManager;
    /// <summary>
    /// �A�C�e��������������ʂ���������
    /// </summary>
    public abstract void Activate();
    private void Awake()
    {
        _pauseManager = FindAnyObjectByType<PauseManager>();
        _pauseManager.OnPauseResume += PauseAction;
    }
    private void OnDisable()
    {
        _pauseManager.OnPauseResume -= PauseAction;
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
    Vector2 _keepVelocity;
    private void Pause()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            _keepVelocity = rb.velocity;
            rb.Sleep();
        }
    }
    private void Resume()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            rb.WakeUp();
            rb.velocity = _keepVelocity;
        }
    }
    private void Update()
    {
        Activate();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isThrowing)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Player = collision.gameObject;
                AudioManager.Instance.PlaySE("itemGet");
                // �����Ȃ����Ɉړ�����
                this.transform.position = Camera.main.transform.position;
                // �R���C�_�[�𖳌��ɂ���
                GetComponent<Collider2D>().enabled = false;
                // �v���C���[�ɃA�C�e����n��
                collision.gameObject.GetComponent<PlayerController>().GetItem(this);
            }
        }
    }
    /// <summary>
    /// �A�C�e���𓊂������ɌĂ�
    /// </summary>
    public void Throwing() => _isThrowing = true;
    public enum ThrowType
    {
        /// <summary>�^������������</summary>
        Straight,
        /// <summary>��������`���悤�ɓ�����</summary>
        Parabola
    }
}
