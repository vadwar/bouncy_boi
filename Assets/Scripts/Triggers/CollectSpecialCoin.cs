using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CollectSpecialCoin : MonoBehaviour
{
    public CoinInfo coin;
    public SpecialCoinManager counter;
    void Start()
    {
        counter = FindFirstObjectByType<SpecialCoinManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D guy)
    {
        if (guy.CompareTag("Player"))
        {
            counter.AddSpecialCoin(coin.value);
            AudioSource.PlayClipAtPoint(coin.pickupSound, transform.position);
            Destroy(gameObject);
        }
    }
}
