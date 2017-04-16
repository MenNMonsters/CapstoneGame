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
    [SyncVar(hook = "OnEnemyHealth2Changed")]
    private int enemyHealth2;
    [SyncVar(hook = "OnEnemyEnergyChanged")]
    private int enemyEnergy;

    [SyncVar(hook = "OnCountChanged")]
    private int count;

    [SyncVar]
    public int numOfPlayer = 4;

    [SyncVar(hook = "OnPlayerName")]
    public string pName;

    private Text turnText;
    private Text playerHealthText;
    private Text playerEnergyText;
    private Text enemyHealthText;
    private Text enemyHealth2Text;
    private Text enemyEnergyText;
    private Text playerNameText;
    private Text PlayerPortraitText;
    private Text countText;

    private Image playerHealthImage;
    private Image playerEnergyImage;
    private Image enemyHealthImage;
    private Image enemyEnergyImage;

    private Transform myTransform;

    GameObject p2;
    GameObject p3;
    GameObject p4;

    bool p2Enabled = false;
    bool p3Enabled = false;
    bool p4Enabled = false;
    bool normal = false;
    bool magic = false;
    bool turnChoosed = false;

    private void Awake()
    {
        myTransform = transform;
    }

    // Use this for initialization
    void Start()
    {
        //Debug.Log(numOfPlayer);
        //p2 = GameObject.Find("PlayerInfoContainer2");
        //p3 = GameObject.Find("PlayerInfoContainer3");
        //p4 = GameObject.Find("PlayerInfoContainer4");

        //if (isLocalPlayer)
        //{
        //    if (numOfPlayer < 4)
        //    {
        //        p4.SetActive(false);
        //        if (numOfPlayer < 3)
        //        {
        //            p3.SetActive(false);
        //            if (numOfPlayer < 2)
        //            {
        //                p2.SetActive(false);
        //            }
        //        }
        //    }

        //    Debug.Log(pName);

        //    playerNameText = GameObject.Find("Name").GetComponent<Text>();
        //    if (playerNameText != null)
        //        playerNameText.text = "You are " + pName;
        //}

        //PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //PlayerPortraitText.text = pName;

        //OnPlayerName(pName);

        playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();



        //playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        turn = pName + ":Your Turn";
        OnTurnChanged(turn);

        playerHealth = PLAYER_HEALTH;
        playerEnergy = PLAYER_ENERGY;

        countText = GameObject.Find("Count").GetComponent<Text>();
        count = 4;
        OnCountChanged(count);

        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        enemyHealth = ENEMY_HEALTH;
        enemyHealth2Text = GameObject.Find("EnemyHealth2").GetComponent<Text>();
        enemyHealth2 = ENEMY_HEALTH;
        enemyEnergyText = GameObject.Find("EnemyEnergy").GetComponent<Text>();
        enemyEnergy = ENEMY_ENERGY;
        enemyHealthImage = GameObject.Find("EnemyHealthBar").GetComponent<Image>();
        enemyEnergyImage = GameObject.Find("EnemyEnergyBar").GetComponent<Image>();
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

        //if (isServer)
        //{
        //    if (!p3Enabled || !p4Enabled)
        //        RpcOnNumOfPlayerChanged(NetworkServer.connections.Count);
        //}

        if (p2 != null && !p2Enabled && numOfPlayer == 2)
        {
            p2.SetActive(true);
            p2Enabled = true;
        }

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

        playerHealthImage.fillAmount = (float)playerHealth / PLAYER_HEALTH;
        playerEnergyImage.fillAmount = (float)playerEnergy / PLAYER_ENERGY;
        enemyHealthImage.fillAmount = (float)enemyHealth / ENEMY_HEALTH;
        enemyEnergyImage.fillAmount = (float)enemyEnergy / ENEMY_ENERGY;




        if (myTransform.name == "" || myTransform.name == "Player(Clone)")
        {
            SetIdentity();
        }
    }

    [ClientRpc]
    void RpcOnNumOfPlayerChanged(int value)
    {
        numOfPlayer = value;
        if (p2 != null && !p2Enabled && numOfPlayer == 2)
        {
            //p2.SetActive(true);
            p2Enabled = true;
        }

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
            if (!turnChoosed)
            {
                normal = false;
                magic = false;
            }

            if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "PASS"))
            {
                if (currentState != BattleStates.LOSE && currentState != BattleStates.WIN && currentState == BattleStates.PLAYERCHOICE)
                {
                    turnChoosed = true;
                    turn = pName + "chooses pass turn!";
                    CmdOnTurnChanged(turn);
                    CmdOnCountChanged();
                }
            }

            if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (0.7f), Screen.width * (0.1f), Screen.height * (0.065f)), "NORMAL") && !turnChoosed)
            {
                if (currentState == BattleStates.PLAYERCHOICE)
                {
                    normal = true;
                    turnChoosed = true;
                    CmdOnEnemyHealth2Changed(10);
                    if (enemyHealth <= 0)
                    {
                        enemyHealth = 0;
                        currentState = BattleStates.WIN;
                    }
                    else
                    {
                        turn = pName + "chooses normal attack!";
                        CmdOnTurnChanged(turn);
                        CmdOnCountChanged();
                    }

                }
            }

            if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.765f), Screen.width * (0.1f), Screen.height * (0.065f)), "Magical") && !turnChoosed)
            {
                if (currentState == BattleStates.PLAYERCHOICE)
                {
                    magic = true;
                    if (playerEnergy >= PLAYER_MAGIC_CONSUME)
                    {
                        turnChoosed = true;
                        CmdOnEnemyHealth2Changed(20);
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
                            turn = pName + "chooses Magical attack!";
                            CmdOnTurnChanged(turn);
                            CmdOnCountChanged();
                        }
                    }
                }
            }

            if (int.Parse(countText.text) == 0)
            {
                currentState = BattleStates.ENEMYCHOICE;
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
            CmdOnEnemyHealthChanged(10);
        }
        else if (magic)
        {
            CmdOnEnemyHealthChanged(20);
        }

        if (int.Parse(enemyHealthText.text) <= 0)
        {
            enemyHealth = 0;
            enemyHealthText.text = enemyHealth.ToString();
            currentState = BattleStates.WIN;
        }

        yield return new WaitForSeconds(1);

        turn = pName + ":Enemy Turn";
        CmdOnTurnChanged(turn);
        int State = Random.Range(0, 2);
        yield return new WaitForSeconds(1);

        if (State == 1)
        {
            if (enemyEnergy >= ENEMY_MAGIC_CONSUME)
            {
                turn = pName + ":Eenmy uses magical attack";

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
            turn = pName + ":Enemy uses normal attack";
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
        turn = pName + ":Your Turn";
        CmdOnTurnChanged(turn);
        CmdOnResetCount();
        count = 4;
        countText.text = count.ToString();
        turnChoosed = false;
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
        enemyHealth = int.Parse(enemyHealth2Text.text);
        enemyHealth -= value;
        enemyHealth += value;
    }

    [Command]
    void CmdOnEnemyHealth2Changed(int value)
    {
        enemyHealth2 = int.Parse(enemyHealth2Text.text);
        enemyHealth2 -= value;
    }

    [Command]
    void CmdOnCountChanged()
    {
        count = int.Parse(countText.text);
        count -= 1;
    }

    [Command]
    void CmdOnResetCount()
    {
        count = 4;
        countText.text = count.ToString();
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

    void OnPlayerName(string value)
    {
        pName = value;

        if (PlayerPortraitText != null)
            PlayerPortraitText.text = pName.ToString();
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

    void OnEnemyHealth2Changed(int value)
    {
        enemyHealth2 = value;

        enemyHealth2Text.text = enemyHealth2.ToString();
    }

    void OnEnemyEnergyChanged(int value)
    {
        enemyEnergy = value;

        enemyEnergyText.text = enemyEnergy.ToString();
    }

    void OnCountChanged(int value)
    {
        count = value;

        countText.text = count.ToString();
    }

    void OnStateChanged(BattleStates value)
    {
        currentState = value;
    }

}