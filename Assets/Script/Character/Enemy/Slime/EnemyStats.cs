
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

    private EnemyExperience enemyExperience;
    private bool experienceGiven = false;

    // Hurtアニメーションの監視用
    private bool wasHurtPlaying = false;

    void Start()
    {
        currentHP = maxHP;

        animator = GetComponent<Animator>();
        damageFlash = GetComponentInChildren<DamageFlash>();
        enemyAI = GetComponent<EnemyAI>();
        enemyExperience = GetComponent<EnemyExperience>();
    }

    void Update()
    {
        if (isDead || !IsTakingHit || animator == null)
        {
            return;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // Hurtアニメーションが再生中か確認
        if (state.IsName("Hurt"))
        {
            wasHurtPlaying = true;

            // Hurtの再生が終わったら、よろけ状態を解除
            if (state.normalizedTime >= 1f &&
                !animator.IsInTransition(0))
            {
                EndHitReaction();
            }
        }
        else if (wasHurtPlaying && !animator.IsInTransition(0))
        {
            // Hurtから別の状態に移った場合も解除
            EndHitReaction();
        }
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

        // HPが0になったら死亡処理
        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // ダメージを受けたら点滅
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        // よろけ可能ならHurtを再生
        if (canFlinch && animator != null)
        {
            IsTakingHit = true;
            wasHurtPlaying = false;
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
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
        if (!IsTakingHit)
        {
            return;
        }

        IsTakingHit = false;
        wasHurtPlaying = false;
        Debug.Log("よろけ終了：移動を再開できます");
    }
}