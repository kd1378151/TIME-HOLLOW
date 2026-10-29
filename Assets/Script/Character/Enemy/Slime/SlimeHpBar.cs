
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [Header("敵のステータス")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("HPゲージ")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image hpFill;

    [Header("HPの色")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHPColor = Color.red;

    [Header("赤色に変わるHP割合")]
    [SerializeField, Range(0f, 1f)]
    private float lowHPThreshold = 0.25f;

    [Header("追従する敵")]
    [SerializeField] private Transform enemyTransform;

    [Header("頭上の位置")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.2f, 0f);

    void Update()
    {
        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        if (enemyStats == null)
            return;

        if (hpSlider != null)
        {
            hpSlider.maxValue = enemyStats.maxHP;
            hpSlider.value = enemyStats.currentHP;
        }

        float hpRatio = enemyStats.maxHP > 0
            ? (float)enemyStats.currentHP / enemyStats.maxHP
            : 0f;

        if (hpFill != null)
        {
            hpFill.color = hpRatio <= lowHPThreshold
                ? lowHPColor
                : normalColor;
        }
    }
    void LateUpdate()
    {
        if (enemyTransform != null)
        {
            transform.position = enemyTransform.position + offset;
        }
    }
}
