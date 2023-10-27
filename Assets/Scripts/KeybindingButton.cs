using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindingButton : MonoBehaviour
{
    public Button keybindingButton;
    public TMP_Text buttonText;

    private KeyCode assignedKeyCode;
    private bool isListeningForInput;

    void Start()
    {
        keybindingButton.onClick.AddListener(OnButtonClick);
    }

    void Update()
    {
        if (isListeningForInput && Input.anyKeyDown)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                assignedKeyCode = e.keyCode;
                buttonText.text = assignedKeyCode.ToString();
                isListeningForInput = false;
            }
            else if (e.isMouse)
            {
                assignedKeyCode = KeyCode.Mouse0 + e.button;
                buttonText.text = assignedKeyCode.ToString();
                isListeningForInput = false;
            }
        }
    }

    void OnButtonClick()
    {
        if (!isListeningForInput)
        {
            isListeningForInput = true;
            buttonText.text = "Press Key...";
        }
    }
}
