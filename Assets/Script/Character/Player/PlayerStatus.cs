using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("体力")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("攻撃力")]
    public int attackPower = 10;

    [Header("探索時間")]
    public float maxExplorationTime = 60f;
    public float currentExplorationTime = 60f;

    void Start()
    {
        currentHP = maxHP;
        currentExplorationTime = maxExplorationTime;
    }
}