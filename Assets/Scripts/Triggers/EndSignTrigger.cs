using UnityEngine;

public class EndSignTrigger : MonoBehaviour
{
    public AudioClip Victory;
    public SpeedrunTimer timer;
    public WinManager winManager;
    void Start()
    {
        timer = FindAnyObjectByType<SpeedrunTimer>();
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            timer.StopTimer();
            winManager.DoWinStuff();
            AudioSource.PlayClipAtPoint(Victory, transform.position);
            //StartCoroutine(DeathRoutine(collider));
        }
    }
}
