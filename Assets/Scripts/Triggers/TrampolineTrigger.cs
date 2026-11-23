using UnityEngine;

public class TrampolineTrigger : MonoBehaviour
{
    [SerializeField] private float direction; // follows NESW as 0123
    [SerializeField] private AudioClip TrampolineBounceSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            AudioSource.PlayClipAtPoint(TrampolineBounceSound, this.transform.position);
            player.trampolineCollide.SetDirection(direction);
            player.stateMachine.ChangeState(player.trampolineCollide);
        }
    }
}
