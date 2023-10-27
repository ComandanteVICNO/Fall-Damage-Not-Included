using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrappleRangeHitbox : MonoBehaviour
{
    public Grappling grapplecode;
    public GameObject GBarrier;
    // Start is called before the first frame update
    void Start()
    {
        grapplecode.maxGrappleDistance = 1;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("uafybuabfujah");
            grapplecode.maxGrappleDistance = 100;
        }


    }
    private void OnTriggerExit(Collider other)
    {
        grapplecode.maxGrappleDistance = 1;
    }
}
