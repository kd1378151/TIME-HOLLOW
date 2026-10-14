
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("敵の基本パラメータ")]
    public int maxHP = 30;
    public int currentHP;
    public int attackPower = 5;
    public int experienceReward = 10;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        // 初期HPを最大HPにする
        currentHP = maxHP;

        // Animatorを取得する
        animator = GetComponent<Animator>();
    }

    // プレイヤーの攻撃を受けたときに呼び出す
    public void TakeDamage(int damage)
    {
        // 死亡後はダメージを受けない
        if (isDead)
        {
            return;
        }

        // HPを減らし、0未満にならないようにする
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log(gameObject.name + " のHP：" + currentHP);

        // HPが0になったら死亡
        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            // 生きていればよろけモーションを再生
            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }
        }
    }

    void Die()
    {
        isDead = true;

        // 死亡モーションを再生
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        Debug.Log(
            gameObject.name + " を倒した！獲得経験値：" + experienceReward
        );
    }

    // 死亡モーションが終わったら呼び出す
    public void FinishDeath()
    {
        gameObject.SetActive(false);
    }
}
