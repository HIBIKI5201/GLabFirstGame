using UnityEngine;

public partial class tuta : MonoBehaviour
{
    private Collider2D col;       // ������Collider2D�R���|�[�l���g
    private Rigidbody2D rb;       // ������Rigidbody2D�R���|�[�l���g

    private bool isFalling = false;   // ���������ǂ����̃t���O

    void Awake()
    {
        // Awake�ŃR���|�[�l���g���擾�i�L���b�V�����Ă����j
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Rigidbody2D��Y�����x���}�C�i�X�Ȃ痎�����Ɣ��f
        isFalling = rb.linearVelocity.y < 0f;

        // �n�ʂɐڐG���Ă��邩�`�F�b�N
        bool isGrounded = IsGrounded();

        // ����������Ȃ��āA�n�ʂɐڐG���Ă��鎞�����ڐG�����L���ɂ���
        if (!isFalling && isGrounded)
        {
            col.enabled = true;   // ����ON
        }
        else
        {
            col.enabled = false;  // ����OFF�i��������󒆂͔���Ȃ��j
        }
    }

    // �n�ʂɐڐG���Ă��邩��Raycast�Ŕ��肷�郁�\�b�h
    private bool IsGrounded()
    {
        // �����̈ʒu����^����0.1�P�ʂ���Raycast���΂�
        // "Ground"���C���[�̂��̂����ɔ�������ݒ�
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        // �n�ʂɓ���������true�A�����łȂ����false��Ԃ�
        return hit.collider != null;
    }

    // ����Collider���ڐG�������ɌĂ΂�郁�\�b�h�iTrigger�ݒ肳��Ă���ꍇ�j
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �������͔��肵�Ȃ��i�������Ȃ��j
        if (isFalling)
        {
            return;
        }
        // �����������肪�v���C���[�������牽�����Ȃ��i�X���[�j
        if (other.CompareTag("Player"))
        {
            return;
        }
        // �����������肪�G�l�~�[�Ȃ画��OK�A�������s��
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy�ɓ��������I");

            // �����ɃG�l�~�[�ɑ΂���U��������_���[�W����������
        }
    }
}
