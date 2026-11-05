
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ExplorationResultManager : MonoBehaviour
{
    [Header("表示するUI")]
    public CanvasGroup resultTextGroup;
    public CanvasGroup restartButtonGroup;
    public CanvasGroup upgradeButtonGroup;

    [Header("フェード設定")]
    public float fadeDuration = 1.0f;
    public float startDelay = 0.3f;

    private bool buttonsEnabled = false;

    void Start()
    {
        // 最初はすべて透明にする
        SetAlpha(resultTextGroup, 0f);
        SetAlpha(restartButtonGroup, 0f);
        SetAlpha(upgradeButtonGroup, 0f);

        // 最初はボタンを押せないようにする
        SetInteractable(restartButtonGroup, false);
        SetInteractable(upgradeButtonGroup, false);

        StartCoroutine(ShowUI());
    }


    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(startDelay);

        // 探索終了テキストを表示
        yield return StartCoroutine(FadeIn(resultTextGroup));

        // 2つのボタンを同時に表示
        Coroutine restartFade = StartCoroutine(FadeIn(restartButtonGroup));
        Coroutine upgradeFade = StartCoroutine(FadeIn(upgradeButtonGroup));

        // 両方のフェードが終わるまで待つ
        yield return restartFade;
        yield return upgradeFade;

        // 両方表示されたらボタンを押せるようにする
        buttonsEnabled = true;

        SetInteractable(restartButtonGroup, true);
        SetInteractable(upgradeButtonGroup, true);
    }


    IEnumerator FadeIn(CanvasGroup group)
    {
        if (group == null) yield break;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            group.alpha = alpha;

            yield return null;
        }

        group.alpha = 1f;
    }

    void SetAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
        {
            group.alpha = alpha;
        }
    }

    void SetInteractable(CanvasGroup group, bool value)
    {
        if (group != null)
        {
            group.interactable = value;
            group.blocksRaycasts = value;
        }
    }

    // リスタートボタンを押したとき
    public void Restart()
    {
        if (!buttonsEnabled) return;

        SceneManager.LoadScene("Stage");
    }

    // 強化ボタンを押したとき
    public void OpenUpgrade()
    {
        if (!buttonsEnabled) return;

        SceneManager.LoadScene("Upgrade");
    }
}
