using UnityEngine;

public class MainTextEffect : MonoBehaviour
{
    [SerializeField] private float _speed = 25.0f;
    [SerializeField] private float _radiusOffset = 10.0f;
    [SerializeField] private RectTransform _mainTex;
    [SerializeField] private float _timeBetweenJitters;
    [SerializeField] private float _timeOfEachJitter;
    private Vector2 _originalPosition;
    private Vector2 _centerObjectPosition;
    private float _angle;
    private float _timer = 0.0f;
    void Start()
    {
        _originalPosition = transform.position;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _timeBetweenJitters)
        {
            Jitter();
        }
        if(_timer >= _timeBetweenJitters + _timeOfEachJitter)
        {
            transform.position = _originalPosition;
            _timer = 0.0f;
        }
    }

    private void Jitter()
    {
        _centerObjectPosition = _mainTex.position;
        _angle += Time.deltaTime * 1.0f * _speed;
        float x = Mathf.Cos(_angle) * _radiusOffset;
        float y = Mathf.Sin(_angle) * _radiusOffset;
        Vector2 targetPosition = new Vector2(_mainTex.transform.position.x + x, _mainTex.transform.position.y + y);
        this.transform.position = targetPosition;
    }

}
