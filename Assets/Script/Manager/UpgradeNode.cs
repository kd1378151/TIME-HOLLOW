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
        DefenseSkill
    }

    [Header("基本設定")]
    public string nodeId;

    [TextArea(2, 5)]
    public string description;

    public int experienceCost = 10;

    [Header("解放設定")]
    public bool startUnlocked = false;
    public UpgradeNode prerequisiteNode;

    [Header("RELEASE設定")]
    public bool isReleaseNode = false;

    public PlayerUpgradeData.SkillType skillToRelease;

    [Header("強化設定")]
    public UpgradeType upgradeType;

    public int hpAmount = 20;
    public int attackAmount1 = 2;
    public int attackAmount2 = 4;
    public float timeAmount = 30f;

    [Header("防御スキル強化")]
    public float defenseDurationAmount = 1f;
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

    [Header("マウスを乗せたときの色")]
    [Range(0f, 1f)]
    public float hoverDarkenAmount = 0.15f;

    [Header("ルートバーの色")]
    public Color lockedRouteColor = Color.gray;
    public Color availableRouteColor = Color.white;

    private UpgradeManager manager;

    // マウスが乗っているか
    private bool isHovered = false;

    // 通常時のパネル色
    private Color basePanelColor;

    public bool IsPurchased
    {
        get { return PlayerUpgradeData.IsPurchased(nodeId); }
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

            // Buttonによる色変化は使用しない
            panelButton.transition = Selectable.Transition.None;

            // 未解放パネルもクリックできるようにする
            panelButton.interactable = true;
        }

        UpdateVisual();
    }

    private void OnClick()
    {
        if (manager != null)
            manager.SelectNode(this);
    }

    // マウスがパネルに乗ったとき
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        ApplyPanelColor();
    }

    // マウスがパネルから離れたとき
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        ApplyPanelColor();
    }

    // パネルの色を適用
    private void ApplyPanelColor()
    {
        if (panelImage == null)
            return;

        if (isHovered)
        {
            // 現在の色を少し暗くする
            panelImage.color = Color.Lerp(
                basePanelColor,
                Color.black,
                hoverDarkenAmount
            );
        }
        else
        {
            // 通常の色に戻す
            panelImage.color = basePanelColor;
        }
    }

    public void UpdateVisual()
    {
        // パネルの通常色を設定
        if (panelImage != null)
        {
            if (IsPurchased)
                basePanelColor = purchasedColor;
            else if (IsUnlocked)
                basePanelColor = availableColor;
            else
                basePanelColor = lockedColor;

            // マウスが乗っている場合も状態に応じて色を更新
            ApplyPanelColor();
        }

        // 未解放パネルも選択可能
        if (panelButton != null)
            panelButton.interactable = true;

        // ルートバーの色
        for (int i = 0; i < routeImages.Length; i++)
        {
            if (routeImages[i] == null)
                continue;

            bool nextUnlocked =
                i < nextNodes.Length &&
                nextNodes[i] != null &&
                nextNodes[i].IsUnlocked;

            routeImages[i].color =
                nextUnlocked
                ? availableRouteColor
                : lockedRouteColor;
        }
    }
}