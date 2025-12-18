using Unity.VisualScripting;
using UnityEngine;

public class LightCubeTrigger : MonoBehaviour
{
    [SerializeField] private GameObject lamp;

    private void OnTriggerEnter(Collider other)
    {
       lamp.SetActive(false);
    }
    private void OnTriggerExit(Collider other) 
    {
        lamp.SetActive(true);
    }
}
