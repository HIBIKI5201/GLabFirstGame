using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : ItemBase
{
    [SerializeField, Header("接地判定の大きさ")] Vector2 _size;
    [SerializeField, Header("接地判定の角度")] float _angle;
    bool _isGround;
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
            if (!_isGround)
            {
                var hit = Physics2D.OverlapBoxAll(transform.position, _size, _angle);
                foreach (var obj in hit)
                {
                    if (obj.gameObject.CompareTag("Ground"))
                    {
                        _isGround = true;
                        //地面についたらコライダーを復活
                        GetComponent<Collider2D>().enabled = true;
                    }
                }
            }
            else
            {
                var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
                foreach (var obj in hit)
                {
                    if (obj.gameObject.CompareTag("Enemy"))
                    {
                        //敵を誘導する処理
                        obj.gameObject.GetComponent<Enemy>().ReactionMeat(transform.position);
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
