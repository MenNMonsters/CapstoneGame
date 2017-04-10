using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour {

    // Use this for initialization
    [SyncVar]
    public string pname = "player";

    [SyncVar]
    public Color playerColor = Color.white;

    public Color ObjectColor;

    private Color currentColor;
    private Material materialColored;

    void Start () {

        //this.GetComponentInChildren<TextMesh>().text = pname;
        //GetComponent<Renderer>().material.color = playerColor;
    }

     void Update()
    {
        
    }
	
}
