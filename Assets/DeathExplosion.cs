using UnityEngine;

public class DeathExplosion : MonoBehaviour
{
    private PlayerDeath playerDeath;

    private void Start()
    {
        playerDeath = FindFirstObjectByType<PlayerDeath>();
    }

    public void DeathAnimationEnd()
    {
        playerDeath.AnimationTriggerEvent(PlayerMovement.AnimationTriggerType.DeathAnimationEnd);
    }
}
