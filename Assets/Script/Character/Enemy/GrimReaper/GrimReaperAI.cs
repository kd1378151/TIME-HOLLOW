
using UnityEngine;

public class GrimReaperAI : MonoBehaviour
{
    [Header("追跡設定")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("攻撃設定")]
    [SerializeField] private float attackInterval = 2.5f;

    [Header("攻撃後の待機設定")]
    [SerializeField] private float postAttackWait = 1.5f;

    [Header("瞬間移動後の待機設定")]
    [SerializeField] private float postTeleportWait = 0.5f;

    [Header("瞬間移動設定")]
    [SerializeField] private float teleportDistance = 3f;

    [Header("参照設定")]
    [SerializeField] private Transform player;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float nextAttackTime;
    private float waitEndTime;

    private bool isAttacking;
    private bool isWaiting;
    private bool isTeleporting;

    private bool isWaitingAfterTeleport;
    private float teleportWaitEndTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (animator == null || spriteRenderer == null)
        {
            return;
        }

        // プレイヤーが未設定なら待機
        if (player == null)
        {
            SetAnimation(false, false, false);
            return;
        }

        // 瞬間移動アニメーション中は操作しない
        if (isTeleporting)
        {
            return;
        }

        // 瞬間移動後の待機中
        if (isWaitingAfterTeleport)
        {
            SetAnimation(false, false, false);

            // 待機時間が終わるまで動かない
            if (Time.time < teleportWaitEndTime)
            {
                return;
            }

            // 待機終了
            isWaitingAfterTeleport = false;
        }

        // 攻撃アニメーション中
        if (isAttacking)
        {
            SetAnimation(false, true, false);

            AnimatorStateInfo state =
                animator.GetCurrentAnimatorStateInfo(0);

            // Attackアニメーションが終了したら待機へ
            if (state.IsName("Attack") &&
                state.normalizedTime >= 1f)
            {
                isAttacking = false;
                isWaiting = true;
                waitEndTime = Time.time + postAttackWait;

                SetAnimation(false, false, false);
            }

            return;
        }

        // 攻撃後の待機中
        if (isWaiting)
        {
            SetAnimation(false, false, false);

            // 待機時間が終わったら瞬間移動を開始
            if (Time.time >= waitEndTime)
            {
                isWaiting = false;
                isTeleporting = true;

                SetAnimation(false, false, true);
            }

            return;
        }

        // 左右方向だけで距離を判定
        float distanceX = Mathf.Abs(
            player.position.x - transform.position.x
        );

        // 攻撃範囲に入った場合
        if (distanceX <= attackRange)
        {
            FacePlayer();

            if (Time.time >= nextAttackTime)
            {
                isAttacking = true;
                nextAttackTime = Time.time + attackInterval;

                SetAnimation(false, true, false);
            }
            else
            {
                SetAnimation(false, false, false);
            }

            return;
        }

        // 追跡範囲外なら待機
        if (distanceX > chaseRange)
        {
            SetAnimation(false, false, false);
            return;
        }

        // プレイヤーの方向へ左右移動
        float direction = Mathf.Sign(
            player.position.x - transform.position.x
        );

        transform.position += Vector3.right
            * direction
            * moveSpeed
            * Time.deltaTime;

        FacePlayer();

        SetAnimation(true, false, false);
    }

    // プレイヤーの方向を向く
    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            // プレイヤーが右側なら右向き
            spriteRenderer.flipX = true;
        }
        else if (player.position.x < transform.position.x)
        {
            // プレイヤーが左側なら左向き
            spriteRenderer.flipX = false;
        }
    }

    // アニメーションのパラメータを設定
    private void SetAnimation(
        bool walking,
        bool attacking,
        bool teleporting)
    {
        animator.SetBool("IsWalking", walking);
        animator.SetBool("IsAttacking", attacking);
        animator.SetBool("IsTeleporting", teleporting);
    }

    // =================================
    // アニメーションイベントから呼び出す
    // =================================

    // 瞬間移動アニメーションの途中で呼び出す
    public void TeleportToPlayerDistance()
    {
        if (player == null)
        {
            return;
        }

        // 現在いる側と反対側へ移動する
        float side;

        if (transform.position.x >= player.position.x)
        {
            side = -1f;
        }
        else
        {
            side = 1f;
        }

        // プレイヤーから一定距離離れた場所へ移動
        float targetX =
            player.position.x + side * teleportDistance;

        transform.position = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );

        // 移動後にプレイヤーの方向を向く
        FacePlayer();
    }

    // 瞬間移動アニメーションの最後で呼び出す

    public void EndTeleportAnimation()
    {
        isTeleporting = false;

        // 瞬間移動後は待機モーションを挟む
        isWaitingAfterTeleport = true;
        teleportWaitEndTime = Time.time + postTeleportWait;

        SetAnimation(false, false, false);
    }
}
