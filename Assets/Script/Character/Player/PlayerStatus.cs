using System;
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

    public event Action<int, int> OnHPChanged;

    void Start()
    {
        currentHP = maxHP;
        currentExplorationTime = maxExplorationTime;
    }
    #region HP methods
    public int GetMaxHP()
    {
        return maxHP;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
    #endregion

    //test only
    public void TakeDamage(int amout)
    {
       currentHP -= amout;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
    }
    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(10);
            OnHPChanged?.Invoke(currentHP, maxHP);
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            Heal(10);
            OnHPChanged?.Invoke(currentHP, maxHP);
        }
    }
}