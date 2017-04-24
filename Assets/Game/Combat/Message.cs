using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Message : MonoBehaviour
{
    public Text playerName;
    public Text message;

    public void Populate(string name, string text)
    {
        playerName.text = name;
        message.text = text;
    }
}