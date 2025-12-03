using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    private Renderer _rend;
    public int richochets = 0;
    public float damage;
    public int numberOfJumps = 0;
    private bool _canTouchBoundary = true;
    private float _chargeAmount = 0.0f;

    void Start()
    {
    }

    private void OnEnable()
    {
        _rend = GetComponent<Renderer>();
        _canTouchBoundary = true;
    }

    void Update()
    {
        transform.Translate(-Vector3.up * _speed * Time.deltaTime);
    }

    public void AdjustCharge(float chargeAmount)
    {
        _rend.material.SetFloat("_Intensity", chargeAmount * 500);
        _chargeAmount = chargeAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Germ"))
        {
            if (numberOfJumps > 0)
            {
                var newLightning = ObjectPool.instance.GetLightning();
                newLightning.SetActive(true);
                newLightning.transform.position = Vector3.zero;
                newLightning.GetComponent<Lightning>().Activate(numberOfJumps, transform.position);
            }
            other.gameObject.GetComponent<Germ1>().TakeDamage(damage);
            var newFX = ObjectPool.instance.GetHitFX();
            newFX.SetActive(true);
            newFX.transform.position = other.gameObject.transform.position;
            if(_chargeAmount < 0.5f)
            {
                gameObject.SetActive(false);
            }
        }
        if(other.gameObject.CompareTag("Edge"))
        {
            if(richochets <= 0 && _canTouchBoundary)
            {
                gameObject.SetActive(false);
            }
            else if (_canTouchBoundary)
            {
                StartCoroutine(AllowRichochet());  
            }
        }
    }

    private IEnumerator AllowRichochet()
    {
        _canTouchBoundary = false;
        Vector3 direction = Vector3.zero - transform.position;
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90.0f, 0.0f, angle + 90.0f);
        yield return new WaitForSeconds(0.45f);
        richochets--;
        _canTouchBoundary = true;
    }
}
