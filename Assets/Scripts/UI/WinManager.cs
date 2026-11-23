using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    [SerializeField] private GameObject WinScreenCanvas;
    public EndSignTrigger EndSign;
    public AudioPlayerManager audioPlayerManager;
    public CoinManager gold;
    public SpecialCoinManager special;
    public SpeedrunTimer timer;
    public bool win;
    void Start()
    {
        WinScreenCanvas.SetActive(false);
        EndSign = FindAnyObjectByType<EndSignTrigger>();
        gold = FindAnyObjectByType<CoinManager>();
        special = FindAnyObjectByType<SpecialCoinManager>();
        timer = FindAnyObjectByType<SpeedrunTimer>();
    }

    public void DoWinStuff()
    {
        win = true;
        ActivateWinScreen();
        InputManager.PlayerInput.SwitchCurrentActionMap("UI");
        audioPlayerManager.TurnOff();
        //KillHUD();
    }

    private void ActivateWinScreen()
    {
        WinScreenCanvas.SetActive(true);
    }

    private void KillHUD()
    {
        gold.SetActive();
        special.SetActive();
        timer.SetActive();
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnRestartPress()
    {
        Restart();
    }
}
