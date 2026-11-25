
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

    [Header("攻撃判定")]
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        originalScale = transform.localScale;
        patrolStartPosition = transform.position;

        patrolWaitTimer = patrolWaitTime;

        // 攻撃判定はColliderを無効にする
        if (attackHitbox != null)
        {
            GolemAttackHitbox hitbox =
                attackHitbox.GetComponent<GolemAttackHitbox>();

            if (hitbox != null)
            {
                hitbox.EndAttackHit();
            }
            else
            {
                attackHitbox.SetActive(false);
            }
        }

        UpdateMovementAnimation(false);
    }


    void Update()
    {
        if (player == null || rb == null) return;

        float distanceX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(distanceX);

        float facingDirection = Mathf.Sign(transform.localScale.x);
        bool isInFront = distanceX * facingDirection > 0f;

        bool playerDetected =
            isInFront && distance <= detectionRange;

        // 発見マーク
        if (playerDetected && !wasPlayerDetected)
        {
            Debug.Log("ゴーレムがプレイヤーを発見しました");

            if (enemyAlert != null)
            {
                enemyAlert.ShowAlert();
            }
        }

        wasPlayerDetected = playerDetected;

        // 攻撃中は行動を変更しない
        if (currentState == GolemState.Attack)
        {
            StopHorizontalMovement();
            UpdateMovementAnimation(false);
            return;
        }

        // クールダウン処理
        if (currentState == GolemState.Cooldown)
        {
            cooldownTimer -= Time.deltaTime;

            StopHorizontalMovement();
            UpdateMovementAnimation(false);

            if (cooldownTimer <= 0f)
            {
                if (distance <= detectionRange)
                {
                    currentState = GolemState.Chase;
                }
                else
                {
                    currentState = GolemState.Idle;
                    patrolWaitTimer = patrolWaitTime;
                }
            }

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

        switch (currentState)
        {
            case GolemState.Idle:

                // 待機時間をカウント
                patrolWaitTimer -= Time.deltaTime;

                if (patrolWaitTimer <= 0f)
                {
                    // 待機終了後に反転
                    patrolDirection *= -1;

                    FaceDirection(patrolDirection);

                    // 現在位置を次の徘徊開始位置にする
                    patrolStartPosition = transform.position;

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
                    // 移動を停止
                    StopHorizontalMovement();
                    UpdateMovementAnimation(false);

                    // 待機状態に移行
                    patrolWaitTimer = patrolWaitTime;
                    currentState = GolemState.Idle;
                }

                break;

            case GolemState.Chase:

                if (distance <= attackRange)
                {
                    StartAttack();
                }
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

                float distanceX =
                    player.position.x - rb.position.x;

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

    void FaceDirection(float direction)
    {
        if (direction == 0f) return;

        Vector3 scale = originalScale;

        scale.x = Mathf.Abs(originalScale.x) * direction;

        transform.localScale = scale;
    }

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

    void UpdateMovementAnimation(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", isMoving ? 1f : 0f);
        }
    }

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

    // 攻撃アニメーション終了時のAnimation Event
    public void FinishAttack()
    {
        if (currentState != GolemState.Attack) return;

        // 念のため攻撃判定を終了
        DisableAttackHitbox();

        currentState = GolemState.Cooldown;
        cooldownTimer = cooldownDuration;

        StopHorizontalMovement();
        UpdateMovementAnimation(false);
    }

    // 攻撃判定ON
    public void EnableAttackHitbox()
    {
        if (attackHitbox == null) return;

        GolemAttackHitbox hitbox =
            attackHitbox.GetComponent<GolemAttackHitbox>();

        if (hitbox != null)
        {
            hitbox.StartAttackHit();
        }
        else
        {
            attackHitbox.SetActive(true);
        }
    }

    // 攻撃判定OFF
    public void DisableAttackHitbox()
    {
        if (attackHitbox == null) return;

        GolemAttackHitbox hitbox =
            attackHitbox.GetComponent<GolemAttackHitbox>();

        if (hitbox != null)
        {
            hitbox.EndAttackHit();
        }
        else
        {
            attackHitbox.SetActive(false);
        }
    }
}
