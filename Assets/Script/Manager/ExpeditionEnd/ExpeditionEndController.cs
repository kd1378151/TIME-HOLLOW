using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExplorationResultManager : MonoBehaviour
{
    [Header("表示するUI")]
    public CanvasGroup resultTextGroup;
    public CanvasGroup restartButtonGroup;
    public CanvasGroup upgradeButtonGroup;

    private bool buttonsEnabled = false;

    void Start()
    {
        // 文字を最初から表示
        SetVisible(resultTextGroup);

        // ボタンを最初から表示
        SetVisible(restartButtonGroup);
        SetVisible(upgradeButtonGroup);

        // ボタンを最初から押せるようにする
        buttonsEnabled = true;

        SetInteractable(restartButtonGroup, true);
        SetInteractable(upgradeButtonGroup, true);
    }

    // UIを表示する
    void SetVisible(CanvasGroup group)
    {
        if (group != null)
        {
            group.alpha = 1f;
        }
    }

    // ボタンの操作設定
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
