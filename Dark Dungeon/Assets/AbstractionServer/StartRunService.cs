using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AbstractionServer
{
    public class StartRunService : MonoBehaviour
    {

        
        public NFTService nftService;
        public string gameScene = "Demo";

        private void Awake()
            {
                nftService = GetComponent<NFTService>();
            }

        public void StartRun()
        {
            StartCoroutine(StartRunCoroutine());
        }

        private IEnumerator StartRunCoroutine()
        {

            yield return StartCoroutine(
                nftService.NFTsSelectedByUser(selectedNFTs =>
                {
                    if (selectedNFTs == null || selectedNFTs.Length == 0)
                    {
                        selectedNFTs = new NFT[0];
                    }

                    Debug.Log("NFTs seleccionados recuperados: " + selectedNFTs.Length);
                    string[] nftIds = new string[selectedNFTs.Length];
                    for (int i = 0; i < selectedNFTs.Length; i++)
                        nftIds[i] = selectedNFTs[i].id;


                    object[] callArgumentsWrapper = new object[]
                    {
                        // AbstractionApiClient.userId,
                        nftIds
                    };

                    StartCoroutine(
                        AbstractionApiClient.PostArray(
                            "/service/command/startrun",
                            callArgumentsWrapper,
                            response =>
                            {
                                Debug.Log("Run iniciada correctamente: " + response);
                                SceneManager.LoadScene(gameScene);
                            },
                            error =>
                            {
                                Debug.LogError("Error al iniciar la run: " + error);
                                StartCoroutine(RefreshTokenAndRetry());
                            }
                        )
                    );
                }, error =>
                {
                    Debug.LogError("No se pudieron recuperar los NFTs: " + error);
                })
            );
        }

        private IEnumerator RefreshTokenAndRetry()
        {
            yield return StartCoroutine(
                AbstractionApiClient.Post<object, string>(
                    "/auth/refresh",
                    new object[] { },
                    refreshResponse =>
                    {
                        Debug.Log("Refresh completado: " + refreshResponse);
                        StartCoroutine(StartRunCoroutine());
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
