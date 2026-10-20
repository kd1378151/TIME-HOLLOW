
using UnityEngine;

public class GolemChase : MonoBehaviour
{
    [Header("追いかける設定")]
    public float detectionRange = 5f;
    public float moveSpeed = 2f;

    [Header("攻撃設定")]
    public float attackRange = 1.5f;
    public float cooldownDuration = 2f;

    [Header("徘徊設定")]
    public float patrolDistance = 3f;
    public float patrolSpeed = 1f;
    public float patrolWaitTime = 2f;

    [Header("発見マーク")]
    public EnemyAlert enemyAlert;

    [Header("対象")]
    public Transform player;

    [Header("攻撃判定（既存の設定を維持）")]
    public GameObject attackHitbox;

    private enum GolemState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Cooldown
    }

    private GolemState currentState = GolemState.Idle;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;

    private Vector2 patrolStartPosition;
    private int patrolDirection = 1;

    private float patrolWaitTimer;
    private float cooldownTimer;

    private bool wasPlayerDetected = false;

    // 初期設定
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        originalScale = transform.localScale;
        patrolStartPosition = transform.position;

        patrolWaitTimer = patrolWaitTime;

        // 攻撃判定は最初は無効
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        UpdateMovementAnimation(false);
    }

    void Update()
    {
        if (player == null || rb == null) return;

        // プレイヤーとの距離を計算
        float distanceX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(distanceX);

        // ゴーレムが向いている方向にプレイヤーがいるか確認
        float facingDirection = Mathf.Sign(transform.localScale.x);
        bool isInFront = distanceX * facingDirection > 0f;
        bool playerDetected = isInFront && distance <= detectionRange;

        // プレイヤーを初めて発見したとき
        if (playerDetected && !wasPlayerDetected)
        {
            Debug.Log("ゴーレムがプレイヤーを発見しました");

            if (enemyAlert != null)
            {
                enemyAlert.ShowAlert();
            }
        }

        wasPlayerDetected = playerDetected;

        // 攻撃中・攻撃後の待機中は新しい行動を開始しない
        if (currentState == GolemState.Attack ||
            currentState == GolemState.Cooldown)
        {
            UpdateMovementAnimation(false);
            return;
        }

        // プレイヤーを発見したら追尾または攻撃
        if (playerDetected)
        {
            FaceDirection(Mathf.Sign(distanceX));

            if (distance <= attackRange)
            {
                StartAttack();
                return;
            }

            currentState = GolemState.Chase;
        }

        // 現在の行動を処理
        switch (currentState)
        {
            case GolemState.Idle:
                patrolWaitTimer -= Time.deltaTime;

                if (patrolWaitTimer <= 0f)
                {
                    currentState = GolemState.Patrol;
                }
                break;

            case GolemState.Patrol:
                float patrolLimit =
                    patrolStartPosition.x +
                    patrolDirection * patrolDistance;

                bool reachedEdge =
                    patrolDirection > 0
                    ? transform.position.x >= patrolLimit
                    : transform.position.x <= patrolLimit;

                if (reachedEdge)
                {
                    patrolDirection *= -1;
                    FaceDirection(patrolDirection);

                    patrolWaitTimer = patrolWaitTime;
                    currentState = GolemState.Idle;
                }
                break;

            case GolemState.Chase:
                // 追尾中に攻撃範囲へ入ったら攻撃
                if (distance <= attackRange)
                {
                    StartAttack();
                }
                // 十分に離れたら追尾を終了
                else if (distance > detectionRange)
                {
                    currentState = GolemState.Idle;
                    patrolWaitTimer = patrolWaitTime;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // 状態に応じて実際の移動速度を設定
        switch (currentState)
        {
            case GolemState.Idle:
            case GolemState.Attack:
            case GolemState.Cooldown:
                StopHorizontalMovement();
                UpdateMovementAnimation(false);
                break;

            case GolemState.Patrol:
                rb.linearVelocity = new Vector2(
                    patrolDirection * patrolSpeed,
                    rb.linearVelocity.y
                );

                FaceDirection(patrolDirection);
                UpdateMovementAnimation(patrolSpeed > 0f);
                break;

            case GolemState.Chase:
                if (player == null)
                {
                    StopHorizontalMovement();
                    UpdateMovementAnimation(false);
                    break;
                }

                // プレイヤーの左右に合わせて向きを変える
                float distanceX = player.position.x - rb.position.x;

                if (Mathf.Abs(distanceX) > 0.01f)
                {
                    float direction = Mathf.Sign(distanceX);

                    FaceDirection(direction);

                    rb.linearVelocity = new Vector2(
                        direction * moveSpeed,
                        rb.linearVelocity.y
                    );

                    UpdateMovementAnimation(moveSpeed > 0f);
                }
                else
                {
                    StopHorizontalMovement();
                    UpdateMovementAnimation(false);
                }
                break;
        }
    }

    // 向きを変更
    void FaceDirection(float direction)
    {
        if (direction == 0f) return;

        Vector3 scale = originalScale;
        scale.x = Mathf.Abs(originalScale.x) * direction;
        transform.localScale = scale;
    }

    // 攻撃を開始
    void StartAttack()
    {
        currentState = GolemState.Attack;

        StopHorizontalMovement();
        UpdateMovementAnimation(false);

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
    }

    // 移動アニメーションを実際の行動に合わせる
    void UpdateMovementAnimation(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", isMoving ? 1f : 0f);
        }
    }

    // 横方向の移動を停止
    void StopHorizontalMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }
    }

    // 攻撃アニメーションの終了時に呼ぶ
    public void FinishAttack()
    {
        if (currentState != GolemState.Attack) return;

        currentState = GolemState.Cooldown;
        cooldownTimer = cooldownDuration;

        StopHorizontalMovement();
        UpdateMovementAnimation(false);
    }

    // 攻撃判定を有効化（既存のAnimation Event用）
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }
    }

    // 攻撃判定を無効化（既存のAnimation Event用）
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }
}
