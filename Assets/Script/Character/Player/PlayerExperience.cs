
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerExperience : MonoBehaviour
{
    [Header("所持経験値")]
    public int currentExperience = 0;

    [Header("経験値の数値表示")]
    public TextMeshProUGUI experienceText;

    [Header("カウントアップ設定")]
    public float countUpDuration = 0.5f;

    private int displayedExperience = 0;
    private Coroutine countUpCoroutine;

    void Start()
    {
        displayedExperience = currentExperience;
        UpdateExperienceText();
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0) return;

        currentExperience += amount;

        Debug.Log("獲得経験値：" + amount
            + " / 所持経験値：" + currentExperience);

        if (countUpCoroutine != null)
        {
            StopCoroutine(countUpCoroutine);
        }

        countUpCoroutine = StartCoroutine(CountUpExperience());
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
            // 数字だけを表示する
            experienceText.text = displayedExperience.ToString();
        }
    }
}
