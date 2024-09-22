using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Bottle : ItemBase
{
    [SerializeField, Header("�ڒn����̑傫��")] Vector2 size;
    [SerializeField, Header("�ڒn����̊p�x")] float angle;
    bool _isGround;
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
        Gizmos.DrawWireCube(transform.position, size);
    }
    public override void Activate()
    {
        if (IsThrowing)
        {
            if (!_isGround)
            {
                var hit = Physics2D.OverlapBox(transform.position, size, angle);
                if (hit != null && hit.gameObject.CompareTag("Ground"))
                {
                    _isGround = true;
                    //�n�ʂɂ�����R���C�_�[�𕜊�
                    GetComponent<Collider2D>().enabled = true;
                }
            }
            else
            {
                var hit = Physics2D.OverlapCircle(transform.position, EffectRange);
                if (hit != null && hit.CompareTag("Enemy"))
                {
                    //�G�������鏈��
                    hit.gameObject.GetComponent<Enemy>().ReactionBottle(transform.position);
                }
            }

        }
    }
}
