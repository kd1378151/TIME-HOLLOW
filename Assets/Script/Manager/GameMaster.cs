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

    [Header("タイマーゲージの色")]
    [SerializeField] private Color m_timerNormalColor = Color.green;
    [SerializeField] private Color m_timerLowColor = Color.red;

    [Header("色が変化し始める残り時間の割合")]
    [SerializeField, Range(0f, 1f)]
    private float m_timerColorChangeThreshold = 0.5f;

    [Header("探索開始の演出")]
    [SerializeField] private float m_fadeInDuration = 3.0f;

    [Header("攻撃時の探索時間減少")]
    [SerializeField] private float attack1TimeReduction = 5f;
    [SerializeField] private float attack2TimeReduction = 7f;

    private bool m_isStarting = true;
    private PlayerAttack m_playerAttack;


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

        m_playerAttack = m_playerController != null
            ? m_playerController.GetComponent<PlayerAttack>()
            : null;

        StartCoroutine(FadeInAndBeginExploration());
    }

    void Update()
    {
        if (m_isStarting)
        {
            return;
        }

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

        m_dispayTime = Mathf.Max(0, maxTime - m_times);

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
                ? Mathf.Clamp01(m_dispayTime / maxTime)
                : 0f;

            float threshold = Mathf.Clamp01(m_timerColorChangeThreshold);

            // 指定した割合までは通常色を維持し、
            // そこから時間切れに向かって低HP色へ変化
            float colorRatio = threshold > 0f
                ? Mathf.Clamp01(ratio / threshold)
                : (ratio > 0f ? 1f : 0f);

            m_timeSliderColor.color = Color.Lerp(
                m_timerLowColor,
                m_timerNormalColor,
                colorRatio
            );
        }

        if (m_timeText != null)
        {
            float ratio = maxTime > 0f
                ? Mathf.Clamp01(m_dispayTime / maxTime)
                : 0f;

            int percentage = Mathf.RoundToInt(ratio * 100f);

            m_timeText.text = percentage + "%";
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
            m_timeText.text = "100%";
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

    private IEnumerator FadeInAndBeginExploration()
    {
        // 暗転パネルを探す
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

        // パネルがあれば、黒から透明へ変化させる
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);

            Color color = Color.black;
            color.a = 1f;
            fadePanel.color = color;

            float elapsed = 0f;

            while (elapsed < m_fadeInDuration)
            {
                elapsed += Time.deltaTime;

                color.a = 1f - Mathf.Clamp01(
                    elapsed / m_fadeInDuration
                );

                fadePanel.color = color;

                yield return null;
            }

            color.a = 0f;
            fadePanel.color = color;
            fadePanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "FadePanelが見つかりません。Canvas内の設定を確認してください。"
            );
        }

        // 演出終了後に操作を許可
        if (m_playerController != null)
            m_playerController.enabled = true;

        if (m_playerAttack != null)
            m_playerAttack.enabled = true;

        m_isStarting = false;
    }

    // 攻撃時に探索時間を減らす
    public void ReduceExplorationTime(float reduction)
    {
        if (m_isStarting || m_isTimeOver) return;
        if (m_playerStatus == null) return;

        m_times += reduction;

        float maxTime = m_playerStatus.maxExplorationTime;

        m_dispayTime = Mathf.Max(0, maxTime - m_times);

        m_playerStatus.currentExplorationTime = m_dispayTime;

        if (m_times >= maxTime)
        {
            m_isTimeOver = true;
        }

        TimerUpdateDisplay();
    }

    // 通常攻撃が可能か確認
    public bool CanAttack1()
    {
        return m_dispayTime >= attack1TimeReduction
            && !m_isTimeOver
            && !m_isStarting;
    }

    // 強攻撃が可能か確認
    public bool CanAttack2()
    {
        return m_dispayTime >= attack2TimeReduction
            && !m_isTimeOver
            && !m_isStarting;
    }

    // 通常攻撃
    public bool ReduceAttack1Time()
    {
        if (!CanAttack1()) return false;

        ReduceExplorationTime(attack1TimeReduction);
        return true;
    }

    public bool ReduceAttack2Time()
    {
        if (!CanAttack2()) return false;

        ReduceExplorationTime(attack2TimeReduction);
        return true;
    }

    private void TimerUpdateDisplay()
    {
        float maxTime = m_playerStatus.maxExplorationTime;

        if (m_timeSlider != null)
        {
            m_timeSlider.value = maxTime > 0
                ? m_dispayTime / maxTime
                : 0f;
        }

        if (m_timeSliderColor != null)
        {
            float ratio = maxTime > 0
                ? Mathf.Clamp01(m_dispayTime / maxTime)
                : 0f;

            float threshold = Mathf.Clamp01(m_timerColorChangeThreshold);

            float colorRatio = threshold > 0f
                ? Mathf.Clamp01(ratio / threshold)
                : (ratio > 0f ? 1f : 0f);

            m_timeSliderColor.color = Color.Lerp(
                m_timerLowColor,
                m_timerNormalColor,
                colorRatio
            );
        }

        if (m_timeText != null)
        {
            float ratio = maxTime > 0
                ? Mathf.Clamp01(m_dispayTime / maxTime)
                : 0f;

            int percentage = Mathf.RoundToInt(ratio * 100f);

            m_timeText.text = percentage + "%";
        }
    }

    // スキル発動時の探索時間消費
    public bool TryReduceSkillTime(float reduction)
    {
        if (m_isStarting || m_isTimeOver) return false;

        // 残り時間が消費時間より少なければ発動しない
        if (m_dispayTime < reduction) return false;

        // 時間を消費
        ReduceExplorationTime(reduction);

        return true;
    }
}