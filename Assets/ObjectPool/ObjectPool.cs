using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private int _amountOfGermsToPool = 250;
    [SerializeField] private int _amountOfLasersToPool = 100;
    [SerializeField] private int _amountOfHitsToPool = 100;
    [SerializeField] private int _amountOfLightningsToPool = 25;
    [SerializeField] private int _amountOfHealsToPool = 200;
    [SerializeField] private float _germSpeedLow;
    [SerializeField] private float _germSpeedHigh;
    [SerializeField] private float _germDuplicationLow;
    [SerializeField] private float _germDuplicationHigh;
    [SerializeField] private float _germHealth;
    [SerializeField] private float _starterGermHealth;
    [SerializeField] private float _starterSpeed;
    [SerializeField] private float _starterDuplication;
    [SerializeField] private float _bigGermSpeed;
    [SerializeField] private float _bigGermHealth;
    [SerializeField] private float _bigGermDuplication;
    [SerializeField] private float _bigGermHealSpeed;
    [SerializeField] private float _bigGermHealPower;
    [SerializeField] private float _tutorialStarterGerms;
    [SerializeField] private float _bossGermHealth;
    [SerializeField] private float _bossGermDuplication;
    public int numberOfStarterGerms = 1;
    public int numberOfBigGerms = 0;
    public int numberOfBossGerms = 0;
    [SerializeField] private Vector3 _startGermSpawnVariation;
    private List<GameObject> _pooledGerms = new List<GameObject>();
    private List<GameObject> _pooledLasers = new List<GameObject>();
    private List<GameObject> _pooledHits = new List<GameObject>();
    private List<GameObject> _starterGerms = new List<GameObject>();
    private List<GameObject> _bigGerms = new List<GameObject>();
    private List<GameObject> _bossGerms = new List<GameObject>();
    private List<GameObject> _lightnings = new List<GameObject>();
    private List<GameObject> _heals = new List<GameObject>();


    [HideInInspector]
    public static ObjectPool instance;

    [SerializeField] private GameObject germ1;
    [SerializeField] private GameObject starterGerm;
    [SerializeField] private GameObject bigGerm;
    [SerializeField] private GameObject laser1;
    [SerializeField] private GameObject hitFX;
    [SerializeField] private Player _player;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _hitSFX;
    [SerializeField] private GameObject _lightning;
    [SerializeField] private GameObject _healing;
    [SerializeField] private GameObject[] bossGerms;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        for (int i = 0; i < _amountOfGermsToPool; i++)
        {
            GameObject obj = Instantiate(germ1, transform.position, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            Germ1 germ = obj.GetComponent<Germ1>();
            int random = Random.Range(0, 10);
            if (random > 6)
            {
                germ.currentSpeed = _germSpeedHigh;
            }
            else
            {
                germ.currentSpeed = _germSpeedLow;
            }
            int newRandom = Random.Range(0, 10);
            if (newRandom > 7)
            {
                germ.duplicationTime = _germDuplicationLow;
            }
            else
            {
                germ.duplicationTime = _germDuplicationHigh;
            }
            germ.health = _germHealth;
            obj.SetActive(false);
            _pooledGerms.Add(obj);
        }

        for (int i = 0; i < _amountOfLasersToPool; i++)
        {
            GameObject obj = Instantiate(laser1, transform.position, Quaternion.identity);
            obj.SetActive(false);
            _pooledLasers.Add(obj);
        }

        for (int i = 0; i < _amountOfHitsToPool; i++)
        {
            GameObject obj = Instantiate(hitFX, transform.position, Quaternion.identity);
            obj.SetActive(false);
            _pooledHits.Add(obj);
        }
        for (int i = 0; i < numberOfStarterGerms; i++)
        {
            Vector3 randomSpawn = new Vector3(Random.Range(-_startGermSpawnVariation.x, _startGermSpawnVariation.x), 1.0f, Random.Range(-_startGermSpawnVariation.z, _startGermSpawnVariation.z));
            GameObject obj = Instantiate(starterGerm, randomSpawn, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            _starterGerms.Add(obj);
            Germ1 germ = obj.GetComponent<Germ1>();
            germ.currentSpeed = _starterSpeed;
            germ.duplicationTime = _starterDuplication;
            germ.health = _starterGermHealth;
            germ.AdjustHealthSlider();
        }
        for (int i = 0; i < numberOfBigGerms; i++)
        {
            Vector3 randomSpawn = new Vector3(Random.Range(-_startGermSpawnVariation.x, _startGermSpawnVariation.x), 1.0f, Random.Range(-_startGermSpawnVariation.z, _startGermSpawnVariation.z));
            GameObject obj = Instantiate(bigGerm, randomSpawn, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            _bigGerms.Add(obj);
            Germ1 germ = obj.GetComponent<Germ1>();
            germ.currentSpeed = _bigGermSpeed;
            germ.duplicationTime = _bigGermDuplication;
            germ.health = _bigGermHealth;
            germ.powerOfHeal = _bigGermHealPower;
            germ.frequencyOfHeal = _bigGermHealSpeed;
            germ.AdjustHealthSlider();
        }
        for (int i = 0; i < _amountOfLightningsToPool; i++)
        {
            GameObject obj = Instantiate(_lightning, transform.position, Quaternion.identity);
            obj.SetActive(false);
            _lightnings.Add(obj);
        }
        for (int i = 0; i < _amountOfHealsToPool; i++)
        {
            GameObject obj = Instantiate(_healing, transform.position, Quaternion.identity);
            obj.SetActive(false);
            _heals.Add(obj);
        }


    }

    public GameObject GetLaser()
    {
        for (int i = 0; i < _pooledLasers.Count; i++)
        {
            if (!_pooledLasers[i].activeInHierarchy)
            {
                return _pooledLasers[i];
            }
        }

        return null;
    }

    public GameObject GetLightning()
    {
        for (int i = 0; i < _lightnings.Count; i++)
        {
            if (!_lightnings[i].activeInHierarchy)
            {
                return _lightnings[i];
            }
        }

        return null;
    }

    public GameObject GetHitFX()
    {
        for (int i = 0; i < _pooledHits.Count; i++)
        {
            if (!_pooledHits[i].activeInHierarchy)
            {
                return _pooledHits[i];
            }
        }

        return null;
    }

    public GameObject GetGerm()
    {
        for (int i = 0; i < _pooledGerms.Count; i++)
        {
            if (!_pooledGerms[i].activeInHierarchy)
            {
                return _pooledGerms[i];
            }
        }

        return null;
    }

    public GameObject GetHeal()
    {
        for (int i = 0; i < _heals.Count; i++)
        {
            if (!_heals[i].activeInHierarchy)
            {
                return _heals[i];
            }
        }

        return null;
    }

    public void ImproveStarterGermSpeed()
    {
        _starterSpeed *= 1.4f;
        _bigGermSpeed *= 1.2f;
    }

    public void ImproveStarterGermRate()
    {
        _starterDuplication *= 0.7f;
        _bigGermDuplication *= 0.9f;
    }

    public void ImproveStarterGermHealth()
    {
        _starterGermHealth *= 1.4f;
        _bigGermHealth *= 1.5f;
    }

    public void ImproveGermsSpeed()
    {
        _germSpeedHigh *= 1.2f;
        _germSpeedLow *= 1.2f;
    }

    public void ImproveGermsRate()
    {
        _germDuplicationHigh *= 0.9f;
        _germDuplicationLow *= 0.9f;
    }

    public void ImproveGermsHealth()
    {
        _germHealth *= 1.4f;
    }

    public void LevelUpIncrease()
    {
        _germSpeedHigh *= 1.02f;
        _germSpeedLow *= 1.02f;
        _germDuplicationHigh *= 0.98f;
        _germDuplicationLow *= 0.98f;
        _germHealth *= 1.1f;
        _starterSpeed *= 1.05f;
        _starterDuplication *= 0.96f;
        _starterGermHealth *= 1.1f;
        _bigGermDuplication *= 0.98f;
        _bigGermHealth *= 1.1f;
        _bigGermSpeed *= 1.1f;
        _bigGermHealSpeed *= 0.95f;
        _bigGermHealPower *= 1.1f;
    }

    public void SpawnStarterGerm()
    {
        for (int i = 0; i < _tutorialStarterGerms; i++)
        {
            Vector3 randomSpawn = new Vector3(Random.Range(-_startGermSpawnVariation.x, _startGermSpawnVariation.x), 1.0f, Random.Range(-_startGermSpawnVariation.z, _startGermSpawnVariation.z));
            GameObject obj = Instantiate(starterGerm, randomSpawn, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            _starterGerms.Add(obj);
            Germ1 germ = obj.GetComponent<Germ1>();
            germ.currentSpeed = _starterSpeed;
            germ.duplicationTime = 3.0f;
            germ.health = 20.0f;
        }
    }

    public void PlayHitSFX()
    {
        _audio.PlayOneShot(_hitSFX);
    }


    public void ResetTutorial()
    {
        foreach (GameObject obj in _pooledGerms)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in _starterGerms)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in _pooledLasers)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in _pooledHits)
        {
            obj.SetActive(false);
        }

        _pooledGerms.Clear();
        _starterGerms.Clear();
        TutorialManager.instance.ResetGerms();
        SpawnStarterGerm();
    }

    public void ClearGame()
    {
        foreach (GameObject obj in _pooledGerms)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in _starterGerms)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in _pooledLasers)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in _pooledHits)
        {
            obj.SetActive(false);
        }

        _pooledGerms.Clear();
        _starterGerms.Clear();
    }

    public void ResetGame(int currentWave)
    {
        foreach (GameObject obj in _pooledGerms)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in _starterGerms)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in _pooledLasers)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in _pooledHits)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in _lightnings)
        {
            obj.SetActive(false);
        }

        _pooledGerms.Clear();
        _starterGerms.Clear();

        LevelUpIncrease();
        if (currentWave % 10 == 0)
        {
            for (int i = 0; i < numberOfBossGerms; i++)
            {
                Vector3 randomSpawn = new Vector3(Random.Range(-_startGermSpawnVariation.x, _startGermSpawnVariation.x), 1.0f, Random.Range(-_startGermSpawnVariation.z, _startGermSpawnVariation.z));
                GameObject obj = Instantiate(bossGerms[i], randomSpawn, Quaternion.Euler(90.0f, 0.0f, 0.0f));
                _bossGerms.Add(obj);
                Germ1 germ = obj.GetComponent<Germ1>();
                germ.duplicationTime = _bossGermDuplication;
                germ.health = _bossGermHealth;
                germ.powerOfHeal = _bigGermHealPower;
                germ.frequencyOfHeal = _bigGermHealSpeed;
                germ.AdjustHealthSlider();
            }
            for (int i = 0; i < _amountOfGermsToPool; i++)
            {
                GameObject obj = Instantiate(germ1, transform.position, Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                Germ1 germ = obj.GetComponent<Germ1>();
                int random = Random.Range(0, 10);
                if (random > 6)
                {
                    germ.currentSpeed = _germSpeedHigh;
                }
                else
                {
                    germ.currentSpeed = _germSpeedLow;
                }
                int newRandom = Random.Range(0, 10);
                if (newRandom > 7)
                {
                    germ.duplicationTime = _germDuplicationLow;
                }
                else
                {
                    germ.duplicationTime = _germDuplicationHigh;
                }
                germ.health = _germHealth;
                germ.AdjustHealthSlider();
                obj.SetActive(false);
                _pooledGerms.Add(obj);
                _player.RefillBoost();
            }
        }
        else
        {
            for (int i = 0; i < numberOfStarterGerms; i++)
            {
                Vector3 randomSpawn = new Vector3(Random.Range(-_startGermSpawnVariation.x, _startGermSpawnVariation.x), 1.0f, Random.Range(-_startGermSpawnVariation.z, _startGermSpawnVariation.z));
                GameObject obj = Instantiate(starterGerm, randomSpawn, Quaternion.Euler(90.0f, 0.0f, 0.0f));
                _starterGerms.Add(obj);
                Germ1 germ = obj.GetComponent<Germ1>();
                germ.currentSpeed = _starterSpeed;
                germ.duplicationTime = _starterDuplication;
                germ.health = _starterGermHealth;
            }

            for (int i = 0; i < numberOfBigGerms; i++)
            {
                Vector3 randomSpawn = new Vector3(Random.Range(-_startGermSpawnVariation.x, _startGermSpawnVariation.x), 1.0f, Random.Range(-_startGermSpawnVariation.z, _startGermSpawnVariation.z));
                GameObject obj = Instantiate(bigGerm, randomSpawn, Quaternion.Euler(90.0f, 0.0f, 0.0f));
                _bigGerms.Add(obj);
                Germ1 germ = obj.GetComponent<Germ1>();
                germ.currentSpeed = _bigGermSpeed;
                germ.duplicationTime = _bigGermDuplication;
                germ.health = _bigGermHealth;
                germ.AdjustHealthSlider();
            }



            for (int i = 0; i < _amountOfGermsToPool; i++)
            {
                GameObject obj = Instantiate(germ1, transform.position, Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                Germ1 germ = obj.GetComponent<Germ1>();
                int random = Random.Range(0, 10);
                if (random > 6)
                {
                    germ.currentSpeed = _germSpeedHigh;
                }
                else
                {
                    germ.currentSpeed = _germSpeedLow;
                }
                int newRandom = Random.Range(0, 10);
                if (newRandom > 7)
                {
                    germ.duplicationTime = _germDuplicationLow;
                }
                else
                {
                    germ.duplicationTime = _germDuplicationHigh;
                }
                germ.health = _germHealth;
                germ.AdjustHealthSlider();
                obj.SetActive(false);
                _pooledGerms.Add(obj);
                _player.RefillBoost();
            }

        }
    }

}
