using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    public NFTSelectionUI nftSelectionUI; // Referencia al Canvas de selección de NFTs
    public GameObject mainMenu; // Nombre de tu escena del menú principal

    // Método que se llama al hacer click en el botón
    public void OnBackButtonPressed()
    {
        // Obtener los NFTs seleccionados (opcional, según tu juego)
        List<NFT> selectedNFTs = nftSelectionUI.GetSelectedNFTs();
        Debug.Log("NFTs seleccionados: " + selectedNFTs.Count);

        // Aquí puedes guardar los NFTs seleccionados en un GameManager o PlayerData si lo necesitas

        // Ocultar panel de selección
        nftSelectionUI.gameObject.SetActive(false);

        // Mostrar panel del menú principal
        if (mainMenu != null)
            mainMenu.SetActive(true);
    }
}
