using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace AbstractionServer
{
    public class FinishRunService : MonoBehaviour
    {
        [Header("UI")]
        public GameObject gameOverCanvas;
        public void OnPlayerDeath(int[] itemsFound, float survivalTime, int monstersDefeated)
        {
            StartCoroutine(FinishRunCoroutine(itemsFound, survivalTime, monstersDefeated));
        }

        private IEnumerator FinishRunCoroutine(int[] itemsFound, float survivalTime, int monstersDefeated)
        {
            Debug.Log("Enviando estadisticas de la Run al smart contract: ");

            int maxItems = 6;
            var safeItems = itemsFound
                .Distinct()
                .Take(maxItems)
                .ToArray();

            object[] callArgumentsWrapper = new object[]
            {
                safeItems,
                (int)survivalTime,
                monstersDefeated
            };

            bool success = false;

            yield return StartCoroutine(
                AbstractionApiClient.PostArray(
                    "/service/command/finishrun",
                    callArgumentsWrapper,
                    response =>
                    {
                        Debug.Log("Run finalizada correctamente: " + response);
                        success = true;
                    },
                    error =>
                    {
                        Debug.Log("Error al finalizar la run: " + error);
                        StartCoroutine(RefreshTokenAndRetry(itemsFound, survivalTime, monstersDefeated));
                        success = false;
                    }
                )
            );

            if (gameOverCanvas != null)
            {
                gameOverCanvas.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Error en el canvas");
            }

        }
        
        private IEnumerator RefreshTokenAndRetry(int [] itemsFound, float  survivalTime, int monstersDefeated)
        {
            yield return StartCoroutine(
                AbstractionApiClient.Post<object, string>(
                    "/auth/refresh",
                    new object[] { },
                    refreshResponse =>
                    {
                        Debug.Log("Refresh completado: " + refreshResponse);
                        StartCoroutine(FinishRunCoroutine(itemsFound, survivalTime, monstersDefeated));
                    },
                    refreshError =>
                    {
                        Debug.LogError("Error al refrescar el token: " + refreshError);
                    }
                )
            );
        }
    }
}
