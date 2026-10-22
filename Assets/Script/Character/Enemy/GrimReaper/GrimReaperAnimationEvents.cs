
using UnityEngine;

public class GrimReaperAnimationEvents : MonoBehaviour
{
    [SerializeField] private EnemyAttackHitbox attackHitbox;

    public void EnableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.EnableHitbox();
        }
    }

    public void DisableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.DisableHitbox();
        }
    }
}
