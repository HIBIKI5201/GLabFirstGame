using UnityEngine;


/// <summary>
/// �v���C���[�̉�����I�u�W�F�N�g��^���ɗ��Ƃ�
/// </summary>
public partial class tuta : MonoBehaviour
{
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private string _landSE = "thump"; // �ڒn���̌��ʉ���
    private Rigidbody2D _rb;
    private bool _landed = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_landed) return;

        // �n�ʂƂ̐ڐG�`�F�b�N
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _checkRadius, _groundLayer);

        if (hit != null)
        {
            _landed = true;
            _rb.velocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            AudioManager.Instance?.PlaySE(_landSE);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, _checkRadius);
    }
}

