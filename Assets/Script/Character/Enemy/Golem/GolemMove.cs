
using UnityEngine;

public class GolemMovement : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 2f;

    // 必要なコンポーネント
    private Rigidbody2D rb;
    private Animator animator;

    // 最初の大きさを保存
    private Vector3 originalScale;

    // 左右キーの入力
    private float moveInput;

    void Start()
    {
        // Rigidbody2DとAnimatorを取得
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 最初の向きと大きさを保存
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 左右キー、またはA・Dキーの入力を取得
        moveInput = Input.GetAxisRaw("Horizontal");
        // キー入力を確認するためのログ
        Debug.Log("moveInput = " + moveInput);

        // 移動中かどうかをAnimatorに伝える
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }

        // 左右の入力に合わせて向きを変える
        if (moveInput < 0)
        {
            // 左向き
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (moveInput > 0)
        {
            // 右向き
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // 左右の移動速度を設定する
            rb.linearVelocity = new Vector2(
                moveInput * moveSpeed,
                rb.linearVelocity.y
            );

            // 入力値と移動速度を確認する
            Debug.Log(
                "入力：" + moveInput +
                " / 移動速度X：" + rb.linearVelocity.x +
                " / Rigidbody：" + rb.bodyType +
                " / X固定：" +
                (rb.constraints & RigidbodyConstraints2D.FreezePositionX)
            );
        }
        else
        {
            Debug.LogError("Rigidbody2D が見つかりません！");
        }
    }
}
