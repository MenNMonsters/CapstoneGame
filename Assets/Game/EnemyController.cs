using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    CombatController combatController;
    public int maxHealth;
    public int currentHealth;
    public Text enemyHealthText;

    //Component enemy;
    //public GameObject enemy;

	// Use this for initialization
	void Start () {
        //enemy = GetComponent<GameObject>();
        //print(enemy.name);

        //maxHealth = 100;
        //currentHealth = 100;
        print(gameObject.name);
        enemyHealthText.text = currentHealth+"/"+maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
