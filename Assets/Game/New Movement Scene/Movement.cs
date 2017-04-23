using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Movement : NetworkBehaviour {
	
	float x; 
	float y;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(isServer){

		 x = Input.GetAxis ("Horizontal") * Time.deltaTime * 3.0f;
		 y = Input.GetAxis ("Vertical") * Time.deltaTime * 3.0f;



		

		//transform.Rotate (0, x, 0);
		
		}
		transform.Translate (x, y, 0);
	}



	void OnGUI(){
		if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "PASS"))
		{
			NetworkManager.singleton.ServerChangeScene("MainMenu");
		}
	
	
	}


}

