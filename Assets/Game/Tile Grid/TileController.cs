using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    public GameObject topTile = null;
    public GameObject bottomTile = null;
    public GameObject leftTile = null;
    public GameObject rightTile = null;

    public bool occupied = false;
    public string occupiedObject;

    public bool rightDoor;
    public bool leftDoor;
    public bool backDoor;

    // Use this for initialization
    void Start () {
        /*if (topTile != null)
        {
            print(topTile.transform.position);
        }
        */
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
