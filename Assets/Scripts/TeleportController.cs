using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    public Transform teleportDestination;
    public Rigidbody rb;
    public TMP_Text countdownText;
    private Vector3 teleportLocation;

    public GameObject wall;
    public AudioSource teleportAudio;
    public AudioClip teleportSound;
    private bool canTeleport = false;
    private float teleportCountdown = 5f;



    // Start is called before the first frame update
    void Start()
    {
        teleportLocation = teleportDestination.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (canTeleport)
        {
            teleportCountdown -= Time.deltaTime;
            countdownText.text = "Teleporting in: " + Mathf.RoundToInt(teleportCountdown);
            if(teleportCountdown <= 0)
            {
                doTeleport();
                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        canTeleport = true;
        wall.SetActive(true);
    }

    private void doTeleport()
    {
        
        rb.transform.position = new Vector3(teleportLocation.x, teleportLocation.y, teleportLocation.z);
        teleportAudio.PlayOneShot(teleportSound);
        canTeleport = false;

    }
}
