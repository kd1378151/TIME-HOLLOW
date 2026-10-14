
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("攻撃設定")]
    public int attackPower = 10;

    [Header("攻撃判定のコライダー")]
    public BoxCollider2D attackCollider;

    // 1回の攻撃でダメージを与えたか
    private bool hasHit = false;

    // 攻撃判定を開始
    public void StartAttack()
    {
        hasHit = false;

        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }

    // 攻撃判定を終了
    public void EndAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    // 攻撃判定が何かに触れたとき
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        // 接触した相手からPlayerStatusを探す
        PlayerStatus playerStatus =
            other.GetComponentInParent<PlayerStatus>();

        if (playerStatus == null)
        {
            return;
        }

        // 1回の攻撃で1回だけダメージを与える
        hasHit = true;

        // 敵の攻撃力をプレイヤーに与える
        playerStatus.TakeDamage(attackPower);

    }
}
