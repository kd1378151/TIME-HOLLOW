
using UnityEngine;

public class GolemStats : MonoBehaviour, IEnemyStatus
{
    [Header("ゴーレムのパラメータ")]
    public int maxHP = 100;
    public int currentHP;
    public int attackPower = 10;

    private DamageFlash damageFlash;
    private Animator animator;
    private GolemAttackHitbox attackHitbox;
    private GolemChase golemChase;

    private bool isDead = false;

    public bool IsDead => isDead;

    // 共通HPバー用
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackHitbox = GetComponentInChildren<GolemAttackHitbox>(true);
        golemChase = GetComponent<GolemChase>();
        damageFlash = GetComponentInChildren<DamageFlash>();
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    // ダメージを受ける
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("ゴーレムHP：" + currentHP);

        if (damageFlash != null && currentHP > 0)
        {
            damageFlash.Flash();
        }

        // HPが0になったら死亡
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // 死亡処理
    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("ゴーレム死亡");

        // 移動・追跡AIを停止
        if (golemChase != null)
        {
            golemChase.enabled = false;
        }

        // Rigidbodyの移動停止
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 攻撃判定を停止
        EndAttackHit();

        // 死亡アニメーション再生
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Die");
            animator.SetTrigger("Die");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 攻撃判定ON
    public void StartAttackHit()
    {
        if (isDead) return;

        if (attackHitbox != null)
        {
            attackHitbox.StartAttackHit();
        }
    }

    // 攻撃判定OFF
    public void EndAttackHit()
    {
        if (attackHitbox != null)
        {
            attackHitbox.EndAttackHit();
        }
    }

    // 死亡アニメーション終了時のAnimation Event
    public void FinishDeath()
    {
        Destroy(gameObject);
    }
}
