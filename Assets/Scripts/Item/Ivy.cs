using System.Collections;
using UnityEngine;

/// <summary>
/// �A�C�e���F�c�^
/// </summary>
public class Ivy : ItemBase
{
    [SerializeField] private float bottom = -10f;
    [SerializeField] private float effectTime = 1f; // ���ʎ���

    private bool IsBottom()
    {
        return transform.position.y < bottom;
    }

    private void Start()
    {
        StartCoroutine(BottomCheck());
    }

    private IEnumerator BottomCheck()
    {
        Debug.Log("�ޗ��ɂ͂܂������Ă��Ȃ�");
        yield return new WaitUntil(IsBottom); // ������true�ɂȂ�܂ő҂�
        Debug.Log("�ޗ��ɗ�����");
        Destroy(gameObject);
    }

    protected override void Activate()
    {

        // �����Ă��Ȃ����ł���΁A�ȍ~�̏����͍s��Ȃ�
        //if (!IsThrowing) return;

        var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);

        foreach (var obj in hit)
        {
            if (obj.CompareTag("Enemy"))
            {
                if (obj.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.ReactionStone(effectTime);

                    if (enemy.State != EnemyStateType.Faint)
                    {
                        // �܂��G���C���Ԃł͂Ȃ���΁ASE���Đ�����
                        AudioManager.Instance.PlaySE("damage_enemy");
                    }
                }

                Destroy(gameObject, 0f);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
    }
}
