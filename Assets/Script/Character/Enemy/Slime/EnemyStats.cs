
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("敵の基本パラメータ")]
    public int maxHP = 30;
    public int currentHP;
    public int attackPower = 5;

    [Header("よろけ設定")]
    public bool canFlinch = true;
    public bool IsTakingHit { get; private set; } = false;

    private EnemyAI enemyAI;
    private Animator animator;
    private bool isDead = false;
    private DamageFlash damageFlash;

    // 経験値を渡すための設定
    private EnemyExperience enemyExperience;
    private bool experienceGiven = false;

    void Start()
    {
        currentHP = maxHP;

        animator = GetComponent<Animator>();
        damageFlash = GetComponentInChildren<DamageFlash>();
        enemyAI = GetComponent<EnemyAI>();

        // 敵が持つ経験値を取得
        enemyExperience = GetComponent<EnemyExperience>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        if (enemyAI != null)
        {
            enemyAI.DetectPlayerWhenAttacked();
        }

        if (currentHP <= 0)
        {
            Die();
        }
        else if (canFlinch && animator != null)
        {
            IsTakingHit = true;
            animator.SetTrigger("Hurt");
        }

        if (damageFlash != null)
        {
            damageFlash.Flash();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("① 敵の死亡処理が呼ばれました");

        if (!experienceGiven)
        {
            experienceGiven = true;

            PlayerExperience playerExperience =
                FindAnyObjectByType<PlayerExperience>();

            Debug.Log("② EnemyExperience: " + (enemyExperience != null));
            Debug.Log("③ PlayerExperience: " + (playerExperience != null));

            if (enemyExperience != null && playerExperience != null)
            {
                Debug.Log("④ 獲得経験値: " + enemyExperience.experienceReward);

                playerExperience.AddExperience(
                    enemyExperience.experienceReward
                );
            }
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }


    public void FinishDeath()
    {
        Destroy(gameObject);
    }

    public void EndHitReaction()
    {
        IsTakingHit = false;
        Debug.Log("よろけ終了：移動を再開できます");
    }
}
