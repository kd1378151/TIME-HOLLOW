using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationResultManager : MonoBehaviour
{
    // リスタートボタンを押したとき
    public void Restart()
    {
        SceneManager.LoadScene("Stage");
    }
    // 強化ボタンを押したとき
    public void OpenUpgrade()
    {
        SceneManager.LoadScene("Upgrade");
    }
}