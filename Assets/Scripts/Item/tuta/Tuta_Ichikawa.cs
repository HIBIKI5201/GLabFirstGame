using UnityEngine;

public partial class tuta : MonoBehaviour
{
    private Collider2D col;       // 自分のCollider2Dコンポーネント
    private Rigidbody2D rb;       // 自分のRigidbody2Dコンポーネント

    private bool isFalling = false;   // 落下中かどうかのフラグ

    void Awake()
    {
        // Awakeでコンポーネントを取得（キャッシュしておく）
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Rigidbody2DのY軸速度がマイナスなら落下中と判断
        isFalling = rb.velocity.y < 0f;

        // 地面に接触しているかチェック
        bool isGrounded = IsGrounded();

        // 落下中じゃなくて、地面に接触している時だけ接触判定を有効にする
        if (!isFalling && isGrounded)
        {
            col.enabled = true;   // 判定ON
        }
        else
        {
            col.enabled = false;  // 判定OFF（落下中や空中は判定なし）
        }
    }

    // 地面に接触しているかをRaycastで判定するメソッド
    private bool IsGrounded()
    {
        // 自分の位置から真下に0.1単位だけRaycastを飛ばす
        // "Ground"レイヤーのものだけに反応する設定
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        // 地面に当たったらtrue、そうでなければfalseを返す
        return hit.collider != null;
    }

    // 他のColliderが接触した時に呼ばれるメソッド（Trigger設定されている場合）
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 落下中は判定しない（処理しない）
        if (isFalling)
        {
            return;
        }
        // 当たった相手がプレイヤーだったら何もしない（スルー）
        if (other.CompareTag("Player"))
        {
            return;
        }
        // 当たった相手がエネミーなら判定OK、処理を行う
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemyに当たった！");

            // ここにエネミーに対する攻撃処理やダメージ処理を書く
        }
    }
}
