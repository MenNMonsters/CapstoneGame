using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Movement : NetworkBehaviour {
	public GameObject mainCamera;
	float x; 
	float y;
	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find ("Canvas");

	}
	
	// Update is called once per frame
	void Update () {
		
		if(isServer){

		 x = Input.GetAxis ("Horizontal") * Time.deltaTime * 3.0f;
		 y = Input.GetAxis ("Vertical") * Time.deltaTime * 3.0f;
		
		}
		transform.Translate (x, y, 0);
	}



	/*void OnGUI(){
		if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "PASS"))
		{
			
			//SceneManager.LoadScene(1);
			//NetworkManager.singleton.ServerChangeScene("Combat");
			//mainCamera.transform.Translate(1,1,0);
			//Destroy(GameObject.Find("Network Manager"));
			//NetworkManager.Shutdown();
			//SceneManager.LoadScene("Combat", LoadSceneMode.Single);

		}
	
	
	}*/


}

