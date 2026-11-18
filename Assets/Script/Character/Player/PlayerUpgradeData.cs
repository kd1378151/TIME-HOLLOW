
using System.Collections.Generic;
using UnityEngine;

public static class PlayerUpgradeData
{
    public enum SkillType
    {
        Attack,
        Dash,
        Defense
    }

    // 基本ステータス
    public static int MaxHP = 100;
    public static int AttackPower1 = 10;
    public static int AttackPower2 = 30;
    public static float MaxExplorationTime = 100f;

    // 防御スキル
    public static float DefenseSkillDuration = 5f;

    // ダメージ軽減率（30%）
    public static float DefenseDamageReduction = 0.3f;

    // 取得済みパネル
    private static HashSet<string> purchasedNodes =
        new HashSet<string>();

    // 解放済みスキル
    private static HashSet<SkillType> releasedSkills =
        new HashSet<SkillType>();

    public static bool IsPurchased(string nodeId)
    {
        return purchasedNodes.Contains(nodeId);
    }

    public static void MarkPurchased(string nodeId)
    {
        if (!string.IsNullOrEmpty(nodeId))
            purchasedNodes.Add(nodeId);
    }

    // スキル解放
    public static void MarkSkillReleased(SkillType skill)
    {
        releasedSkills.Add(skill);
    }

    // スキル解放確認
    public static bool IsSkillReleased(SkillType skill)
    {
        return releasedSkills.Contains(skill);
    }

    // 初期化
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetData()
    {
        MaxHP = 100;
        AttackPower1 = 10;
        AttackPower2 = 30;
        MaxExplorationTime = 100f;

        DefenseSkillDuration = 5f;
        DefenseDamageReduction = 0.3f;

        purchasedNodes = new HashSet<string>();
        releasedSkills = new HashSet<SkillType>();
    }
}
