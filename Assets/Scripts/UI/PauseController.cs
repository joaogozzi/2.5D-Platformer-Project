using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        InputManager.Instance.OnPause += TogglePause;
    }

    void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneLoader.ReloadCurrentScene();
    }

    public void GoToMainMenu()
    {
        SceneLoader.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        SceneLoader.QuitGame();
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnPause -= TogglePause;
    }
}
