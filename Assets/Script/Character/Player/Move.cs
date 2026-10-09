using UnityEngine;

public class Move : MonoBehaviour
{
    // プレイヤーの移動速度
    public float moveSpeed = 5f;

    // プレイヤーの物理移動に使うコンポーネント
    private Rigidbody2D rb;

    // アニメーションを切り替えるコンポーネント
    private Animator animator;

    // キャラクター画像の向きを変えるコンポーネント
    private SpriteRenderer spriteRenderer;

    // キー入力による移動方向を保存する変数
    // -1 = 左、0 = 停止、1 = 右
    private float moveInput;


    void Start()
    {
        // Playerに付いているRigidbody2Dを取得する
        rb = GetComponent<Rigidbody2D>();

        // Player本体または子オブジェクトからAnimatorを取得する
        animator = GetComponentInChildren<Animator>();

        // Player本体または子オブジェクトから画像を取得する
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    void Update()
    {
        // 毎フレーム、移動方向をいったん停止状態にする
        moveInput = 0f;

        // Aキーが押されていたら左に移動する
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
        }
        // Dキーが押されていたら右に移動する
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
        }


        // 移動しているかどうかをアニメーションに伝える
        if (animator != null)
        {
            // 移動中は1、停止中は0になる
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }


        // 移動方向に合わせてキャラクターの向きを変更する
        if (spriteRenderer != null)
        {
            if (moveInput < 0f)
            {
                // 左に移動すると画像を左右反転する
                spriteRenderer.flipX = true;
            }
            else if (moveInput > 0f)
            {
                // 右に移動すると元の向きに戻す
                spriteRenderer.flipX = false;
            }
        }
    }


    void FixedUpdate()
    {
        //// Rigidbody2Dがなければ処理を行わない
        //if (rb == null) return;

        //// 横方向の速度を設定する
        //// Y方向の速度は維持するので、重力による落下を妨げない
        //rb.velocity = new Vector2(
        //    moveInput * moveSpeed,
        //    rb.velocity.y
        //);
    }
}