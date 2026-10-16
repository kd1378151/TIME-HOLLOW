
using UnityEngine;

public class GolemChase : MonoBehaviour
{
    [Header("追いかける設定")]
    public float detectionRange = 5f; // プレイヤーを見つける距離
    public float moveSpeed = 2f;      // ゴーレムの移動速度

    [Header("攻撃設定")]
    public float attackRange = 1.5f;   // 攻撃が届く距離

    // ゴーレムの行動状態
    private enum GolemState
    {
        Idle,       // 待機
        Patrol,     // 徘徊
        Chase,      // 追尾
        Attack,     // 攻撃
        Cooldown    // 攻撃後の待機
    }

    private GolemState currentState = GolemState.Idle;

    [Header("徘徊設定")]
    public float patrolDistance = 3f;       // 初期位置から徘徊する距離
    public float patrolSpeed = 1f;          // 徘徊中の移動速度
    public float patrolWaitTime = 2f;       // 方向転換前の待機時間

    private Vector2 patrolStartPosition;    // 徘徊の基準位置
    private float patrolWaitTimer = 0f;     // 徘徊中の待機タイマー
    private int patrolDirection = 1;        // 1なら右、-1なら左

    // 攻撃後の待機時間
    public float cooldownDuration = 2f;
    private float cooldownTimer = 0f;

    // 攻撃判定
    public GameObject attackHitbox;

    [Header("発見マーク")]
    public EnemyAlert enemyAlert;

    // プレイヤーを発見済みか
    private bool wasPlayerDetected = false;

    [Header("対象")]
    public Transform player;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;

    void Start()
    {
        // 必要なコンポーネントを取得する
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 最初の大きさと向きを保存する
        originalScale = transform.localScale;

        //徘徊
        patrolStartPosition = transform.position;
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        // プレイヤーがゴーレムの左右どちらにいるか
        float distanceX = player.position.x - transform.position.x;

        // プレイヤーとの距離を調べる
        float distance = Mathf.Abs(distanceX);

        // ゴーレムが現在向いている方向を取得
        float facingDirection = Mathf.Sign(transform.localScale.x);

        // ゴーレムの正面側にプレイヤーがいるか
        bool isInFront = distanceX * facingDirection > 0f;

        // 正面側かつ一定距離以内なら発見
        bool playerDetected =
            isInFront && Mathf.Abs(distanceX) <= detectionRange;

        // 発見判定の確認
        if (playerDetected && !wasPlayerDetected)
        {
            if (enemyAlert != null)
            {
                enemyAlert.ShowAlert();
            }
        }

        wasPlayerDetected = playerDetected;

        // 現在の発見状態を保存
        wasPlayerDetected = playerDetected;

        // プレイヤーが近くにいる場合、向きを合わせる
        if (playerDetected && distanceX != 0f)
        {
            FaceDirection(Mathf.Sign(distanceX));
        }

        // 攻撃アニメーション中か確認する
        bool attackAnimationPlaying = false;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo =
                animator.GetCurrentAnimatorStateInfo(0);

            attackAnimationPlaying =
                stateInfo.IsName("GolemAttack") ||
                (animator.IsInTransition(0) &&
                 animator.GetNextAnimatorStateInfo(0).IsName("GolemAttack"));
        }

        // プレイヤーを発見したら徘徊から追尾へ切り替える
        if (playerDetected &&
            currentState != GolemState.Attack &&
            currentState != GolemState.Cooldown)
        {
            if (distance <= attackRange)
            {
                StartAttack();
            }
            else
            {
                currentState = GolemState.Chase;
            }
        }

        // 行動状態ごとの処理
        switch (currentState)
        {
            case GolemState.Idle:
                // 待機時間が終わったら徘徊を開始する
                patrolWaitTimer -= Time.deltaTime;

                if (patrolWaitTimer <= 0f)
                {
                    currentState = GolemState.Patrol;
                }
                break;

            case GolemState.Patrol:
                // 徘徊の端に着いたら方向転換する
                float patrolLimit =
                    patrolStartPosition.x + patrolDirection * patrolDistance;

                bool reachedEdge =
                    patrolDirection > 0
                    ? transform.position.x >= patrolLimit
                    : transform.position.x <= patrolLimit;

                if (reachedEdge)
                {
                    patrolDirection *= -1;
                    patrolWaitTimer = patrolWaitTime;
                    currentState = GolemState.Idle;
                }
                break;

            case GolemState.Chase:
                // 追尾中、攻撃範囲に入ったら攻撃
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

            case GolemState.Attack:
                // 攻撃アニメーション終了後にクールタイムへ
                if (!attackAnimationPlaying &&
                    animator != null &&
                    !animator.IsInTransition(0))
                {
                    currentState = GolemState.Cooldown;
                    cooldownTimer = cooldownDuration;
                    animator.SetFloat("Speed", 0f);
                }
                break;

            case GolemState.Cooldown:
                cooldownTimer -= Time.deltaTime;

                // クールタイム終了後、距離に応じて次の行動を決める
                if (cooldownTimer <= 0f)
                {
                    if (distance <= attackRange)
                    {
                        StartAttack();
                    }
                    else if (distance <= detectionRange)
                    {
                        currentState = GolemState.Chase;
                    }
                    else
                    {
                        currentState = GolemState.Idle;
                        patrolWaitTimer = patrolWaitTime;
                    }
                }
                break;
        }

        // 移動アニメーションを切り替える
        if (animator != null && currentState != GolemState.Attack)
        {
            animator.SetFloat(
                "Speed",
                currentState == GolemState.Chase ||
                currentState == GolemState.Patrol ? 1f : 0f
            );
        }
    }

    // ゴーレムの向きを変更する
    void FaceDirection(float direction)
    {
        Vector3 scale = originalScale;
        scale.x = Mathf.Abs(originalScale.x) * direction;
        transform.localScale = scale;
    }


    // 攻撃を開始する
    void StartAttack()
    {
        currentState = GolemState.Attack;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        // 待機・攻撃・クールタイム中は移動しない
        if (currentState == GolemState.Idle ||
            currentState == GolemState.Attack ||
            currentState == GolemState.Cooldown)
        {
            rb.linearVelocity =
                new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // 徘徊中は決められた方向へ移動する
        if (currentState == GolemState.Patrol)
        {
            rb.linearVelocity = new Vector2(
                patrolDirection * patrolSpeed,
                rb.linearVelocity.y
            );

            FaceDirection(patrolDirection);
            return;
        }

        // 追尾中はプレイヤーへ向かって移動する
        if (currentState == GolemState.Chase && player != null)
        {
            float distanceX = player.position.x - rb.position.x;

            rb.linearVelocity = new Vector2(
                Mathf.Sign(distanceX) * moveSpeed,
                rb.linearVelocity.y
            );
        }
    }

    // 横方向の移動を止める
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

    // 攻撃判定を有効にする
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }
    }

    // 攻撃判定を無効にする
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }
}
