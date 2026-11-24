using UnityEngine;

public class SettingSceneManager : MonoBehaviour
{
    // メインメニューへ戻るボタン
    public void ReturnToMainMenu()
    {
        // 時間停止している場合でも解除
        Time.timeScale = 1f;

        // メインメニューシーンへ移行
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