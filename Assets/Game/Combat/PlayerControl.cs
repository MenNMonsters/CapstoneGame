using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{
    const int ENEMY_MAGIC_CONSUME = 20;
    const int PLAYER_MAGIC_CONSUME = 20;
    const int PLAYER_HEALTH = 100;
    const int PLAYER_ENERGY = 100;
    const int ENEMY_HEALTH = 100;
    const int ENEMY_ENERGY = 100;

    private bool beingHandled = false;

    public enum BattleStates
    {
        START,
        PLAYERCHOICE,
        ENEMYCHOICE,
        LOSE,
        WIN
    }

    private NetworkInstanceId playerNetID;

    [SyncVar]
    public string playerUniqueIdentity;
    [SyncVar(hook = "OnStateChanged")]
    private BattleStates currentState;
    [SyncVar(hook = "OnTurnChanged")]
    private string turn;
    [SyncVar(hook = "OnPlayerHealthChanged")]
    private int playerHealth;
    [SyncVar(hook = "OnPlayerEnergyChanged")]
    private int playerEnergy;
    [SyncVar(hook = "OnEnemyHealthChanged")]
    private int enemyHealth;
    [SyncVar(hook = "OnEnemyEnergyChanged")]
    private int enemyEnergy;

    private Text turnText;
    private Text playerHealthText;
    private Text playerEnergyText;
    private Text enemyHealthText;
    private Text enemyEnergyText;

    private Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
    }

    // Use this for initialization
    void Start()
    {
        turnText = GameObject.Find("Turn").GetComponent<Text>();
        playerHealthText = GameObject.Find("PlayerHealth").GetComponent<Text>();
        playerHealth = PLAYER_HEALTH;
        playerEnergyText = GameObject.Find("PlayerEnergy").GetComponent<Text>();
        playerEnergy = PLAYER_ENERGY;

        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        enemyHealth = ENEMY_HEALTH;
        enemyEnergyText = GameObject.Find("EnemyEnergy").GetComponent<Text>();
        enemyEnergy = ENEMY_ENERGY;

        currentState = BattleStates.PLAYERCHOICE;

        turn = "Your Turn";

    }

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }

    // Update is called once per frame
    void Update()
    {
        if(myTransform.name == "" || myTransform.name == "Player(Clone)")
        {
            SetIdentity();
        }
    }

    [Client]
    void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());

    }

    void SetIdentity()
    {
        if (!isLocalPlayer)
        {
            myTransform.name = playerUniqueIdentity;
        }
        else
        {
            myTransform.name = MakeUniqueIdentity();
        }
    }

    string MakeUniqueIdentity()
    {
        string uniqueName = "Player " + playerNetID.ToString();
        return uniqueName;
    }

    [Command]
    void CmdTellServerMyIdentity(string name)
    {
        playerUniqueIdentity = name;
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
        }

    }

    IEnumerator enemyTurn()
    {
        turn = "Enemy Turn";
        CmdOnTurnChanged(turn);
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
        CmdOnTurnChanged(turn);
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
        turn = "Your Turn";
        CmdOnTurnChanged(turn);
    }

    [Command]
    void CmdOnTurnChanged(string turn)
    {
        this.turn = turn;
        Debug.Log(turn); 
        turnText.text = turn;
    }

    void OnTurnChanged(string turn)
    {
        this.turn = turn;

        if (isLocalPlayer) { Debug.Log(turn); }
            turnText.text = turn;
    }

    void OnPlayerHealthChanged(int value)
    {
        playerHealth = value;

        playerHealthText.text = playerHealth.ToString();
    }

    void OnPlayerEnergyChanged(int value)
    {
        playerEnergy = value;

        playerEnergyText.text = playerEnergy.ToString();
    }

    void OnEnemyHealthChanged(int value)
    {
        enemyHealth = value;

        enemyHealthText.text = enemyHealth.ToString();
    }

    void OnEnemyEnergyChanged(int value)
    {
        enemyEnergy = value;

        enemyEnergyText.text = enemyEnergy.ToString();
    }

    void OnStateChanged(BattleStates value)
    {
        currentState = value;
    }

}
