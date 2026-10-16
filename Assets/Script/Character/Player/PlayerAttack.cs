
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    [Header("攻撃判定")]
    public BoxCollider2D attackCollider;

    // プレイヤーのパラメータを管理するスクリプト
    private PlayerStatus playerStatus;

    // 攻撃判定が有効な間、同じ敵への連続ヒットを防ぐ
    private bool hasHit = false;

    void Start()
    {
        // 子オブジェクトにあるAnimatorを取得
        animator = GetComponentInChildren<Animator>();

        //プレイヤーのパラメータ
        playerStatus = GetComponent<PlayerStatus>();

        // ゲーム開始時は攻撃判定を無効にする
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

        // 現在のアニメーションステートを取得
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        // 攻撃アニメーション中は新しい攻撃を受け付けない
        if (state.IsName("Attack") ||
            state.IsName("Attack2"))
        {
            return;
        }

        // 攻撃1：Jキー
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack1");
        }
        // 攻撃2：Kキー
        else if (Input.GetKeyDown(KeyCode.K))
        {
            animator.ResetTrigger("Attack1");
            animator.SetTrigger("Attack2");
        }
    }

    // Animation Eventから呼び出す：攻撃判定を有効にする
    public void StartHit()
    {
        if (attackCollider == null) return;

        hasHit = false;
        attackCollider.enabled = true;
    }

    // Animation Eventから呼び出す：攻撃判定を無効にする
    public void EndHit()
    {
        if (attackCollider == null) return;

        attackCollider.enabled = false;
    }

    // 攻撃判定が敵に触れたとき

    // 攻撃判定が敵に触れたとき
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackCollider == null ||
            !attackCollider.enabled ||
            hasHit)
        {
            return;
        }

        if (playerStatus == null) return;

        // 現在の攻撃アニメーションを取得
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        // 攻撃1か攻撃2かでダメージを決める
        int damage;

        if (state.IsName("Attack2"))
        {
            damage = playerStatus.attackPower2;
        }
        else
        {
            damage = playerStatus.attackPower1;
        }

        // ゴーレム専用のステータスを取得
        GolemStats golem = other.GetComponentInParent<GolemStats>();

        if (golem != null)
        {
            // ゴーレムにダメージを与える
            golem.TakeDamage(damage);

            hasHit = true;
            return;
        }

        // 既存の敵ステータスも引き続き利用する
        EnemyStats enemy = other.GetComponentInParent<EnemyStats>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            hasHit = true;
        }
    }

}
