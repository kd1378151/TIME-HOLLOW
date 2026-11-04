
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("遷移先シーン名")]
    [SerializeField] private string firstStageSceneName = "Stage";
    [SerializeField] private string titleSceneName = "Title";

    // はじめから
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstStageSceneName);
    }

    // タイトルへ戻る
    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}
