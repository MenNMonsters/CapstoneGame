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

        LobbyServerList lobbyServerList;
        LobbyManager lobbyManager;
        ulong networkID;

        public void Populate(MatchInfoSnapshot match, LobbyManager lm, Color c, LobbyServerList lobbySL)
        {
            serverInfoText.text = match.name;

            slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString(); ;

            networkID = (ulong) match.networkId;

            lobbyManager = lm;

            lobbyServerList = lobbySL;

            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => { onJoinButtonClick(); });

            GetComponent<Image>().color = c;
        }

        public void onJoinButtonClick()
        {
            lobbyServerList.showPasswordPromptPanel();

            lobbyServerList.submitButton.onClick.RemoveAllListeners();
            lobbyServerList.submitButton.onClick.AddListener(() => { JoinMatch(); });

            lobbyServerList.passwordInput.onEndEdit.RemoveAllListeners();
            lobbyServerList.passwordInput.onEndEdit.AddListener(onEndEditPassword);
        }

        void JoinMatch()
        {
            string password = lobbyServerList.passwordInput.text;
            lobbyServerList.passwordInput.text = "";

            lobbyManager.matchMaker.JoinMatch((NetworkID) networkID, password, "", "", 0, 0, lobbyManager.OnMatchJoined);
            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();
        }

        void onEndEditPassword(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                JoinMatch();
            }
        }
    }
}