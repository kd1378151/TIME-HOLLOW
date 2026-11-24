
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeNode : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public enum UpgradeType
    {
        HP,
        Attack,
        ExplorationTime,
        AttackSkillAll,
        DashSkillAll,
        DefenseSkillAll
    }

    [Header("基本設定")]
    public string nodeId;

    [TextArea(2, 5)]
    public string description;

    public int experienceCost = 10;

    [Header("解放設定")]
    public bool startUnlocked = false;
    public UpgradeNode prerequisiteNode;

    [Header("スキル解放設定")]
    public bool isReleaseNode = false;
    public PlayerUpgradeData.SkillType skillToRelease;

    [Header("強化種類")]
    public UpgradeType upgradeType;

    [Header("基本ステータス強化")]
    public int hpAmount = 20;
    public int attackAmount1 = 2;
    public int attackAmount2 = 4;
    public float timeAmount = 30f;

    [Header("攻撃スキル強化")]
    public float attackSkillCostReduction = 5f;
    public float attackSkillMultiplierAmount = 0.3f;

    [Header("高速移動スキル強化")]
    public float dashSkillCostReduction = 2f;
    public float dashSkillSpeedAmount = 2f;

    [Header("防御スキル強化")]
    public float defenseSkillCostReduction = 2f;
    public float defenseReductionAmount = 0.05f;

    [Header("パネル")]
    public Button panelButton;
    public Image panelImage;

    [Header("次のパネルとルートバー")]
    public UpgradeNode[] nextNodes;
    public Image[] routeImages;

    [Header("パネルの色")]
    public Color lockedColor = Color.gray;
    public Color availableColor = Color.white;
    public Color purchasedColor = Color.green;

    [Header("ホバー設定")]
    [Range(0f, 1f)]
    public float hoverDarkenAmount = 0.15f;

    [Header("ルートバーの色")]
    public Color lockedRouteColor = Color.gray;
    public Color availableRouteColor = Color.white;

    private UpgradeManager manager;
    private bool isHovered;
    private Color basePanelColor;

    public bool IsPurchased
    {
        get
        {
            return PlayerUpgradeData.IsPurchased(nodeId);
        }
    }

    public bool IsUnlocked
    {
        get
        {
            if (startUnlocked)
                return true;

            if (prerequisiteNode == null)
                return false;

            return prerequisiteNode.IsPurchased;
        }
    }

    public bool IsAvailable
    {
        get
        {
            return IsUnlocked && !IsPurchased;
        }
    }

    public void Initialize(UpgradeManager upgradeManager)
    {
        manager = upgradeManager;

        if (panelButton == null)
            panelButton = GetComponent<Button>();

        if (panelImage == null)
            panelImage = GetComponent<Image>();

        if (panelButton != null)
        {
            panelButton.onClick.RemoveListener(OnClick);
            panelButton.onClick.AddListener(OnClick);

            panelButton.transition = Selectable.Transition.None;
            panelButton.interactable = true;
        }

        UpdateVisual();
    }

    private void OnClick()
    {
        if (manager != null)
            manager.SelectNode(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        ApplyPanelColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        ApplyPanelColor();
    }

    private void ApplyPanelColor()
    {
        if (panelImage == null)
            return;

        panelImage.color = isHovered
            ? Color.Lerp(
                basePanelColor,
                Color.black,
                hoverDarkenAmount
            )
            : basePanelColor;
    }

    public void UpdateVisual()
    {
        if (panelImage != null)
        {
            if (IsPurchased)
                basePanelColor = purchasedColor;
            else if (IsUnlocked)
                basePanelColor = availableColor;
            else
                basePanelColor = lockedColor;

            ApplyPanelColor();
        }

        if (panelButton != null)
            panelButton.interactable = true;

        UpdateRoutes();
    }

    private void UpdateRoutes()
    {
        if (routeImages == null)
            return;

        for (int i = 0; i < routeImages.Length; i++)
        {
            if (routeImages[i] == null)
                continue;

            bool nextUnlocked =
                nextNodes != null &&
                i < nextNodes.Length &&
                nextNodes[i] != null &&
                nextNodes[i].IsUnlocked;

            routeImages[i].color = nextUnlocked
                ? availableRouteColor
                : lockedRouteColor;
        }
    }
}
