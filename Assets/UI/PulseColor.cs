using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PulseColor : MonoBehaviour
{
    [SerializeField] private Image _myImage;
    [SerializeField] private float _speed = 25.0f;
    private Color _color;
    private float _timer;
    void Start()
    {
        _color = _myImage.color;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        _color.a = Mathf.PingPong(_timer * _speed, 1.0f);
        _myImage.color = _color;
    }
}
