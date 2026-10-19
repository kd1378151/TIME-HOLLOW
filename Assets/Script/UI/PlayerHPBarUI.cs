using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPBarUI : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private PlayerStatus m_playerStatus;
    [SerializeField] private Slider m_hpSlider;
    [SerializeField] private Image m_fillImage;
    [SerializeField] private TMP_Text m_hpText;

    [Header("Colors")]
    [SerializeField]
    private Color m_normalColor = new Color(0.21f, 0.78f, 0.35f);

    [SerializeField]
    private Color m_lowHPColor = new Color(0.94f, 0.27f, 0.27f);

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)]
    private float m_lowHPThreshold = 0.2f;

    #endregion

    #region Unity Methods


    private void OnEnable()
    {
        FindPlayerStatus();
    }

    private void OnDisable()
    {
        if (m_playerStatus != null)
        {
            m_playerStatus.OnHPChanged -= UpdateHPBar;
        }
    }

    private void FindPlayerStatus()
    {
        m_playerStatus = FindAnyObjectByType<PlayerStatus>();

        if (m_playerStatus == null)
        {
            Debug.LogWarning(
                "PlayerHPBarUI: PlayerStatus not found in scene.",
                this
            );

            return;
        }

        m_playerStatus.OnHPChanged += UpdateHPBar;

        UpdateHPBar( m_playerStatus.GetCurrentHP(), m_playerStatus.GetMaxHP()
        );
    }


    #endregion

    #region UI Methods

    private void UpdateHPBar(int currentHP, int maxHP)
    {
        if (m_hpSlider == null || m_fillImage == null)
            return;

        float normalizedHP = maxHP > 0
            ? (float)currentHP / maxHP
            : 0f;

        m_hpSlider.value = normalizedHP;

        m_fillImage.color = normalizedHP < m_lowHPThreshold
            ? m_lowHPColor
            : m_normalColor;

        if (m_hpText != null)
        {
            m_hpText.text = $"HP: {currentHP} / {maxHP}";
        }
    }

    #endregion
}
