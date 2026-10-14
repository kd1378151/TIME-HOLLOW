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
        // 攻撃中は移動・ジャンプ・反転の入力を受け付けない
        if (IsAttacking())
        {
            moveInput = 0f;
            jumpRequested = false;
            return;
        }

        moveInput = 0f;

        // 左右移動
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
        }

        // 移動方向に合わせて向きを変更
        if (spriteRenderer != null)
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

        // ジャンプ入力
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

        bool isAttacking = IsAttacking();

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

    // 攻撃中、または攻撃ステートへの遷移中か確認する
    private bool IsAttacking()
    {
        if (animator == null) return false;

        // 現在再生しているステートを確認
        AnimatorStateInfo current =
            animator.GetCurrentAnimatorStateInfo(0);

        if (current.IsName("Attack") ||
            current.IsName("Attack2"))
        {
            return true;
        }

        // 攻撃アニメーションへの切り替え中も操作を止める
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
