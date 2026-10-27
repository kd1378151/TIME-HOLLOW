using UnityEngine;

public class TitleSceneUI : MonoBehaviour
{
    public void StartGameButton()
    {
        SceneFlowManager _manager = FindAnyObjectByType<SceneFlowManager>();
        if (_manager != null)
        {
            _manager.LoadStageScene();
        }
    }
}
