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
        private string password;

        public void Populate(MatchInfoSnapshot match, LobbyManager lobbyManager, Color c)
        {
            serverInfoText.text = match.name;

            slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString(); ;

            NetworkID networkID = match.networkId;

            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => { JoinMatch(networkID, lobbyManager); });

            GetComponent<Image>().color = c;
        }

        void JoinMatch(NetworkID networkID, LobbyManager lobbyManager)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = "";
            }

            lobbyManager.matchMaker.JoinMatch(networkID, password, "", "", 0, 0, lobbyManager.OnMatchJoined);
            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();
        }

        void ShowPasswordGUI()
        {
            password = GUI.PasswordField(new Rect(10, 10, 200, 20), password, "*"[0], 25);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }
        }
    }
}