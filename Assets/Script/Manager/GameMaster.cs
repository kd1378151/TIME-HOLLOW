using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    #region Prefabs
    [SerializeField] private GameObject m_appRootPrefab;
    #endregion

    #region Variables
    public bool m_timerEnabled = true;

    public Slider m_timeSlider;
    public Image m_timeSliderColor;
    public TextMeshProUGUI m_timeText;
    public float m_dispayTime;

    private bool m_isCountDown = true;
    private bool m_isTimeOver = false;
    private float m_times = 0;

    private bool m_isEndSceneLoading = false;

    public PlayerMovement m_playerController;

    [SerializeField] private GameObject m_spawnPoint;
    [SerializeField] private GameObject m_selectMenu;

    // プレイヤーのパラメータを取得
    private PlayerStatus m_playerStatus;
    #endregion

    #region Unity Methods
    void Start()
    {
        SceneFlowManager manager =
            FindAnyObjectByType<SceneFlowManager>();

        if (manager == null)
        {
            CreateAppRoot();
        }

        // PlayerMovementからPlayerStatusを取得
        if (m_playerController != null)
        {
            m_playerStatus =
                m_playerController.GetComponent<PlayerStatus>();

            if (m_playerStatus == null)
            {
                m_playerStatus =
                    m_playerController.GetComponentInChildren<PlayerStatus>();
            }
        }
        else
        {
            Debug.LogWarning(
                "PlayerStatusが見つかりません。GameMasterの設定を確認してください。");
        }

        NewGameStart();
    }

    void Update()
    {
if (m_isTimeOver)
{
    if (!m_isEndSceneLoading)
    {
        m_isEndSceneLoading = true;
        StartCoroutine(LoadExpeditionEnd());
    }
    return;
}

        if (m_timerEnabled)
        {
            TimerUpdate();
        }
    }
    #endregion

    void TimerUpdate()
    {
        if (m_playerStatus == null) return;

        float maxTime = m_playerStatus.maxExplorationTime;

        m_times += Time.deltaTime;

        if (m_isCountDown)
        {
            m_dispayTime = Mathf.Max(0, maxTime - m_times);
        }
        else
        {
            m_dispayTime = Mathf.Min(m_times, maxTime);
        }

        if (m_times >= maxTime)
        {
            m_isTimeOver = true;
        }

        m_playerStatus.currentExplorationTime = m_dispayTime;

        if (m_timeSlider != null)
        {
            m_timeSlider.value = maxTime > 0
                ? m_dispayTime / maxTime
                : 0f;
        }

        if (m_timeSliderColor != null)
        {
            float ratio = maxTime > 0
                ? m_dispayTime / maxTime
                : 0f;

            m_timeSliderColor.color =
                Color.Lerp(Color.red, Color.green, ratio);
        }

        if (m_timeText != null)
        {
            m_timeText.text = m_dispayTime.ToString("F0");
        }
    }

    public void RestartGame()
    {
        NewGameStart();
        ShowSelectMenu(false);
    }

    void NewGameStart()
    {
        m_times = 0;
        m_isTimeOver = false;

        if (m_playerStatus != null)
        {
            m_playerStatus.currentExplorationTime =
                m_playerStatus.maxExplorationTime;
        }

        if (m_playerStatus != null && m_isCountDown)
        {
            m_dispayTime = m_playerStatus.maxExplorationTime;
        }
        else
        {
            m_dispayTime = 0;
        }

        if (m_playerController != null && m_spawnPoint != null)
        {
            m_playerController.transform.position =
                m_spawnPoint.transform.position;
        }

        if (m_timeSlider != null && m_playerStatus != null)
        {
            float maxTime = m_playerStatus.maxExplorationTime;

            m_timeSlider.value = maxTime > 0
                ? m_dispayTime / maxTime
                : 0f;
        }

        if (m_timeText != null)
        {
            m_timeText.text = m_dispayTime.ToString("F0");
        }
    }

    void CreateAppRoot()
    {
        if (m_appRootPrefab != null)
        {
            Instantiate(m_appRootPrefab);
        }
    }

    void ShowSelectMenu(bool isShow = true)
    {
        if (m_selectMenu != null)
        {
            m_selectMenu.SetActive(isShow);
        }
        else
        {
            Debug.LogWarning("Select Menu is not assigned.");
        }
    }

    private IEnumerator LoadExpeditionEnd()
    {
        // 現在の画面を暗くする
        if (m_selectMenu != null)
            m_selectMenu.SetActive(false);

        // 暗転用パネルを探す
        // ※このパネルは現在のゲームシーンにも必要です
        Image fadePanel = null;
        Canvas[] canvases = FindObjectsByType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            Transform panel = canvas.transform.Find("FadePanel");
            if (panel != null)
            {
                fadePanel = panel.GetComponent<Image>();
                if (fadePanel != null)
                    break;
            }
        }

        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;
            fadePanel.gameObject.SetActive(true);

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsed / duration);
                fadePanel.color = color;
                yield return null;
            }

            color.a = 1f;
            fadePanel.color = color;
        }

        SceneManager.LoadScene("ExpeditionEnd");
    }
}
