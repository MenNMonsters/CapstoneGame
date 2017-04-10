using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, 
        GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerControl localPlayer = gamePlayer.GetComponent<PlayerControl>();

        localPlayer.numOfPlayer = LobbyManager._playerNumber;

        //localPlayer.pname = lobby.playerName;
        //localPlayer.playerColor = lobby.playerColor;
    }

    void Update()
    {
        //Debug.Log(LobbyManager._playerNumber);
    }
}
