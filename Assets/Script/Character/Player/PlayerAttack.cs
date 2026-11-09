
using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    private GameMaster gameMaster;

    [Header("攻撃判定")]
    public BoxCollider2D attackCollider;

    private PlayerStatus playerStatus;

    // 今回の攻撃ですでにダメージを与えた敵
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Start()
    {
        gameMaster = FindAnyObjectByType<GameMaster>();
        animator = GetComponentInChildren<Animator>();
        playerStatus = GetComponent<PlayerStatus>();

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }


    void Update()
    {
        if (animator == null) return;

        // 操作不能中は入力を受け付けない
        if (playerStatus != null && !playerStatus.CanControl)
        {
            return;
        }

        // 攻撃中・攻撃への遷移中は入力を受け付けない
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Attack") ||
            state.IsName("Attack2") ||
            animator.IsInTransition(0))
        {
            return;
        }

        // 空中では攻撃できない
        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement == null || !movement.IsGrounded)
        {
            return;
        }

        // 左クリック：通常攻撃
        if (Input.GetMouseButtonDown(0))
        {
            if (gameMaster != null && !gameMaster.CanAttack1())
            {
                return;
            }

            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack1");
        }
        // 右クリック：強攻撃
        else if (Input.GetMouseButtonDown(1))
        {
            if (gameMaster != null && !gameMaster.CanAttack2())
            {
                return;
            }

            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack2");
        }
    }


    public void StartHit()
    {
        if (attackCollider == null) return;
        if (attackCollider.enabled) return;

        hitEnemies.Clear();

        bool success = true;

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Attack2"))
        {
            if (gameMaster != null)
                success = gameMaster.ReduceAttack2Time();
        }
        else
        {
            if (gameMaster != null)
                success = gameMaster.ReduceAttack1Time();
        }

        if (!success) return;

        attackCollider.enabled = true;
    }

    public void EndHit()
    {
        if (attackCollider == null) return;

        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackCollider == null || !attackCollider.enabled)
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
            GameObject enemyObject = grimReaper.gameObject;

            if (!hitEnemies.Add(enemyObject)) return;

            grimReaper.TakeDamage(damage);

            DamageFlash damageFlash =
                grimReaper.GetComponent<DamageFlash>();

            if (damageFlash != null)
            {
                damageFlash.Flash();
            }

            return;
        }

        // 通常の敵への攻撃
        EnemyStats enemy =
            other.GetComponentInParent<EnemyStats>();

        if (enemy != null)
        {
            GameObject enemyObject = enemy.gameObject;

            if (!hitEnemies.Add(enemyObject)) return;

            enemy.TakeDamage(damage);
        }
    }
}
