using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    public void BackToResult()
    {
        SceneManager.LoadScene("ExpeditionEnd");
    }
}