using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    float elapsed = 0f;
    bool running = false;

    void Start()
    {
        StartTimer();
    }

    void Update()
    {
        if (!running) return;

        elapsed += Time.deltaTime;
        timerText.text = FormatTime(elapsed);
    }

    public void StartTimer()
    {
        running = true;
    }
    
    public void StopTimer()
    {
        running = false;
    }

    public void ResetTimer()
    {
        elapsed = 0f;
        timerText.text = FormatTime(0f);
    }

    public float GetTime() { return elapsed; }

    string FormatTime(float t)
    {
        int minutes = (int)(t / 60f);
        int seconds = (int)(t % 60f);
        int millis = (int)((t * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}.{millis:000}";
    }
}
