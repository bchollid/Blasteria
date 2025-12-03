using UnityEngine;
using UnityEngine.UI;

public class MaskController : MonoBehaviour
{
    [SerializeField] private Sprite[] _masks;
    [SerializeField] private float _tempo;
    private float _timer = 0.0f;
    private int _currentMask;
    private Image _myImage;
   
    void Start()
    {
        _myImage = GetComponent<Image>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _tempo)
        {
            _currentMask++;
            if( _currentMask == _masks.Length )
            {
                _currentMask = 0;
            }
            _myImage.sprite = _masks[_currentMask];
            _timer = 0.0f;
        }
    }
}
