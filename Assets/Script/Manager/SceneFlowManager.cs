using UnityEngine;

public class SceneFlowManager : MonoBehaviour
{
    #region Variables
    public static SceneFlowManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string m_titleSceneName = "Title";
    [SerializeField] private string m_mainMenuSceneName = "MainMenu";
    [SerializeField] private string m_stageSceneName = "Stage";
    [SerializeField] private string m_resultSceneName = "Result";

    private bool m_isLoadingScene = false;
    #endregion

    #region UnityCallingFunc
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region LoadingFunc
    /// <summary>
    /// Loads the specified scene if not already loading another scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    private void LoadScene(string sceneName)
    {
        if (!m_isLoadingScene)
        {
            m_isLoadingScene = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            m_isLoadingScene = false;
        }
    }
    public void LoadTitleScene()
    {
        LoadScene(m_titleSceneName);
    }
    public void LoadMainMenuScene()
    {
        LoadScene(m_mainMenuSceneName);
    }
    public void LoadStageScene()
    {
        LoadScene(m_stageSceneName);
    }
    public void LoadResultScene()
    {
        LoadScene(m_resultSceneName);
    }
    #endregion
}
