using UnityEngine;

[CreateAssetMenu(fileName = "NewCoin", menuName = "Game/Coin Data")]
public class CoinInfo : ScriptableObject
{
    [SerializeField] public int value = 1;
    [SerializeField] public AudioClip pickupSound;

}