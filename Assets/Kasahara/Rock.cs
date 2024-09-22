using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ItemBase
{
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
    }
    public override void Activate()
    {
        if (IsThrowing)
        {
            var hit = Physics2D.OverlapCircle(transform.position, EffectRange);
            if (hit != null && hit.gameObject.CompareTag("Enemy"))
            {
                //ìGÇÉXÉ^ÉìÇ≥ÇπÇÈèàóù
                hit.gameObject.GetComponent<Enemy>().ReactionStone(ActivateTtime);
            }
        }
    }
}
