using UnityEngine;

public class BossGerm : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private Vector3[] _positions;
    [SerializeField] private float _spawnDelay = 3.0f;
    [SerializeField] private int _totalSpawnsPerPosition = 10;
    [SerializeField] private float _rateOfSpawns = 2.0f;

    private bool _inPosition = true;

    private float _spawnTimer = 0.0f;

    private float _spawnDelayTimer = 0.0f;

    private int _currentPosition = 0;

    private int _numberOfSpawns = 0;

    void Start()
    {
        
    }

    void Update()
    {
        Move();
        RapidSpawn();
    }

    private void Move()
    {
        if( !_inPosition )
        {
            Vector3 direction = (_positions[_currentPosition] - transform.position).normalized;
            transform.Translate(direction * _speed * Time.deltaTime, Space.World);
        }
        if((_positions[_currentPosition] - transform.position).magnitude < 0.2f)
        {
            _inPosition = true;
        }
        else
        {
            _inPosition = false;
        }
    }

    private void RapidSpawn()
    {
        if(_inPosition)
        {
            _spawnDelayTimer += Time.deltaTime;
            if(_spawnDelayTimer >= _spawnDelay )
            {
                _spawnTimer += Time.deltaTime;
                if(_spawnTimer >= _rateOfSpawns)
                {
                    GameObject newGerm = ObjectPool.instance.GetGerm();
                    if (newGerm == null)
                    {
                        return;
                    }
                    newGerm.transform.position = transform.position;
                    newGerm.SetActive(true);
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.AddScore();
                    }
                    _numberOfSpawns++;
                    _spawnTimer = 0.0f;
                }
                if(_numberOfSpawns >= _totalSpawnsPerPosition)
                {
                    _currentPosition++;
                    if(_currentPosition >= _positions.Length -1)
                    {
                        _currentPosition = 0;
                    }
                    _numberOfSpawns = 0;
                    _spawnDelayTimer = 0.0f;
                    _spawnTimer = 0.0f;
                }


            }
        }
    }
}
