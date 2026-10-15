
using UnityEngine;
using System.Collections;

public class EnemyAlert : MonoBehaviour
{
    [Header("発見マーク")]
    public GameObject alertIcon;

    [Header("表示時間（秒）")]
    public float displayTime = 1f;

    private Coroutine alertCoroutine;

    void Awake()
    {
        // ゲーム開始時は非表示にする
        if (alertIcon != null)
        {
            alertIcon.SetActive(false);
        }
    }

    public void ShowAlert()
    {
        if (alertIcon == null) return;

        // 表示中に再度呼ばれたら、表示時間をリセットする
        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
        }

        alertCoroutine = StartCoroutine(ShowAlertRoutine());
    }

    private IEnumerator ShowAlertRoutine()
    {
        alertIcon.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        alertIcon.SetActive(false);
        alertCoroutine = null;
    }
}
