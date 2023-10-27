using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHitbox3 : MonoBehaviour
{
    public bool hitboxCheck;
    public void OnTriggerEnter(Collider other)
    {

        hitboxCheck = true;

    }
    public void OnTriggerExit(Collider other)
    {
        hitboxCheck = false;

    }
}
