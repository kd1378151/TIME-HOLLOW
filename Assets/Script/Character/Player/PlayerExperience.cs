
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerExperience : MonoBehaviour
{
    [Header("所持経験値")]
    public int currentExperience = 0;

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
            SavedExperience = currentExperience;
            isInitialized = true;
        }
        else
        {
            currentExperience = SavedExperience;
        }

        displayedExperience = currentExperience;
        UpdateExperienceText();
    }

    // 経験値を獲得
    public void AddExperience(int amount)
    {
        if (amount <= 0) return;

        currentExperience += amount;
        SavedExperience = currentExperience;

        Debug.Log("獲得経験値：" + amount
            + " / 所持経験値：" + currentExperience);

        if (countUpCoroutine != null)
        {
            StopCoroutine(countUpCoroutine);
        }

        countUpCoroutine = StartCoroutine(CountUpExperience());
    }

    // 経験値を消費
    public static bool TrySpendExperience(int amount)
    {
        if (amount <= 0) return false;

        if (SavedExperience < amount)
        {
            Debug.Log("経験値が足りません");
            return false;
        }

        SavedExperience -= amount;

        Debug.Log("経験値を消費：" + amount
            + " / 残り経験値：" + SavedExperience);

        return true;
    }

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

    void UpdateExperienceText()
    {
        if (experienceText != null)
        {
            experienceText.text = displayedExperience.ToString();
        }
    }
}
