using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("All Tips")]
    [SerializeField] private GameObject[] _allTips;

    [Header("Tutorial Values")]
    [SerializeField] private float _distanceSpeed = 10.0f;
    [SerializeField] private float _boostMultiplier = 10.0f;

    [Header("SERIALIZED REFERENCES")]
    [SerializeField] private TextMeshProUGUI _trackerText;
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _boost;
    [SerializeField] private GameObject _score;
    [SerializeField] private GameObject _defeatText;
    [SerializeField] private MenuManager _menuManager;

    private float _distance;
    private float _totalShots;
    private float _totalBoost;
    private int _currentTip;
    private int _germsAlive;
    private bool _spawnedGerms = false;

    public static TutorialManager instance;

    void Start()
    {
        instance = this;
        foreach (var t in _allTips)
        {
            t.SetActive(false);
        }
        _allTips[0].SetActive(true);
        _score.SetActive(false);
        _boost.SetActive(false);
        _defeatText.SetActive(false);
    }

    void Update()
    {
        TrackTutBenchmarks();
    }

    private void TrackTutBenchmarks()
    {
        if(_currentTip == 0)
        {
            TrackDistance();
        }
        else if (_currentTip == 1)
        {
            TrackShots();
        }
        else if (_currentTip == 2)
        {
            TrackBoost();
        }
        else if (_currentTip == 3)
        {
            TrackGerms();
        }
        else
        {
            _trackerText.text = "";
        }
    }

    private void TrackDistance()
    {
        _trackerText.text = "Distance: " + Mathf.FloorToInt(_distance) + " / 100";
        if(_player.isMoving)
        {
            _distance += Time.deltaTime * _distanceSpeed;
        }
        if(_distance >= 100.0f)
        {
            AdvanceTip();
            _distance = 0;
        }
    }

    private void TrackBoost()
    {
        _trackerText.text = "Boost used: " + Mathf.FloorToInt(_totalBoost) + " / 100";

        if (_player.isMoving && _player.isBoosting)
        {
            _totalBoost += Time.deltaTime * _boostMultiplier;
        }
        if(_totalBoost >= 100.0f)
        {
            //_score.SetActive(true);
            AdvanceTip();
            _totalBoost = 0;
        }
    }

    private void TrackShots()
    {
        _trackerText.text = "Shots: " + Mathf.FloorToInt(_totalShots) + " / 5";
        if(_totalShots >= 5)
        {
            _boost.SetActive(true);
            AdvanceTip();
            _totalShots = 0;
        }
    }

    private void TrackGerms()
    {
        if(!_spawnedGerms)
        {
            ObjectPool.instance.SpawnStarterGerm();
            _player.RefillBoost();
            _germsAlive = 2;
            _spawnedGerms = true;
        }
        if(_germsAlive <=0)
        {
            AdvanceTip();
            _germsAlive = 1;
        }

        _trackerText.text = "Germs Remaining: " + _germsAlive;
    }

    public void ShowDefeat()
    {
        StartCoroutine(DefeatTextEnabled());
        ObjectPool.instance.ResetTutorial();
    }

    private IEnumerator DefeatTextEnabled()
    {
        _defeatText.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        _defeatText.SetActive(false);
    }

    public void AddGerm()
    {
        _germsAlive++;
    }

    public void RemoveGerm()
    {
        _germsAlive--;
    }

    public void ResetGerms()
    {
        _germsAlive = 2;
    }

    public void Next()
    {
        AdvanceTip();
    }

    public void FiredShot()
    {
        if (_currentTip == 1)
        {
            _totalShots++;
        }
    }

    private void AdvanceTip()
    {
        _allTips[_currentTip].SetActive(false);
        _currentTip++;
        if(_currentTip == 4)
        {
            _player.canShoot = false;
        }
        if(_currentTip >= _allTips.Length)
        {
            _menuManager.BackToMenu();
        }
        _allTips[_currentTip].SetActive(true);
    }
}
