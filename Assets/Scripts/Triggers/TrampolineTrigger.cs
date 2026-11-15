using UnityEngine;

public class TrampolineTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.stateMachine.ChangeState(player.trampolineCollide);
        }
    }
}
