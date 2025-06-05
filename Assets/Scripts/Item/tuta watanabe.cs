using UnityEngine;

public partial class TutaWatanabe : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    private Rigidbody2D _rb;
    private bool _landed = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    [System.Obsolete]
    private void Update()
    {
        if (_landed) return;

        // 地面との接触チェック
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _checkRadius, _groundLayer);

        if (hit != null)
        {
            _landed = true;
            _rb.velocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, _checkRadius);
    }
}
