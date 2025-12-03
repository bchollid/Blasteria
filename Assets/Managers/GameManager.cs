using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Serialized References")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private GameObject _wingame;
    [SerializeField] private GameObject _gameover;
    [SerializeField] private GameObject _pause;
    [SerializeField] private GameObject _boostMeter;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private Button _upgrade1Button;
    [SerializeField] private Button _upgrade2Button;
    [SerializeField] private Player _player;
    [SerializeField] private CutsceneManager _cutsceneManager;
    [SerializeField] private MenuManager _menuManager;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private GameObject _gameoverCircle;
    //[SerializeField] private GameObject _keyboardControls;
    //[SerializeField] private GameObject _gamepadControls;

    private int _scoreCount;
    private int _numberOfStarterGerms = 2;
    private int _numberOfBigGerms = 0;
    private int _numberOfBossGerms = 0;
    private int _positiveRandom1;
    private int _positiveRandom2;
    private int _positiveRandom3;
    private int _negativeRandom1;
    private int _negativeRandom2;
    private int _negativeRandom3;
    private int _previousPositiveRandom1;
    private int _previousPositiveRandom2;
    private int _previousPositiveRandom3;
    private int _previousNegativeRandom1;
    private int _previousNegativeRandom2;
    private int _previousNegativeRandom3;
    private int _currentWave = 1;

    private bool _isPaused = false;
    private bool _gameFinished = false;
    private bool _gameReady = false;

    private bool _hasRicochet = false;
    private bool _hasPenShot = false;
    private bool _hasLightning = false;
    private bool _hasFlamethrower = false;


    public UnityEvent gameOverEvent;
    public UnityEvent winEvent;
    public UnityEvent pauseEvent;
    public UnityEvent unpauseEvent;
    public static GameManager instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        _gameover.SetActive(false);
        _wingame.SetActive(false);
        _pause.SetActive(false);
        _gameoverCircle.SetActive(false);
        //if(Gamepad.current == null)
        //{
        //    _gamepadControls.SetActive(false);
        //}
        //else
        //{
        //    _keyboardControls.SetActive(false);
        //}
        StartCoroutine(LoadOrder());
    }

    private IEnumerator LoadOrder()
    {
        yield return new WaitForEndOfFrame();
        _scoreCount = ObjectPool.instance.numberOfStarterGerms;
        _scoreText.text = "Germs Remaining: " + _scoreCount.ToString();
        _gameReady = true;
    }

    void Update()
    {
        if (_scoreCount <= 0 && !_gameFinished && _gameReady)
        {
            _wingame.SetActive(true);
            _boostMeter.SetActive(false);
            if (CheckForGamepad())
            {
                _eventSystem.SetSelectedGameObject(_wingame.transform.Find("Option1").gameObject);
            }
            else
            {
                _eventSystem.SetSelectedGameObject(null);
            }
            _scoreText.text = "";
            _waveText.text = "";
            GeneratePositive(_upgrade1Button, 0);
            GeneratePositive(_upgrade2Button, 1);
            GenerateNegative(_upgrade1Button, 0);
            GenerateNegative(_upgrade2Button, 1);
            _player.canShoot = false;
            _gameFinished = true;
        }
    }

    private bool CheckForGamepad()
    {
        if (Gamepad.current == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddScore()
    {
        _scoreCount++;
        _scoreText.text = "Germs Remaining: " + _scoreCount.ToString();
    }

    public void RemoveScore()
    {
        _scoreCount--;
        _scoreText.text = "Germs Remaining: " + _scoreCount.ToString();
    }

    public void GameOver(Vector3 position)
    {
        gameOverEvent.Invoke();
        _gameover.SetActive(true);
        _gameoverCircle.SetActive(true);
        _gameoverCircle.transform.position = position;
        if (CheckForGamepad())
        {
            _eventSystem.SetSelectedGameObject(_gameover.transform.Find("Restart").gameObject);
        }
        else
        {
            _eventSystem.SetSelectedGameObject(null);
        }
    }

    public void PauseGame()
    {
        _isPaused = !_isPaused;
        _pause.SetActive(_isPaused);
        if (_isPaused)
        {
            pauseEvent.Invoke();
            if (CheckForGamepad())
            {
                _eventSystem.SetSelectedGameObject(_pause.transform.Find("Restart").gameObject);
            }
            else
            {
                _eventSystem.SetSelectedGameObject(null);
            }
        }
        else
        {
            unpauseEvent.Invoke();
        }
    }

    public void GeneratePositive(Button button, int which)
    {
        button.onClick.RemoveAllListeners();
        if (which == 0)
        {
            _positiveRandom1 = Random.Range(6, 7);
            GeneratePositiveOption(button, _positiveRandom1);
        }
        else
        {
            _positiveRandom2 = Random.Range(0, 8);
            while (_positiveRandom2 == _positiveRandom1)
            {
                _positiveRandom2 = Random.Range(0, 8);
            }
            GeneratePositiveOption(button, _positiveRandom2);
        }
        button.onClick.AddListener(LoadReset);
        button.onClick.AddListener(_menuManager.ClickSFX);
    }

    public void GeneratePositiveOption(Button button, int random)
    {
        switch (random)
        {
            case 0:
                button.onClick.AddListener(ImproveFireRate);
                button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Improve how fast you shoot.";
                break;
            case 1:
                button.onClick.AddListener(ImproveDamage);
                button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Improve the damage of your lasers.";
                break;
            case 2:
                button.onClick.AddListener(ImproveSpeed);
                button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Improve your boost speed and standard speed.";
                break;
            case 3:
                button.onClick.AddListener(AdditionalLaser);
                button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Fire an additional laser.";
                break;
            case 4:
                button.onClick.AddListener(AddRichochets);
                if (_hasRicochet)
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Lasers bounce off walls an additional time.";
                }
                else
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Lasers can now bounce off walls. They will not hit you.";
                }
                break;
            case 5:
                button.onClick.AddListener(AddLightning);
                if (_hasLightning)
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Lightning strikes an additional germ.";
                }
                else
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "When your laser hits, lightning will strike a nearby germ.";
                }
                break;
            case 6:
                button.onClick.AddListener(AddChargeShot);
                if (_hasPenShot)
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Your penetrating shot does more damage.";
                }
                else
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Wait between shooting to charge a penetrating shot.";
                }
                break;
            case 7:
                button.onClick.AddListener(AddFlamethrower);
                if (_hasFlamethrower)
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Increase the distance of your flamethrower.";
                }
                else
                {
                    button.transform.Find("Positive").GetComponent<TextMeshProUGUI>().text = "Firing now activates a flamethrower.";
                }
                break;

        }
    }

    public void GenerateNegativeOption(Button button, int random)
    {
        switch (random)
        {
            case 0:
                button.onClick.AddListener(AddAnotherStarterGerm);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Additional Red Germ";
                break;
            case 1:
                button.onClick.AddListener(AddAnotherBigGerm);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Additional Green Germ";
                break;
            case 2:
                button.onClick.AddListener(ImproveStarterGermRate);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Red and Green Germs Spawn Other Germs Faster";
                break;
            case 3:
                button.onClick.AddListener(ImproveStarterGermSpeed);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Red and Green Germs Get Faster";
                break;
            case 4:
                button.onClick.AddListener(ImproveGermsHealth);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Blue Germs Gain Health";
                break;
            case 5:
                button.onClick.AddListener(ImproveGermsSpeed);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Blue Germs Get Faster";
                break;
            case 6:
                button.onClick.AddListener(ImproveGermsRate);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Blue Germs Can Spawn Other Germs Faster";
                break;
            case 7:
                button.onClick.AddListener(ImproveStarterGermHealth);
                button.transform.Find("Negative").GetComponent<TextMeshProUGUI>().text = "Red and Green Germs Gain Health";
                break;
        }
    }

    public void GenerateNegative(Button button, int which)
    {
        if (which == 0)
        {
            if (_currentWave % 3 == 0)
            {
                _negativeRandom1 = 0;
                GenerateNegativeOption(button, _negativeRandom1);
            }
            else
            {
                _negativeRandom1 = Random.Range(0, 8);
                GenerateNegativeOption(button, _negativeRandom1);
            }
        }
        else
        {
            if (_currentWave % 3 == 0)
            {
                _negativeRandom1 = 1;
                GenerateNegativeOption(button, _negativeRandom1);
            }
            else
            {
                _negativeRandom2 = Random.Range(0, 8);
                while (_negativeRandom2 == _negativeRandom1)
                {
                    _negativeRandom2 = Random.Range(0, 8);
                }
                GenerateNegativeOption(button, _negativeRandom2);
            }
        }
    }

    public void ImproveFireRate()
    {
        _player.ImproveFireRate();
    }

    public void ImproveDamage()
    {
        _player.ImproveDamage();
    }

    public void ImproveSpeed()
    {
        _player.ImproveSpeed();
        _player.ImproveBoost();
    }

    public void AdditionalLaser()
    {
        _player.AddAdditionalLaser();
    }

    public void AddAnotherStarterGerm()
    {
        _numberOfStarterGerms++;
        ObjectPool.instance.numberOfStarterGerms = _numberOfStarterGerms;
    }

    public void ImproveStarterGermSpeed()
    {
        ObjectPool.instance.ImproveStarterGermSpeed();
    }

    public void ImproveStarterGermRate()
    {
        ObjectPool.instance.ImproveStarterGermRate();
    }

    public void ImproveStarterGermHealth()
    {
        ObjectPool.instance.ImproveStarterGermHealth();
    }

    public void ImproveGermsSpeed()
    {
        ObjectPool.instance.ImproveGermsSpeed();
    }

    public void ImproveGermsHealth()
    {
        ObjectPool.instance.ImproveGermsHealth();
    }

    public void ImproveGermsRate()
    {
        ObjectPool.instance.ImproveGermsRate();
    }

    public void AddAnotherBigGerm()
    {
        _numberOfBigGerms++;
        ObjectPool.instance.numberOfBigGerms = _numberOfBigGerms;
    }

    public void AddRichochets()
    {
        _player.AddRichochet();
        _hasRicochet = true;
    }

    public void AddLightning()
    {
        _player.AddLightning();
        _hasLightning = true;
    }

    public void AddChargeShot()
    {
        _player.AddChargeShot();
        _hasPenShot = true;
    }

    public void AddFlamethrower()
    {
        _player.AddFlamethrower();
        _hasFlamethrower = true;
    }

    public IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(0.25f);
        _cutsceneManager._currentWave++;
        _currentWave++;
        if (_currentWave % 10 == 0)
        {
            _numberOfBossGerms++;
            ObjectPool.instance.numberOfBossGerms = _numberOfBossGerms;
            _scoreCount = ObjectPool.instance.numberOfBossGerms;
            _scoreText.text = "Germs Remaining: " + _scoreCount.ToString();
        }
        else
        {
            _scoreCount = ObjectPool.instance.numberOfStarterGerms + ObjectPool.instance.numberOfBigGerms;
            _scoreText.text = "Germs Remaining: " + _scoreCount.ToString();
        }
        _gameReady = true;
        _gameFinished = false;
        _gameover.SetActive(false);
        _wingame.SetActive(false);
        _pause.SetActive(false);
        _boostMeter.SetActive(true);
        _waveText.text = "Current Wave: " + _currentWave.ToString();
        ObjectPool.instance.ResetGame(_currentWave);
        _player.canShoot = true;
        yield return new WaitForSeconds(0.75f);
    }

    public void LoadReset()
    {
        StartCoroutine(ResetGame());
    }

    public void Restart()
    {
        _menuManager.StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
