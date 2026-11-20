using UnityEngine;

public class TitleSceneUI : MonoBehaviour
{
    // ゲーム開始ボタン
    public void StartGameButton()
    {
        SceneFlowManager _manager = FindAnyObjectByType<SceneFlowManager>();
        if (_manager != null)
        {
            _manager.LoadStageScene();

            PlayerExperience.ResetExperience();
        }
    }

    // 設定画面ボタン
    public void SettingsButton()
    {
        SceneFlowManager _manager = FindAnyObjectByType<SceneFlowManager>();
        if (_manager != null)
        {
            _manager.LoadSettingsScene();
        }
    }
}