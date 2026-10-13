
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
    public float heightOffset = 1f;

    [Header("カメラ移動範囲（ワールド座標）")]
    public float minX = 0f;
    public float maxX = 30f;
    public float minY = 0f;
    public float maxY = 10f;

    private SpriteRenderer targetSprite;
    private bool followStarted = false;

    private void Start()
    {
        if (target != null)
        {
            targetSprite = target.GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 指定したX座標に到達したら追従開始
        if (!followStarted)
        {
            if (target.position.x < followStartX) return;

            followStarted = true;
        }

        // プレイヤーの向きに合わせて、進行方向の先を映す
        float direction = 1f;

        if (targetSprite != null && targetSprite.flipX)
        {
            direction = -1f;
        }

        Vector3 targetPosition = new Vector3(
            target.position.x + direction * lookAheadX,
            target.position.y + heightOffset,
            transform.position.z
        );

        // カメラの移動範囲を制限
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // 滑らかに追従
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}