
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    [Header("攻撃判定")]
    [SerializeField] private BoxCollider2D hitbox;

    private GrimReaperStats enemyStats;

    // 同じ攻撃中に何度もダメージを与えない
    private bool hasHitPlayer;

    private void Awake()
    {
        // 親オブジェクトから死神のステータスを取得
        enemyStats = GetComponentInParent<GrimReaperStats>();

        if (hitbox == null)
        {
            hitbox = GetComponent<BoxCollider2D>();
        }

        // 最初は攻撃判定を無効にする
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }

    // 攻撃アニメーションイベントから呼び出す
    public void EnableHitbox()
    {
        hasHitPlayer = false;

        if (hitbox != null)
        {
            hitbox.enabled = true;
        }
    }

    // 攻撃アニメーションイベントから呼び出す
    public void DisableHitbox()
    {
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤー以外には当たらない
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // 同じ攻撃で複数回ダメージを与えない
        if (hasHitPlayer)
        {
            return;
        }

        if (enemyStats == null)
        {
            Debug.LogWarning("GrimReaperStatsが見つかりません。");
            return;
        }

        // プレイヤーのHP管理スクリプトを取得
        PlayerStatus playerStatus =
            other.GetComponentInParent<PlayerStatus>();

        if (playerStatus == null)
        {
            Debug.LogWarning("PlayerStatusが見つかりません。");
            return;
        }

        // 死神のステータスから攻撃力を取得
        int damage = enemyStats.AttackPower;

        // プレイヤーにダメージを与える
        playerStatus.TakeDamage(damage);

        hasHitPlayer = true;

        Debug.Log("死神の攻撃！ ダメージ：" + damage);
    }
}
