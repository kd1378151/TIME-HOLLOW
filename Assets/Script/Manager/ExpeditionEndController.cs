
using UnityEngine;
using TMPro;

public class ExpeditionEndController : MonoBehaviour
{
    [Header("探索終了の文字")]
    [SerializeField] private TextMeshProUGUI endText;

    private void Start()
    {
        if (endText != null)
        {
            endText.gameObject.SetActive(true);
        }
    }
}
