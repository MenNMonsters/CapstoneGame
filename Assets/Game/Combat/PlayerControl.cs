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
    const int ENEMY_HEALTH = 300;
    const int ENEMY_ENERGY = 300;


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

    private int numOfPlayer;

    private Text turnText;
    private Text playerHealthText;
    private Text playerEnergyText;
    private Text enemyHealthText;
    private Text enemyEnergyText;

    private Image playerHealthImage;
    private Image playerEnergyImage;
    private Image enemyHealthImage;
    private Image enemyEnergyImage;

    private Transform myTransform;

    public Texture2D bgImage;
    public Texture2D fgImage;

    GameObject p3;
    GameObject p4;

    bool p3Enabled = false;
    bool p4Enabled = false;
    bool normal = false;
    bool magic = false;

    public NetworkView nView;


    private void Awake()
    {
        myTransform = transform;
    }

    // Use this for initialization
    void Start()
    {
        nView = GetComponent<NetworkView>();

        playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        playerHealth = PLAYER_HEALTH;
        playerEnergy = PLAYER_ENERGY;

        turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        turn = "Your Turn";


        p3 = GameObject.Find("PlayerInfoContainer3");
        p4 = GameObject.Find("PlayerInfoContainer4");

        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        enemyHealth = ENEMY_HEALTH;
        enemyEnergyText = GameObject.Find("EnemyEnergy").GetComponent<Text>();
        enemyEnergy = ENEMY_ENERGY;
        enemyHealthImage = GameObject.Find("EnemyHealthBar").GetComponent<Image>();
        enemyEnergyImage = GameObject.Find("EnemyEnergyBar").GetComponent<Image>();

        if (isLocalPlayer)
        {
            p3.SetActive(false);
            p4.SetActive(false);
        }
        currentState = BattleStates.PLAYERCHOICE;

    }

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }

    // Update is called once per frame
    void Update()
    {

        if (isServer)
        {
            if (!p3Enabled || !p4Enabled)
                RpcOnNumOfPlayerChanged(NetworkServer.connections.Count);
        }


        playerHealthImage.fillAmount = (float)playerHealth / 100f;
        playerEnergyImage.fillAmount = (float)playerEnergy / 100f;
        enemyHealthImage.fillAmount = (float)enemyHealth / 100f;
        enemyEnergyImage.fillAmount = (float)enemyEnergy / 100f;




        if (myTransform.name == "" || myTransform.name == "Player(Clone)")
        {
            SetIdentity();
        }
    }

    [ClientRpc]
    void RpcOnNumOfPlayerChanged(int value)
    {
        numOfPlayer = value;
        if (p3 != null && !p3Enabled && numOfPlayer == 3)
        {
            p3.SetActive(true);
            p3Enabled = true;
        }

        if (p4 != null && !p4Enabled && numOfPlayer == 4)
        {
            p4.SetActive(true);
            p4Enabled = true;
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

        if (isLocalPlayer)
        {
            normal = false;
            magic = false;

            if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "PASS"))
            {
                if (currentState != BattleStates.LOSE && currentState != BattleStates.WIN && currentState == BattleStates.PLAYERCHOICE)
                {
                    currentState = BattleStates.ENEMYCHOICE;
                }
            }

            if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (0.7f), Screen.width * (0.1f), Screen.height * (0.065f)), "NORMAL"))
            {
                if (currentState == BattleStates.PLAYERCHOICE)
                {
                    normal = true;
                    CmdOnEnemyHealthChanged(10);
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

            if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.765f), Screen.width * (0.1f), Screen.height * (0.065f)), "Magical"))
            {
                if (currentState == BattleStates.PLAYERCHOICE)
                {
                    magic = true;
                    if (playerEnergy >= PLAYER_MAGIC_CONSUME)
                    {
                        CmdOnEnemyHealthChanged(20);
                        CmdOnPlayerEnergyChanged(20);
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
        beingHandled = true;
        if (normal)
        {
            turn = "normal attack!";
        }else if (magic)
        {
            turn = "magical attack!";
        }else
        {
            turn = "pass turn";
        }
        CmdOnTurnChanged(turn);
        yield return new WaitForSeconds(1);

        turn = "Enemy Turn";
        CmdOnTurnChanged(turn);
        int State = Random.Range(0, 2);
        yield return new WaitForSeconds(1);

        if (State == 1)
        {
            if (enemyEnergy >= ENEMY_MAGIC_CONSUME)
            {
                turn = "Eenmy uses magical attack";

                CmdOnPlayerHealthChanged(20);
                CmdOnEnemyEnergyChanged(ENEMY_MAGIC_CONSUME);
                if (enemyEnergy <= 0)
                {
                    enemyEnergy = 0;
                }
            }
        }
        else
        {
            turn = "Eenmy uses normal attack";
            CmdOnPlayerHealthChanged(5);
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
        turnText.text = turn;
    }

    [Command]
    void CmdOnPlayerHealthChanged(int value)
    {
        playerHealth -= value;  
    }

    [Command]
    void CmdOnPlayerEnergyChanged(int value)
    {
        playerEnergy -= value;
    }

    [Command]
    void CmdOnEnemyHealthChanged(int value)
    {
        enemyHealth = int.Parse(enemyHealthText.text);
        enemyHealth -= value;
    }

    [Command]
    void CmdOnEnemyEnergyChanged(int value)
    {
        enemyEnergy = int.Parse(enemyEnergyText.text);
        enemyEnergy -= value;
    }

    void OnTurnChanged(string value)
    {
        turn = value;

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