using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour
{
    // Switches to a scene that will be specified by the event handler.
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    // Switches the the scene that was previously loaded.
    public void LoadPreviousLevel()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("previousScene"));
    }

    // Closes the application.
    public void Exit()
    {
        Application.Quit();
    }
}
