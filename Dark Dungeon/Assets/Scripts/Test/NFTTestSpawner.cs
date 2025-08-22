using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NFTTestSpawner : MonoBehaviour
{
    public NFTSelectionUI nftSelectionUI; // Tu script del Canvas
    public Sprite[] nftSprites;           // Imágenes de prueba para los NFTs

    void Start()
    {
        // Crear lista de NFTs de prueba
        List<NFT> testNFTs = new List<NFT>();

        int count = Mathf.Min(nftSprites.Length, 10); // Máximo 10 NFTs para el test
        for (int i = 0; i < count; i++)
        {
            testNFTs.Add(new NFT
            {
                name = "NFT " + (i + 1),
                image = nftSprites[i],
                selected = false
            });
        }

        // Asignar al Canvas y mostrar
        nftSelectionUI.playerNFTs = testNFTs;
        nftSelectionUI.DisplayNFTs();
    }
}
