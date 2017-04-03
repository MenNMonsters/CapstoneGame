using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BattleGUI : NetworkBehaviour
{
    const int ENEMY_MAGIC_CONSUME = 20;
    const int PLAYER_MAGIC_CONSUME = 20;

    [SyncVar(hook = "OnTextChanged")]
    public string turn;

    private Text turnText;

    private Text playerName;

    private Text playerHealthText;

    private Text playerEnergyText;

    public Text enemyName;

    public Text enemyHealthText;

    public Text enemyEnergyText;

    public int enemyEnergy;

    public bool beingHandled = false;

    public int playerHealth;

    public int playerEnergy;

    public int enemyHealth;

    public enum BattleStates
    {
        START,
        PLAYERCHOICE,
        ENEMYCHOICE,
        LOSE,
        WIN
    }

    public BattleStates currentState;

    // Use this for initialization
    void Start()
    {

        playerHealth = 100;
        playerEnergy = 100;
        enemyHealth = 100;
        enemyEnergy = 100;

        turn = "Your turn";

        playerName = GameObject.Find("PlayerName").GetComponent<Text>();
        playerName.text = "Player";
        playerHealthText = GameObject.Find("PlayerHealth").GetComponent<Text>();
        playerHealthText.text = playerHealth.ToString();
        playerEnergyText = GameObject.Find("PlayerEnergy").GetComponent<Text>();
        playerEnergyText.text = "100";

        turnText = GameObject.Find("Turn").GetComponent<Text>();

        enemyName = GameObject.Find("EnemyName").GetComponent<Text>();
        enemyName.text = "Enemy";

        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        enemyHealthText.text = enemyHealth.ToString();
        enemyEnergyText = GameObject.Find("EnemyEnergy").GetComponent<Text>();
        enemyEnergyText.text = enemyEnergy.ToString();


        currentState = BattleStates.PLAYERCHOICE;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);

    }

    void OnGUI()
    {

        if (isServer)
        {

            if (GUI.Button(new Rect(Screen.width * (1f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "PASS"))
            {
                if (currentState != BattleStates.LOSE && currentState != BattleStates.WIN && currentState == BattleStates.PLAYERCHOICE)
                {
                    currentState = BattleStates.ENEMYCHOICE;
                }

            }

        }

        if (GUI.Button(new Rect(Screen.width * (1f / 100f), Screen.height * (0.7f), Screen.width * (0.1f), Screen.height * (0.065f)), "NORMAL"))
        {
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

        if (GUI.Button(new Rect(Screen.width * (1f / 100f), Screen.height * (1f * 0.765f), Screen.width * (0.1f), Screen.height * (0.065f)), "Magical"))
        {
            if (currentState == BattleStates.PLAYERCHOICE)
            {
                if (playerEnergy >= PLAYER_MAGIC_CONSUME)
                {
                    enemyHealth -= 20;
                    playerEnergy -= 20;
                    if (playerEnergy < 0)
                    {
                        playerEnergy = 0;
                    }
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
        }


        switch (currentState)
        {
            case (BattleStates.START):
                break;
            case (BattleStates.PLAYERCHOICE):
                turn = "Your Turn";
                break;
            case (BattleStates.ENEMYCHOICE):
                if (!beingHandled)
                {
                    StartCoroutine(enemyTurn());
                }
                break;
            case (BattleStates.LOSE):
                turn = "You Lose";
                break;
            case (BattleStates.WIN):
                turn = "You Win";
                break;
        }


        //CmdChangeTurnText();
        //CmdChangeEnemyHealthText();
        CmdChangePlayerHealthText();
        CmdChangeEnergyText();
        CmdChangeEnemyEnergyText();


    }

    IEnumerator enemyTurn()
    {
        turn = "Enemy Turn";

        int State = Random.Range(0, 2);
        beingHandled = true;
        yield return new WaitForSeconds(1);

        if (State == 1)
        {
            if (enemyEnergy >= ENEMY_MAGIC_CONSUME)
            {
                turn = "Eenmy uses magical attack";

                playerHealth -= 20;
                enemyEnergy -= ENEMY_MAGIC_CONSUME;
                if (enemyEnergy <= 0)
                {
                    enemyEnergy = 0;
                }
            }
        }
        else
        {
            turn = "Eenmy uses normal attack";
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

    //[Command]
    public void CmdChangePlayerHealthText()
    {

        playerHealthText.text = playerHealth.ToString();
    }

    // [Command]
    public void CmdChangeEnergyText()
    {

        playerEnergyText.text = playerEnergy.ToString();
    }

    // [Command]
    public void CmdChangeEnemyEnergyText()
    {

        enemyEnergyText.text = enemyEnergy.ToString();
    }

    // [Command]
    //public void CmdChangeEnemyHealthText()
    //{

    //    enemyHealthText.text = enemyHealth.ToString();
    //}

    //[Command]
    public void CmdChangeTurnText()
    {

        turnText.text = turn;
    }

    void OnTextChanged(string turn)
    {
        this.turn = turn;
        turnText.text = turn;
    }

}
