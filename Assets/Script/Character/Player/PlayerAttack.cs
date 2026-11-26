
using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private GameMaster gameMaster;
    private PlayerStatus playerStatus;

    [Header("攻撃判定")]
    public BoxCollider2D attackCollider;

    // 今回の攻撃ですでにダメージを与えた敵
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    // 攻撃中の種類を保持
    private bool isStrongAttack = false;

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

        if (playerStatus != null && !playerStatus.CanControl)
            return;

        if (playerStatus != null && playerStatus.IsDashActive)
            return;

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Attack") ||
            state.IsName("Attack2") ||
            animator.IsInTransition(0))
        {
            return;
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement == null || !movement.IsGrounded)
            return;

        // 通常攻撃
        if (Input.GetMouseButtonDown(0))
        {
            if (gameMaster != null && !gameMaster.CanAttack1())
                return;

            isStrongAttack = false;

            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack1");
        }
        // 強攻撃
        else if (Input.GetMouseButtonDown(1))
        {
            if (gameMaster != null && !gameMaster.CanAttack2())
                return;

            isStrongAttack = true;

            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack2");
        }
    }

    // Animation Event：攻撃判定開始
    public void StartHit()
    {
        if (attackCollider == null) return;
        if (attackCollider.enabled) return;

        hitEnemies.Clear();

        bool success = true;

        if (gameMaster != null)
        {
            if (isStrongAttack)
                success = gameMaster.ReduceAttack2Time();
            else
                success = gameMaster.ReduceAttack1Time();
        }

        if (!success) return;

        attackCollider.enabled = true;
    }

    // Animation Event：攻撃判定終了
    public void EndHit()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamageEnemy(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamageEnemy(other);
    }

    private void TryDamageEnemy(Collider2D other)
    {
        if (attackCollider == null || !attackCollider.enabled)
            return;

        if (playerStatus != null && playerStatus.IsDashActive)
            return;

        if (playerStatus == null || animator == null)
            return;

        int damage = isStrongAttack
            ? playerStatus.attackPower2
            : playerStatus.attackPower1;

        // 死神
        GrimReaperStats grimReaper =
            other.GetComponentInParent<GrimReaperStats>();

        if (grimReaper != null)
        {
            GameObject enemyObject = grimReaper.gameObject;

            if (!hitEnemies.Add(enemyObject))
                return;

            grimReaper.TakeDamage(damage);

            DamageFlash flash =
                grimReaper.GetComponent<DamageFlash>();

            if (flash != null)
                flash.Flash();

            return;
        }

        // ゴーレム ★追加
        GolemStats golem =
            other.GetComponentInParent<GolemStats>();

        if (golem != null)
        {
            GameObject enemyObject = golem.gameObject;

            if (!hitEnemies.Add(enemyObject))
                return;

            golem.TakeDamage(damage);

            Debug.Log("プレイヤーの攻撃がゴーレムに命中！ ダメージ：" + damage);

            return;
        }

        // コウモリ
        BatStats bat =
            other.GetComponentInParent<BatStats>();

        if (bat != null)
        {
            GameObject enemyObject = bat.gameObject;

            if (!hitEnemies.Add(enemyObject))
                return;

            bat.TakeDamage(damage);

            Debug.Log(
                "プレイヤーの攻撃がコウモリに命中！ ダメージ：" + damage
            );

            return;
        }

        // 通常の敵
        EnemyStats enemy =
            other.GetComponentInParent<EnemyStats>();

        if (enemy != null)
        {
            GameObject enemyObject = enemy.gameObject;

            if (!hitEnemies.Add(enemyObject))
                return;

            enemy.TakeDamage(damage);
        }
    }

    public bool IsAttackingNow()
    {
        if (animator == null) return false;

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Attack") ||
            state.IsName("Attack2"))
        {
            return true;
        }

        if (animator.IsInTransition(0))
        {
            AnimatorStateInfo next =
                animator.GetNextAnimatorStateInfo(0);

            if (next.IsName("Attack") ||
                next.IsName("Attack2"))
            {
                return true;
            }
        }

        return false;
    }
}
