using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class MainScreenController : MonoBehaviour
{
    public TextMeshProUGUI welcomeText; // Ссылка на текстовое поле для приветственного сообщения
    public TextMeshProUGUI startingNumberText; // Ссылка на текстовое поле для стартового числа
    public Image backgroundImage; // Ссылка на изображение для фона
    public Button incrementButton; // Кнопка увеличения счетчика
    public Button refreshButton; // Кнопка обновления контента
    public TextMeshProUGUI counterText; // Текст для отображения счётчика

    private int counter;
    private string counterFilePath;

    private void Start()
    {
        // Путь для сохранения счётчика
        counterFilePath = Application.persistentDataPath + "/counter.txt";

        // Загружаем сохранённое значение счётчика или начинаем с начального значения
        LoadCounter();

        // Подключаем обработчики событий для кнопок
        incrementButton.onClick.AddListener(IncrementCounter);
        refreshButton.onClick.AddListener(RefreshContent);

        // Проверяем, что данные загружены
        if (DataStore.message != null)
        {
            welcomeText.text = DataStore.message.welcomeMessage;
        }
        else
        {
            welcomeText.text = "Welcome to the Game!";
        }

        if (DataStore.settings != null)
        {
            startingNumberText.text = "Starting Number: " + DataStore.settings.startingNumber;
        }

        // Загружаем спрайт фона из AssetBundle
        if (DataStore.loadedBundle != null)
        {
            Sprite buttonBackground = DataStore.loadedBundle.LoadAsset<Sprite>("buttonbackground.png");
            if (buttonBackground != null)
            {
                backgroundImage.sprite = buttonBackground;
            }
            else
            {
                Debug.LogError("Button background not found.");
            }
        }

        // Обновляем отображение счётчика
        counterText.text = "Counter: " + counter.ToString();
    }

    // Метод для инкрементации счётчика
    private void IncrementCounter()
    {
        counter++;
        counterText.text = "Counter: " + counter.ToString();
        SaveCounter();
    }

    // Метод для обновления контента
    private void RefreshContent()
    {
        StartCoroutine(LoadNewContent());
    }

    // Метод для загрузки нового контента (AssetBundle и JSON)
    private IEnumerator LoadNewContent()
    {
        // Проверим, если уже загружен старый AssetBundle, то выгрузим его
        if (DataStore.loadedBundle != null)
        {
            DataStore.loadedBundle.Unload(true); // выгружаем текущий AssetBundle
            DataStore.loadedBundle = null;
        }

        // Загрузить новый AssetBundle
        string assetBundlePath = Application.streamingAssetsPath + "/buttonbackground";
        UnityWebRequest bundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            AssetBundle newBundle = DownloadHandlerAssetBundle.GetContent(bundleRequest);
            if (newBundle != null)
            {
                // Загружаем новый фон для кнопки
                Sprite newButtonBackground = newBundle.LoadAsset<Sprite>("assets/bundles/buttonbackground.png");
                if (newButtonBackground != null)
                {
                    backgroundImage.sprite = newButtonBackground;
                }
                else
                {
                    Debug.LogError("New button background not found.");
                }

                // Сохраняем новый AssetBundle
                DataStore.loadedBundle = newBundle;
            }
            else
            {
                Debug.LogError("Failed to load new AssetBundle.");
            }
        }
        else
        {
            Debug.LogError("Failed to load AssetBundle: " + bundleRequest.error);
        }

        // Загрузить новый JSON с настройками
        string settingsPath = Application.streamingAssetsPath + "/Settings.json";
        UnityWebRequest settingsRequest = UnityWebRequest.Get(settingsPath);
        yield return settingsRequest.SendWebRequest();

        if (settingsRequest.result == UnityWebRequest.Result.Success)
        {
            Settings newSettings = JsonUtility.FromJson<Settings>(settingsRequest.downloadHandler.text);
            if (newSettings != null)
            {
                // Обновить настройки
                startingNumberText.text = "Starting Number: " + newSettings.startingNumber;
                counter = newSettings.startingNumber;
            }
        }
        else
        {
            Debug.LogError("Failed to load newSettings.json");
        }
    }


    // Метод для загрузки сохранённого значения счётчика
    private void LoadCounter()
    {
        if (File.Exists(counterFilePath))
        {
            string savedCounter = File.ReadAllText(counterFilePath);
            if (int.TryParse(savedCounter, out int savedValue))
            {
                counter = savedValue;
            }
        }
        else
        {
            // Если нет сохранённого значения, начинаем с начального значения из настроек
            counter = DataStore.settings.startingNumber;
        }
    }

    // Метод для сохранения значения счётчика
    private void SaveCounter()
    {
        File.WriteAllText(counterFilePath, counter.ToString());
    }
}

// Пример классов Settings и Message
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
