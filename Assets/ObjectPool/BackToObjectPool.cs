using System.Collections;
using UnityEngine;

public class BackToObjectPool : MonoBehaviour
{
    [SerializeField] private float _totalTimeToWait = 0.1f;
    void OnEnable()
    {
        StartCoroutine(GoBackToObjectPool());
    }

    private IEnumerator GoBackToObjectPool()
    {
        yield return new WaitForSeconds(_totalTimeToWait);
        gameObject.SetActive(false);
    }
}
