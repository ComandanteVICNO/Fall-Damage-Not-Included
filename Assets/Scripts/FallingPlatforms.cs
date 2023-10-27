using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;


public class FallingPlatforms : MonoBehaviour
{
    private float MoveDelay = 1.5f;
    private bool PlatCollided;
    public Rigidbody rb;
    public Transform OriginalPos;
    public GameObject Platforms;
    Vector3 PlatformPos;
    private float FallDelay = 1.5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlatCollided = true;
        }

    }

    private void Start()
    {
        
        PlatCollided = false;
        PlatformPos = OriginalPos.transform.position;
        rb.useGravity = false;
        rb.isKinematic = true;

    }
    private void Update()
    {
        if (PlatCollided == true)
        {
            MoveDelay -= Time.deltaTime;
            if (MoveDelay <= 0)
            {

                rb.useGravity = true;
                rb.isKinematic = false;
                Invoke(nameof(resetPos), FallDelay);
            }
        }
    }
    private void resetPos()
    {
        rb.transform.position = new Vector3(PlatformPos.x, PlatformPos.y, PlatformPos.z);
        PlatCollided = false;
        rb.useGravity = false;
        rb.isKinematic = true;
        MoveDelay = 3f;
    }
    }