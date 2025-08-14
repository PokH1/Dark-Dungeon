using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
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
        if (scene.name == "Demo")  // Nombre exacto de la escena que cargas
        {
            GamePaused gamePaused = FindObjectOfType<GamePaused>();
            if (gamePaused != null)
            {
                gamePaused.ResetPause();
                Debug.Log("ResetPause() llamado tras cargar la escena Demo");
            }
            else
            {
                Debug.LogWarning("No se encontró GamePaused en la escena Demo");
            }
        }
    }
}
