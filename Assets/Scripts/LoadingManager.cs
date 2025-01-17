using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar; // Ссылка на прогресс-бар
    private string settingsURL = "https://example.com/Settings.json"; // URL для загрузки JSON
    private string messageURL = "https://example.com/Message.json";  // URL для приветственного сообщения
    private string assetBundleURL = "https://example.com/mybundle"; // URL для Asset Bundle

    private AssetBundle loadedBundle;

    [System.Serializable]
    public class Settings
    {
        public int startingNumber;
    }

    [System.Serializable]
    public class Message
    {
        public string welcomeMessage;
    }


    private void Start()
    {
        StartCoroutine(LoadContent());
    }

    private Settings settings;
    private Message message;

    private IEnumerator LoadContent()
    {
        // Искусственная задержка
        yield return new WaitForSeconds(1.0f);

        // Шаг 1: Загрузка и парсинг Settings.json
        string settingsPath = Application.streamingAssetsPath + "/Settings.json";
        UnityWebRequest settingsRequest = UnityWebRequest.Get(settingsPath);
        yield return settingsRequest.SendWebRequest();

        if (settingsRequest.result == UnityWebRequest.Result.Success)
        {
            settings = JsonUtility.FromJson<Settings>(settingsRequest.downloadHandler.text);
            Debug.Log("Settings Loaded: Starting Number = " + settings.startingNumber);
        }
        else
        {
            Debug.LogError("Failed to load Settings.json");
        }
        progressBar.value = 0.33f;

        // Шаг 2: Загрузка и парсинг Message.json
        string messagePath = Application.streamingAssetsPath + "/Message.json";
        UnityWebRequest messageRequest = UnityWebRequest.Get(messagePath);
        yield return messageRequest.SendWebRequest();

        if (messageRequest.result == UnityWebRequest.Result.Success)
        {
            message = JsonUtility.FromJson<Message>(messageRequest.downloadHandler.text);
            Debug.Log("Message Loaded: " + message.welcomeMessage);
        }
        else
        {
            Debug.LogError("Failed to load Message.json");
        }
        progressBar.value = 0.66f;

        // Шаг 3: Загрузка Asset Bundle
        string assetBundlePath = Application.streamingAssetsPath + "/buttonBackground";
        UnityWebRequest bundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            loadedBundle = DownloadHandlerAssetBundle.GetContent(bundleRequest);

            if (loadedBundle != null)
            {
                foreach (string assetName in loadedBundle.GetAllAssetNames())
                {
                    Debug.Log("Asset in bundle: " + assetName);
                }

                string spriteName = "assets/bundles/buttonbackground.png"; // Укажите имя спрайта
                Sprite buttonBackground = loadedBundle.LoadAsset<Sprite>(spriteName);

                if (buttonBackground != null)
                {
                    Debug.Log("Successfully loaded sprite: " + spriteName);
                }
                else
                {
                    Debug.LogError("Sprite not found in Asset Bundle");
                }
            }
            else
            {
                Debug.LogError("Loaded bundle is null");
            }
        }
        else
        {
            Debug.LogError("Failed to load Asset Bundle: " + bundleRequest.error);
        }



        // Искусственная задержка перед переходом на основной экран
        yield return new WaitForSeconds(1.0f);

        // Переход на основной экран
        SceneManager.LoadScene("MainScreen");
    }

}
