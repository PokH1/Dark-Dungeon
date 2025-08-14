using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePaused : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject shieldHealthHUD;
    public GameObject bossHealth;
    public GameObject enemyHealthHUD;
    public GameObject weaponsSlotsPanels;
    public GameObject playerHealthHUD;
    public GameObject gamePaused;
    public GameObject waveText;
    public GameObject arrowCanva;
    public AudioSource backgroundMusic;
    private AudioSource[] allAudioSources;
    private bool isPaused = false;


    void Start()
    {
        ResetPause();
    }

       void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reiniciar el estado de pausa al cargar la escena
        ResetPause();
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameOver.isGameOver)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void EnsureCanvasReference()
    {
        if (gamePaused == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                Transform pausedTransform = canvas.transform.Find("Paused");
                if (pausedTransform != null)
                    gamePaused = pausedTransform.gameObject;
                else
                    Debug.LogError("No se encontró 'Paused' dentro del Canvas.");
            }
            else
            {
                Debug.LogError("No se encontró 'Canvas' en la escena.");
            }
        }
    }

    public void PauseGame()
    {
        EnsureCanvasReference();

        isPaused = true;

        if (gamePaused != null)
            gamePaused.SetActive(true);
        else
            Debug.LogWarning("Paused es null");
        // optionsMenu.SetActive(false); // Ocultar opciones si estaban abiertas

        shieldHealthHUD.SetActive(false);
        weaponsSlotsPanels.SetActive(false);
        playerHealthHUD.SetActive(false);
        bossHealth.SetActive(false);
        arrowCanva.SetActive(false);
        waveText.SetActive(false);

        Time.timeScale = 0f;

        allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            if (audio.isPlaying)
            {
                audio.Pause();
            }
        }

        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        EnsureCanvasReference();

        isPaused = false;
        if (gamePaused != null)
            gamePaused.SetActive(false);
        // optionsMenu.SetActive(false);

        shieldHealthHUD.SetActive(true);
        weaponsSlotsPanels.SetActive(true);
        playerHealthHUD.SetActive(true);
        arrowCanva.SetActive(true);
        waveText.SetActive(true);

        // Verificar si el jefe está activo en la escena
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null && boss.activeInHierarchy)
        {
            bossHealth.SetActive(true);
        }

        Time.timeScale = 1f;

        if (allAudioSources != null)
        {
            foreach (AudioSource audio in allAudioSources)
            {
                audio.UnPause();
            }
        }

        // Ocultar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void ResetPause()
    {
        EnsureCanvasReference();

        isPaused = false;
        if(gamePaused != null)
            gamePaused.SetActive(false);

        shieldHealthHUD.SetActive(true);
        weaponsSlotsPanels.SetActive(true);
        playerHealthHUD.SetActive(true);
        bossHealth.SetActive(false);
        waveText.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


}
