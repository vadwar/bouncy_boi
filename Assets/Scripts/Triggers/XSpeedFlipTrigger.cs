using UnityEngine;

public class XSpeedFlipTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.attachedRigidbody.linearVelocityX = -collision.attachedRigidbody.linearVelocityX;
        }
    }
}
