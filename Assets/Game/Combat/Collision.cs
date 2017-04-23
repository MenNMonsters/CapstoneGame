using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    public static bool normalCollide = false;
    public static bool bossCollide = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Normal Door")
            normalCollide = true;
        if (col.gameObject.name == "Boss Door")
            bossCollide = true;
    }
}
