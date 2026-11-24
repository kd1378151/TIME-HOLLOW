using UnityEngine;
<<<<<<< HEAD
using UnityEngine.SceneManagement;

public class SettingSceneManager : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneFlowManager.Instance.LoadMainMenuScene();
    }
}
=======

public class SettingSceneManager : MonoBehaviour
{
    // タイトルへ戻るボタン
    public void ReturnToMainMenu()
    {
        // 時間停止している場合でも解除
        Time.timeScale = 1f;

        // タイトルシーンへ移行
        if (SceneFlowManager.Instance != null)
        {
            SceneFlowManager.Instance.LoadMainMenuScene();
        }
        else
        {
            Debug.LogError("SceneFlowManager.Instance が見つかりません！");
        }
    }
}
>>>>>>> save-main-project
