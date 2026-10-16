
using UnityEngine;

public class GolemAttackHitbox : MonoBehaviour
{
    [Header("攻撃判定")]
    public BoxCollider2D attackCollider;

    // ゴーレムのステータスを取得
    private GolemStats golemStats;

    // 同じ攻撃中に何度もダメージを与えない
    private bool hasHit = false;

    private void Start()
    {
        // 親オブジェクトからゴーレムのステータスを取得
        golemStats = GetComponentInParent<GolemStats>();

        // 開始時は攻撃判定を無効にする
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    // Animation Eventから呼ぶ
    public void StartAttackHit()
    {
        hasHit = false;

        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }

    // Animation Eventから呼ぶ
    public void EndAttackHit()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackCollider == null ||
            !attackCollider.enabled ||
            hasHit)
        {
            return;
        }

        // プレイヤーのステータスを取得
        PlayerStatus player =
            other.GetComponentInParent<PlayerStatus>();

        if (player != null && golemStats != null)
        {
            // ゴーレムのステータスに設定された攻撃力を使用
            player.TakeDamage(golemStats.attackPower);

            hasHit = true;
        }
    }
}
