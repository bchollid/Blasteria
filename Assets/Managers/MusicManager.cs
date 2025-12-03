using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] _allSongs;
    [SerializeField] private AudioSource _audio;
    public static MusicManager instance;


    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
            DontDestroyOnLoad(gameObject);
    }


    public void SwitchToCutsceneMusic()
    {
        if(_audio.clip != _allSongs[1])
        {
            StartCoroutine(MusicCrossFade(1));
        }
    }

    private IEnumerator MusicCrossFade(int song)
    {
        float fadeTime = 0; 
        while (fadeTime < 1.0f)
        {
            fadeTime += Time.deltaTime;
            _audio.volume -= Time.deltaTime;
            yield return null;
        }
        _audio.clip = _allSongs[song];
        _audio.Play();
        float fadeInTime = 0;
        while (fadeInTime < 1.0f)
        {
            fadeInTime += Time.deltaTime;
            _audio.volume += Time.deltaTime;
            yield return null;
        }
    }

    public void SwitchToCombatMusic()
    {
        if (_audio.clip != _allSongs[2])
        {
            StartCoroutine(MusicCrossFade(2));
        }
    }

    public void SwitchToMenuMusic()
    {
        if (_audio.clip != _allSongs[0])
        {
            StartCoroutine(MusicCrossFade(0));
        }
    }

}
