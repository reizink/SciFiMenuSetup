using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject NoSavedGamePopUp;
    [SerializeField] private GameObject LoadQPopup;

    [SerializeField] private GameObject ConfirmChange;

    [Header("Sound Tab")]
    [SerializeField] private Slider volSlider;

    [Header("Gameplay Tab")]
    [SerializeField] private Toggle fontToggle;
    private bool isFont;
    [SerializeField] private TMP_FontAsset DefaultFont;
    [SerializeField] private TMP_FontAsset DysFont;

    [Header("Graphics Tab")]
    [SerializeField] private Slider BrightSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private int QualityLevel;
    private bool isFullscreen;
    private float brightLevel;

    [Header("Resolution")]
    public TMP_Dropdown resDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        PlayerPrefs.SetString("SavedLevel", ""); // Level2"); //for testing

        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> options = new List<string>();
        int curIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                curIndex = i;
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = curIndex;
        resDropdown.RefreshShownValue();


        //quality
        qualityDropdown.ClearOptions(); // Clear any existing options

        string[] qualityLevels = QualitySettings.names;
        List<string> qOptions = new List<string>(qualityLevels);

        qualityDropdown.AddOptions(qOptions);

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    public void SetResolution(TMP_Dropdown dropdown)
    {
        Resolution resolution = resolutions[dropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen); //Screen.Fullscreen
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGameQPanel()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            LoadQPopup.SetActive(true);
        }
        else
        {
            NoSavedGamePopUp.SetActive(true);
        }
    }

    public void LoadGameSave()
    {
        string pullLevel = PlayerPrefs.GetString("SavedLevel");
        SceneManager.LoadScene(pullLevel);

    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void SetVolume(Slider volume)
    {
        AudioListener.volume = volume.value;
    }

    public void SetFont(Toggle selected)
    {
        TMP_FontAsset tmp = DefaultFont;

        if(selected.isOn) //if true, dyslexic friendly font
            tmp = DysFont;

        TextMeshProUGUI[] allTMPTextComponents = FindObjectsOfType<TextMeshProUGUI>();

        foreach (TextMeshProUGUI tmpText in allTMPTextComponents)
        {
            tmpText.font = tmp;
        }

        isFont = selected.isOn;
    }

    //apply button function or set this each time you set an individual element
    public void ApplySoundValues() 
    {
        PlayerPrefs.SetFloat("volume", AudioListener.volume);
        PlayerPrefs.Save();
        StartCoroutine(ConfirmApply());
    }

    public void ApplyGameplayValues()
    {
        PlayerPrefs.SetInt("font", (isFont ? 1:0));
        PlayerPrefs.Save();
        StartCoroutine(ConfirmApply());
    }

    //coroutine
    public IEnumerator ConfirmApply()
    {
        ConfirmChange.SetActive(true);
        yield return new WaitForSeconds(2f);
        ConfirmChange.SetActive(false);
    }

    public void SetBrightness(Slider brightness)
    {
        brightLevel = brightness.value;
    }

    public void SetFullscreen(Toggle fullToggle)
    {
        isFullscreen = fullToggle.isOn;
    }

    public void SetQuality(TMP_Dropdown index)
    {
        QualityLevel = index.value;
    }

    public void GraphicsApplyButton()
    {
        PlayerPrefs.SetFloat("brightness", brightLevel);
        //post-processing global volume needed for this
        /*if (postProcessingVolume.profile.TryGet(out exposure) != null)
        {
            exposure.fixedExposure.value = brightLevel;
        }*/

        PlayerPrefs.SetInt("quality", QualityLevel);
        QualitySettings.SetQualityLevel(QualityLevel);

        //tonary expression example
        PlayerPrefs.SetInt("fullscreen", (isFullscreen ? 1 : 0));
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.Save();
        StartCoroutine(ConfirmApply());
    }
}
