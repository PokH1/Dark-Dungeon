using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace AbstractionServer
{
    [System.Serializable]
    public class NFT
    {
        public string id;
        public string name;
        public Sprite image;
        public bool selected;
    }

    public class NFTService : MonoBehaviour
    {

        public IEnumerator NFTsFromUser(Action<NFT[]> onSuccess, Action<string> onError)
        {

            string userId = AbstractionApiClient.userId;

            JObject bodyJson = new JObject();
            bodyJson["callArguments"] = new JArray(userId);
            string bodyString = bodyJson.ToString();

            string url = "http://localhost:3001/service/query/nftsfromuser";

            UnityWebRequest request = new UnityWebRequest(url, "GET");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
            }
            else
            {
                try
                {
                    JObject json = JObject.Parse(request.downloadHandler.text);

                    if (json["response"]?["error"]?["userDoesNotExists"] != null ||
                        json["response"]?["nfts"] == null)
                    {
                        onSuccess(new NFT[0]);
                        yield break;
                    }

                    JArray nftsArray = json["response"]["nfts"] as JArray;
                    NFT[] nfts = nftsArray.ToObject<NFT[]>();
                    onSuccess(nfts);
                }
                catch (JsonException ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                    onError?.Invoke("Error parsing JSON: " + ex.Message);
                }
            }
        }


        private IEnumerator LoadSpriteFromURL(string url, Action<Sprite> onLoaded)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    onLoaded?.Invoke(sprite);
                }
                else
                {
                    Debug.LogError("Error loading image: " + request.error);
                    onLoaded?.Invoke(null);
                }
            }
        }
        
        public IEnumerator NFTsSelectedByUser(Action<NFT[]> onSuccess, Action<string> onError)
        {
            // Obtenemos el userId desde AbstractionApiClient
            string userId = AbstractionApiClient.userId;

            // Creamos el JSON con callArguments
            JObject bodyJson = new JObject();
            bodyJson["callArguments"] = new JArray(userId);
            string bodyString = bodyJson.ToString();

            string url = "http://localhost:3001/service/query/nftsselectedbyuser";

            UnityWebRequest request = new UnityWebRequest(url, "GET");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
            }
            else
            {
                try
                {
                    JObject json = JObject.Parse(request.downloadHandler.text);

                    if (json["response"]?["error"]?["userDoesNotExists"] != null ||
                        json["response"]?["nfts"] == null)
                    {
                        onSuccess(new NFT[0]);
                        yield break;
                    }

                    JArray nftsArray = json["response"]["nfts"] as JArray;
                    NFT[] nfts = nftsArray.ToObject<NFT[]>();
                    onSuccess(nfts);
                }
                catch (JsonException ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                    onError?.Invoke("Error parsing JSON: " + ex.Message);
                }
            }
        }
    }
}

