using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Bottle : ItemBase
{
    [SerializeField, Header("接地判定の大きさ")] Vector2 size;
    [SerializeField, Header("接地判定の角度")] float angle;
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
                    //地面についたらコライダーを復活
                    GetComponent<Collider2D>().enabled = true;
                }
            }
            else
            {
                var hit = Physics2D.OverlapCircle(transform.position, EffectRange);
                if (hit != null && hit.CompareTag("Enemy"))
                {
                    //敵が逃げる処理
                    hit.gameObject.GetComponent<Enemy>().ReactionBottle(transform.position);
                }
            }

        }
    }
}
