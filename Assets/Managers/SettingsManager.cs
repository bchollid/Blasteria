using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject _resolutionDropdown;
    [SerializeField] private AudioMixer _am;
    [SerializeField] private MenuManager _menuManager;

    void Start()
    {


    }

    void Update()
    {

    }

    public void ChangeResolution()
    {
        string value = _resolutionDropdown.transform.Find("Label").GetComponent<TextMeshProUGUI>().text;
        bool isItFullscreen = Screen.fullScreen;
        switch (value)
        {
            case "1280x720":
                Screen.SetResolution(1280, 720, isItFullscreen);
                break;
            case "1920x1080":
                Screen.SetResolution(1920, 1080, isItFullscreen);
                break;
            case "2560x1440":
                Screen.SetResolution(2560, 1440, isItFullscreen);
                break;
        }
        _menuManager.ClickSFX();
    }

    public void ChangeFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        _menuManager.ClickSFX();
    }

    public void ChangeMasterVolume(Slider sliderValue)
    {
        float value = sliderValue.value;
        _am.SetFloat("Master", value);
    }

    public void ChangeSFXVolume(Slider sliderValue)
    {
        float value = sliderValue.value;
        _am.SetFloat("SFX", value);
    }

    public void ChangeMusicVolume(Slider sliderValue)
    {
        float value = sliderValue.value;
        _am.SetFloat("Music", value);
    }

    public void ChangeUIVolume(Slider sliderValue)
    {
        float value = sliderValue.value;
        _am.SetFloat("UI", value);
    }

}
