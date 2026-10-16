
using UnityEngine;

public class GolemStats : MonoBehaviour
{
    [Header("ゴーレムのパラメータ")]
    public int maxHP = 100;
    public int currentHP;
    public int attackPower = 10;

    // 被弾時の白色点滅処理
    private DamageFlash damageFlash;

    private Animator animator;
    private bool isDead = false;

    private void Start()
    {
        // 初期HPを最大HPにする
        currentHP = maxHP;

        // Animatorを取得する
        animator = GetComponent<Animator>();

        damageFlash = GetComponentInChildren<DamageFlash>();
    }

    // プレイヤーから攻撃を受けたときに呼び出す
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        // ダメージを受けたら白く点滅
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }


        if (currentHP <= 0)
        {
            Die();
        }
    }

    // 死亡処理
    private void Die()
    {
        isDead = true;

        // 死亡アニメーションを再生する
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    // 死亡アニメーション終了時に呼び出す
    public void FinishDeath()
    {
        Destroy(gameObject);
    }

    // 攻撃判定のスクリプト
    private GolemAttackHitbox attackHitbox;

    // 攻撃判定を取得
    private void Awake()
    {
        attackHitbox = GetComponentInChildren<GolemAttackHitbox>();
    }

    // Animation Eventから呼び出す：攻撃判定ON
    public void StartAttackHit()
    {
        if (attackHitbox != null)
        {
            attackHitbox.StartAttackHit();
        }
    }

    // Animation Eventから呼び出す：攻撃判定OFF
    public void EndAttackHit()
    {
        if (attackHitbox != null)
        {
            attackHitbox.EndAttackHit();
        }
    }
}
