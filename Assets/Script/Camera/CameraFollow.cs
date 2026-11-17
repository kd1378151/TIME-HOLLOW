
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従する対象")]
    public Transform target;

    [Header("追従開始設定")]
    public float followStartX = 5f;

    [Header("追従設定")]
    public float smoothSpeed = 5f;
    public float lookAheadX = 2f;
    public float heightOffset = 5f;

    [Header("ジャンプ中のカメラ設定")]
    public bool keepYWhileJumping = true;
    public float verticalLimit = 6f;

    [Header("着地後のプレイヤー画面位置")]
    [Range(0f, 1f)]
    public float playerScreenY = 0.22f;

    [Header("カメラ移動範囲（ワールド座標）")]
    public float minX = 0f;
    public float maxX = 30f;
    public float minY = 0f;
    public float maxY = 10f;

    private SpriteRenderer targetSprite;
    private PlayerMovement playerMovement;
    private Camera cam;

    private bool followStarted = false;
    private bool wasGrounded = true;

    private float fixedCameraY;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (target != null)
        {
            targetSprite =
                target.GetComponentInChildren<SpriteRenderer>();

            playerMovement =
                target.GetComponent<PlayerMovement>();
        }

        fixedCameraY = transform.position.y;

        if (playerMovement != null)
        {
            wasGrounded = playerMovement.IsGrounded;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        if (playerMovement == null) return;
        if (cam == null) return;

        // 追従開始
        if (!followStarted)
        {
            if (target.position.x < followStartX) return;

            followStarted = true;
            fixedCameraY = transform.position.y;
        }

        // プレイヤーの向き
        float direction = 1f;

        if (targetSprite != null && targetSprite.flipX)
        {
            direction = -1f;
        }

        float targetX =
            target.position.x + direction * lookAheadX;

        bool isGrounded = playerMovement.IsGrounded;

        // ジャンプ・落下開始時
        if (wasGrounded && !isGrounded)
        {
            fixedCameraY = transform.position.y;
        }

        float targetY;

        if (!isGrounded && keepYWhileJumping)
        {
            // 空中ではY軸を基本固定
            targetY = fixedCameraY;

            float distance =
                target.position.y - transform.position.y;

            // 画面外に出そうな場合だけ追従
            if (distance > verticalLimit)
            {
                targetY =
                    target.position.y - verticalLimit;
            }
            else if (distance < -verticalLimit)
            {
                targetY =
                    target.position.y + verticalLimit;
            }
        }
        else
        {
            // 着地後はプレイヤーの位置を基準にする
            // プレイヤーが画面下から約22%の位置に来る
            float cameraOffset =
                (0.5f - playerScreenY) *
                cam.orthographicSize * 2f;

            targetY = target.position.y + cameraOffset;
        }

        // カメラの移動範囲
        targetX = Mathf.Clamp(targetX, minX, maxX);
        targetY = Mathf.Clamp(targetY, minY, maxY);

        Vector3 targetPosition = new Vector3(
            targetX,
            targetY,
            transform.position.z
        );

        // 滑らかに追従
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        wasGrounded = isGrounded;
    }
}
