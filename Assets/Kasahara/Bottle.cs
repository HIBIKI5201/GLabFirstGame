using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : ItemBase
{
    [SerializeField, Header("�ڒn����̑傫��")] Vector2 _size;
    [SerializeField, Header("�ڒn����̊p�x")] float _angle;
    bool _effected;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
        Gizmos.DrawWireCube(transform.position, _size);
    }
    public override void Activate()
    {
        if (IsThrowing)
        {
            if (!Landing)
            {
                var hit = Physics2D.OverlapBoxAll(transform.position, _size, _angle);
                foreach (var obj in hit)
                {
                    if (obj.gameObject.CompareTag("Ground"))
                    {
                        Landing = true;
                        //�n�ʂɂ�����R���C�_�[�𕜊�
                        GetComponent<Collider2D>().enabled = true;
                    }
                }
            }
            else
            {
                if (!_effected)
                {
                    var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
                    foreach (var obj in hit)
                    {
                        if (obj.CompareTag("Enemy"))
                        {
                            //�G�������鏈��
                            obj.gameObject.GetComponent<Enemy>().ReactionBottle(transform.position, ActivatetTime);
                        }
                    }
                    Destroy(gameObject, ActivatetTime);
                    _effected = true;
                }
            }

        }
    }
}
