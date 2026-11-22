using UnityEngine;

public class CollectSpecialCoin : MonoBehaviour
{
    public CoinInfo coin;
    void Start()
    {

    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D guy)
    {
        if (guy.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(coin.pickupSound, transform.position);
            Destroy(gameObject);
        }
    }
}
