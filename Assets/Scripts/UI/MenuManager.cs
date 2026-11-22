using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject mainMenuCanvas;

    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuFirst;

    private bool _isPaused;

    void Start()
    {
        mainMenuCanvas.SetActive(false);
    }

    void Update()
    {
        if ((InputManager.instance.menuInput))
        {
            if (!_isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    #region Pause/Unpause/Restart Functions

    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;

        InputManager.PlayerInput.SwitchCurrentActionMap("UI");

        OpenMainMenu();
    }

    public void Unpause()
    {
        _isPaused = false;
        Time.timeScale = 1f;

        InputManager.PlayerInput.SwitchCurrentActionMap("Player");

        CloseAllMenus();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Menu Activation/Deactivation
    private void OpenMainMenu()
    {
        mainMenuCanvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    private void CloseAllMenus()
    {
        mainMenuCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion

    #region Main Menu Button Actions

    public void OnResumePress()
    {
        Unpause();
    }

    public void OnRestartPress()
    {
        Restart();
    }

    #endregion
}
