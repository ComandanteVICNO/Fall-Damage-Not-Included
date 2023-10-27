using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndControllerScript : MonoBehaviour
{
    public TMP_Text countdownText;
    private bool playerEnded = false;
    private float countdown = 20f;
    private float countdownGoodbye = 3f;

    void Start()
    {
        playerEnded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerEnded)
        {
            countdown -= Time.deltaTime;
            countdownText.text = "Turning off simulation in: " + Mathf.RoundToInt(countdown);
            if(countdown <= 0)
            {
                countdownText.text = "Goodbye";
                Invoke(nameof(Quitgame), countdownGoodbye);

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        playerEnded = true;
    }

    private void Quitgame()
    {
        Application.Quit();
    }
}
