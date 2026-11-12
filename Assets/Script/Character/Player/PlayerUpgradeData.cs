
using UnityEngine;

public static class PlayerUpgradeData
{
    // 強化後の能力値
    public static int MaxHP = 100;
    public static int AttackPower1 = 10;
    public static int AttackPower2 = 30;
    public static float MaxExplorationTime = 100f;

    // 強化回数
    public static int HPUpgradeCount = 0;
    public static int AttackUpgradeCount = 0;
    public static int TimeUpgradeCount = 0;

    // ゲーム開始時に初期化
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetData()
    {
        MaxHP = 100;
        AttackPower1 = 10;
        AttackPower2 = 30;
        MaxExplorationTime = 100f;

        HPUpgradeCount = 0;
        AttackUpgradeCount = 0;
        TimeUpgradeCount = 0;
    }
}
