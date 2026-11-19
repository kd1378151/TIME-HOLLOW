
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerExperience : MonoBehaviour
{
    [Header("所持経験値")]
    public int currentExperience = 0;

    // 経験値の最大所持数
    public const int MaxExperience = 1000;

    // シーンを切り替えても保持する経験値
    public static int SavedExperience { get; private set; }

    private static bool isInitialized = false;

    [Header("経験値の数値表示")]
    public TextMeshProUGUI experienceText;

    [Header("カウントアップ設定")]
    public float countUpDuration = 0.5f;

    private int displayedExperience = 0;
    private Coroutine countUpCoroutine;

    void Start()
    {
        if (!isInitialized)
        {
            // 初回起動時はInspectorの数値を使用
            SavedExperience = Mathf.Clamp(
                currentExperience,
                0,
                MaxExperience
            );

            isInitialized = true;

            Debug.Log("初期所持経験値：" + SavedExperience);
        }
        else
        {
            // シーン切り替え時は保存した経験値を使用
            SavedExperience = Mathf.Clamp(
                SavedExperience,
                0,
                MaxExperience
            );

            currentExperience = SavedExperience;
        }

        displayedExperience = currentExperience;
        UpdateExperienceText();
    }

    // =========================
    // 経験値を獲得
    // =========================

    public void AddExperience(int amount)
    {
        if (amount <= 0) return;

        // 現在の所持経験値を取得
        currentExperience = SavedExperience;

        // 上限までの残り獲得可能量
        int remainingExperience =
            MaxExperience - SavedExperience;

        // 実際に獲得できる経験値
        int actualAmount = Mathf.Min(
            amount,
            remainingExperience
        );

        // 上限に達している場合
        if (actualAmount <= 0)
        {
            Debug.Log("経験値は最大所持数の1000に達しています");
            return;
        }

        // 経験値を加算
        currentExperience += actualAmount;

        // 念のため上限を適用
        currentExperience = Mathf.Clamp(
            currentExperience,
            0,
            MaxExperience
        );

        SavedExperience = currentExperience;

        Debug.Log(
            "獲得経験値：" + actualAmount
            + " / 所持経験値：" + currentExperience
        );

        // 上限に達した場合のログ
        if (amount > actualAmount)
        {
            Debug.Log(
                "経験値上限に達したため、超過分は獲得できませんでした"
            );
        }

        // カウントアップ演出
        if (countUpCoroutine != null)
        {
            StopCoroutine(countUpCoroutine);
        }

        countUpCoroutine = StartCoroutine(
            CountUpExperience()
        );
    }

    // =========================
    // 経験値を消費
    // =========================

    public static bool TrySpendExperience(int amount)
    {
        if (amount <= 0) return false;

        if (SavedExperience < amount)
        {
            Debug.Log("経験値が足りません");
            return false;
        }

        SavedExperience -= amount;

        // 消費後も上限・下限を適用
        SavedExperience = Mathf.Clamp(
            SavedExperience,
            0,
            MaxExperience
        );

        Debug.Log(
            "経験値を消費：" + amount
            + " / 残り経験値：" + SavedExperience
        );

        return true;
    }

    // =========================
    // 経験値カウントアップ
    // =========================

    IEnumerator CountUpExperience()
    {
        int startExperience = displayedExperience;
        int targetExperience = currentExperience;

        if (countUpDuration <= 0f)
        {
            displayedExperience = targetExperience;
            UpdateExperienceText();

            countUpCoroutine = null;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < countUpDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / countUpDuration
            );

            displayedExperience = Mathf.RoundToInt(
                Mathf.Lerp(
                    startExperience,
                    targetExperience,
                    progress
                )
            );

            UpdateExperienceText();

            yield return null;
        }

        displayedExperience = targetExperience;
        UpdateExperienceText();

        countUpCoroutine = null;
    }

    // =========================
    // 経験値表示更新
    // =========================

    void UpdateExperienceText()
    {
        if (experienceText != null)
        {
            experienceText.text =
                displayedExperience.ToString();
        }
    }

    // =========================
    // 経験値を初期化
    // =========================

    public static void ResetExperience()
    {
        SavedExperience = 0;
        isInitialized = true;

        Debug.Log("所持経験値を初期化しました");
    }

    // =========================
    // Unity終了時にリセット
    // =========================

    void OnApplicationQuit()
    {
        SavedExperience = 0;

        // 次回のエディタ実行ではInspectorの値を読み込む
        isInitialized = false;
    }
}
