
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class BatAI : MonoBehaviour
{
    private enum BatState
    {
        Chase,       // プレイヤー追尾
        Waiting,     // 突進前の待機
        Retreating,  // 後退して助走
        Charging,    // 突進
        Recovery     // 突進後の待機
    }

    [Header("プレイヤー設定")]
    public Transform player;
    [Header("追尾・突進の目標位置")]
    public Vector2 playerTargetOffset = new Vector2(0f, 1f);

    [Header("感知範囲設定")]
    public float detectionWidth = 6f;
    public float detectionHeight = 5f;

    [Header("追尾設定")]
    public float chaseSpeed = 3f;
    public float stopDistance = 0.1f;

    [Header("突進設定")]
    public float chargeRange = 2.5f;
    public float waitDuration = 1f;
    public float retreatDistance = 1.5f;
    public float retreatSpeed = 3f;
    public float chargeSpeed = 10f;
    public float chargeDuration = 1f;
    public float chargeCooldown = 2f;

    [Header("突進後の待機設定")]
    public float normalRecoveryDuration = 1.5f;
    public float playerHitRecoveryDuration = 3f;
    public float obstacleHitRecoveryDuration = 5f;

    [Header("攻撃判定")]
    public Collider2D attackCollider;
    public int attackDamage = 10;

    [Header("衝突設定")]
    public LayerMask obstacleLayers;

    [Header("向き設定")]
    public bool spriteFacesRight = true;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isAwake = false;
    private bool isWakeUpPlaying = false;

    private BatState currentState = BatState.Chase;

    private float stateTimer = 0f;
    private float cooldownTimer = 0f;
    private float retreatTimer = 0f;
    private float retreatDuration = 0f;

    private Vector2 chargeDirection;

    // 同じ突進中に同じプレイヤーへ重複攻撃しない
    private HashSet<EntityId> hitTargets =
        new HashSet<EntityId>();

    // 衝突状況の記録
    private bool hasHitPlayer = false;
    private bool hasHitObstacle = false;


    // =====================================
    // 初期設定
    // =====================================

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // 最初は攻撃判定OFF
        if (attackCollider != null)
            attackCollider.enabled = false;
    }


    // =====================================
    // Update
    // =====================================

    void Update()
    {
        // 起床前
        if (!isAwake)
        {
            if (player != null)
                CheckPlayerDetection();

            return;
        }

        // 起床アニメーション中
        if (isWakeUpPlaying)
            return;

        // 突進クールタイム
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            // -------------------------
            // 追尾
            // -------------------------

            case BatState.Chase:

                if (player == null)
                    break;

                float distance = Vector2.Distance(
                    transform.position,
                    player.position
                );

                if (distance <= chargeRange &&
                    cooldownTimer <= 0f)
                {
                    StartWaiting();
                }

                break;


            // -------------------------
            // 突進前の待機
            // -------------------------

            case BatState.Waiting:

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    StartRetreat();
                }

                break;


            // -------------------------
            // 後退・助走
            // -------------------------

            case BatState.Retreating:

                retreatTimer -= Time.deltaTime;

                if (retreatTimer <= 0f)
                {
                    StartCharge();
                }

                break;


            // -------------------------
            // 突進
            // -------------------------

            case BatState.Charging:

                stateTimer -= Time.deltaTime;

                // 一定時間経過したら通常終了
                if (stateTimer <= 0f)
                {
                    EndCharge();
                }

                break;


            // -------------------------
            // 突進後の待機
            // -------------------------

            case BatState.Recovery:

                // 移動せずプレイヤーの方向だけを向く
                if (player != null)
                {
                    Vector2 direction =
                        GetPlayerCenter() - rb.position;

                    UpdateFacing(direction);
                }

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    currentState = BatState.Chase;

                    Debug.Log("コウモリが追尾を再開！");
                }

                break;
        }
    }


    // =====================================
    // FixedUpdate
    // =====================================

    void FixedUpdate()
    {
        // 起床前・起床中
        if (!isAwake || isWakeUpPlaying)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            // -------------------------
            // 追尾
            // -------------------------

            case BatState.Chase:

                if (player != null)
                    ChasePlayer();
                else
                {
                    rb.linearVelocity = Vector2.zero;
                    animator.SetBool("IsMoving", false);
                }

                break;


            // -------------------------
            // 突進前の待機
            // -------------------------

            case BatState.Waiting:

                rb.linearVelocity = Vector2.zero;
                animator.SetBool("IsMoving", false);

                break;


            case BatState.Retreating:

                // プレイヤーとは反対方向に後退
                rb.linearVelocity =
                    -chargeDirection * retreatSpeed;

                animator.SetBool("IsMoving", true);

                break;


            case BatState.Charging:

                // 後退方向の真逆へ突進
                rb.linearVelocity =
                    chargeDirection * chargeSpeed;

                animator.SetBool("IsMoving", true);

                break;


            // -------------------------
            // 突進後の待機
            // -------------------------

            case BatState.Recovery:

                // 完全停止
                rb.linearVelocity = Vector2.zero;

                // 待機モーション
                animator.SetBool("IsMoving", false);

                break;
        }
    }

    // =====================================
    // プレイヤーの中心座標を取得
    // =====================================
    // =====================================
    // プレイヤーのTop座標を取得
    // =====================================
    // =====================================
    // プレイヤーの追尾・突進目標座標
    // =====================================
    Vector2 GetPlayerCenter()
    {
        if (player == null)
            return Vector2.zero;

        return (Vector2)player.position + playerTargetOffset;
    }

    // =====================================
    // プレイヤー追尾
    // =====================================
    void ChasePlayer()
    {
        // プレイヤーの中心を取得
        Vector2 targetPosition = GetPlayerCenter();

        Vector2 direction =
            targetPosition - rb.position;

        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        direction.Normalize();

        rb.linearVelocity = direction * chaseSpeed;

        animator.SetBool("IsMoving", true);

        // 追尾中だけ向きを変更
        UpdateFacing(direction);
    }


    // =====================================
    // 突進前の待機
    // =====================================

    void StartWaiting()
    {
        currentState = BatState.Waiting;

        stateTimer = waitDuration;

        rb.linearVelocity = Vector2.zero;

        animator.SetBool("IsMoving", false);

        // プレイヤーの方向を記録
        chargeDirection =
            ((Vector2)player.position - rb.position).normalized;

        // プレイヤーの方向を向く
        UpdateFacing(chargeDirection);

        Debug.Log("コウモリが突進準備！");
    }


    // =====================================
    // 後退して助走
    // =====================================
    void StartRetreat()
    {
        currentState = BatState.Retreating;

        // 助走開始時にプレイヤーの方向を記録
        if (player != null)
        {
            chargeDirection =
                (GetPlayerCenter() - rb.position).normalized;
        }

        if (retreatSpeed <= 0f)
        {
            StartCharge();
            return;
        }

        retreatDuration =
            retreatDistance / retreatSpeed;

        retreatTimer = retreatDuration;

        Debug.Log("コウモリが後退して助走！");
    }

    // =====================================
    // 突進開始
    // =====================================
    void StartCharge()
    {
        // chargeDirectionは変更しない
        // 助走開始時に決めた方向をそのまま使用する

        currentState = BatState.Charging;

        stateTimer = chargeDuration;

        hasHitPlayer = false;
        hasHitObstacle = false;

        hitTargets.Clear();

        if (attackCollider != null)
            attackCollider.enabled = true;

        // 突進方向に向きを合わせる
        UpdateFacing(chargeDirection);

        animator.SetBool("IsMoving", true);

        Debug.Log("コウモリが突進！");
    }

    // =====================================
    // 突進終了（通常）
    // =====================================

    void EndCharge()
    {
        FinishCharge(normalRecoveryDuration);
    }


    // =====================================
    // 突進終了（共通処理）
    // =====================================

    void FinishCharge(float recoveryTime)
    {
        // 突進中でなければ処理しない
        if (currentState != BatState.Charging)
            return;

        // 攻撃判定OFF
        if (attackCollider != null)
            attackCollider.enabled = false;

        // 移動を即座に停止
        rb.linearVelocity = Vector2.zero;

        // 次の突進までのクールタイム
        cooldownTimer = chargeCooldown;

        Debug.Log(
            "突進終了！ プレイヤー命中：" +
            hasHitPlayer +
            " / 障害物衝突：" +
            hasHitObstacle
        );

        // 終了理由に応じた待機へ
        StartRecovery(recoveryTime);
    }


    // =====================================
    // 突進後の待機
    // =====================================

    void StartRecovery(float duration)
    {
        currentState = BatState.Recovery;

        stateTimer = duration;

        // 完全停止
        rb.linearVelocity = Vector2.zero;

        // 攻撃判定を確実にOFF
        if (attackCollider != null)
            attackCollider.enabled = false;

        // 待機モーション
        animator.SetBool("IsMoving", false);

        Debug.Log("コウモリが待機！ 時間：" + duration);
    }


    // =====================================
    // プレイヤー感知
    // =====================================

    void CheckPlayerDetection()
    {
        Vector2 batPosition = transform.position;
        Vector2 playerPosition = player.position;

        float xDistance =
            Mathf.Abs(playerPosition.x - batPosition.x);

        float yDistance =
            batPosition.y - playerPosition.y;

        // プレイヤーが上にいる場合は反応しない
        if (yDistance <= 0f)
            return;

        // 四角形の範囲内か確認
        if (xDistance <= detectionWidth / 2f &&
            yDistance <= detectionHeight)
        {
            WakeUp();
        }
    }


    // =====================================
    // 起床
    // =====================================

    void WakeUp()
    {
        isAwake = true;
        isWakeUpPlaying = true;

        rb.linearVelocity = Vector2.zero;

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAwake", true);
        animator.SetTrigger("WakeUp");

        Debug.Log("コウモリが目を覚ました！");
    }


    // =====================================
    // 起床アニメーション終了イベント
    // =====================================

    public void EndWakeUp()
    {
        isWakeUpPlaying = false;

        Debug.Log("コウモリが起き上がった！");
    }


    // =====================================
    // 左右の向き変更
    // =====================================

    void UpdateFacing(Vector2 direction)
    {
        if (spriteRenderer == null ||
            Mathf.Abs(direction.x) < 0.001f)
            return;

        spriteRenderer.flipX =
            spriteFacesRight
            ? direction.x < 0f
            : direction.x > 0f;
    }


    // =====================================
    // プレイヤーへの攻撃判定
    // =====================================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != BatState.Charging)
            return;

        if (player == null)
            return;

        // プレイヤー本体または子オブジェクトか確認
        if (other.transform != player &&
            !other.transform.IsChildOf(player))
            return;

        // プレイヤーを識別
        EntityId targetID = player.gameObject.GetEntityId();

        // 同じ突進で重複攻撃しない
        if (hitTargets.Contains(targetID))
            return;

        hitTargets.Add(targetID);

        hasHitPlayer = true;

        Debug.Log(
            "コウモリがプレイヤーに命中！ ダメージ：" +
            attackDamage
        );

        // ここにPlayerStatusのダメージ処理を接続する

        // プレイヤー命中時は即座に突進を中止
        FinishCharge(playerHitRecoveryDuration);
    }


    // =====================================
    // 壁・床との衝突判定
    // =====================================

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != BatState.Charging)
            return;

        // プレイヤーとの衝突は除外
        if (player != null &&
            (collision.transform == player ||
             collision.transform.IsChildOf(player)))
            return;

        // 指定されたレイヤーか確認
        if (((1 << collision.gameObject.layer) &
             obstacleLayers.value) == 0)
            return;

        hasHitObstacle = true;

        Debug.Log("コウモリが壁・床に衝突！");

        // 壁・床に当たったら即座に突進を中止
        FinishCharge(obstacleHitRecoveryDuration);
    }


    // =====================================
    // 感知範囲の表示
    // =====================================

    void OnDrawGizmosSelected()
    {
        // 感知範囲
        Gizmos.color = Color.green;

        Vector3 center = transform.position;

        Vector3 detectionCenter = new Vector3(
            center.x,
            center.y - detectionHeight / 2f,
            center.z
        );

        Gizmos.DrawWireCube(
            detectionCenter,
            new Vector3(
                detectionWidth,
                detectionHeight,
                0.1f
            )
        );

        // 突進範囲
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            chargeRange
        );
    }
}
