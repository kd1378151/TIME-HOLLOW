
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
    [Header("スキルツリー")]
    public UpgradeNode[] allNodes;

    [Header("所持経験値")]
    public TextMeshProUGUI expNumText;

    [Header("説明表示")]
    public GameObject descriptionArea;
    public TextMeshProUGUI descriptionText;

    [Header("共通ボタン")]
    public Button upgradeButton;
    public TextMeshProUGUI actionButtonText;
    public TextMeshProUGUI costText;

    [Header("RELEASE専用ボタン")]
    public GameObject releaseButtonArea;

    [Header("ステータス表示")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI timeText;

    [Header("カウント時間")]
    public float countDuration = 0.5f;

    [Header("スキル初期値（Inspectorで変更可能）")]

    [Header("攻撃スキル")]
    public float initialAttackSkillCost = 30f;
    public float initialAttackSkillDuration = 60f;
    public float initialAttackSkillMultiplier = 1.5f;

    [Header("高速移動スキル")]
    public float initialDashSkillCost = 10f;
    public float initialDashSkillDuration = 4f;
    public float initialDashSkillSpeed = 12f;
    public float initialDashAnimatorSpeed = 1.8f;

    [Header("防御スキル")]
    public float initialDefenseSkillCost = 10f;
    public float initialDefenseSkillDuration = 5f;

    [Range(0f, 0.9f)]
    public float initialDefenseDamageReduction = 0.3f;

    private UpgradeNode selectedNode;

    private int displayedExperience;

    private Coroutine expCoroutine;
    private Coroutine hpCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine timeCoroutine;

    // =========================
    // 初期化
    // =========================

    private void Start()
    {
        displayedExperience =
            PlayerExperience.SavedExperience;

        // スキルの初期値をInspector設定から適用
        PlayerUpgradeData.InitializeSkillDefaults(
            initialAttackSkillCost,
            initialAttackSkillDuration,
            initialAttackSkillMultiplier,
            initialDashSkillCost,
            initialDashSkillDuration,
            initialDashSkillSpeed,
            initialDashAnimatorSpeed,
            initialDefenseSkillCost,
            initialDefenseSkillDuration,
            initialDefenseDamageReduction
        );

        // 各パネルを初期化
        foreach (UpgradeNode node in allNodes)
        {
            if (node != null)
                node.Initialize(this);
        }

        // ボタン設定
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(
                ExecuteSelectedAction
            );

            upgradeButton.onClick.AddListener(
                ExecuteSelectedAction
            );
        }

        // 初期表示
        UpdateExperienceText();
        UpdateStatusText();

        ClearSelection();
        UpdateAllNodes();
    }

    // =========================
    // パネル選択
    // =========================

    public void SelectNode(UpgradeNode node)
    {
        if (node == null)
            return;

        selectedNode = node;

        if (descriptionArea != null)
            descriptionArea.SetActive(true);

        if (descriptionText != null)
            descriptionText.text = node.description;

        RefreshSelectedUI();
    }

    // =========================
    // 選択解除
    // =========================

    public void ClearSelection()
    {
        selectedNode = null;

        if (descriptionArea != null)
            descriptionArea.SetActive(false);

        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(false);
            upgradeButton.interactable = false;
        }

        if (costText != null)
            costText.gameObject.SetActive(false);

        if (releaseButtonArea != null)
            releaseButtonArea.SetActive(false);
    }

    // =========================
    // 選択中パネルの表示更新
    // =========================

    private void RefreshSelectedUI()
    {
        if (selectedNode == null)
        {
            ClearSelection();
            return;
        }

        bool unlocked = selectedNode.IsUnlocked;
        bool purchased = selectedNode.IsPurchased;
        bool available = selectedNode.IsAvailable;

        // RELEASE専用ボタンは一旦非表示
        if (releaseButtonArea != null)
            releaseButtonArea.SetActive(false);

        // 解放済み＆未購入の場合だけボタンを表示
        bool showUpgradeButton =
            unlocked && !purchased;

        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(
                showUpgradeButton
            );

            upgradeButton.interactable =
                showUpgradeButton &&
                available &&
                PlayerExperience.SavedExperience >=
                selectedNode.experienceCost;
        }

        // ボタン文字
        if (actionButtonText != null)
        {
            if (selectedNode.isReleaseNode)
                actionButtonText.text = "RELEASE";
            else
                actionButtonText.text = "UPGRADE";
        }

        // 必要経験値表示
        bool showCost =
            unlocked &&
            !purchased &&
            available;

        if (costText != null)
        {
            costText.gameObject.SetActive(showCost);

            if (showCost)
            {
                int cost = selectedNode.experienceCost;

                costText.text = cost.ToString();

                costText.color =
                    PlayerExperience.SavedExperience >= cost
                    ? Color.black
                    : Color.red;
            }
        }
    }

    // =========================
    // 強化・スキル解放実行
    // =========================

    private void ExecuteSelectedAction()
    {
        if (selectedNode == null)
            return;

        // 未解放なら実行しない
        if (!selectedNode.IsUnlocked)
            return;

        // 購入済みなら実行しない
        if (selectedNode.IsPurchased)
            return;

        if (!selectedNode.IsAvailable)
            return;

        int cost = selectedNode.experienceCost;

        // 経験値消費
        if (!PlayerExperience.TrySpendExperience(cost))
        {
            RefreshSelectedUI();
            return;
        }

        // =========================
        // スキル解放
        // =========================

        if (selectedNode.isReleaseNode)
        {
            PlayerUpgradeData.MarkSkillReleased(
                selectedNode.skillToRelease
            );

            Debug.Log(
                "スキル解放：" +
                selectedNode.skillToRelease
            );
        }
        else
        {
            // 強化前の数値
            int oldHP = PlayerUpgradeData.MaxHP;
            int oldAttack = PlayerUpgradeData.AttackPower1;

            float oldTime =
                PlayerUpgradeData.MaxExplorationTime;

            // =========================
            // 強化処理
            // =========================

            switch (selectedNode.upgradeType)
            {
                // =====================
                // HP強化
                // =====================

                case UpgradeNode.UpgradeType.HP:

                    PlayerUpgradeData.MaxHP +=
                        selectedNode.hpAmount;

                    break;

                // =====================
                // 攻撃力強化
                // =====================

                case UpgradeNode.UpgradeType.Attack:

                    PlayerUpgradeData.AttackPower1 +=
                        selectedNode.attackAmount1;

                    PlayerUpgradeData.AttackPower2 +=
                        selectedNode.attackAmount2;

                    break;

                // =====================
                // 探索時間強化
                // =====================

                case UpgradeNode.UpgradeType.ExplorationTime:

                    PlayerUpgradeData.MaxExplorationTime +=
                        selectedNode.timeAmount;

                    break;

                // =====================
                // 攻撃スキル強化
                // 消費時間減少 ＋ 効果アップ
                // =====================

                case UpgradeNode.UpgradeType.AttackSkillAll:

                    PlayerUpgradeData.UpgradeAttackSkill(
                        selectedNode.attackSkillCostReduction,
                        selectedNode.attackSkillMultiplierAmount
                    );

                    break;

                // =====================
                // 高速移動スキル強化
                // 消費時間減少 ＋ 移動速度アップ
                // =====================

                case UpgradeNode.UpgradeType.DashSkillAll:

                    PlayerUpgradeData.UpgradeDashSkill(
                        selectedNode.dashSkillCostReduction,
                        selectedNode.dashSkillSpeedAmount
                    );

                    break;

                // =====================
                // 防御スキル強化
                // 消費時間減少 ＋ ダメージ軽減アップ
                // =====================

                case UpgradeNode.UpgradeType.DefenseSkillAll:

                    PlayerUpgradeData.UpgradeDefenseSkillAll(
                        selectedNode.defenseSkillCostReduction,
                        selectedNode.defenseReductionAmount
                    );

                    break;
            }

            // ステータス表示アニメーション
            StartStatusCount(
                oldHP,
                oldAttack,
                oldTime
            );

            Debug.Log(
                "強化完了：" + selectedNode.nodeId
            );
        }

        // =========================
        // 購入済みにする
        // =========================

        PlayerUpgradeData.MarkPurchased(
            selectedNode.nodeId
        );

        // =========================
        // 経験値表示更新
        // =========================

        StartExperienceCount();

        // =========================
        // 全パネル更新
        // =========================

        UpdateAllNodes();

        // =========================
        // 選択中パネル更新
        // =========================

        RefreshSelectedUI();
    }

    // =========================
    // 全パネル更新
    // =========================

    private void UpdateAllNodes()
    {
        foreach (UpgradeNode node in allNodes)
        {
            if (node != null)
                node.UpdateVisual();
        }
    }

    // =========================
    // 経験値表示
    // =========================

    private void UpdateExperienceText()
    {
        if (expNumText != null)
        {
            expNumText.text =
                displayedExperience.ToString();
        }
    }

    // =========================
    // 経験値カウント開始
    // =========================

    private void StartExperienceCount()
    {
        if (expCoroutine != null)
            StopCoroutine(expCoroutine);

        expCoroutine = StartCoroutine(
            CountExperience()
        );
    }

    // =========================
    // 経験値カウント
    // =========================

    private IEnumerator CountExperience()
    {
        int start = displayedExperience;
        int target = PlayerExperience.SavedExperience;

        float elapsed = 0f;

        if (countDuration <= 0f)
        {
            displayedExperience = target;
            UpdateExperienceText();

            expCoroutine = null;
            yield break;
        }

        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / countDuration
            );

            displayedExperience = Mathf.RoundToInt(
                Mathf.Lerp(start, target, progress)
            );

            UpdateExperienceText();

            yield return null;
        }

        displayedExperience = target;
        UpdateExperienceText();

        expCoroutine = null;
    }

    // =========================
    // ステータス表示初期化
    // =========================

    private void UpdateStatusText()
    {
        if (hpText != null)
        {
            hpText.text =
                PlayerUpgradeData.MaxHP.ToString();
        }

        if (attackText != null)
        {
            attackText.text =
                PlayerUpgradeData.AttackPower1.ToString();
        }

        if (timeText != null)
        {
            timeText.text =
                PlayerUpgradeData.MaxExplorationTime.ToString("0")
                + "秒";
        }
    }

    // =========================
    // ステータスカウント開始
    // =========================

    private void StartStatusCount(
        int oldHP,
        int oldAttack,
        float oldTime)
    {
        if (hpCoroutine != null)
            StopCoroutine(hpCoroutine);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        hpCoroutine = StartCoroutine(
            CountInt(
                hpText,
                oldHP,
                PlayerUpgradeData.MaxHP
            )
        );

        attackCoroutine = StartCoroutine(
            CountInt(
                attackText,
                oldAttack,
                PlayerUpgradeData.AttackPower1
            )
        );

        timeCoroutine = StartCoroutine(
            CountFloat(
                timeText,
                oldTime,
                PlayerUpgradeData.MaxExplorationTime
            )
        );
    }

    // =========================
    // 整数カウント
    // =========================

    private IEnumerator CountInt(
        TextMeshProUGUI target,
        int start,
        int end)
    {
        if (target == null)
            yield break;

        float elapsed = 0f;

        if (countDuration <= 0f)
        {
            target.text = end.ToString();
            yield break;
        }

        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / countDuration
            );

            int value = Mathf.RoundToInt(
                Mathf.Lerp(start, end, progress)
            );

            target.text = value.ToString();

            yield return null;
        }

        target.text = end.ToString();
    }

    // =========================
    // 小数カウント
    // =========================

    private IEnumerator CountFloat(
        TextMeshProUGUI target,
        float start,
        float end)
    {
        if (target == null)
            yield break;

        float elapsed = 0f;

        if (countDuration <= 0f)
        {
            target.text = end.ToString("0") + "秒";
            yield break;
        }

        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / countDuration
            );

            float value = Mathf.Lerp(
                start,
                end,
                progress
            );

            target.text =
                value.ToString("0") + "秒";

            yield return null;
        }

        target.text = end.ToString("0") + "秒";
    }

    // =========================
    // リザルト画面へ戻る
    // =========================

    public void BackToResult()
    {
        SceneManager.LoadScene("ExpeditionEnd");
    }
}
