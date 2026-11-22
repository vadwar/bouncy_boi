using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    int coins = 0;

    public void AddCoin(int amount)
    {
        coins += amount;
        coinText.text = "" + coins;
    }
}
