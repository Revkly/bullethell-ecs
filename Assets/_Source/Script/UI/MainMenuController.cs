using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Load gameplay scene dengan ECS yang di-reset bersih
        ECSWorldResetter.ResetAndLoadScene("Main");
    }

    public void QuitGame()
    {
        Debug.Log("Game Exited!");
        Application.Quit();
    }
}
