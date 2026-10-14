
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    #region Prefabs
    [SerializeField] private GameObject m_appRootPrefab;
    #endregion

    #region Variables
    public bool m_timerEnabled = true;
    [SerializeField] private float m_timerMaxTime;
    public Slider m_timeSlider;
    public Image m_timeSliderColor;

    public TextMeshProUGUI m_timeText;

    public float m_dispayTime;

    private bool m_isCountDown = true;
    private bool m_isTimeOver = false;

    private float m_times = 0;

    public PlayerMovement m_playerController;

    [SerializeField] private GameObject m_spawnPoint;
    #endregion
    #region GUI
    [SerializeField] private GameObject m_selectMenu;
    #endregion

    #region Unity Methods
    void Start()
    {
        SceneFlowManager _manager = FindAnyObjectByType<SceneFlowManager>();

        if (_manager == null)
        {
            CreateAppRoot();
        }

        NewGameStart();
    }

    void Update()
    {
        if (m_isTimeOver)
        {
            ShowSelectMenu();
        }

        if (m_timerEnabled)
        {
            TimerUpdate();
        }
    }
    #endregion

    void TimerUpdate()
    {
        m_times += Time.deltaTime;

        if (m_isCountDown)
        {
            m_dispayTime = Mathf.Max(0, m_timerMaxTime - m_times);
        }
        else
        {
            m_dispayTime = Mathf.Min(m_times, m_timerMaxTime);
        }

        if (m_times >= m_timerMaxTime)
        {
            m_isTimeOver = true;
            
        }

        if (m_timeSlider != null)
        {
            m_timeSlider.value = m_dispayTime / m_timerMaxTime;
        }
        if(m_timeSliderColor != null)
        {
            m_timeSliderColor.color = Color.Lerp(Color.red, Color.green, m_dispayTime / m_timerMaxTime);
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

        if (m_isCountDown)
        {
            m_dispayTime = m_timerMaxTime;
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
}
