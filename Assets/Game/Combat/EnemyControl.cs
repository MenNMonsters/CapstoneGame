using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyControl : NetworkBehaviour {

    const int ENEMY_HEALTH = 100;
    const int ENEMY_ENERGY = 100;

    public bool destroyOnDeath;

    //[SyncVar(hook = "OnChangeHealth")]
    public int health = ENEMY_HEALTH;
    public static int energy = ENEMY_ENERGY;

    //public RectTransform healthBar;

    public void TakeDamage(int amount)
    {
        Debug.Log(isServer);
        if (!isServer)
            return;

        health -= amount;
        if (health <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }

        }
    }

    void OnChangeHealth(int currentHealth)
    {
        currentHealth = health;
        Debug.Log("123");
        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
