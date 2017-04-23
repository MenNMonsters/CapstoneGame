using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassData : MonoBehaviour
{
    public static PassData passData;
    public int test = 0;

    void Awake()
    {
        if (passData == null)
        {
            DontDestroyOnLoad(transform.gameObject);
            passData = this;
        }
        else if(passData != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
