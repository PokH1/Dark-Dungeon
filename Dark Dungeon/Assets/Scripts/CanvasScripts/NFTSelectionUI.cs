using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using AbstractionServer;
using System.Text;
using Newtonsoft.Json;

[System.Serializable]
public class NFT
{
    public string id;
    public string name;
    public string imageUrl;  // URL de la imagen
    public Sprite image;     // Sprite descargado
    public bool selected;
}

// Clase contenedora para deserializar la respuesta del servidor
[System.Serializable]
public class NFTResponse
{
    public List<NFT> nfts;
    public ContractMessage contractMessage;
}

[System.Serializable]
public class ContractMessage
{
    public List<NFT> userNFTs;
}

public class NFTSelectionUI : MonoBehaviour
{
    [Header("Prefab y Content")]
    public GameObject nftPrefab;       // Prefab del NFT (Button o Image)
    public Transform contentParent;    // Content del ScrollView

    [Header("Lista de NFTs")]
    public List<NFT> playerNFTs = new List<NFT>();       // NFTs del jugador cargados desde el backend

    private List<GameObject> nftItems = new List<GameObject>();

    void Start()
    {
        StartCoroutine(LoadNFTsFromService());
    }

    IEnumerator LoadNFTsFromService()
    {
        string url = "http://localhost:3001/service/query/nftsfromuser";

        // Construimos el JSON con el userId
        JObject bodyJson = new JObject();
        bodyJson["callArguments"] = new JArray(AbstractionApiClient.userId);
        string bodyString = bodyJson.ToString();

        UnityWebRequest request = new UnityWebRequest(url, "GET");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error al obtener NFTs: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Respuesta del servidor: " + json);

            // Parsear JSON
            var jsonObj = JObject.Parse(json);
            var userNFTsArray = jsonObj["contractMessage"]["userNFTs"] as JArray;

            playerNFTs.Clear();

            foreach (var item in userNFTsArray)
            {
                string tokenId = item[0].ToString();
                var metadata = item[1];

                NFT nft = new NFT
                {
                    id = tokenId,
                    name = metadata["name"].ToString(),
                    imageUrl = metadata["media"].ToString()
                };

                playerNFTs.Add(nft);
                yield return StartCoroutine(LoadImage(nft)); // Cargar imagen
            }

            DisplayNFTs();
        }
    }



    IEnumerator LoadImage(NFT nft)
    {
        if (string.IsNullOrEmpty(nft.imageUrl)) yield break;

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(nft.imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            nft.image = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
        else
        {
            Debug.LogWarning("No se pudo descargar imagen de NFT: " + nft.imageUrl);
        }
    }

    // Muestra todos los NFTs del jugador
    public void DisplayNFTs()
    {
        // Limpiar NFTs anteriores
        foreach (var item in nftItems)
            Destroy(item);
        nftItems.Clear();

        // Crear un botón por cada NFT
        foreach (var nft in playerNFTs)
        {
            GameObject obj = Instantiate(nftPrefab, contentParent);

            // Asignar imagen
            Image img = obj.transform.Find("NFTImage").GetComponent<Image>();
            if (img != null && nft.image != null)
                img.sprite = nft.image;

            // Asignar nombre al Text hijo (si existe)
            TextMeshProUGUI txt = obj.transform.Find("NFTName").GetComponent<TextMeshProUGUI>();
            if (txt != null)
                txt.text = nft.name;

            // Botón para seleccionar NFT
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => ToggleNFTSelection(nft, obj));

            // Actualizar color según selección
            UpdateNFTColor(obj, nft.selected);

            nftItems.Add(obj);

            Debug.Log("Instanciando NFT: " + nft.name);
        }
    }

    // Cambia la selección de un NFT
    void ToggleNFTSelection(NFT nft, GameObject obj)
    {
        nft.selected = !nft.selected;
        UpdateNFTColor(obj, nft.selected);
    }

    // Cambia el color del botón según si está seleccionado
    void UpdateNFTColor(GameObject obj, bool selected)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            ColorBlock colors = btn.colors;
            colors.normalColor = selected ? Color.green : Color.white;
            btn.colors = colors;
        }
    }

    // Obtener la lista de NFTs seleccionados
    public List<NFT> GetSelectedNFTs()
    {
        return playerNFTs.FindAll(n => n.selected);
    }
}

[System.Serializable]
public class NFTListWrapper
{
    public List<NFT> nfts;
}
