
using UnityEngine;

public class EnemyAttackPointFacing : MonoBehaviour
{
    [Header("敵本体のスプライト")]
    public SpriteRenderer enemySprite;

    [Header("攻撃判定のコライダー")]
    public BoxCollider2D attackCollider;

    // 最初のOffset Xを保存
    private Vector2 originalOffset;

    void Awake()
    {
        if (attackCollider != null)
        {
            originalOffset = attackCollider.offset;
        }
    }

    void LateUpdate()
    {
        if (enemySprite == null || attackCollider == null)
        {
            return;
        }

        // 元のOffsetを基準に左右を切り替える
        Vector2 offset = originalOffset;

        if (enemySprite.flipX)
        {
            offset.x = -originalOffset.x;
        }

        attackCollider.offset = offset;
    }
}
