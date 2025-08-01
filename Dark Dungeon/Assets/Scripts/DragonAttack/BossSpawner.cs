using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPerWave;
    private Animator bossAnimator;
    void Start()
    {
        bossPerWave.SetActive(false);
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

            Debug.Log("¡El Jefe final apareció con animación!");
        }
    }

}
