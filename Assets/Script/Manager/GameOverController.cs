
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverFadeController : MonoBehaviour
{
    [Header("表示するUI")]
    public CanvasGroup gameTextGroup;
    public CanvasGroup overTextGroup;
    public CanvasGroup restartButtonGroup;
    public CanvasGroup titleButtonGroup;

    [Header("フェード設定")]
    public float fadeDuration = 1.0f;
    public float startDelay = 0.3f;

    private bool buttonsEnabled = false;

    [Header("遷移先シーン名")]
    public string firstStageSceneName = "Stage";
    public string titleSceneName = "Title";


    void Start()
    {
        // 最初はすべて透明にする
        SetAlpha(gameTextGroup, 0f);
        SetAlpha(overTextGroup, 0f);
        SetAlpha(restartButtonGroup, 0f);
        SetAlpha(titleButtonGroup, 0f);

        // 最初はボタンを押せないようにする
        SetInteractable(restartButtonGroup, false);
        SetInteractable(titleButtonGroup, false);

        StartCoroutine(ShowUI());
    }


    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(startDelay);

        // GAMEとOVERのテキストを同時に表示
        Coroutine gameFade = StartCoroutine(FadeIn(gameTextGroup));
        Coroutine overFade = StartCoroutine(FadeIn(overTextGroup));

        // 両方のフェードが終わるまで待つ
        yield return gameFade;
        yield return overFade;

        // 少し待機
        yield return new WaitForSeconds(startDelay);

        // 2つのボタンを同時に表示
        Coroutine restartFade = StartCoroutine(FadeIn(restartButtonGroup));
        Coroutine titleFade = StartCoroutine(FadeIn(titleButtonGroup));

        // 両方のフェードが終わるまで待つ
        yield return restartFade;
        yield return titleFade;

        // 両方表示されたらボタンを押せるようにする
        buttonsEnabled = true;

        SetInteractable(restartButtonGroup, true);
        SetInteractable(titleButtonGroup, true);
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


    // はじめからボタンを押したとき
    public void RestartGame()
    {
        if (!buttonsEnabled) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(firstStageSceneName);
    }


    // タイトルへ戻るボタンを押したとき
    public void ReturnToTitle()
    {
        SceneFlowManager.Instance.LoadMainMenuScene();
    }
}
