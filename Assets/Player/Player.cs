using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Vitals")]
    [SerializeField] private float _damage = 25.0f;

    [Space(15)]
    [Header("Movement")]
    [SerializeField] private float _speed = 25.0f;
    [SerializeField] private float _radius = 15.0f;
    private float _angle = 0.0f;
    [SerializeField] private Vector3 _centerPoint = Vector3.zero;

    [Space(15)]
    [Header("Shooting")]
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private float _additionalLaserOffset;
    [SerializeField] private float _cannonAnimationSpeed = 2.0f;
    private int _numberOfJumps = 0;

    private int _numberOfRichochets = 0;
    private int _numberOfLasers = 1;
    private float _fireTimer = 0.0f;
    private bool _isFiring = false;

    [Space(15)]
    [Header("Flamethrower")]
    [SerializeField] private float _distance = 2.0f;
    [SerializeField] private LayerMask _enemyLayer;
    private bool _flameThrowerActivated = false;
    private float _currentDistance;
    

    [Space(15)]
    [Header("Charge Shot")]
    [SerializeField] private float _damageMultiplier = 2.0f;
    [SerializeField] private float _chargeShotSpeedMultiplier = 1.0f;
    private bool _chargeShotActivated = false;
    private float _timeBetweenShot = 0.0f;

    [Space(15)]
    [Header("Boost")]
    [SerializeField] private float _boostSpeed = 35.0f;
    [SerializeField] private float _boostDrainRate = 25.0f;
    [SerializeField] private float _boostRechargeRate = 10.0f;
    [SerializeField] private float _minBoostRequiredToActivate = 10.0f;
    [SerializeField] private float _maxBoost = 100.0f;
    private float _totalBoost = 100.0f;

    [Space(15)]
    [Header("VisualFX")]
    [SerializeField] private float _screenShakeAmount = 0.25f;
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeSpeed = 10.0f;

    [Space(15)]
    [Header("Serialized Refs")]
    [SerializeField] private GameObject _laser;
    [SerializeField] private ParticleSystem _fireFX;
    [SerializeField] private Image _boostImage;
    [SerializeField] private GameObject _cylinder;
    [SerializeField] private GameObject _cannon;
    [SerializeField] private AudioClip[] _fireSFX;
    [SerializeField] private AudioClip _chargedShotSFX;
    [SerializeField] private AudioSource _boostSource;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private GameObject _chargeShot;
    [SerializeField] private Image _chargeShotImage;
    [SerializeField] private ParticleSystem _flameThrower;

    private PlayerActions playerActions;
    [HideInInspector]
    public bool isBoosting;
    [HideInInspector]
    public bool canShoot = true;
    private float _currentSpeed;
    private bool _canPlay = false;

    [HideInInspector]
    public bool isMoving = false;

    void Start()
    {
        StartCoroutine(LoadOrder());
    }

    private IEnumerator LoadOrder()
    {
        yield return new WaitForEndOfFrame();
        SetUpController();
        if(GameManager.instance != null)
        {
            GameManager.instance.gameOverEvent.AddListener(SwitchPlayAbility);
            GameManager.instance.winEvent.AddListener(SwitchPlayAbility);
        }
        _totalBoost = _maxBoost;
        _canPlay = true;
        _boostSource.Stop();
        _currentDistance = 0.2f;
        _flameThrower.Stop();
    }

    void Update()
    {
        if(_canPlay)
        {
            PollMovement();

            ProcessMovement();

            ProcessShooting();

            ProcessBoost();
        }
    }

    protected void PollMovement()
    {
        if (playerActions.Default.MoveClockwise.ReadValue<float>() > 0.0f)
        {
            _angle -= Time.deltaTime * 1.0f * _currentSpeed;
            MoveAnimation(-1.0f);
            isMoving = true;
        }
        else if (playerActions.Default.MoveCounterClockwise.ReadValue<float>() > 0.0f)
        {
            _angle += Time.deltaTime * 1.0f * _currentSpeed;
            MoveAnimation(1.0f);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    protected void ProcessMovement()
    {
        float x = Mathf.Cos(_angle) * _radius;
        float y = _centerPoint.y;
        float z = Mathf.Sin(_angle) * _radius;

        transform.position = new Vector3(x, y, z);

        transform.LookAt(_centerPoint);
    }

    protected void ProcessShooting()
    {
        _fireTimer += Time.deltaTime;
        if (_isFiring && canShoot)
        {
            if(_fireTimer >= _fireRate)
            {

                for(int i = 0; i < _numberOfLasers; i++)
                {
                    if (TutorialManager.instance != null)
                    {
                        TutorialManager.instance.FiredShot();
                    }
                    StartCoroutine(ShootAnimation());
                    _fireFX.Play();
                    var newLaser = ObjectPool.instance.GetLaser();
                    newLaser.SetActive(true);
                    newLaser.transform.position = transform.position;
                    Vector3 direction = _centerPoint - newLaser.transform.position;
                    float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                    if(i % 2 == 0)
                    {
                        newLaser.transform.rotation = Quaternion.Euler(90.0f, 0.0f, angle + 90.0f + (_additionalLaserOffset * i));
                    }
                    else
                    {
                        newLaser.transform.rotation = Quaternion.Euler(90.0f, 0.0f, angle + 90.0f + (_additionalLaserOffset * -i));
                    }
                    Laser myLaser = newLaser.GetComponent<Laser>();
                    if(_chargeShotActivated)
                    {
                        if(_timeBetweenShot >= 2.0f)
                        {
                            myLaser.damage = _damage * (_timeBetweenShot * _damageMultiplier);
                            myLaser.AdjustCharge(_timeBetweenShot);
                            _audio.PlayOneShot(_chargedShotSFX, 1.0f);
                            _timeBetweenShot = 0.0f;
                            _chargeShotImage.rectTransform.localPosition = new Vector3(_chargeShotImage.rectTransform.localPosition.x, (_timeBetweenShot * 100) - (2.0f * 100), 0.0f);
                        }
                        else
                        {
                            myLaser.damage = _damage;
                            myLaser.AdjustCharge(0.0f);
                            int random = Random.Range(0, _fireSFX.Length);
                            _audio.PlayOneShot(_fireSFX[random], 0.5f);
                        }
                    }
                    else
                    {
                        myLaser.damage = _damage;
                        int random = Random.Range(0, _fireSFX.Length);
                        _audio.PlayOneShot(_fireSFX[random], 0.5f);
                        myLaser.AdjustCharge(0.0f);
                    }
                    if(_flameThrowerActivated)
                    {
                        RaycastHit[] allHits = Physics.SphereCastAll(transform.position, _distance, Vector3.one, 150.0f, _enemyLayer);
                        foreach(RaycastHit hit in allHits)
                        {
                            if(hit.collider != null)
                            {
                                hit.collider.gameObject.GetComponent<Germ1>().TakeDamage(_damage);
                                var newFX = ObjectPool.instance.GetHitFX();
                                newFX.SetActive(true);
                                newFX.transform.position = hit.collider.gameObject.transform.position;
                            }
                        }
                    }
                    myLaser.richochets = _numberOfRichochets;
                    if(_numberOfJumps > 0)
                    {
                        myLaser.numberOfJumps = _numberOfJumps;
                    }

                }
                _fireTimer = 0.0f;
            }
        }
        else if (!_isFiring && canShoot && _chargeShotActivated)
        {
            _timeBetweenShot += Time.deltaTime * _chargeShotSpeedMultiplier;
            _timeBetweenShot = Mathf.Clamp(_timeBetweenShot, 0.0f, 2.1f);
            _chargeShotImage.rectTransform.localPosition = new Vector3(_chargeShotImage.rectTransform.localPosition.x, (_timeBetweenShot * 100) - (2.0f * 100), 0.0f);
        }
    }

    private IEnumerator ScreenShake()
    {
        Vector3 startPosition = Camera.main.transform.position;
        Vector3 randomPosition = new Vector3(Random.Range(-_screenShakeAmount, _screenShakeAmount), 0.0f, Random.Range(-_screenShakeAmount, _screenShakeAmount));
        float elapsedTime = 0.0f;

        while(elapsedTime < _shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, startPosition + randomPosition, elapsedTime * _shakeSpeed);
            yield return null;
        }
        EndScreenShake();
    }

    private void EndScreenShake()
    {
        Camera.main.transform.position = new Vector3(0.0f, 20.0f, 0.0f);
    }

    protected void Shoot(InputAction.CallbackContext context)
    {
        if(context.performed && canShoot)
        {
            _isFiring = true;
            if (_flameThrowerActivated)
            {
                _flameThrower.Play();
            }
        }
        if(context.canceled)
        {
            _isFiring = false;
            if (_flameThrowerActivated)
            {
                _flameThrower.Stop();
            }
        }
    }

    protected void ProcessBoost()
    {
        if(isBoosting)
        {
            if (playerActions.Default.MoveCounterClockwise.ReadValue<float>() > 0.0f || playerActions.Default.MoveClockwise.ReadValue<float>() > 0.0f)
            {
                _currentSpeed = _boostSpeed;
                _totalBoost -= _boostDrainRate * Time.deltaTime;
            }
            else
            {
                if (_totalBoost < _maxBoost)
                {
                    _totalBoost += _boostRechargeRate * Time.deltaTime;
                }
            }
        }
        else
        {
            _currentSpeed = _speed;
            if(_totalBoost < _maxBoost)
            {
                _totalBoost += _boostRechargeRate * Time.deltaTime;
            }
        }
        if(isBoosting && _totalBoost <= 0)
        {
            isBoosting = false;
            _boostSource.Stop();
        }
        _boostImage.rectTransform.localPosition = new Vector3(_boostImage.rectTransform.localPosition.x, (_totalBoost *3.0f - 300.0f), 0.0f);
    }

    protected void Boost(InputAction.CallbackContext context)
    {
        if(context.performed && _totalBoost >= _minBoostRequiredToActivate)
        {
            isBoosting = true;
            _boostSource.Play();
        }
        if (context.canceled)
        {
            isBoosting = false;
            _boostSource.Stop();
        }
    }

    protected void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(GameManager.instance != null)
            {
                GameManager.instance.PauseGame();
            }
            SwitchPlayAbility();
        }
    }

    public void SwitchPlayAbility()
    {
        _canPlay = !_canPlay;
    }

    //protected void StartFreeze(InputAction.CallbackContext context)
    //{
    //    GameManager.instance.StartFreeze();
    //}

    public void ImproveFireRate()
    {
        _fireRate *= 0.85f;
    }

    public void ImproveSpeed()
    {
        _speed *= 1.5f;
        _currentSpeed = _speed;
    }

    public void ImproveDamage()
    {
        _damage *= 1.15f;
    }

    public void ImproveBoost()
    {
        _boostSpeed *= 1.5f;
        _boostRechargeRate *= 2.0f;
    }

    public void AddAdditionalLaser()
    {
        _numberOfLasers++;
    }

    public void AddRichochet()
    {
        _numberOfRichochets++;
    }

    public void RefillBoost()
    {
        _totalBoost = _maxBoost;
    }

    public void AddLightning()
    {
        if(_numberOfJumps == 0)
        {
            _numberOfJumps = 2;
        }
        else
        {
            _numberOfJumps++;
        }
    }

    public void AddChargeShot()
    {
        _chargeShotActivated = true;
        _damageMultiplier *= 1.3f;
        _chargeShotSpeedMultiplier *= 1.3f;
        _chargeShot.SetActive(true);
    }

    public void AddFlamethrower()
    {
        _flameThrowerActivated = true;
        _distance += 0.45f;
        _currentDistance += 0.15f;
        var throwerMain = _flameThrower.main;
        throwerMain.startLifetime = _currentDistance;
    }

    private void MoveAnimation(float direction)
    {
        if(isBoosting)
        {
            _cylinder.transform.Rotate(Vector3.forward * 3.0f * direction);
        }
        else
        {
            _cylinder.transform.Rotate(Vector3.forward * 0.5f * direction);
        }
    }

    private IEnumerator ShootAnimation()
    {
        float t = 0.0f;
        while (t < 0.2f * _fireRate)
        { 
            t += Time.deltaTime;
            _cannon.transform.position += transform.forward * Time.deltaTime * _cannonAnimationSpeed;
            yield return null;
        }
        float n = 0.0f;
        while(n < 0.2f * _fireRate)
        {
            n += Time.deltaTime;
            _cannon.transform.position -= transform.forward * Time.deltaTime * _cannonAnimationSpeed;
            yield return null;
        }
    }
    
    private void SetUpController()
    {
        playerActions = new PlayerActions();
        playerActions.Default.Shoot.performed += Shoot;
        playerActions.Default.Shoot.canceled += Shoot;
        //playerActions.Default.Freeze.performed += StartFreeze;
        playerActions.Default.Boost.performed += Boost;
        playerActions.Default.Boost.canceled += Boost;
        playerActions.Default.Pause.performed += Pause;
        playerActions.Enable();
    }
}
