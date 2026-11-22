using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private AudioClip BirdDeathSound;
    [SerializeField] private GameObject DeathExplosion;
    private bool _explosionOver;

    [Range(0f, 5f)] public float heightAdjustY = 1f;
    Vector3 heightAdjust = new Vector3();

    void Start()
    {
        heightAdjust.y = heightAdjustY;
    }

    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            StartCoroutine(DeathRoutine(collider));
        }
    }

    IEnumerator DeathRoutine(Collider2D collider)
    {
        Vector3 colliderPos = collider.transform.position;
        Destroy(collider.gameObject);
        AudioSource.PlayClipAtPoint(BirdDeathSound, colliderPos);
        Instantiate(DeathExplosion, colliderPos + heightAdjust, Quaternion.identity);
        yield return new WaitUntil(() => _explosionOver);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        if (PlayerMovement.AnimationTriggerType.DeathAnimationEnd == triggerType)
        {
            _explosionOver = true;
        }
    }
}
