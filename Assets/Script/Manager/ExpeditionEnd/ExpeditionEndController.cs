using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationResultManager : MonoBehaviour
{
    // リスタートボタンを押したとき
    public void Restart()
    {
        SceneManager.LoadScene("Stage");
    }
}