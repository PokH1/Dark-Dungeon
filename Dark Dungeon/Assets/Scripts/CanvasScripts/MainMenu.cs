using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    void Start()
    {
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Demo");
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego");
        // Application.Quit();
    }
}
