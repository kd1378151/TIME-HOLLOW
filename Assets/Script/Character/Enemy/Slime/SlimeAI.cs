
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("追跡ターゲット")]
    public Transform player;

    [Header("移動設定")]
    public float moveSpeed = 2f;
    public float chaseRange = 6f;
    public float attackRange = 1.2f;

    [Header("攻撃・待機設定")]
    public float attackInterval = 2.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 攻撃判定の処理
    public EnemyAttack enemyAttack;

    // 攻撃中かどうか
    private bool isAttacking = false;

    // 攻撃後の待機中かどうか
    private bool isWaiting = false;

    // 待機が終了する時刻
    private float waitEndTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ターゲットが設定されていなければ停止
        if (player == null)
        {
            StopMoving();
            return;
        }

        // 攻撃中は、プレイヤーが離れても移動しない
        if (isAttacking)
        {
            StopMoving();

            // Attackアニメーションが最後まで再生されたか確認
            if (animator != null)
            {
                AnimatorStateInfo state =
                    animator.GetCurrentAnimatorStateInfo(0);

                if (state.IsName("Attack") &&
                    state.normalizedTime >= 1f)
                {
                    // 攻撃終了後、待機時間を開始
                    isAttacking = false;
                    isWaiting = true;
                    waitEndTime = Time.time + attackInterval;
                }
            }

            return;
        }

        // 攻撃後の待機中は、時間が経過するまで停止
        if (isWaiting)
        {
            StopMoving();

            if (Time.time >= waitEndTime)
            {
                isWaiting = false;
            }

            return;
        }

        // プレイヤーとの距離を計算
        float distance =
            Vector2.Distance(transform.position, player.position);

        // 追跡範囲外なら停止
        if (distance > chaseRange)
        {
            StopMoving();
            return;
        }

        // プレイヤーの方向を向く
        float direction = player.position.x - transform.position.x;

        if (spriteRenderer != null)
        {
            // 今のスライム素材に合わせた向き
            if (direction < 0)
                spriteRenderer.flipX = false;
            else if (direction > 0)
                spriteRenderer.flipX = true;
        }

        // 攻撃範囲内なら停止して攻撃
        if (distance <= attackRange)
        {
            StopMoving();

            if (animator != null)
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
            }

            return;
        }

        // 攻撃範囲外ならプレイヤーを追いかける
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                Mathf.Sign(direction) * moveSpeed,
                rb.linearVelocity.y
            );
        }

        // 移動アニメーション
        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
        }
    }

    // 敵をその場で停止させる
    void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    // Animation Event：攻撃判定を開始
    public void StartAttackHit()
    {
        if (enemyAttack != null)
        {
            enemyAttack.StartAttack();
        }
    }

    // Animation Event：攻撃判定を終了
    public void EndAttackHit()
    {
        if (enemyAttack != null)
        {
            enemyAttack.EndAttack();
        }
    }
}
