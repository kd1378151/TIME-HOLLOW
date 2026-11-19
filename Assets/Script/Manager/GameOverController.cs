
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverFadeController : MonoBehaviour
{
    [Header("表示するUI")]
    public CanvasGroup gameTextGroup;
    public CanvasGroup overTextGroup;
    public CanvasGroup restartButtonGroup;
    public CanvasGroup titleButtonGroup;

    private bool buttonsEnabled = true;

    [Header("遷移先シーン名")]
    public string firstStageSceneName = "Stage";
    public string titleSceneName = "Title";

    void Start()
    {
        // 最初からすべて表示する
        SetAlpha(gameTextGroup, 1f);
        SetAlpha(overTextGroup, 1f);
        SetAlpha(restartButtonGroup, 1f);
        SetAlpha(titleButtonGroup, 1f);

        // 最初からボタンを押せるようにする
        SetInteractable(restartButtonGroup, true);
        SetInteractable(titleButtonGroup, true);
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

    // はじめからボタン
    public void RestartGame()
    {
        if (!buttonsEnabled) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(firstStageSceneName);
    }

    // タイトルへ戻るボタン
    public void ReturnToTitle()
    {
        if (!buttonsEnabled) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}
