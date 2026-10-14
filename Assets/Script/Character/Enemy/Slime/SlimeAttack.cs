
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // 攻撃アニメーションを再生するAnimator
    private Animator animator;

    void Start()
    {
        // このオブジェクトに付いているAnimatorを取得
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Lキーを押した瞬間に攻撃する
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
}
