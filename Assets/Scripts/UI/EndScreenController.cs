using UnityEngine;

public class EndScreenController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName;

    public void RestartLevel()
    {
        SceneLoader.ReloadCurrentScene();
    }

    public void GoToMainMenu()
    {
        SceneLoader.LoadScene(mainMenuSceneName);
    }

    public void Quit()
    {
        SceneLoader.QuitGame();
    }
}
