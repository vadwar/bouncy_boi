using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CollectGoldCoinTrigger : MonoBehaviour
{
    public CoinInfo coin;
    CoinManager counter;
    void Start()
    {
        counter = FindFirstObjectByType<CoinManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D guy)
    {
        if (guy.CompareTag("Player"))
        {
            counter.AddCoin(coin.value);
            AudioSource.PlayClipAtPoint(coin.pickupSound, transform.position);
            Destroy(gameObject);
        }
    }
}
