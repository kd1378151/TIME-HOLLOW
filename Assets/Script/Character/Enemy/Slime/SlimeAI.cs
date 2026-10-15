
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("追跡ターゲット")]
    public Transform player;

    [Header("移動設定")]
    public float moveSpeed = 2f;
    public float chaseRange = 6f;
    public float attackRange = 1.2f;

    [Header("徘徊設定")]
    public float patrolDistance = 2f;   // 初期位置から徘徊する距離
    public float patrolSpeed = 1f;      // 徘徊中の移動速度
    public float patrolMoveTime = 2f;   // 何秒歩くか
    public float patrolWaitTime = 1.5f; // 何秒立ち止まるか

    [Header("攻撃・待機設定")]
    public float attackInterval = 2.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public EnemyAttack enemyAttack;

    private bool isAttacking = false;
    private bool isWaiting = false;
    private float waitEndTime;

    // 徘徊用の変数
    private float startX;
    private int patrolDirection = 1;
    private bool isPatrolWaiting = false;
    private float patrolTimer;

    // 発見演出
    private EnemyAlert alertSystem;
    private bool wasChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 敵が最初にいた位置を記録
        startX = transform.position.x;

        // 最初は少し歩く
        patrolTimer = patrolMoveTime;

        //発見時
        alertSystem = GetComponent<EnemyAlert>();
    }

    void Update()
    {
        // 攻撃中は移動しない
        if (isAttacking)
        {
            StopMoving();

            if (animator != null)
            {
                AnimatorStateInfo state =
                    animator.GetCurrentAnimatorStateInfo(0);

                if (state.IsName("Attack") &&
                    state.normalizedTime >= 1f)
                {
                    isAttacking = false;
                    isWaiting = true;
                    waitEndTime = Time.time + attackInterval;
                }
            }

            return;
        }

        // 攻撃後の待機
        if (isWaiting)
        {
            // 待機時間が終わったら通常の移動処理に戻る
            if (Time.time >= waitEndTime)
            {
                isWaiting = false;
            }
            else
            {
                StopMoving();
                return;
            }
        }

        // プレイヤーが設定されている場合
        if (player != null)
        {
            // プレイヤーとの距離を計算
            float distance =
                Vector2.Distance(transform.position, player.position);

            // プレイヤーが追跡範囲内にいるか確認
            if (distance <= chaseRange)
            {
                // プレイヤーが敵の右側にいるか確認
                bool playerIsRight =
                    player.position.x > transform.position.x;

                // 敵が向いている方向とプレイヤーの位置が一致するか
                bool facingPlayer =
                    (spriteRenderer.flipX && playerIsRight) ||
                    (!spriteRenderer.flipX && !playerIsRight);

                // 向いている方向にプレイヤーがいる場合だけ追尾・攻撃
                if (facingPlayer)
                {
                    // 今回、新しく追跡を開始したときだけ表示
                    if (!wasChasing)
                    {
                        if (alertSystem != null)
                        {
                            alertSystem.ShowAlert();
                        }

                        wasChasing = true;
                    }

                    float direction =
                        player.position.x - transform.position.x;

                    // 攻撃範囲内なら攻撃
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

                    // 追尾する
                    if (rb != null)
                    {
                        rb.linearVelocity = new Vector2(
                            Mathf.Sign(direction) * moveSpeed,
                            rb.linearVelocity.y
                        );
                    }

                    if (animator != null)
                    {
                        animator.SetFloat("Speed", 1f);
                    }

                    return;
                }
            }
        }

        // 条件に合わなければ徘徊を続ける
        Patrol();
        // 追跡していない状態に戻す
        wasChasing = false;
    }

    // 左右に徘徊する

    void Patrol()
    {
        // 待機中なら、時間が経過するまで停止
        if (isPatrolWaiting)
        {
            StopMoving();
            patrolTimer -= Time.deltaTime;

            if (patrolTimer <= 0f)
            {
                isPatrolWaiting = false;

                // 反対方向を向く
                patrolDirection *= -1;

                // 次に歩く時間を設定
                patrolTimer = patrolMoveTime;
            }

            return;
        }

        // 初期位置からどれだけ離れているか
        float distanceFromStart =
            transform.position.x - startX;

        // 徘徊範囲の端に到達したら待機する
        if (patrolDirection > 0 &&
            distanceFromStart >= patrolDistance)
        {
            StartPatrolWait();
            return;
        }

        if (patrolDirection < 0 &&
            distanceFromStart <= -patrolDistance)
        {
            StartPatrolWait();
            return;
        }

        // 徘徊方向に向きを変える
        FaceDirection(patrolDirection);

        // 徘徊方向へ移動する
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                patrolDirection * patrolSpeed,
                rb.linearVelocity.y
            );
        }

        // 歩行アニメーション
        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
        }
    }


    // 徘徊中の停止を開始
    void StartPatrolWait()
    {
        isPatrolWaiting = true;
        patrolTimer = patrolWaitTime;
        StopMoving();
    }

    // 向きを変更する
    void FaceDirection(float direction)
    {
        if (spriteRenderer == null || direction == 0f)
        {
            return;
        }

        // 今のスライム素材に合わせた向き
        if (direction < 0f)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    // 敵を停止させる
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
