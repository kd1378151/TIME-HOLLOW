using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BatStats : MonoBehaviour, IEnemyStatus
{
    [Header("コウモリのステータス")]
    public int maxHP = 20;
    public int currentHP;
    public int attackPower = 5;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    [Header("死亡設定")]
    public string deathAnimationName = "BatDie";

    private bool isDead = false;

    private BatAI batAI;
    private Animator animator;
    private Rigidbody2D rb;
    private DamageFlash damageFlash;
    private Collider2D[] colliders;

    // 同じ突進中にプレイヤーへ重複ダメージを与えない
    private bool hasDamagedPlayerThisCharge = false;

    void Start()
    {
        currentHP = maxHP;

        batAI = GetComponent<BatAI>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        damageFlash = GetComponentInChildren<DamageFlash>();
        colliders = GetComponentsInChildren<Collider2D>(true);
    }

    // =====================================
    // プレイヤーの攻撃を受けたとき
    // =====================================

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("コウモリのHP：" + currentHP);

        // ダメージを受けたら白く点滅
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        Debug.Log("コウモリがダメージを受けた！");
    }

    // =====================================
    // 突進開始時に攻撃済み記録をリセット
    // =====================================

    public void ResetChargeHit()
    {
        hasDamagedPlayerThisCharge = false;
    }

    // =====================================
    // 突進中にプレイヤーへダメージ
    // =====================================

    public void TryDamagePlayer(Collider2D other)
    {
        if (isDead) return;

        if (batAI == null || !batAI.IsCharging)
            return;

        if (hasDamagedPlayerThisCharge)
            return;

        PlayerStatus playerStatus = null;

        // プレイヤー本体から取得
        if (batAI.player != null)
        {
            playerStatus =
                batAI.player.GetComponent<PlayerStatus>();
        }

        // 見つからなければ衝突したオブジェクトの親から取得
        if (playerStatus == null && other != null)
        {
            playerStatus =
                other.GetComponentInParent<PlayerStatus>();
        }

        if (playerStatus == null)
            return;

        hasDamagedPlayerThisCharge = true;

        playerStatus.TakeDamage(attackPower);

        Debug.Log("コウモリの突進がプレイヤーに命中！ ダメージ：" + attackPower);
    }

    // =====================================
    // 死亡処理
    // =====================================

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("コウモリが死亡！");

        // AI停止
        if (batAI != null)
        {
            batAI.enabled = false;
        }

        // 移動停止
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 全ての当たり判定を停止
        foreach (Collider2D col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        // 死亡アニメーション再生
        if (animator != null)
        {
            animator.speed = 1f;
            animator.Play(deathAnimationName, 0, 0f);
        }
        else
        {
            Debug.LogWarning("コウモリのAnimatorが見つかりません！");
            Destroy(gameObject);
        }
    }

    // =====================================
    // 死亡アニメーション終了イベント
    // =====================================

    public void OnDeathAnimationEnd()
    {
        if (!isDead) return;

        Debug.Log("コウモリの死亡アニメーション終了！");

        Destroy(gameObject);
    }
}
