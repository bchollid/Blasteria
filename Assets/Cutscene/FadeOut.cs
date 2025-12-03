using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _duration;
    [SerializeField] private float _timeUntilStart = 2.0f;
    private Image _image;
    private Color _color;
    void OnEnable()
    {
        _image = GetComponent<Image>();
        _color = _image.color;
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        yield return new WaitForSeconds(_timeUntilStart);
        float t = 0.0f;
        float alpha = 1.0f;
        while(t < _duration)
        {
            t += Time.deltaTime;
            alpha -= Time.deltaTime * _speed;
            _image.color = new Color(_color.r, _color.g, _color.b, alpha);
            yield return null;
        }
    }

    void Update()
    {
        
    }
}
