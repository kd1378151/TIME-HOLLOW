
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("所持経験値")]
    public TextMeshProUGUI expNumText;

    [Header("強化後の数値表示")]
    public TextMeshProUGUI hpUpgradeText;
    public TextMeshProUGUI attackUpgradeText;
    public TextMeshProUGUI timeUpgradeText;

    [Header("必要経験値の表示")]
    public TextMeshProUGUI hpCostText;
    public TextMeshProUGUI attackCostText;
    public TextMeshProUGUI timeCostText;

    [Header("強化設定")]
    public int hpUpgradeAmount = 20;
    public int attackUpgradeAmount = 5;
    public int attack2UpgradeAmount = 10;
    public float timeUpgradeAmount = 15f;

    [Header("初回の必要経験値")]
    public int baseUpgradeCost = 50;

    [Header("文字色")]
    public Color normalColor = Color.white;
    public Color insufficientColor = Color.red;

    private void Start()
    {
        UpdateUpgradeUI();
    }

    // 最大HP強化
    public void UpgradeHP()
    {
        int cost = GetCost(PlayerUpgradeData.HPUpgradeCount);

        if (!PlayerExperience.TrySpendExperience(cost))
            return;

        PlayerUpgradeData.MaxHP += hpUpgradeAmount;
        PlayerUpgradeData.HPUpgradeCount++;

        UpdateUpgradeUI();
    }

    // 攻撃力強化
    public void UpgradeAttack()
    {
        int cost = GetCost(PlayerUpgradeData.AttackUpgradeCount);

        if (!PlayerExperience.TrySpendExperience(cost))
            return;

        PlayerUpgradeData.AttackPower1 += attackUpgradeAmount;
        PlayerUpgradeData.AttackPower2 += attack2UpgradeAmount;
        PlayerUpgradeData.AttackUpgradeCount++;

        UpdateUpgradeUI();
    }

    // 探索時間強化
    public void UpgradeTime()
    {
        int cost = GetCost(PlayerUpgradeData.TimeUpgradeCount);

        if (!PlayerExperience.TrySpendExperience(cost))
            return;

        PlayerUpgradeData.MaxExplorationTime += timeUpgradeAmount;
        PlayerUpgradeData.TimeUpgradeCount++;

        UpdateUpgradeUI();
    }

    // 次回の必要経験値
    private int GetCost(int upgradeCount)
    {
        return baseUpgradeCost * (upgradeCount + 1);
    }

    // UI更新
    public void UpdateUpgradeUI()
    {
        int experience = PlayerExperience.SavedExperience;

        if (expNumText != null)
            expNumText.text = experience.ToString();

        if (hpUpgradeText != null)
        {
            hpUpgradeText.text =
                PlayerUpgradeData.MaxHP + " → " +
                (PlayerUpgradeData.MaxHP + hpUpgradeAmount);
        }

        if (attackUpgradeText != null)
        {
            attackUpgradeText.text =
                PlayerUpgradeData.AttackPower1 + " → " +
                (PlayerUpgradeData.AttackPower1 + attackUpgradeAmount);
        }

        if (timeUpgradeText != null)
        {
            timeUpgradeText.text =
                PlayerUpgradeData.MaxExplorationTime.ToString("0") + "秒 → " +
                (PlayerUpgradeData.MaxExplorationTime + timeUpgradeAmount).ToString("0") + "秒";
        }

        UpdateCostText(
            hpCostText,
            GetCost(PlayerUpgradeData.HPUpgradeCount),
            experience
        );

        UpdateCostText(
            attackCostText,
            GetCost(PlayerUpgradeData.AttackUpgradeCount),
            experience
        );

        UpdateCostText(
            timeCostText,
            GetCost(PlayerUpgradeData.TimeUpgradeCount),
            experience
        );
    }

    private void UpdateCostText(
        TextMeshProUGUI target,
        int cost,
        int experience)
    {
        if (target == null) return;

        target.text = "(" + cost + ")";
        target.color = experience >= cost
            ? normalColor
            : insufficientColor;
    }

    // 結果画面へ戻る
    public void BackToResult()
    {
        SceneManager.LoadScene("ExpeditionEnd");
    }
}
