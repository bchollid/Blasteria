using UnityEngine;

public class Lightning : MonoBehaviour
{

    public float radius = 5.0f;
    public float damage = 5.0f;

    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _sfx;

    public void Activate(int numberOfJumps, Vector3 position)
    {
        _lineRenderer.enabled = true;
        RaycastHit[] allHits = Physics.SphereCastAll(position, radius, Vector3.one, 150.0f, _enemyLayer);
        _lineRenderer.positionCount = numberOfJumps;
        if (allHits.Length == 1)
        {
            _lineRenderer.enabled = false;
        }
        else
        {
            _audioSource.PlayOneShot(_sfx);
            if (allHits.Length >= numberOfJumps)
            {
                for (int i = 0; i < numberOfJumps; i++)
                {
                    if (allHits[i].collider != null && allHits[i].collider.gameObject.activeInHierarchy)
                    {
                        _lineRenderer.SetPosition(i, allHits[i].transform.position);
                        allHits[i].collider.gameObject.GetComponent<Germ1>().TakeDamage(damage);
                    }
                    else
                    {
                        _lineRenderer.SetPosition(i, Vector3.zero);
                    }
                }
            }
            else
            {
                for (int i = 0; i < allHits.Length; i++)
                {
                    if (allHits[i].collider != null && allHits[i].collider.gameObject.activeInHierarchy)
                    {
                        _lineRenderer.SetPosition(i, allHits[i].transform.position);
                        allHits[i].collider.gameObject.GetComponent<Germ1>().TakeDamage(damage);
                    }
                    else
                    {
                        _lineRenderer.SetPosition(i, Vector3.zero);
                    }
                }
            }
        }
       
    }

    void Update()
    {

    }


}
