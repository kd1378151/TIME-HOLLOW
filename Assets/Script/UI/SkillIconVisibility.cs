using UnityEngine;

public class SkillIconVisibility : MonoBehaviour
{
    [Header("表示するアイコン")]
    [SerializeField] private GameObject skillIcon;

    [Header("解放するスキル")]
    [SerializeField] private PlayerUpgradeData.SkillType skillType;

    private void Update()
    {
        if (skillIcon == null)
            return;

        bool isReleased =
            PlayerUpgradeData.IsSkillReleased(skillType);

        if (skillIcon.activeSelf != isReleased)
        {
            skillIcon.SetActive(isReleased);
        }
    }
}