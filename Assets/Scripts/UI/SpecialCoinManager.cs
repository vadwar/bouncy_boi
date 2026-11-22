using UnityEngine;
using UnityEngine.UI;

public class SpecialCoinManager : MonoBehaviour
{
    public Image special1;
    public Image special2;
    public Image special3;
    public Color defaultColor = Color.gray;
    public Color collectedColor = Color.white;
    int specialCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        special1.color = defaultColor;
        special2.color = defaultColor;
        special3.color = defaultColor;
    }

    public void AddSpecialCoin(int amount)
    {
        specialCounter += amount;
        if (specialCounter==1)
        {
            special1.color = collectedColor;
        } else if (specialCounter == 2)
        {
            special2.color = collectedColor;
        } else
        {
            special3.color = collectedColor;
        }
    }
}
