
using UnityEngine;
using UnityEngine.UI;

public class SlimeHPBar : MonoBehaviour
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

    private void Update()
    {
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (enemyStats == null)
            return;

        // 最大HPが0以下の場合
        if (enemyStats.maxHP <= 0)
        {
            if (hpSlider != null)
                hpSlider.value = 0;

            if (hpFill != null)
                hpFill.color = lowHPColor;

            return;
        }

        // HPの割合を計算
        float hpRatio = Mathf.Clamp01(
            (float)enemyStats.currentHP / enemyStats.maxHP
        );

        // HPゲージを更新
        if (hpSlider != null)
        {
            hpSlider.maxValue = enemyStats.maxHP;
            hpSlider.value = enemyStats.currentHP;
        }

        // HP25%以下なら赤、それ以外は緑
        if (hpFill != null)
        {
            hpFill.color = hpRatio <= lowHPThreshold
                ? lowHPColor
                : normalColor;
        }
    }

    private void LateUpdate()
    {
        // 敵の頭上にHPバーを追従させる
        if (enemyTransform != null)
        {
            transform.position = enemyTransform.position + offset;
        }
    }
}
