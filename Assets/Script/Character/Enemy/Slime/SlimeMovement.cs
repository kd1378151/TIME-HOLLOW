
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 2f;

    [Header("地面判定")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool isGrounded;

    // 物理演算とアニメーションに使うコンポーネント
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 左右キーの入力値
    private float moveInput;

    void Start()
    {
        // 必要なコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 左右キーの入力を取得（← →、A D）
        moveInput = Input.GetAxisRaw("Horizontal");

        // 移動速度をAnimatorに渡してIdleとWalkを切り替える
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }

        // 移動方向に合わせて画像を左右反転
        if (spriteRenderer != null)
        {
            if (moveInput < 0)
                spriteRenderer.flipX = false;
            else if (moveInput > 0)
                spriteRenderer.flipX = true;
        }

        // 足元に地面があるか判定
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }
    }

    void FixedUpdate()
    {
        // 左右の速度だけ変更し、落下などの縦方向の速度は維持
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                moveInput * moveSpeed,
                rb.linearVelocity.y
            );
        }
    }
}
