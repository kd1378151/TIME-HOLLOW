using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingSceneManager : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneFlowManager.Instance.LoadMainMenuScene();
    }
}
