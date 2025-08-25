using System.Collections.Generic;
using UnityEngine;
using NFTType = AbstractionServer.NFT;


public class NFTSpawner : MonoBehaviour
{
    [Header("Prefab que representará un NFT en el juego")]
    public GameObject nftPrefab;

    [Header("Posiciones de spawn")]
    public Transform[] spawnPoints;

    void Start()
    {
        List<NFTType> nfts = GameData.SelectedNFTs;

        if (nfts == null || nfts.Count == 0)
        {
            Debug.Log("No hay NFTs seleccionados. Juego inicia vacío.");
            return;
        }

        Debug.Log("NFTs recibidos en la escena: " + nfts.Count);

        for (int i = 0; i < nfts.Count && i < spawnPoints.Length; i++)
        {
            // Instancia el prefab en la posición
            GameObject obj = Instantiate(nftPrefab, spawnPoints[i].position, Quaternion.identity);

            // Opcional: asignar nombre al objeto
            obj.name = nfts[i].name;

            // Si tu prefab tiene un script para mostrar la imagen del NFT, aquí puedes asignarlo
            // NFTDisplay display = obj.GetComponent<NFTDisplay>();
            // if (display != null)
            // {
            //     display.SetNFT(nfts[i]);
            // }
        }
    }
}
