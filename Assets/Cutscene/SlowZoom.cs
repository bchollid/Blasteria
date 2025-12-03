using UnityEngine;

public class SlowZoom : MonoBehaviour
{
    [SerializeField] private float _speed;
    void OnEnable()
    {
        transform.localPosition = new Vector3(0.0f, -125.0f, 0.0f);
    }

    void Update()
    {
        transform.localPosition += Vector3.up * Time.deltaTime * _speed;
    }
}
