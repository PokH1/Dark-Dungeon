using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using NFTType = AbstractionServer.NFT;

namespace AbstractionServer
{
    public class StartRunService : MonoBehaviour
    {

        
        public NFTService nftService;
        public string gameScene = "Demo";
        public NFTSelectionUI nFTSelectionUI;

        private void Awake()
        {
            nftService = GetComponent<NFTService>();
            if (nFTSelectionUI == null)
                nFTSelectionUI = FindObjectOfType<NFTSelectionUI>();
        }

        public void StartRun()
        {

            List<NFTType> selectedNFTs = nFTSelectionUI.GetSelectedNFTs()
                    .Select(n => new NFTType
                    {
                        id = n.id,
                        name = n.name,
                        image = n.image,
                        selected = n.selected
                    }).ToList();

            if (selectedNFTs.Count == 0)
            {
                Debug.LogWarning("No se han seleccionado NFTs.");
                // return;
            }

            Debug.Log("IDs de armas que se enviarán a StartRun:");
            foreach (var nft in selectedNFTs)
            {
                Debug.Log("NFT ID: " + nft.id + " | Name: " + nft.name);
            }

            object[] callArgumentsWrapper = selectedNFTs
                        .Select(n => (object)n.id)
                        .ToArray();

                    StartCoroutine(
                        AbstractionApiClient.PostArray(
                            "/service/command/startrun",
                            callArgumentsWrapper,
                            response =>
                            {
                                
                                Debug.Log("Run iniciada correctamente: " + response);
                                GameData.SelectedNFTs = selectedNFTs;
                                if (selectedNFTs.Count > 0)
                                {
                                    GameData.InitialWeaponName = selectedNFTs[0].name;
                                    Debug.Log("Nombre del arma inicial: " + GameData.InitialWeaponName);
                                    // GameData.InitialWeaponID = selectedNFTs[0].id;
                                    // Debug.Log("ID del arma inicial: " + GameData.InitialWeaponID);
                                }
                                SceneManager.LoadScene(gameScene);
                            },
                            error =>
                            {
                                Debug.LogError("Error al iniciar la run: " + error);
                                StartCoroutine(RefreshTokenAndRetry());
                            }
                        )
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
                StartRun();
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
