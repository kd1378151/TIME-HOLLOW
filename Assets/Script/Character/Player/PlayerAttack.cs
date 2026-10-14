
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackCollider == null ||
            !attackCollider.enabled ||
            hasHit)
        {
            return;
        }

        // 接触した相手、または親オブジェクトから敵のHPを取得
        EnemyStats enemy = other.GetComponentInParent<EnemyStats>();

        if (enemy != null)
        {
            // 敵にダメージを与える
            enemy.TakeDamage(playerStatus.attackPower);
            // 1回の攻撃で同じ敵に何度もダメージを与えない
            hasHit = true;
        }
    }
}
