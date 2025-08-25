using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft;

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
    
    public static class AbstractionApiClient
    {
        public static string BaseUrl = "http://localhost:3001";

        public static string Token = null;

        public static string userId;

        public static IEnumerator Get<T>(string path, Action<T> onSuccess, Action<string> onError)
        {
            string url = $"{BaseUrl}{path}";
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(Token))
                request.SetRequestHeader("Authorization", $"Bearer {Token}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[AbstractionApiClient] Conexión exitosa con {url} (Status: {request.responseCode})");

                if (typeof(T) == typeof(string))
                {
                    onSuccess?.Invoke((T)(object)request.downloadHandler.text);
                }
                else
                {
                    T result = JsonUtility.FromJson<T>(request.downloadHandler.text);
                    onSuccess?.Invoke(result);
                }
            }
            else
            {
                onError?.Invoke(request.downloadHandler.text);
            }
        }

        public static IEnumerator Post<TReq, TRes>(string path, TReq body, Action<TRes> onSuccess, Action<string> onError)
        {
            string url = $"{BaseUrl}{path}";
            string json = JsonUtility.ToJson(body);

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(Token))
                request.SetRequestHeader("Authorization", $"Bearer {Token}");

            yield return request.SendWebRequest();

            string rawJson = request.downloadHandler.text;
            Debug.Log("Response: " + rawJson);

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[AbstractionApiClient] Conexión exitosa con {url} (Status: {request.responseCode})");

                if (typeof(TRes) == typeof(string))
                {
                    onSuccess?.Invoke((TRes)(object)rawJson);
                }
                else
                {
                    try
                    {
                        TRes result = Newtonsoft.Json.JsonConvert.DeserializeObject<TRes>(rawJson);
                        onSuccess?.Invoke(result);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Json parse error: " + ex.Message);
                        Debug.LogError("Raw JSON: " + rawJson);
                        onError?.Invoke("Failed to parse response.");
                    }
                }
            }
            else
            {
                onError?.Invoke(rawJson);
            }
        }

        public static IEnumerator Post(string path, Action<string> onSuccess, Action<string> onError)
        {
            return Post<object, string>(path, new object(), onSuccess, onError);
        }

        public static IEnumerator PostArray(string path, object[] funcArguments, Action<string> onSuccess, Action<string> onError)
        {
            if (funcArguments == null)
                funcArguments = new object[] { };


            var payload = new { callArguments = funcArguments };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

            UnityWebRequest request = new UnityWebRequest($"{BaseUrl}{path}", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(Token))
                request.SetRequestHeader("Authorization", $"Bearer {Token}");

            yield return request.SendWebRequest();

            string rawJson = request.downloadHandler.text;
            Debug.Log("Response: " + rawJson);

            if (request.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(rawJson);
            else
                onError?.Invoke(rawJson);
        }

        // Clase de ayuda para serializar arrays directamente
        [System.Serializable]
        public class SerializationWrapper
        {
            public object[] items;

            public SerializationWrapper(object[] arr)
            {
                items = arr;
            }
        }


    }
    
}
