using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject shieldHealthHUD;
    public GameObject bossHealth;
    public GameObject enemyHealthHUD;
    public GameObject weaponsSlotsPanels;
    public GameObject playerHealthHUD;
    public GameObject gamePaused;
    public GameObject arrowCanva;
    public GameObject waveCanva;
    public AudioSource gameOverMusic;

    public static bool isGameOver = false;
    public void GameOverCanvas()
    {
        // Canvas a desactivar
        shieldHealthHUD.SetActive(false);
        bossHealth.SetActive(false);
        enemyHealthHUD.SetActive(false);
        weaponsSlotsPanels.SetActive(false);
        playerHealthHUD.SetActive(false);
        gamePaused.SetActive(false);
        arrowCanva.SetActive(false);
        waveCanva.SetActive(false);


        gameOverCanvas.SetActive(true);

        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudio)
        {
            audio.Stop();
        }

        if (gameOverMusic != null)
        {
            gameOverMusic.Play();
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isGameOver = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isGameOver = false;

        SceneManager.LoadScene("Demo");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }
}
