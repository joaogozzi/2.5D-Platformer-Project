using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string firstLevelSceneName;

    public void Play()
    {
        SceneLoader.LoadScene(firstLevelSceneName);
    }

    public void Quit()
    {
        SceneLoader.QuitGame();
    }
}