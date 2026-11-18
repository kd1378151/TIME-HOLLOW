using UnityEngine;

public class MainMenuSceneUI : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneFlowManager.Instance.LoadStageScene();
    }

    public void OnSettingButtonClicked()
    {
        SceneFlowManager.Instance.LoadSettingScene();
    }
}
