using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ItemBase
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
    }
    public override void Activate()
    {
        if (IsThrowing)
        {
            var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
            foreach (var obj in hit)
            {
                if (obj.gameObject.CompareTag("Enemy"))
                {
                    //�G���X�^�������鏈��
                    obj.gameObject.GetComponent<Enemy>().ReactionStone(ActivatetTime);
                    if(obj.gameObject.GetComponent<Enemy>().State != EnemyStateType.Faint)
                    AudioManager.Instance.PlaySE("damage_enemy");
                    Destroy(gameObject,0.3f);
                }
            }
        }
    }
}
