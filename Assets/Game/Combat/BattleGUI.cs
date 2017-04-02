using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleGUI : MonoBehaviour {

    private Text playerName;
    private Text playerHealthText;
    private Text playerEnergyText;

    private Text enemyName;
    private Text enemyHealthText;
    private Text enemyEnergy;

    private Text turn;

    private bool beingHandled = false;

    private int playerHealth;
    private int playerEnergy;
    private int enemyHealth;

    public enum BattleStates
    {
        START,
        PLAYERCHOICE,
        ENEMYCHOICE,
        LOSE,
        WIN
    }

    private BattleStates currentState;

    // Use this for initialization
    void Start () {

        playerHealth = 100;
        playerEnergy = 100;
        enemyHealth = 100;

        turn = transform.FindChild("Turn").GetComponent<Text>();
        turn.text = "Your turn";

        playerName = transform.FindChild("PlayerInfoContainer").FindChild("PlayerPortrait").FindChild("PlayerName").GetComponent<Text>();
        playerName.text = "Player";
        playerHealthText = transform.FindChild("PlayerInfoContainer").FindChild("PlayerHealthBar").FindChild("PlayerHealth").GetComponent<Text>();
        playerHealthText.text = playerHealth.ToString();
        playerEnergyText = transform.FindChild("PlayerInfoContainer").FindChild("PlayerEnergyBar").FindChild("PlayerEnergy").GetComponent<Text>();
        playerEnergyText.text = "100";

        enemyName = transform.FindChild("EnemyInfoContainer").FindChild("EnemyPortrait").FindChild("EnemyName").GetComponent<Text>();
        enemyName.text = "Enemy";
        enemyHealthText = transform.FindChild("EnemyInfoContainer").FindChild("EnemyHealthBar").FindChild("EnemyHealth").GetComponent<Text>();
        enemyHealthText.text = enemyHealth.ToString();
        enemyEnergy = transform.FindChild("EnemyInfoContainer").FindChild("EnemyEnergyBar").FindChild("EnemyEnergy").GetComponent<Text>();
        enemyEnergy.text = "100";

        currentState = BattleStates.PLAYERCHOICE;
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(currentState);
    }

    void OnGUI()
    {

        if (GUI.Button(new Rect(10,450,100,50),"PASS")) {
                if(currentState != BattleStates.LOSE && currentState != BattleStates.WIN && currentState == BattleStates.PLAYERCHOICE)
            {
                currentState = BattleStates.ENEMYCHOICE;
            }

        }
        
        if (GUI.Button(new Rect(10, 350, 100, 50), "NORMAL")) {
            if (currentState == BattleStates.PLAYERCHOICE)
            {
                enemyHealth -= 10;
                if (enemyHealth <= 0)
                {
                    enemyHealth = 0;
                    currentState = BattleStates.WIN;
                }
                else
                {
                    currentState = BattleStates.ENEMYCHOICE;
                }

            }
        }

        if (GUI.Button(new Rect(10, 400, 100, 50), "Magical"))
        {
            if (currentState == BattleStates.PLAYERCHOICE)
            {
                enemyHealth -= 20;
                playerEnergy -= 20;
                if (enemyHealth <= 0)
                {
                    enemyHealth = 0;
                    currentState = BattleStates.WIN;
                }
                else
                {
                    currentState = BattleStates.ENEMYCHOICE;
                }
            }
        }


        switch (currentState)
        {
            case (BattleStates.START):
                break;
            case (BattleStates.PLAYERCHOICE):
                turn.text = "Your Turn";
                break;
            case (BattleStates.ENEMYCHOICE):
                if (!beingHandled) { 
                    StartCoroutine(enemyTurn());
                }
                break;
            case (BattleStates.LOSE):
                turn.text = "You Lose";
                break;
            case (BattleStates.WIN):
                turn.text = "You Win";
                break;
        }

        playerHealthText.text = playerHealth.ToString();
        playerEnergyText.text = playerEnergy.ToString();
        enemyHealthText.text = enemyHealth.ToString();

    }

    IEnumerator enemyTurn()
    {
        turn.text = "Enemy Turn";
        int State = Random.Range(0, 2);
        beingHandled = true;
        yield return new WaitForSeconds(1);

        if (State == 1)
        {
            turn.text = "Eenmy uses magical attack";
            playerHealth -= 20;
        }
        else
        {
            turn.text = "Eenmy uses normal attack";
            playerHealth -= 5;
        }

        yield return new WaitForSeconds(1);

        if (playerHealth <= 0)
        {
            currentState = BattleStates.LOSE;
        }
        else
        {
            currentState = BattleStates.PLAYERCHOICE;
        }

        beingHandled = false;
    }
}
