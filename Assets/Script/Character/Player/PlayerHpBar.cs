
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    [Header("プレイヤーのステータス")]
    [SerializeField] private PlayerStatus playerStatus;

    [Header("HPゲージ")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image hpFill;

    [Header("HP数値表示")]
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("HPの色")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHPColor = Color.red;

    [Header("赤色に変わるHP割合")]
    [SerializeField, Range(0f, 1f)]
    private float lowHPThreshold = 0.25f;

    void Update()
    {
        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        if (playerStatus == null)
            return;

        if (hpSlider != null)
        {
            hpSlider.maxValue = playerStatus.maxHP;
            hpSlider.value = playerStatus.currentHP;
        }

        float hpRatio = playerStatus.maxHP > 0
            ? (float)playerStatus.currentHP / playerStatus.maxHP
            : 0f;

        if (hpFill != null)
        {
            hpFill.color = hpRatio <= lowHPThreshold
                ? lowHPColor
                : normalColor;
        }

        if (hpText != null)
        {
            hpText.text = playerStatus.currentHP
                + "/" + playerStatus.maxHP;
        }

    }
}
