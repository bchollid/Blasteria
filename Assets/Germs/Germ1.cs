using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Germ1 : MonoBehaviour
{
    [Header("Vitals")]
    [SerializeField] private bool _isStarterGerm = false;
    [SerializeField] private bool _isBigGerm = false;
    [SerializeField] private bool _isBossGerm = false;
    public float health = 100.0f;
    private float _currentMaxHealth = 100.0f;

    [Space(15)]
    [Header("Duplication Settings")]
    [SerializeField] private GameObject _germToDuplicate;
    [SerializeField] private Material _material;
    private float _duplicationTimer = 0.0f;
    public float duplicationTime = 5.0f;

    [Space(15)]
    [Header("Movement Settings")]
    public float currentSpeed = 0.1f;
    private Vector3 _randomPoint;

    [Space(15)]
    [Header("Pushback")]
    [SerializeField] private float _pushbackDuration = 0.5f;
    [SerializeField] private float _pushbackSpeed = 2.0f;

    [Space(15)]
    [Header("Heal")]
    [SerializeField] private float _distanceOfHeal = 5.0f;
    public float frequencyOfHeal = 5.0f;
    public float powerOfHeal = 5.0f;
    [SerializeField] private LayerMask _germLayer;
    private float _healTimer = 0.0f;

    private float _normalSpeed;
    private bool _isFrozen = false;
    private bool _canRemoveScore = true;
    [SerializeField] private Slider healthBar;
    [SerializeField] private ParticleSystem _heal;

    private void Start()
    {
        if(GameManager.instance != null)
        {
            GameManager.instance.gameOverEvent.AddListener(Freeze);
            GameManager.instance.pauseEvent.AddListener(Freeze);
            GameManager.instance.unpauseEvent.AddListener(UnFreeze);
        }
        _normalSpeed = currentSpeed;
        _currentMaxHealth = health;
        AdjustHealthSlider();
    }

    void OnEnable()
    {
        _randomPoint = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
        _canRemoveScore = true;
        _currentMaxHealth = health;
    }


    void Update()
    {
        if(!_isFrozen)
        {
            if(!_isBossGerm)
            {
                transform.Translate(_randomPoint * currentSpeed * Time.deltaTime, Space.World);
            }
            if (ObjectPool.instance != null)
            {
                DuplicateGerm();
            }
        }
        ActivateHeal();
    }

    protected void Freeze()
    {
        _isFrozen = true;
    }

    protected void UnFreeze()
    {
        _isFrozen = false;
    }
    
    private IEnumerator PushBack()
    {
        float timer = 0.0f;
        Vector3 originalPoint = _randomPoint;
        while (timer < _pushbackDuration)
        {
            timer += Time.deltaTime;
            _randomPoint = Vector3.zero;
            currentSpeed = _pushbackSpeed;
            yield return null;
        }
        currentSpeed = _normalSpeed;
        _randomPoint = originalPoint;
    }

    protected void DuplicateGerm()
    {
        _duplicationTimer += Time.deltaTime;

        if(_duplicationTimer >= duplicationTime)
        {
            GameObject newGerm = ObjectPool.instance.GetGerm();
            if(newGerm == null)
            {
                _duplicationTimer = 0.0f;
                return;
            }
            newGerm.transform.position = transform.position;
            newGerm.SetActive(true);
            if(GameManager.instance != null)
            {
                GameManager.instance.AddScore();
            }
            if(TutorialManager.instance != null)
            {
                TutorialManager.instance.AddGerm();
            }
            _duplicationTimer = 0.0f;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        ObjectPool.instance.PlayHitSFX();
        if (_isStarterGerm || _isBigGerm || _isBossGerm)
        {
            healthBar.value = health;
        }
        if (health <= 0.0f)
        {
            currentSpeed = _normalSpeed;
            if(GameManager.instance != null && _canRemoveScore)
            {
                GameManager.instance.RemoveScore();
                _canRemoveScore = false;
            }
            else if (TutorialManager.instance != null)
            {
                TutorialManager.instance.RemoveGerm();
            }
                gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(PushBack());
        }
    }

    public void AdjustHealthSlider()
    {
        if (_isStarterGerm || _isBigGerm || _isBossGerm)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }
    }

    public void ActivateHeal()
    {
        if(_isBigGerm || _isBossGerm)
        {
            _healTimer += Time.deltaTime;
            if(_healTimer >= frequencyOfHeal)
            {
                _heal.Play();
                RaycastHit[] allHits = Physics.SphereCastAll(transform.position, _distanceOfHeal, Vector3.one, 150.0f, _germLayer);
                foreach (RaycastHit hit in allHits)
                {
                    if (hit.collider != null)
                    {
                        hit.collider.gameObject.GetComponent<Germ1>().Heal(powerOfHeal);
                    }
                }
                _healTimer = 0.0f;
            }
        }
    }

    public void Heal(float amountToHeal)
    {
        if(health <= _currentMaxHealth - amountToHeal)
        {
            health += amountToHeal;
        }
        else
        {
            health = _currentMaxHealth;
        }
        if(_isStarterGerm || _isBigGerm)
        {
            healthBar.value = health;
        }
        var newFX = ObjectPool.instance.GetHeal();
        newFX.SetActive(true);
        newFX.transform.position = transform.position;
    }
}
