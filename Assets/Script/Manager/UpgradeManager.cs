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

    private UpgradeNode selectedNode;

    private int displayedExperience;

    private Coroutine expCoroutine;
    private Coroutine hpCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine timeCoroutine;

    private void Start()
    {
        displayedExperience = PlayerExperience.SavedExperience;

        // 各パネルを初期化
        foreach (UpgradeNode node in allNodes)
        {
            if (node != null)
                node.Initialize(this);
        }

        // ボタン設定
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(ExecuteSelectedAction);
            upgradeButton.onClick.AddListener(ExecuteSelectedAction);
        }

        // 初期表示
        UpdateExperienceText();
        UpdateStatusText();

        ClearSelection();
        UpdateAllNodes();
    }

    // パネルを選択
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

    // 選択解除
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

    // 選択中パネルの表示更新
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

        // =====================================
        // 強化ボタンの表示条件
        // 解放済み ＆ 未強化の場合だけ表示
        // =====================================

        bool showUpgradeButton = unlocked && !purchased;

        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(showUpgradeButton);

            // 強化可能かどうか
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
            {
                actionButtonText.text = "RELEASE";
            }
            else
            {
                actionButtonText.text = "UPGRADE";
            }
        }

        // =====================================
        // 必要経験値の表示条件
        // 解放済み ＆ 未強化 ＆ 利用可能の場合だけ表示
        // =====================================

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

    // 強化・スキル解放実行
    private void ExecuteSelectedAction()
    {
        if (selectedNode == null)
            return;

        // 未解放の場合は実行しない
        if (!selectedNode.IsUnlocked)
            return;

        // すでに強化済みの場合は実行しない
        if (selectedNode.IsPurchased)
            return;

        if (!selectedNode.IsAvailable)
            return;

        int cost = selectedNode.experienceCost;

        // 経験値を消費
        if (!PlayerExperience.TrySpendExperience(cost))
        {
            RefreshSelectedUI();
            return;
        }

        // RELEASEの場合
        if (selectedNode.isReleaseNode)
        {
            PlayerUpgradeData.MarkSkillReleased(
                selectedNode.skillToRelease
            );

            Debug.Log(
                "スキル解放：" + selectedNode.skillToRelease
            );
        }
        else
        {
            // 強化前の数値
            int oldHP = PlayerUpgradeData.MaxHP;
            int oldAttack = PlayerUpgradeData.AttackPower1;
            float oldTime = PlayerUpgradeData.MaxExplorationTime;

            // 強化処理
            switch (selectedNode.upgradeType)
            {
                case UpgradeNode.UpgradeType.HP:

                    PlayerUpgradeData.MaxHP +=
                        selectedNode.hpAmount;

                    break;

                case UpgradeNode.UpgradeType.Attack:

                    PlayerUpgradeData.AttackPower1 +=
                        selectedNode.attackAmount1;

                    PlayerUpgradeData.AttackPower2 +=
                        selectedNode.attackAmount2;

                    break;

                case UpgradeNode.UpgradeType.ExplorationTime:

                    PlayerUpgradeData.MaxExplorationTime +=
                        selectedNode.timeAmount;

                    break;

                case UpgradeNode.UpgradeType.DefenseSkill:

                    PlayerUpgradeData.DefenseSkillDuration +=
                        selectedNode.defenseDurationAmount;

                    PlayerUpgradeData.DefenseDamageReduction =
                        Mathf.Clamp(
                            PlayerUpgradeData.DefenseDamageReduction +
                            selectedNode.defenseReductionAmount,
                            0f,
                            0.9f
                        );

                    break;
            }

            StartStatusCount(
                oldHP,
                oldAttack,
                oldTime
            );

            Debug.Log(
                "強化完了：" + selectedNode.nodeId
            );
        }

        // 取得済みにする
        PlayerUpgradeData.MarkPurchased(
            selectedNode.nodeId
        );

        // 経験値表示更新
        StartExperienceCount();

        // 全パネル更新
        UpdateAllNodes();

        // 選択中パネルの表示更新
        RefreshSelectedUI();
    }

    // 全パネル更新
    private void UpdateAllNodes()
    {
        foreach (UpgradeNode node in allNodes)
        {
            if (node != null)
                node.UpdateVisual();
        }
    }

    // 経験値表示
    private void UpdateExperienceText()
    {
        if (expNumText != null)
        {
            expNumText.text =
                displayedExperience.ToString();
        }
    }

    private void StartExperienceCount()
    {
        if (expCoroutine != null)
            StopCoroutine(expCoroutine);

        expCoroutine = StartCoroutine(
            CountExperience()
        );
    }

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

    // ステータス表示初期化
    private void UpdateStatusText()
    {
        if (hpText != null)
            hpText.text = PlayerUpgradeData.MaxHP.ToString();

        if (attackText != null)
            attackText.text = PlayerUpgradeData.AttackPower1.ToString();

        if (timeText != null)
        {
            timeText.text =
                PlayerUpgradeData.MaxExplorationTime.ToString("0") + "秒";
        }
    }

    // ステータスのカウントアップ
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
            CountInt(hpText, oldHP, PlayerUpgradeData.MaxHP)
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

            target.text = value.ToString("0") + "秒";

            yield return null;
        }

        target.text = end.ToString("0") + "秒";
    }

    public void BackToResult()
    {
        SceneManager.LoadScene("ExpeditionEnd");
    }
}