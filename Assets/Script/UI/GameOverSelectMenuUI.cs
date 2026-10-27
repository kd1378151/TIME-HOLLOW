using UnityEngine;

public class GameOverSelectMenuUI : MonoBehaviour
{
    #region Variables
    private SceneFlowManager m_sceneFlowManager;
    #endregion

    #region Unity Methods
    void Start()
    {
         m_sceneFlowManager = FindAnyObjectByType<SceneFlowManager>();
    }
    #endregion
    public void OnClickBackToTitle()
    {
        m_sceneFlowManager.LoadTitleScene();
    }
}
