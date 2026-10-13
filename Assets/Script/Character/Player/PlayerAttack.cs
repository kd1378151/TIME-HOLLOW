using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        // 現在のアニメーションステートを取得
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        // 攻撃アニメーション中は新しい攻撃を受け付けない
        if (state.IsName("Attack1") ||
            state.IsName("Attack2"))
        {
            return;
        }

        // 攻撃1：Jキー
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack1");
        }
        // 攻撃2：Kキー
        else if (Input.GetKeyDown(KeyCode.K))
        {
            animator.ResetTrigger("Attack1");
            animator.SetTrigger("Attack2");
        }
    }
}