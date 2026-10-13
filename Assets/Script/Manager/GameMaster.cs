
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    #region Variables
    [SerializeField] private float m_timerMaxTime;

    public float m_dispayTime;

    private bool m_isCountDown = true;
    private bool m_isTimeOver = false;

    private float m_times = 0;

    public PlayerMovement m_playerController;

    [SerializeField] private GameObject m_spawnPoint;
    #endregion

    #region Unity Methods
    void Start()
    {
        NewGameStart();
    }

    void Update()
    {
        if (m_isTimeOver)
        {
            RestartGame();
            return;
        }

        TimerUpdate();
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
    }

    void RestartGame()
    {
        NewGameStart();
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
}
