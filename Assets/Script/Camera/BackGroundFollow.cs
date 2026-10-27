
using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [Header("追従するカメラ")]
    public Transform targetCamera;

    [Header("背景の移動倍率")]
    [Range(0f, 1f)]
    public float followRate = 1f;

    private Vector3 previousCameraPosition;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main.transform;
        }

        previousCameraPosition = targetCamera.position;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 cameraMovement =
            targetCamera.position - previousCameraPosition;

        transform.position += new Vector3(
            cameraMovement.x * followRate,
            cameraMovement.y * followRate,
            0f
        );

        previousCameraPosition = targetCamera.position;
    }
}
