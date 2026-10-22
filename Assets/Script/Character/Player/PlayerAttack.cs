
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    [Header("攻撃判定")]
    public BoxCollider2D attackCollider;

    private PlayerStatus playerStatus;
    private bool hasHit = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerStatus = GetComponent<PlayerStatus>();

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    void Update()
    {
        if (playerStatus != null && !playerStatus.CanControl)
        {
            return;
        }

        if (animator == null) return;

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Attack") ||
            state.IsName("Attack2"))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack1");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            animator.ResetTrigger("Attack1");
            animator.SetTrigger("Attack2");
        }
    }

    public void StartHit()
    {
        if (attackCollider == null) return;

        hasHit = false;
        attackCollider.enabled = true;
    }

    public void EndHit()
    {
        if (attackCollider == null) return;

        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackCollider == null ||
            !attackCollider.enabled ||
            hasHit)
        {
            return;
        }

        if (playerStatus == null || animator == null) return;

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        int damage;

        if (state.IsName("Attack2"))
        {
            damage = playerStatus.attackPower2;
        }
        else
        {
            damage = playerStatus.attackPower1;
        }

        // 死神への攻撃
        GrimReaperStats grimReaper =
            other.GetComponentInParent<GrimReaperStats>();


        if (grimReaper != null)
        {
            // 死神にダメージを与える
            grimReaper.TakeDamage(damage);

            // 死神を白く点滅させる
            DamageFlash damageFlash =
                grimReaper.GetComponent<DamageFlash>();

            if (damageFlash != null)
            {
                damageFlash.Flash();
            }

            hasHit = true;
            return;
        }


        // ゴーレムへの攻撃
        GolemStats golem =
            other.GetComponentInParent<GolemStats>();

        if (golem != null)
        {
            golem.TakeDamage(damage);
            hasHit = true;
            return;
        }

        // 通常の敵への攻撃
        EnemyStats enemy =
            other.GetComponentInParent<EnemyStats>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            hasHit = true;
        }
    }
}
