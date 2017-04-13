using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyServerEntry : MonoBehaviour
    {
        public Text serverInfoText;
        public Text slotInfo;
        public Button joinButton;
        public GameObject passwordPanel;
        public InputField passwordInput;
        public Button enterButton;

        public void Populate(MatchInfoSnapshot match, LobbyManager lobbyManager, Color c)
        {
            serverInfoText.text = match.name;

            slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString(); ;

            NetworkID networkID = match.networkId;

            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => { ShowPasswordPanel(networkID, lobbyManager); });
            //joinButton.onClick.AddListener(() => { JoinMatch(networkID, lobbyManager); });

            passwordPanel = GameObject.Find("PasswordPanel");
            //passwordPanel.SetActive(false);

            GetComponent<Image>().color = c;
        }

        void ShowPasswordPanel(NetworkID networkID, LobbyManager lobbyManager)
        {
            passwordPanel.SetActive(true);

            enterButton.onClick.RemoveAllListeners();
            enterButton.onClick.AddListener(() => { JoinMatch(networkID, lobbyManager); });
        }

        void JoinMatch(NetworkID networkID, LobbyManager lobbyManager)
        {
            string password = passwordInput.text;
            if (string.IsNullOrEmpty(password))
            {
                password = "";
            }

            Debug.Log("MatchName: " + serverInfoText);
            Debug.Log("Password: " + password);

            lobbyManager.matchMaker.JoinMatch(networkID, password, "", "", 0, 0, lobbyManager.OnMatchJoined);
            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();
        }

        public void CancelButtonOnClick()
        {
            passwordPanel.SetActive(false);
            passwordInput.text = "";
        }
    }
}