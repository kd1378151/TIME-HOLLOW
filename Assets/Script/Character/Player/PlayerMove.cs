using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // =========================
    // 移動設定
    // =========================

    [Header("移動設定")]

    public float moveSpeed = 5f;   // 左右の移動速度
    public float jumpForce = 10f;  // ジャンプの強さ


    // =========================
    // 地面判定設定
    // =========================

    [Header("地面判定設定")]

    public Transform groundCheck;       // 足元の判定位置
    public float groundCheckRadius = 0.15f; // 判定する円の半径
    public LayerMask groundLayer;       // 床のLayer


    // =========================
    // コンポーネント
    // =========================

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    // =========================
    // プレイヤーの状態
    // =========================

    private float moveInput;       // 左右の入力
    private bool jumpRequested;    // ジャンプ入力の記録
    private bool isGrounded;       // 地面にいるかどうか


    // =========================
    // 初期設定
    // =========================

    void Start()
    {
        // Player本体のRigidbody2Dを取得
        rb = GetComponent<Rigidbody2D>();

        // 子オブジェクトにあるAnimatorを取得
        animator = GetComponentInChildren<Animator>();

        // 子オブジェクトにあるSpriteRendererを取得
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    // =========================
    // キーボード入力
    // =========================

    void Update()
    {
        // 最初に入力をリセット
        moveInput = 0f;

        // Aキーで左に移動
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
        }
        // Dキーで右に移動
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
        }

        // 移動方向に合わせてキャラクターの向きを変える


        // 攻撃中かどうかを確認
        bool isAttacking = false;

        if (animator != null)
        {
            AnimatorStateInfo state =
                animator.GetCurrentAnimatorStateInfo(0);

            isAttacking =
                state.IsName("Attack") ||
                state.IsName("Attack2");
        }

        // 攻撃中でなければ、移動方向に合わせて向きを変える
        if (!isAttacking && spriteRenderer != null)
        {
            if (moveInput < 0f)
            {
                spriteRenderer.flipX = true;
            }
            else if (moveInput > 0f)
            {
                spriteRenderer.flipX = false;
            }
        }

        // Spaceキーを押した瞬間にジャンプを記録
        // 実際にジャンプできるかはFixedUpdateで判定する
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
    }


    // =========================
    // 移動・地面判定・アニメーション
    // =========================

    void FixedUpdate()
    {
        // Rigidbody2Dがなければ処理しない
        if (rb == null)
        {
            return;
        }


        // -------------------------
        // 1. 地面判定
        // -------------------------

        if (groundCheck != null)
        {
            // 足元の円が床に触れているか調べる
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            ) != null;
        }
        else
        {
            // GroundCheckが未設定なら接地していない扱い
            isGrounded = false;
        }

        AnimatorStateInfo state =
        animator.GetCurrentAnimatorStateInfo(0);

        bool isAttacking =
            state.IsName("Attack") ||
            state.IsName("Attack2");

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            //攻撃中にジャンプ入力を保持しない
            jumpRequested = false;

            return;
        }

        // -------------------------
        // 2. 左右移動
        // -------------------------

        // 横方向だけ速度を変更する
        // 縦方向は重力やジャンプの速度を維持する
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );


        // -------------------------
        // 3. ジャンプ
        // -------------------------

        if (jumpRequested)
        {
            // 地面にいる場合だけジャンプ
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );

                // ジャンプ直後は空中扱い
                isGrounded = false;
            }

            // ジャンプ入力をリセット
            jumpRequested = false;
        }


        // -------------------------
        // 4. Animatorに状態を渡す
        // -------------------------

        if (animator != null)
        {
            // 移動入力を渡す
            // 空中でも横移動できるように、入力値自体は維持する
            animator.SetFloat(
                "Speed",
                Mathf.Abs(moveInput)
            );

            // 上昇中はプラス、下降中はマイナス
            animator.SetFloat(
                "VerticalSpeed",
                rb.linearVelocity.y
            );

            // 地面にいるかどうかを渡す
            animator.SetBool(
                "IsGrounded",
                isGrounded
            );
        }
    }


    // =========================
    // Sceneビューで判定範囲を表示
    // =========================

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        // 選択時に黄色い円で判定位置を表示
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}
