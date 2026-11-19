
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

    // =========================
    // 基本ステータス
    // =========================

    public static int MaxHP = 100;
    public static int AttackPower1 = 10;
    public static int AttackPower2 = 30;
    public static float MaxExplorationTime = 100f;

    // =========================
    // 攻撃スキル
    // =========================

    public static float AttackSkillTimeCost = 30f;
    public static float AttackSkillDuration = 60f;
    public static float AttackSkillMultiplier = 1.5f;

    // =========================
    // 高速移動スキル
    // =========================

    public static float DashSkillTimeCost = 10f;
    public static float DashSkillDuration = 4f;
    public static float DashSkillSpeed = 12f;
    public static float DashAnimatorSpeed = 1.8f;

    // =========================
    // 防御スキル
    // =========================

    public static float DefenseSkillTimeCost = 10f;
    public static float DefenseSkillDuration = 5f;
    public static float DefenseDamageReduction = 0.3f;

    // =========================
    // 初期値設定済みフラグ
    // =========================

    private static bool skillDefaultsInitialized = false;

    // =========================
    // 購入済みパネル
    // =========================

    private static HashSet<string> purchasedNodes =
        new HashSet<string>();

    // =========================
    // 解放済みスキル
    // =========================

    private static HashSet<SkillType> releasedSkills =
        new HashSet<SkillType>();

    // =========================
    // 初期スキル設定
    // =========================

    public static void InitializeSkillDefaults(
        float attackCost,
        float attackDuration,
        float attackMultiplier,
        float dashCost,
        float dashDuration,
        float dashSpeed,
        float dashAnimatorSpeed,
        float defenseCost,
        float defenseDuration,
        float defenseReduction)
    {
        if (skillDefaultsInitialized)
            return;

        AttackSkillTimeCost = attackCost;
        AttackSkillDuration = attackDuration;
        AttackSkillMultiplier = attackMultiplier;

        DashSkillTimeCost = dashCost;
        DashSkillDuration = dashDuration;
        DashSkillSpeed = dashSpeed;
        DashAnimatorSpeed = dashAnimatorSpeed;

        DefenseSkillTimeCost = defenseCost;
        DefenseSkillDuration = defenseDuration;
        DefenseDamageReduction = defenseReduction;

        skillDefaultsInitialized = true;
    }

    // =========================
    // 購入済み確認
    // =========================

    public static bool IsPurchased(string nodeId)
    {
        return purchasedNodes.Contains(nodeId);
    }

    // =========================
    // 購入済みに登録
    // =========================

    public static void MarkPurchased(string nodeId)
    {
        if (!string.IsNullOrEmpty(nodeId))
        {
            purchasedNodes.Add(nodeId);
        }
    }

    // =========================
    // スキル解放
    // =========================

    public static void MarkSkillReleased(SkillType skill)
    {
        releasedSkills.Add(skill);
    }

    // =========================
    // スキル解放確認
    // =========================

    public static bool IsSkillReleased(SkillType skill)
    {
        return releasedSkills.Contains(skill);
    }

    // =========================
    // 攻撃スキル強化
    // =========================

    public static void UpgradeAttackSkillCost(float amount)
    {
        AttackSkillTimeCost =
            Mathf.Max(0f, AttackSkillTimeCost - amount);
    }

    public static void UpgradeAttackSkillPower(float amount)
    {
        AttackSkillMultiplier += amount;
    }

    // =========================
    // 攻撃スキル一括強化
    // 消費時間減少 ＋ 攻撃倍率アップ
    // =========================

    public static void UpgradeAttackSkill(
        float costReduction,
        float powerAmount)
    {
        UpgradeAttackSkillCost(costReduction);
        UpgradeAttackSkillPower(powerAmount);
    }

    // =========================
    // 高速移動スキル強化
    // =========================

    public static void UpgradeDashSkillCost(float amount)
    {
        DashSkillTimeCost =
            Mathf.Max(0f, DashSkillTimeCost - amount);
    }

    public static void UpgradeDashSkillSpeed(float amount)
    {
        DashSkillSpeed += amount;
    }

    // =========================
    // 高速移動スキル一括強化
    // 消費時間減少 ＋ 移動速度アップ
    // =========================

    public static void UpgradeDashSkill(
        float costReduction,
        float speedAmount)
    {
        UpgradeDashSkillCost(costReduction);
        UpgradeDashSkillSpeed(speedAmount);
    }

    // =========================
    // 防御スキル強化
    // =========================

    public static void UpgradeDefenseSkillCost(float amount)
    {
        DefenseSkillTimeCost =
            Mathf.Max(0f, DefenseSkillTimeCost - amount);
    }

    public static void UpgradeDefenseSkillReduction(float amount)
    {
        DefenseDamageReduction =
            Mathf.Clamp(
                DefenseDamageReduction + amount,
                0f,
                0.9f
            );
    }

    // =========================
    // 防御スキル一括強化
    // 消費時間減少 ＋ ダメージ軽減率アップ
    // =========================

    public static void UpgradeDefenseSkillAll(
        float costReduction,
        float reductionAmount)
    {
        UpgradeDefenseSkillCost(costReduction);
        UpgradeDefenseSkillReduction(reductionAmount);
    }

    // =========================
    // 初期化
    // =========================

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetData()
    {
        // 基本ステータス
        MaxHP = 100;
        AttackPower1 = 10;
        AttackPower2 = 30;
        MaxExplorationTime = 100f;

        // 攻撃スキル
        AttackSkillTimeCost = 30f;
        AttackSkillDuration = 60f;
        AttackSkillMultiplier = 1.5f;

        // 高速移動スキル
        DashSkillTimeCost = 10f;
        DashSkillDuration = 4f;
        DashSkillSpeed = 12f;
        DashAnimatorSpeed = 1.8f;

        // 防御スキル
        DefenseSkillTimeCost = 10f;
        DefenseSkillDuration = 5f;
        DefenseDamageReduction = 0.3f;

        skillDefaultsInitialized = false;

        // 購入状態リセット
        purchasedNodes = new HashSet<string>();

        // 解放状態リセット
        releasedSkills = new HashSet<SkillType>();
    }
}
