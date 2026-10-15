
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("体力")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("攻撃力")]
    public int attackPower1 = 10;  // 攻撃1の攻撃力
    public int attackPower2 = 25;  // 攻撃2の攻撃力

    [Header("探索時間")]
    public float maxExplorationTime = 60f;
    public float currentExplorationTime = 60f;

    private Animator animator;
    private Rigidbody2D rb;

    // よろけ中かどうか
    private bool isTakingHit = false;

    // 死亡したかどうか
    private bool isDead = false;

    //プレイヤーの攻撃ヒット
    private DamageFlash damageFlash;

    // 移動・攻撃が可能かをほかのスクリプトから確認する
    public bool CanControl
    {
        get { return !isTakingHit && !isDead; }
    }

    void Start()
    {
        currentHP = maxHP;
        currentExplorationTime = maxExplorationTime;

        // Animatorを取得する
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Rigidbody2Dを取得する
        rb = GetComponent<Rigidbody2D>();

        //プレイヤーが攻撃を受けた時の演出
        damageFlash = GetComponentInChildren<DamageFlash>();
    }

    // 敵からダメージを受けたときに呼ばれる
    public void TakeDamage(int damage)
    {
        // よろけ中・死亡後の追加ダメージを防ぐ
        if (!CanControl) return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);

        // HPが0になった場合
        if (currentHP <= 0)
        {
            isDead = true;
            StopPlayer();

            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
        }
        else
        {
            // HPが残っている場合はよろける
            isTakingHit = true;
            StopPlayer();

            if (animator != null)
            {
                animator.SetTrigger("TakeHit");
            }
        }

        // ダメージを受けたら白く点滅する
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }
    }

    // プレイヤーの移動を停止する
    private void StopPlayer()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    // よろけアニメーション終了時に呼び出す
    public void EndHitReaction()
    {
        // 死亡していたら操作を戻さない
        if (isDead) return;

        isTakingHit = false;
    }
}
