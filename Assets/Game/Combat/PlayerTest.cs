using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerTest : NetworkBehaviour
{

    private Text turnText;

    [SyncVar]
    public string turn;

    // Use this for initialization
    void Start()
    {
        turnText = GameObject.Find("Turn").GetComponent<Text>();
        turn = "Text";
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //Debug.Log(turn);
           
        }
    }

    void OnGUI()
    {
        Debug.Log(isLocalPlayer);
        if (isLocalPlayer)
        {
            if (GUI.Button(new Rect(Screen.width * (1f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "PASS"))
            {
                turn = "Player2";
                CmdChangeTurn(turn);
            }
        }
    }

    [Command]
    void CmdChangeTurn(string value)
    {
        Debug.Log(turn);
        turn = value;
        turnText.text = turn;
    }

    void OnTurnChanged(string turn)
    {
        this.turn = turn;

        if (isLocalPlayer) { turnText.text = turn; }
        
    }
}
