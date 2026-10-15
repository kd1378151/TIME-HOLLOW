
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("攻撃判定のコライダー")]
    public BoxCollider2D attackCollider;

    // 1回の攻撃で複数回ダメージを与えないためのフラグ
    private bool hasHit = false;

    // 攻撃アニメーションの開始時に呼び出す
    public void StartAttack()
    {
        hasHit = false;

        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }

    // 攻撃アニメーションの終了時に呼び出す
    public void EndAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    // 攻撃判定に何かが触れたときに呼び出す
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // 接触した相手がプレイヤーか確認する
        PlayerStatus playerStatus =
            other.GetComponentInParent<PlayerStatus>();

        if (playerStatus == null) return;

        // この攻撃判定の親から敵のパラメータを取得する
        EnemyStats enemyStats =
            GetComponentInParent<EnemyStats>();

        if (enemyStats == null) return;

        // 1回の攻撃で1回だけダメージを与える
        hasHit = true;

        // EnemyStatsに設定した攻撃力を使用する
        playerStatus.TakeDamage(enemyStats.attackPower);

        Debug.Log("敵の攻撃力：" + enemyStats.attackPower);
    }
}
