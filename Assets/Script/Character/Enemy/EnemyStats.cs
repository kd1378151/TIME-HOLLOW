
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

    //攻撃を受けた時の演出
    private DamageFlash damageFlash;

    void Start()
    {
        // 初期HPを最大HPにする
        currentHP = maxHP;

        // Animatorを取得する
        animator = GetComponent<Animator>();

        //攻撃を受けた時の演出
        damageFlash = GetComponentInChildren<DamageFlash>();
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

        // ダメージを受けたら白く点滅する
        if (damageFlash != null)
        {
            damageFlash.Flash();
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
    }

    // 死亡モーションが終わったら敵を削除する
    public void FinishDeath()
    {
        Destroy(gameObject);
    }
}
