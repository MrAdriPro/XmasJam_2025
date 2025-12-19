using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class SettingsController : MonoBehaviour {
    public Toggle fullscreenToggle;
    public Dropdown resolutionDrop;
    public Dropdown textQualityDrop;
    public Dropdown vSyncDrop;
    public Slider volume;
    public Button saveButton;
    public Resolution[] resolutions;
    public Settings gameSettings;


    void OnEnable()
    {
        gameSettings = new Settings();
        fullscreenToggle.onValueChanged.AddListener(delegate { FullscreenToggle(); });
        resolutionDrop.onValueChanged.AddListener(delegate { ResolutionChange(); });
        saveButton.onClick.AddListener(delegate { saveSettings(); });

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDrop.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        loadSettings();
    }

    public void FullscreenToggle()
    {
       gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void ResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDrop.value].width, resolutions[resolutionDrop.value].height, Screen.fullScreen, resolutions[resolutionDrop.value].refreshRate);
        gameSettings.resolutionIndex = resolutionDrop.value;
    }

    public void AntialiasingChange()
    {
    }

    public void VsyncChange()
    {
        QualitySettings.vSyncCount = gameSettings.vSync = vSyncDrop.value;
    }

    public void TextQChange()
    {
        gameSettings.textureQuality = QualitySettings.globalTextureMipmapLimit = textQualityDrop.value;
    }

    public void VolumeChange()
    {
        gameSettings.volume = AudioListener.volume = volume.value;
    }

    public void saveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings,true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
        MenuController.instance.closeOptions();
    }

    public void loadSettings()
    {
        string path = Application.persistentDataPath + "/gamesettings.json";

        if (File.Exists(path))
        {
            gameSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(path));

            fullscreenToggle.isOn = gameSettings.fullscreen;
            resolutionDrop.value = gameSettings.resolutionIndex;



            resolutionDrop.RefreshShownValue();

            Debug.Log("Configuración cargada correctamente.");
        }
        else
        {
            Debug.Log("No se encontró archivo de guardado. Se usarán valores por defecto.");
        }
    }
}
