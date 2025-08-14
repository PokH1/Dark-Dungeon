using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPerWave;
    private Animator bossAnimator;
    public AudioSource backgroundMusic;
    public AudioSource bossMusic;
    void Start()
    {
        bossPerWave.SetActive(false);

        if (bossMusic != null)
        {
            bossMusic.Stop();
            bossMusic.loop = true;
        }
    }

    // Update is called once per frame
void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bossPerWave.SetActive(true);

            if (bossAnimator == null)
                bossAnimator = bossPerWave.GetComponent<Animator>();

            if (bossAnimator != null)
            {
                bossAnimator.SetTrigger("BattleStance");
            }

            BossHealth bossHealth = bossPerWave.GetComponent<BossHealth>();

            if (bossHealth != null && bossHealth.bossHealthBarCanvas != null)
            {
                bossHealth.bossHealthBarCanvas.SetActive(true);
            }

            Debug.Log("¡El Jefe final apareció con animación!");

            if (backgroundMusic != null)

                backgroundMusic.Stop();

            if (bossMusic != null)
                bossMusic.Play();
        }
    }

}
