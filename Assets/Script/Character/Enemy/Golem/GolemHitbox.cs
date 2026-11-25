
using UnityEngine;

public class GolemAttackHitbox : MonoBehaviour
{
    [Header("攻撃判定")]
    public BoxCollider2D attackCollider;

    private GolemStats golemStats;
    private bool hasHit = false;

    private void Awake()
    {
        golemStats = GetComponentInParent<GolemStats>();

        if (attackCollider == null)
        {
            attackCollider = GetComponent<BoxCollider2D>();
        }

        // 初期状態は攻撃判定OFF
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    // Animation Event：攻撃判定開始
    public void StartAttackHit()
    {
        hasHit = false;

        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }

    // Animation Event：攻撃判定終了
    public void EndAttackHit()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    // 接触した瞬間
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    // 接触中も確認
    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (attackCollider == null ||
            !attackCollider.enabled ||
            hasHit)
        {
            return;
        }

        PlayerStatus player =
            other.GetComponentInParent<PlayerStatus>();

        if (player == null) return;
        if (golemStats == null) return;

        // ダメージを与える
        player.TakeDamage(golemStats.attackPower);

        hasHit = true;

        Debug.Log("ゴーレムの攻撃がプレイヤーに命中！");
    }
}
