using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class MainScreenController : MonoBehaviour
{
    public TextMeshProUGUI welcomeText; // ������ �� ��������� ���� ��� ��������������� ���������
    public TextMeshProUGUI startingNumberText; // ������ �� ��������� ���� ��� ���������� �����
    public Image backgroundImage; // ������ �� ����������� ��� ����
    public Button incrementButton; // ������ ���������� ��������
    public Button refreshButton; // ������ ���������� ��������
    public TextMeshProUGUI counterText; // ����� ��� ����������� ��������

    private int counter;
    private string counterFilePath;

    private void Start()
    {
        // ���� ��� ���������� ��������
        counterFilePath = Application.persistentDataPath + "/counter.txt";

        // ��������� ���������� �������� �������� ��� �������� � ���������� ��������
        LoadCounter();

        // ���������� ����������� ������� ��� ������
        incrementButton.onClick.AddListener(IncrementCounter);
        refreshButton.onClick.AddListener(RefreshContent);

        // ���������, ��� ������ ���������
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

        // ��������� ������ ���� �� AssetBundle
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

        // ��������� ����������� ��������
        counterText.text = "Counter: " + counter.ToString();
    }

    // ����� ��� ������������� ��������
    private void IncrementCounter()
    {
        counter++;
        counterText.text = "Counter: " + counter.ToString();
        SaveCounter();
    }

    // ����� ��� ���������� ��������
    private void RefreshContent()
    {
        StartCoroutine(LoadNewContent());
    }

    // ����� ��� �������� ������ �������� (AssetBundle � JSON)
    private IEnumerator LoadNewContent()
    {
        // ��������, ���� ��� �������� ������ AssetBundle, �� �������� ���
        if (DataStore.loadedBundle != null)
        {
            DataStore.loadedBundle.Unload(true); // ��������� ������� AssetBundle
            DataStore.loadedBundle = null;
        }

        // ��������� ����� AssetBundle
        string assetBundlePath = Application.streamingAssetsPath + "/buttonbackground";
        UnityWebRequest bundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            AssetBundle newBundle = DownloadHandlerAssetBundle.GetContent(bundleRequest);
            if (newBundle != null)
            {
                // ��������� ����� ��� ��� ������
                Sprite newButtonBackground = newBundle.LoadAsset<Sprite>("assets/bundles/buttonbackground.png");
                if (newButtonBackground != null)
                {
                    backgroundImage.sprite = newButtonBackground;
                }
                else
                {
                    Debug.LogError("New button background not found.");
                }

                // ��������� ����� AssetBundle
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

        // ��������� ����� JSON � �����������
        string settingsPath = Application.streamingAssetsPath + "/Settings.json";
        UnityWebRequest settingsRequest = UnityWebRequest.Get(settingsPath);
        yield return settingsRequest.SendWebRequest();

        if (settingsRequest.result == UnityWebRequest.Result.Success)
        {
            Settings newSettings = JsonUtility.FromJson<Settings>(settingsRequest.downloadHandler.text);
            if (newSettings != null)
            {
                // �������� ���������
                startingNumberText.text = "Starting Number: " + newSettings.startingNumber;
                counter = newSettings.startingNumber;
            }
        }
        else
        {
            Debug.LogError("Failed to load newSettings.json");
        }
    }


    // ����� ��� �������� ����������� �������� ��������
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
            // ���� ��� ����������� ��������, �������� � ���������� �������� �� ��������
            counter = DataStore.settings.startingNumber;
        }
    }

    // ����� ��� ���������� �������� ��������
    private void SaveCounter()
    {
        File.WriteAllText(counterFilePath, counter.ToString());
    }
}

// ������ ������� Settings � Message
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
