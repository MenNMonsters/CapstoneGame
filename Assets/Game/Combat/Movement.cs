using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Movement : NetworkBehaviour
{
    float x;
    float y;

    public static bool combat = false;
    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

        if (isServer)
        {

            x = Input.GetAxis("Horizontal") * Time.deltaTime * 4.0f;
            y = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;

        }
        transform.Translate(x, y, 0);
    }

    void OnGUI()
    {
        if (isServer)
        {
            if (GUI.Button(new Rect(Screen.width * (50f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "Left"))
            {
                x = Input.GetAxis("Horizontal") * Time.deltaTime * 4.0f;
                y = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;
                transform.Translate(-0.5f, 0, 0);
            }
            if (GUI.Button(new Rect(Screen.width * (70f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "Right"))
            {


                x = Input.GetAxis("Horizontal") * Time.deltaTime * 4.0f;
                y = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;
                transform.Translate(0.5f, 0, 0);
            }
            if (GUI.Button(new Rect(Screen.width * (60f / 100f), Screen.height * (1f * 0.767f), Screen.width * (0.1f), Screen.height * (0.065f)), "Up"))
            {


                x = Input.GetAxis("Horizontal") * Time.deltaTime * 4.0f;
                y = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;
                transform.Translate(0, 0.5f, 0);
            }
            if (GUI.Button(new Rect(Screen.width * (60f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "Down"))
            {


                x = Input.GetAxis("Horizontal") * Time.deltaTime * 4.0f;
                y = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;
                transform.Translate(0, -0.5f, 0);
            }

        }


    }


}

