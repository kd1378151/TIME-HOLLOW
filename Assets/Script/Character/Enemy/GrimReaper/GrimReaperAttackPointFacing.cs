
using UnityEngine;

public class GrimReaperAttackPointFacing : MonoBehaviour
{
    [Header("敵本体のスプライト")]
    [SerializeField] private SpriteRenderer enemySprite;

    [Header("攻撃判定のコライダー")]
    [SerializeField] private BoxCollider2D attackCollider;

    private Vector2 originalOffset;

    private void Awake()
    {
        if (attackCollider != null)
        {
            originalOffset = attackCollider.offset;
        }
    }

    private void LateUpdate()
    {
        if (enemySprite == null || attackCollider == null)
        {
            return;
        }

        Vector2 offset = originalOffset;

        if (enemySprite.flipX)
        {
            offset.x = -originalOffset.x;
        }

        attackCollider.offset = offset;
    }
}
