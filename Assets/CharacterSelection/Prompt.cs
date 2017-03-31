using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Prompt : MonoBehaviour {

    public void promptSelect(Button startButton)
    {
        startButton.interactable = false;
    }
}
