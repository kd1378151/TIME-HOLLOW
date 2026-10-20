
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("敵の基本パラメータ")]
    public int maxHP = 30;
    public int currentHP;
    public int attackPower = 5;

    [Header("よろけ設定")]
    public bool canFlinch = true;
    public bool IsTakingHit { get; private set; } = false;

    //スライムのAIスクリプト
    private EnemyAI enemyAI;

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

        //スライムのAIスクリプト
        enemyAI = GetComponent<EnemyAI>();
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

        // 攻撃されたらプレイヤーを発見する
        if (enemyAI != null)
        {
            enemyAI.DetectPlayerWhenAttacked();
        }

        // HPが0になったら死亡
        if (currentHP <= 0)
        {
            Die();
        }
        else if (canFlinch && animator != null)
        {
            IsTakingHit = true;
            animator.SetTrigger("Hurt");
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

    // よろけアニメーション終了時に呼び出す
    // よろけアニメーション終了時に呼び出す
    public void EndHitReaction()
    {
        IsTakingHit = false;
        Debug.Log("よろけ終了：移動を再開できます");
    }
}
