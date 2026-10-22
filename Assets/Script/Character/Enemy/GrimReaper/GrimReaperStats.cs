
using UnityEngine;

public class GrimReaperStats : MonoBehaviour
{
    [Header("HP設定")]
    [SerializeField] private int maxHP = 300;

    [SerializeField] private int currentHP;

    [Header("攻撃力設定")]
    [SerializeField] private int attackPower = 20;

    // 最大HPを取得
    public int MaxHP => maxHP;

    // 現在HPを取得
    public int CurrentHP => currentHP;

    // 攻撃力を取得
    public int AttackPower => attackPower;

    private void Start()
    {
        // ゲーム開始時にHPを最大まで回復
        currentHP = maxHP;
    }

    // ダメージを受けたときに呼び出す
    public void TakeDamage(int damage)
    {
        // HPが0以下なら何もしない
        if (currentHP <= 0)
        {
            return;
        }

        // ダメージ分だけHPを減らす
        currentHP -= damage;

        // HPがマイナスにならないようにする
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log(
            "死神のHP：" + currentHP + " / " + maxHP
        );

        // HPが0になった
        if (currentHP <= 0)
        {
            Debug.Log("死神のHPが0になりました！");
        }
    }

    // HPを回復するときに呼び出す
    public void Heal(int amount)
    {
        currentHP += amount;

        // 最大HPを超えないようにする
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    // 最大HPを変更する
    public void SetMaxHP(int value)
    {
        maxHP = Mathf.Max(1, value);

        // 現在HPも新しい最大HP以下にする
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    // 攻撃力を変更する
    public void SetAttackPower(int value)
    {
        attackPower = Mathf.Max(0, value);
    }
}
