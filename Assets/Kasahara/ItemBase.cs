using UnityEngine;

/// <summary>
/// �A�C�e���𐧌䂷����N���X
/// �A�C�e���̋��ʋ@�\����������
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>�A�C�e������������ɖ���ʉ�</summary>
    [Tooltip("�A�C�e������������ɖ炷���ʉ�")]
    [SerializeField] AudioClip _sound = default;
    /// <summary>�A�C�e���̌��ʂ����������邩</summary>
    [Tooltip("Get ��I�ԂƁA��������Ɍ��ʂ���������BUse ��I�ԂƁA�A�C�e�����g�������ɔ�������")]
    [SerializeField] ActivateTiming _whenActivated = ActivateTiming.Use;
    /// <summary>�A�C�e�����ǂ������邩/summary>
    [Tooltip("Straight �܂������AParabola �����I")]
    [SerializeField] ThrowType _throwType = ThrowType.Straight;
    [Tooltip("�A�C�e���𓊂������G�Ɠ����邩")]
    [SerializeField] bool _hitEnemy;
    public ThrowType Throw => _throwType;
    bool _isThrowing;
    public GameObject Player { get; private set; }
    /// <summary>
    /// �A�C�e��������������ʂ���������
    /// </summary>
    public abstract void Activate();
    private void Update()
    {
        //�����蔻��ŔY��ł�
        if(_isThrowing)
        {
            RaycastHit2D hit;
            //hit = Physics2D.OverlapCircle(Vector2.zero,1f,);
            if(_hitEnemy)
            {

            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isThrowing)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Player = collision.gameObject;
                if (_sound)
                {
                    AudioSource.PlayClipAtPoint(_sound, Camera.main.transform.position);
                }

                // �A�C�e�������^�C�~���O�ɂ���ď����𕪂���
                if (_whenActivated == ActivateTiming.Get)
                {
                    Activate();
                    Destroy(this.gameObject);
                }
                else if (_whenActivated == ActivateTiming.Use)
                {
                    // �����Ȃ����Ɉړ�����
                    this.transform.position = Camera.main.transform.position;
                    // �R���C�_�[�𖳌��ɂ���
                    GetComponent<Collider2D>().enabled = false;
                    // �v���C���[�ɃA�C�e����n��
                    collision.gameObject.GetComponent<PlayerController>().GetItem(this);
                }
            }
        }
    }

    /// <summary>
    /// �A�C�e�������A�N�e�B�x�[�g���邩
    /// </summary>
    enum ActivateTiming
    {
        /// <summary>��������ɂ����g��</summary>
        Get,
        /// <summary>�u�g���v�R�}���h�Ŏg��</summary>
        Use,
    }
    public enum ThrowType
    {
        /// <summary>�^������������</summary>
        Straight,
        /// <summary>��������`���悤�ɓ�����</summary>
        Parabola
    }
}
