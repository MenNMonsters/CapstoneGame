using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public struct MessageStruct
{
    public string name;
    public string message;

    public MessageStruct(string name, string message)
    {
        this.name = name;
        this.message = message;
    }
}

public class PlayerControl : NetworkBehaviour
{
    // Chat Message Stuff Start
    GameObject ChatPanel;
    GameObject ChatContent;
    InputField chatInput;
    Button ChatButton;
    Button ExitButton;
    Button SendButton;
    Scrollbar scrollbar;

    public Texture2D myTexture;

    ChatObjects co;
    GameObject messagePrefab;

    [SyncVar(hook = "OnChatMessageSent")]
    private MessageStruct messageStruct;
    // Chat Message Stuff End

    public GameObject playerBox;

    public Sprite characterImg;

    bool handle = false;

    const int ENEMY_MAGIC_CONSUME = 20;
    const int PLAYER_MAGIC_CONSUME = 20;
    const int PLAYER_HEALTH = 100;
    const int PLAYER_ENERGY = 100;
    const int ENEMY_HEALTH = 100;
    const int ENEMY_ENERGY = 100;

    [SyncVar(hook = "OnPlayerCountChanged")]
    int playerCount = 2;

    Vector3 iniPos = new Vector3(309, 193, 0);


    private bool beingHandled = false;

    bool normalDefeated = false;
    bool bossDefeated = false;

    public enum BattleStates
    {
        START,
        PLAYERCHOICE,
        ENEMYCHOICE,
        LOSE,
        WIN
    }

    private int id;

    private NetworkInstanceId playerNetID;

    [SyncVar]
    public string playerUniqueIdentity;

    [SyncVar]
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

    [SyncVar(hook = "OnCanvasChanged")]
    private bool canvas;

    [SyncVar]
    public int numOfPlayer = 4;

    [SyncVar(hook = "OnPlayerName")]
    public string pName;

    [SyncVar(hook = "OnCharacterName")]
    public string characterName;

    private Text turnText;
    private Text playerHealthText;
    private Text playerEnergyText;
    private Text enemyHealthText;
    private Text enemyHealth2Text;
    private Text enemyEnergyText;
    private Text playerNameText;
    private Text PlayerPortraitText;
    private Text countText;
    private Text playerCountText;
    private Text canvasText;

    private Image characterImage;

    private Image playerHealthImage;
    private Image playerEnergyImage;
    private Image enemyHealthImage;
    private Image enemyEnergyImage;

    private Transform myTransform;

    GameObject p1;
    GameObject p2;
    GameObject p3;
    GameObject p4;
    GameObject enemy;
    GameObject container;
    GameObject normalRoom;
    GameObject bossRoom;

    bool p2Enabled = false;
    bool p3Enabled = false;
    bool p4Enabled = false;
    bool normal = false;
    bool magic = false;
    bool pass = false;

    [SyncVar]
    bool turnChoosed = false;

    private void Awake()
    {
        myTransform = transform;
    }

    // Use this for initialization
    void Start()
    {
        ChatButton = GameObject.Find("ChatButton").GetComponent<Button>();
        co = ChatButton.GetComponent<ChatObjects>();

        ChatPanel = co.ChatPanel;
        ChatContent = co.ChatContent;

        ExitButton = co.ExitButton;
        SendButton = co.SendButton;
        chatInput = co.chatInput;
        scrollbar = co.scrollbar;

        //ChatPanel = GameObject.Find("ChatPanel");
        //ChatContent = GameObject.Find("ChatContent");

        //ExitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        //SendButton = GameObject.Find("SendButton").GetComponent<Button>();
        //chatInput = GameObject.Find("ChatInputField").GetComponent<InputField>();
        //scrollbar = GameObject.Find("Scrollbar Vertical").GetComponent<Scrollbar>();

        chatInput.onEndEdit.RemoveAllListeners();
        chatInput.onEndEdit.AddListener(onEndEditChatInput);

        ChatButton.onClick.RemoveAllListeners();
        ChatButton.onClick.AddListener(() => { onChatButtonClick(); });

        ExitButton.onClick.RemoveAllListeners();
        ExitButton.onClick.AddListener(() => { onExitButtonClick(); });

        SendButton.onClick.RemoveAllListeners();
        SendButton.onClick.AddListener(() => { onSendButtonClick(); });

        ChatPanel.SetActive(false);

        playerBox = GameObject.Find("Player Box");
        normalRoom = GameObject.Find("Normal Door");
        bossRoom = GameObject.Find("Boss Door");

        p1 = GameObject.Find("PlayerInfoContainer1");

        p2 = GameObject.Find("PlayerInfoContainer2");

        p3 = GameObject.Find("PlayerInfoContainer3");

        p4 = GameObject.Find("PlayerInfoContainer4");
        enemy = GameObject.Find("EnemyInfoContainer");
        container = GameObject.Find("Container");


        if (isLocalPlayer)
        {
            if (numOfPlayer < 4)
            {
                p4.SetActive(false);
                if (numOfPlayer < 3)
                {
                    p3.SetActive(false);
                    if (numOfPlayer < 2)
                    {
                        p2.SetActive(false);
                    }
                }
            }

            if (string.IsNullOrEmpty(pName))
            {
                co.localPlayerName = "";
            }
            else
            {
                co.localPlayerName = pName;
            }

            playerNameText = GameObject.Find("Name").GetComponent<Text>();
            if (playerNameText != null)
            {
                playerNameText.text = "You are " + pName;
            }
        }

        id = int.Parse(GetComponent<NetworkIdentity>().netId.ToString());

        PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        PlayerPortraitText.text = pName;

        OnPlayerName(pName);

        //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();

        //if (numOfPlayer == 1)
        //{
        //    PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //    PlayerPortraitText.text = pName;

        //    OnPlayerName(pName);

        //    //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //    //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //    //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //    //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //    //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        //    characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //    playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //    playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //    playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //    playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //    turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();

        //}

        //if (numOfPlayer == 2)
        //{

        //    if (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) == 3)
        //    {
        //        PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        PlayerPortraitText.text = pName;

        //        OnPlayerName(pName);

        //        //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        //        characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //    }
        //    else
        //    {
        //        PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        PlayerPortraitText.text = pName;

        //        OnPlayerName(pName);

        //        //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        //        characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //    }
        //}

        //if (numOfPlayer == 3)
        //{

        //    if (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) == 4)
        //    {
        //        PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        PlayerPortraitText.text = pName;

        //        OnPlayerName(pName);

        //        //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        //        characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //    }
        //    else
        //    {
        //        PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        PlayerPortraitText.text = pName;

        //        OnPlayerName(pName);

        //        //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        //        characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //    }

        //}

        //if (numOfPlayer == 4)
        //{

        //    if (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) == 5)
        //    {
        //        PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        PlayerPortraitText.text = pName;

        //        OnPlayerName(pName);

        //        //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        //        characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //        playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Image>();
        //        turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer)).GetComponent<Text>();
        //    }
        //    else
        //    {
        //        PlayerPortraitText = GameObject.Find("PlayerName" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        PlayerPortraitText.text = pName;

        //        OnPlayerName(pName);

        //        //playerHealthText = GameObject.Find("PlayerHealth" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerEnergyText = GameObject.Find("PlayerEnergy" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();
        //        //playerHealthImage = GameObject.Find("PlayerHealthBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //playerEnergyImage = GameObject.Find("PlayerEnergyBar" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Image>();
        //        //turnText = GameObject.Find("Turn" + int.Parse(GetComponent<NetworkIdentity>().netId.ToString())).GetComponent<Text>();

        //        characterImage = GameObject.Find("PlayerPortrait" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        playerHealthText = GameObject.Find("PlayerHealth" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        playerEnergyText = GameObject.Find("PlayerEnergy" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //        playerHealthImage = GameObject.Find("PlayerHealthBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        playerEnergyImage = GameObject.Find("PlayerEnergyBar" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Image>();
        //        turnText = GameObject.Find("Turn" + (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) - numOfPlayer - 1)).GetComponent<Text>();
        //    }
        //}


        turn = pName + ":Your Turn";
        OnTurnChanged(turn);

        playerHealth = PLAYER_HEALTH;
        playerEnergy = PLAYER_ENERGY;


        playerCountText = GameObject.Find("PlayerCount").GetComponent<Text>();

        countText = GameObject.Find("Count").GetComponent<Text>();
        playerCount = numOfPlayer;
        count = playerCount;
        OnCountChanged(count);
        OnPlayerCountChanged(playerCount);

        canvasText = GameObject.Find("Canvas").GetComponent<Text>();
        canvas = false;
        OnCanvasChanged(canvas);

        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        enemyHealth = ENEMY_HEALTH;
        enemyHealth2Text = GameObject.Find("EnemyHealth2").GetComponent<Text>();
        enemyHealth2 = ENEMY_HEALTH;
        enemyEnergyText = GameObject.Find("EnemyEnergy").GetComponent<Text>();
        enemyEnergy = ENEMY_ENERGY;
        enemyHealthImage = GameObject.Find("EnemyHealthBar").GetComponent<Image>();
        enemyEnergyImage = GameObject.Find("EnemyEnergyBar").GetComponent<Image>();
        currentState = BattleStates.PLAYERCHOICE;

        characterImage.sprite = Resources.Load<Sprite>(characterName) as Sprite;
        if(isServer)
            RpcChangeBoxImage();

        container.GetComponent<CanvasGroup>().alpha = 0;


    }

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }

    [ClientRpc]
    void RpcChangeBoxImage()
    {
        
        myTexture = Resources.Load<Texture2D>(characterName);
        if (playerBox != null)
            playerBox.gameObject.GetComponent<Renderer>().material.mainTexture = myTexture;
    }

    // Update is called once per frame
    void Update()
    {


        //Debug.Log(canvas);

        //if (isServer)
        //{
        //    if (!p3Enabled || !p4Enabled)
        //        RpcOnNumOfPlayerChanged(NetworkServer.connections.Count);
        //}

        //if (p2 != null && !p2Enabled && numOfPlayer == 2)
        //{
        //    p2.SetActive(true);
        //    p2Enabled = true;
        //}

        //if (p3 != null && !p3Enabled && numOfPlayer == 3)
        //{
        //    p3.SetActive(true);
        //    p3Enabled = true;
        //}

        //if (p4 != null && !p4Enabled && numOfPlayer == 4)
        //{
        //    p4.SetActive(true);
        //    p4Enabled = true;
        //}

        playerHealthImage.fillAmount = (float)int.Parse(playerHealthText.text.ToString()) / PLAYER_HEALTH;
        playerEnergyImage.fillAmount = (float)int.Parse(playerEnergyText.text.ToString()) / PLAYER_ENERGY;
        if (!normalDefeated)
        {
            enemyHealthImage.fillAmount = (float)int.Parse(enemyHealthText.text.ToString()) / ENEMY_HEALTH;
            enemyEnergyImage.fillAmount = (float)int.Parse(enemyEnergyText.text.ToString()) / ENEMY_ENERGY;
        }else
        {
            enemyHealthImage.fillAmount = (float)int.Parse(enemyHealthText.text.ToString()) / 400;
            enemyEnergyImage.fillAmount = (float)int.Parse(enemyEnergyText.text.ToString()) / 400;
        }




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

    [ClientRpc]
    void RpcChangeToCombat()
    {
        if (container != null)
            container.GetComponent<CanvasGroup>().alpha = 1;
        if (playerBox != null)
        {
            playerBox.SetActive(false);
        }
        else
        {
            playerBox = GameObject.Find("Player Box");
            playerBox.SetActive(false);
        }
        if (normalRoom != null)
        {
            normalRoom.SetActive(false);
        }
        if (bossRoom != null)
        {
            bossRoom.SetActive(false);
        }

    }

    [ClientRpc]
    void RpcChangeToMove()
    {
        if (container != null)
            container.GetComponent<CanvasGroup>().alpha = 0;
        if (playerBox != null)
        {
            playerBox.SetActive(true);
        }
        if (normalRoom != null)
        {
            normalRoom.SetActive(true);
        }
        else
        {
            playerBox = GameObject.Find("Player Box");
            playerBox.SetActive(false);
        }
        if (bossRoom != null)
        {
            bossRoom.SetActive(true);
        }

        currentState = BattleStates.PLAYERCHOICE;

    }

    [ClientRpc]
    void RpcMoveBox(float x, float y)
    {
        playerBox.transform.Translate(x, y, 0);
    }

    [ClientRpc]
    void RpcChangeToBoos()
    {
        enemyHealth = 400;
        enemyEnergy = 400;
        enemyHealth2 = 400;
        OnEnemyHealth2Changed(enemyHealth2);
        OnEnemyHealthChanged(enemyHealth);
        OnEnemyEnergyChanged(enemyEnergy);
        currentState = BattleStates.PLAYERCHOICE;
        turnChoosed = false;
        if (container != null)
            container.GetComponent<CanvasGroup>().alpha = 1;
        if (playerBox != null)
        {
            playerBox.SetActive(false);
        }
        if (normalRoom != null)
        {
            normalRoom.SetActive(false);
        }
        if (bossRoom != null)
        {
            bossRoom.SetActive(false);
        }

    }

    [Command]
    void CmdPlayerCountChage(int count)
    {
        playerCount = count;
        playerCountText.text = playerCount.ToString();
    }


    void OnGUI()
    {

        //Debug.Log("Canvas Enabled " + canvasEnabled + "handle " + handle);
        //Debug.Log(Collision.collide);
        if (isServer)
        {
            if (isLocalPlayer)
            {
                if (canvasText != null && !bool.Parse(canvasText.text))
                {

                    if (GUI.Button(new Rect(Screen.width * (50f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "Left"))
                    {

                        RpcMoveBox(-0.5f, 0);
                    }
                    if (GUI.Button(new Rect(Screen.width * (70f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "Right"))
                    {

                        RpcMoveBox(0.5f, 0);
                    }
                    if (GUI.Button(new Rect(Screen.width * (60f / 100f), Screen.height * (1f * 0.767f), Screen.width * (0.1f), Screen.height * (0.065f)), "Up"))
                    {

                        RpcMoveBox(0, 0.5f);
                    }
                    if (GUI.Button(new Rect(Screen.width * (60f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "Down"))
                    {

                        RpcMoveBox(0, -0.5f);
                    }

                    if (Collision.normalCollide && !normalDefeated)
                    {
                        RpcChangeToCombat();
                        CmdOnCanvasChanged(true);
                    }

                    if (Collision.bossCollide && !bossDefeated && normalDefeated)
                    {
                        RpcChangeToBoos();
                        CmdOnCanvasChanged(true);
                    }



                }
            }
        }
        if (isLocalPlayer)
        {

            if (canvasText != null && bool.Parse(canvasText.text))
            {

                if (isServer)
                {
                    playerCount = int.Parse(GUI.TextField(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.895f), Screen.width * (0.1f), Screen.height * (0.065f)), playerCount.ToString()));
                }

                playerCount = int.Parse(playerCountText.text);

                if (!turnChoosed)
                {
                    normal = false;
                    magic = false;
                    pass = false;
                    count = playerCount;
                    OnCountChanged(count);
                }

                if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.765f), Screen.width * (0.1f), Screen.height * (0.065f)), "Dodge") && !turnChoosed)
                {

                    if (currentState != BattleStates.LOSE && currentState != BattleStates.WIN && currentState == BattleStates.PLAYERCHOICE)
                    {
                        pass = true;
                        turnChoosed = true;
                        turn = pName + "chose to dodge!";
                        CmdOnTurnChanged(turn);
                        CmdOnCountChanged();
                    }
                }

                if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (0.635f), Screen.width * (0.1f), Screen.height * (0.065f)), "NORMAL") && !turnChoosed)
                {
                    if (currentState == BattleStates.PLAYERCHOICE)
                    {
                        normal = true;
                        turnChoosed = true;
                        CmdOnEnemyHealth2Changed(10);
                        if (enemyHealth <= 0)
                        {
                            enemyHealth = 0;
                        }

                        turn = pName + ":chose normal attack!";
                        CmdOnTurnChanged(turn);
                        CmdOnCountChanged();


                    }
                }

                if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.7f), Screen.width * (0.1f), Screen.height * (0.065f)), "Magical") && !turnChoosed)
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
                            }

                            turn = pName + ":chose magical attack!";
                            CmdOnTurnChanged(turn);
                            CmdOnCountChanged();
                        }
                    }
                }

                if (GUI.Button(new Rect(Screen.width * (85f / 100f), Screen.height * (1f * 0.83f), Screen.width * (0.1f), Screen.height * (0.065f)), "PASS") || int.Parse(countText.text) == 0)
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

        if (int.Parse(enemyHealth2Text.text) <= 0)
        {
            enemyHealth = 0;
            enemyHealthText.text = enemyHealth.ToString();
            if (Collision.normalCollide && !normalDefeated)
            {
                normalDefeated = true;
                Collision.normalCollide = false;
            }

            if (Collision.bossCollide && !bossDefeated)
            {

                bossDefeated = true;
                Collision.bossCollide = false;
            }

            turn = pName + ":Enemy defeated!";
            CmdOnTurnChanged(turn);

            yield return new WaitForSeconds(2);

            if (bossDefeated)
                NetworkManager.singleton.ServerChangeScene("MainMenu");

            if (isServer)
            {
                RpcChangeToMove();
            }

            CmdOnCanvasChanged(false);


        }
        else
        {

            yield return new WaitForSeconds(1);

            turn = pName + ":Enemy Turn";
            CmdOnTurnChanged(turn);
            int State = Random.Range(0, 2);
            int hitChance = 0;
            if (pass)
                hitChance = Random.Range(0, 4);
            else
                hitChance = Random.Range(0, 2);
            yield return new WaitForSeconds(1);

            if (hitChance == 1)
            {
                if (State == 1)
                {
                    if (enemyEnergy >= ENEMY_MAGIC_CONSUME)
                    {
                        turn = pName + ":Eenmy used magical attack";

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
                    turn = pName + ":Enemy used normal attack";

                    CmdOnPlayerHealthChanged(5);

                }
            }

            CmdOnTurnChanged(turn);
            yield return new WaitForSeconds(1);

            if (playerHealth <= 0)
            {
                turn = pName + ":you lose!";
                CmdOnTurnChanged(turn);
                CmdOnPlayerCountChanged();
                yield return new WaitForSeconds(2);
                NetworkManager.singleton.ServerChangeScene("MainMenu");
            }
            else
            {
                currentState = BattleStates.PLAYERCHOICE;
            }
        }

        currentState = BattleStates.PLAYERCHOICE;
        beingHandled = false;
        turn = pName + ":Your Turn";
        CmdOnTurnChanged(turn);
        CmdOnResetCount();
        count = playerCount;
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
        countText.text = count.ToString();
    }

    [Command]
    void CmdOnPlayerCountChanged()
    {
        playerCount = int.Parse(playerCountText.text);
        playerCount -= 1;
        playerCountText.text = playerCount.ToString();
    }

    [Command]
    void CmdOnCanvasChanged(bool value)
    {
        canvas = value;
        canvasText.text = canvas.ToString();
    }

    [Command]
    void CmdOnResetCount()
    {
        count = int.Parse(playerCountText.text);
        countText.text = count.ToString();
    }

    [Command]
    void CmdOnEnemyEnergyChanged(int value)
    {
        enemyEnergy = int.Parse(enemyEnergyText.text);
        enemyEnergy -= value;
        enemyEnergyText.text = enemyEnergy.ToString();
    }

    void OnTurnChanged(string value)
    {
        turn = value;

        if (turnText != null)
            turnText.text = turn;
    }

    void OnPlayerHealthChanged(int value)
    {
        playerHealth = value;

        if (playerHealthText != null)
            playerHealthText.text = playerHealth.ToString();
    }

    void onHandleChanged(bool value)
    {
        handle = value;
    }

    void OnPlayerName(string value)
    {
        pName = value;

        if (PlayerPortraitText != null)
            PlayerPortraitText.text = pName.ToString();
    }

    void OnCharacterName(string value)
    {
        characterName = value;

        if (PlayerPortraitText != null)
            PlayerPortraitText.text = pName.ToString();
    }

    void OnPlayerEnergyChanged(int value)
    {
        playerEnergy = value;

        if (playerEnergyText != null)
            playerEnergyText.text = playerEnergy.ToString();
    }

    void OnEnemyHealthChanged(int value)
    {
        enemyHealth = value;

        if (enemyHealthText != null)
            enemyHealthText.text = enemyHealth.ToString();
    }

    void OnEnemyHealth2Changed(int value)
    {
        enemyHealth2 = value;

        if (enemyHealth2Text != null)
            enemyHealth2Text.text = enemyHealth2.ToString();
    }

    void OnEnemyEnergyChanged(int value)
    {
        enemyEnergy = value;

        if (enemyEnergyText != null)
            enemyEnergyText.text = enemyEnergy.ToString();
    }

    void OnCountChanged(int value)
    {
        count = value;

        if (countText != null)
            countText.text = count.ToString();
    }

    void OnPlayerCountChanged(int value)
    {
        playerCount = value;

        if (playerCountText != null)
            playerCountText.text = playerCount.ToString();
    }

    void OnCanvasChanged(bool value)
    {
        canvas = value;

        if (canvasText != null)
            canvasText.text = canvas.ToString();
    }

    // Chat Functions

    public void onChatButtonClick()
    {
        ChatPanel.SetActive(true);
    }

    public void onExitButtonClick()
    {
        chatInput.text = "";
        ChatPanel.SetActive(false);
    }

    public void onSendButtonClick()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            CmdSendMessage(new MessageStruct(co.localPlayerName, chatInput.text));
        }
    }

    void onEndEditChatInput(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            onSendButtonClick();
        }
    }

    void OnChatMessageSent(MessageStruct message)
    {
        GameObject o = Instantiate(Resources.Load("Message")) as GameObject;
        Debug.Log(message.name);
        Debug.Log(message.message);
        o.GetComponent<Message>().Populate(message.name + ":", message.message);

        o.transform.SetParent(ChatContent.transform);
        chatInput.text = "";
        //scrollbar.value = 0;
    }

    [Command]
    void CmdSendMessage(MessageStruct message)
    {
        messageStruct = message;
    }
}