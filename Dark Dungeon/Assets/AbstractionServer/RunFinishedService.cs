using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractionServer
{
    [Serializable]
    public class RunFinishedRequest
    {
        public int monsters_defeated;
        public List<ulong> items_found;
        public List<ulong> new_items_selected;
        public ulong survival_time;
    }

    public class RunFinishedService : MonoBehaviour
    {
        public void RunFinished(int monstersDefeated, List<ulong> itemsFound, List<ulong> newItemsSelected, ulong survivalTime)
        {
            var runFinishedData = new RunFinishedRequest
            {
                monsters_defeated = monstersDefeated,
                items_found = itemsFound,
                new_items_selected = newItemsSelected,
                survival_time = survivalTime
            };

            StartCoroutine(RunFinishedCoroutine(runFinishedData));
        }

        private IEnumerator RunFinishedCoroutine(RunFinishedRequest data)
        {
            yield return AbstractionApiClient.Post<RunFinishedRequest, string>(
                "/service/command/runfinished",
                data,
                response =>
                {
                    Debug.Log("Run finalizada correctamente: " + response);
                },
                error =>
                {
                    Debug.LogError("Error al finalizar run: " + error);
                    StartCoroutine(RefreshTokenAndRetry(data));
                }
            );
        }

        private IEnumerator RefreshTokenAndRetry(RunFinishedRequest data)
        {
            yield return AbstractionApiClient.Post<object, string>(
                "/auth/refresh",
                new object(),
                refreshResponse =>
                {
                    Debug.Log("Refresh completado: " + refreshResponse);
                    StartCoroutine(RunFinishedCoroutine(data));
                },
                refreshError =>
                {
                    Debug.LogError("Error al refrescar el token: " + refreshError);
                }
            );
        }
    }
}
