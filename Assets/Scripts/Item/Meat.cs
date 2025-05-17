using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : ItemBase
{
    [SerializeField, Header("�ڒn����̑傫��")] Vector2 _size;
    [SerializeField, Header("�ڒn����̊p�x")] float _angle;
    bool _effected;
    Rigidbody2D _rb;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
        Gizmos.DrawWireCube(transform.position, _size);
    }

    protected override void Activate()
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
                        AudioManager.Instance.PlaySE("meat");
                    }
                }
            }
            else
            {
                if (!_effected)
                {
                    var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
                    _rb = GetComponent<Rigidbody2D>();
                    foreach (var obj in hit)
                    {
                        if (obj.gameObject.CompareTag("Enemy"))
                        {
                            //�G��U�����鏈��
                            obj.gameObject.GetComponent<Enemy>().ReactionMeat(transform.position, ActiveTime);
                        }
                    }
                    Destroy(gameObject, ActiveTime);
                    _effected = true;
                }
                if (_effected && _rb.velocity.y == 0)
                {
                    _rb.velocity = Vector2.zero;
                    _rb.angularVelocity = 0;
                    _rb.bodyType = RigidbodyType2D.Kinematic;
                    gameObject.GetComponent<Collider2D>().enabled = false;
                }
            }
        }
    }
}
