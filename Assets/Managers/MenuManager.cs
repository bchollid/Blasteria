using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _alphaIncrease = 1.0f;
    [SerializeField] private float _densityIncrease = 1.0f;
    [SerializeField] private float _speedIncrease = 1.0f;
    [SerializeField] private AudioClip _hoverSFX;
    [SerializeField] private AudioClip _clickSFX;
    [SerializeField] private AudioSource _audio;
    private float _value = 0.0f;
    private float _loadValue = 1.0f;

    private void Start()
    {
        StartCoroutine(SceneLoadTransition());
    }

    void Update()
    {

    }



    public void StartGame()
    {
        _audio.PlayOneShot(_clickSFX);
        StartCoroutine(SceneTransition(1));
        MusicManager.instance.SwitchToCutsceneMusic();
    }

    public void StartEndless()
    {
        _audio.PlayOneShot(_clickSFX);
        StartCoroutine(SceneTransition(4));
        MusicManager.instance.SwitchToCombatMusic();
    }

    public void ActivateSceneTransition()
    {
        StartCoroutine(FullTransition());
    }

    private IEnumerator FullTransition()
    {
        _material.SetFloat("_Alpha", _alphaIncrease);
        _material.SetFloat("_Density", _densityIncrease);
        _material.SetFloat("_Speed", _speedIncrease);
        _loadValue = 1.0f;
        while (_loadValue > 0.0f)
        {
            _loadValue -= Time.deltaTime;
            _material.SetFloat("_Alpha", _loadValue * _alphaIncrease);
            _material.SetFloat("_Density", _loadValue * _densityIncrease);
            _material.SetFloat("_Speed", _loadValue * _speedIncrease);
            yield return null;
        }
    }

    private IEnumerator SceneTransition(int scene)
    {
        while (_value < 1.0f)
        {
            _value += Time.deltaTime;
            _material.SetFloat("_Alpha", _value * _alphaIncrease);
            _material.SetFloat("_Density", _value * _densityIncrease);
            _material.SetFloat("_Speed", _value * _speedIncrease);
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    private IEnumerator SceneLoadTransition()
    {
        yield return new WaitForEndOfFrame();
        while (_loadValue > 0.0f)
        {
            _loadValue -= Time.deltaTime;
            _material.SetFloat("_Alpha", _loadValue * _alphaIncrease);
            _material.SetFloat("_Density", _loadValue * _densityIncrease);
            _material.SetFloat("_Speed", _loadValue * _speedIncrease);
            yield return null;
        }
    }

    public void StartTutorial()
    {
        _audio.PlayOneShot(_clickSFX);
        StartCoroutine(SceneTransition(2));
        MusicManager.instance.SwitchToMenuMusic();
    }

    public void GoToSettings()
    {
        _audio.PlayOneShot(_clickSFX);
        StartCoroutine(SceneTransition(3));
        MusicManager.instance.SwitchToMenuMusic();
    }

    public void BackToMenu()
    {
        _audio.PlayOneShot(_clickSFX);
        StartCoroutine(SceneTransition(0));
        MusicManager.instance.SwitchToMenuMusic();
    }

    public void HoverSFX()
    {
        _audio.PlayOneShot(_hoverSFX);
    }

    public void ClickSFX()
    {
        _audio.PlayOneShot(_clickSFX);
    }

    public void ToEndOfDemo()
    {
        StartCoroutine(SceneTransition(5));
        MusicManager.instance.SwitchToMenuMusic();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
