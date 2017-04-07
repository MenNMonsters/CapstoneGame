using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour {


    GameObject startTile;
    GameObject currentTile;
	// Use this for initialization
	void Start () {
        startTile = GameObject.Find("tile(0,7)");
        currentTile = startTile;
        transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
