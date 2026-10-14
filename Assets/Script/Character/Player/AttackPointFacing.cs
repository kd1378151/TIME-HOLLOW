
using UnityEngine;

public class AttackPointFacing : MonoBehaviour
{
    [Header("攻撃判定の親オブジェクト")]
    public Transform attackPoint;

    [Header("攻撃判定のコライダー")]
    public BoxCollider2D attackCollider;

    [Header("向きを判定するスプライト")]
    public SpriteRenderer playerSprite;

    // 右向きのときは 1、左向きのときは -1
    private int facingDirection = 1;

    void LateUpdate()
    {
        if (attackPoint == null ||
            attackCollider == null ||
            playerSprite == null)
        {
            return;
        }

        // スプライトの反転状態から向きを取得
        facingDirection = playerSprite.flipX ? -1 : 1;

        // 攻撃判定の位置を左右反転
        Vector3 pos = attackPoint.localPosition;
        pos.x = Mathf.Abs(pos.x) * facingDirection;
        attackPoint.localPosition = pos;

        // アニメーションで変化する判定の位置も左右反転
        Vector2 offset = attackCollider.offset;
        offset.x = Mathf.Abs(offset.x) * facingDirection;
        attackCollider.offset = offset;
    }
}
