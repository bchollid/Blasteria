using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerGO;
    [SerializeField] private GameObject _objectPoolGO;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _gamemanager;
    [SerializeField] private GameObject _cutscene;
    [SerializeField] private MenuManager _menuManager;
    [SerializeField] private GameObject[] _scene1Stories;
    [SerializeField] private float _timeUntilStart = 1.0f;
    [SerializeField] private float _tempo = 2.0f;

    public int _currentWave = 0;
    private bool _canStart = false;
    private bool _setUpScene = true;
    private int _currentScene = 0;
    private int _currentStory;
    private float _timer = 0.0f;



    private void Awake()
    {
        _playerGO.SetActive(false);
        _objectPoolGO.SetActive(false);
        _gameUI.SetActive(false);
        _gamemanager.SetActive(false);
    }

    void Start()
    {
        foreach (var item in _scene1Stories)
        {
            item.SetActive(false);
        }
        _scene1Stories[_currentStory].SetActive(true);
    }

    void Update()
    {
        if(_canStart && _setUpScene)
        {
            _timer += Time.deltaTime;
            if (_timer >= _tempo)
            {
                if(_currentScene == 0)
                {
                    NextScene1Story();
                    _timer = 0.0f;
                }
                if(_currentScene == 1)
                {
                    NextScene1Story();
                    _timer = 0.0f;
                }
            }
        }
        else
        {
            _timer += Time.deltaTime;
            if (_timer >= _timeUntilStart)
            {
                _canStart = true;
                _timer = 0.0f;
            }
        }
        //if(_currentWave == 3 && _currentScene == 1)
        //{
        //    if(!_setUpScene)
        //    {
        //        SetUpCutscene();
        //        _setUpScene = true;
        //    }
        //}
    }

    public void NextScene1Story()
    {
        _scene1Stories[_currentStory].SetActive(false);
        _currentStory++;
        if(_currentStory >= _scene1Stories.Length)
        {
            PlayGame();
        }
        else
        {
            _scene1Stories[_currentStory].SetActive(true);
        }
    }

    public void SetUpCutscene()
    {
        _menuManager.ActivateSceneTransition();
        _playerGO.SetActive(false);
        _objectPoolGO.SetActive(false);
        _gameUI.SetActive(false);
        _gamemanager.SetActive(false);
        _cutscene.SetActive(true);
        _timer = 0.0f;
        _scene1Stories[_currentStory].SetActive(true);
        MusicManager.instance.SwitchToCutsceneMusic();
        ObjectPool.instance.ClearGame();
    }

    public void PlayGame()
    {
        _menuManager.ActivateSceneTransition();
        _playerGO.SetActive(true);
        _objectPoolGO.SetActive(true);
        _gameUI.SetActive(true);
        _gamemanager.SetActive(true);
        _cutscene.SetActive(false);
        MusicManager.instance.SwitchToCombatMusic();
        _setUpScene = false;
        _currentStory = 0;
        _currentScene++;
        _currentWave++;
    }
}
