
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    [Header("探索シーンの名前")]
    public string explorationSceneName = "Stage";

    void Start()
    {
        UpdateCursor();
    }

    void Update()
    {
        UpdateCursor();
    }

    void UpdateCursor()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == explorationSceneName)
        {
            // 探索中は最初からカーソルを消す
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // それ以外の画面では表示する
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
