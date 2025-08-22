using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;       // Panel principal
    public GameObject nftSelection;   // Panel de selección de NFTs

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

        // Método para abrir el panel de NFTs
    public void OpenNFTSelection()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);

        if (nftSelection != null)
            nftSelection.SetActive(true);
    }

    // Método para volver al menú principal (opcional)
    public void BackToMainMenu()
    {
        if (nftSelection != null)
            nftSelection.SetActive(false);

        if (mainMenu != null)
            mainMenu.SetActive(true);
    }
}
