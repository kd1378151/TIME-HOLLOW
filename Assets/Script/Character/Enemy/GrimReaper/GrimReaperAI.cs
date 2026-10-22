
using UnityEngine;

public class GrimReaperAI : MonoBehaviour
{
    [Header("追跡設定")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float attackRange = 1.2f;
    [Header("接近後の行動設定")]
    [Range(0f, 1f)]
    [SerializeField] private float attackAfterApproachChance = 0.7f;

    [Header("攻撃設定")]
    [SerializeField] private float attackInterval = 2.5f;

    [Header("攻撃後の待機設定")]
    [SerializeField] private float postAttackWait = 1.5f;
    [SerializeField] private float attackWaitRandom = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float repeatAttackChance = 0.4f;

    [Header("瞬間移動後の待機設定")]
    [SerializeField] private float postTeleportWait = 0.5f;
    [SerializeField] private float teleportWaitRandom = 0.3f;

    [Range(0f, 1f)]
    [SerializeField] private float repeatTeleportChance = 0.35f;

    [Header("瞬間移動設定")]
    [SerializeField] private float teleportDistance = 3f;
    [SerializeField] private float teleportDistanceRandom = 1f;
    [SerializeField] private float minTeleportDistance = 2.5f;

    [Header("参照設定")]
    [SerializeField] private Transform player;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float nextAttackTime;
    private float waitEndTime;
    private float teleportWaitEndTime;

    private bool isAttacking;
    private bool isWaiting;
    private bool isTeleporting;
    private bool isWaitingAfterTeleport;

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

        if (player == null)
        {
            SetAnimation(false, false, false);
            return;
        }

        // 瞬間移動アニメーション中
        if (isTeleporting)
        {
            return;
        }

        // 瞬間移動後の待機
        if (isWaitingAfterTeleport)
        {
            SetAnimation(false, false, false);

            if (Time.time < teleportWaitEndTime)
            {
                return;
            }

            isWaitingAfterTeleport = false;

            // 確率で再び瞬間移動
            if (Random.value < repeatTeleportChance)
            {
                StartTeleport();
                return;
            }
        }

        // 攻撃アニメーション中
        if (isAttacking)
        {
            SetAnimation(false, true, false);

            AnimatorStateInfo state =
                animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Attack") &&
                state.normalizedTime >= 1f)
            {
                isAttacking = false;
                isWaiting = true;

                waitEndTime = Time.time +
                    Random.Range(
                        Mathf.Max(0f, postAttackWait - attackWaitRandom),
                        postAttackWait + attackWaitRandom
                    );

                SetAnimation(false, false, false);
            }

            return;
        }

        // 攻撃後の待機
        if (isWaiting)
        {
            SetAnimation(false, false, false);

            if (Time.time < waitEndTime)
            {
                return;
            }

            isWaiting = false;

            // 確率で連続攻撃
            if (Random.value < repeatAttackChance)
            {
                StartAttack();
            }
            else
            {
                StartTeleport();
            }

            return;
        }

        // プレイヤーとの左右の距離
        float distanceX = Mathf.Abs(
            player.position.x - transform.position.x
        );

        // 攻撃範囲内

        // 攻撃範囲内
        if (distanceX <= attackRange)
        {
            FacePlayer();

            if (Time.time >= nextAttackTime)
            {
                // 攻撃するか、瞬間移動するかをランダムに選ぶ
                if (Random.value < attackAfterApproachChance)
                {
                    StartAttack();
                }
                else
                {
                    StartTeleport();
                }
            }
            else
            {
                SetAnimation(false, false, false);
            }

            return;
        }


        // 追跡範囲外
        if (distanceX > chaseRange)
        {
            SetAnimation(false, false, false);
            return;
        }

        // プレイヤーを追跡
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

    // 攻撃開始
    private void StartAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackInterval;

        FacePlayer();
        SetAnimation(false, true, false);
    }

    // 瞬間移動開始
    private void StartTeleport()
    {
        isWaiting = false;
        isWaitingAfterTeleport = false;
        isTeleporting = true;

        SetAnimation(false, false, true);
    }

    // プレイヤーの方向を向く
    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }

    // アニメーション設定
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
    // アニメーションイベント
    // =================================

    // 瞬間移動アニメーションの途中で呼び出す
    public void TeleportToPlayerDistance()
    {
        if (player == null)
        {
            return;
        }

        // 左右を毎回ランダムに選ぶ
        float side = Random.value < 0.5f ? -1f : 1f;

        // 出現距離にもランダム性を持たせる
        float distance = Random.Range(
            Mathf.Max(
                minTeleportDistance,
                teleportDistance - teleportDistanceRandom
            ),
            Mathf.Max(
                minTeleportDistance,
                teleportDistance + teleportDistanceRandom
            )
        );

        float targetX = player.position.x + side * distance;

        transform.position = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );

        FacePlayer();
    }

    // 瞬間移動アニメーションの最後で呼び出す
    public void EndTeleportAnimation()
    {
        isTeleporting = false;
        isWaitingAfterTeleport = true;

        teleportWaitEndTime = Time.time +
            Random.Range(
                Mathf.Max(0f, postTeleportWait - teleportWaitRandom),
                postTeleportWait + teleportWaitRandom
            );

        SetAnimation(false, false, false);
    }
}
