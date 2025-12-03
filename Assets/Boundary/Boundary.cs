using UnityEngine;

public class Boundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Germ"))
        {
            if(GameManager.instance != null)
            {
                GameManager.instance.GameOver(other.gameObject.transform.position);
            }
            else if (TutorialManager.instance != null)
            {
                TutorialManager.instance.ShowDefeat();
            }
        }
    }

    //private void OnTr(Collision collision)
    //{
        
    //}
}
