using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby, Room } // 메인메뉴에 로비가 같이있어서 음..???

    [SerializeField] LoginPanel _loginPanel;
    [SerializeField] MainPanel _mainPanel;
    [SerializeField] LobbyPanel _lobbyPanel;
    [SerializeField] RoomPanel _roomPanel;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.InRoom)
        {
            SetActivePanel(Panel.Room);
        }
        else if (PhotonNetwork.InLobby)
        {
            SetActivePanel(Panel.Lobby);
        }
        else if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Menu);
        }
        else
        {
            SetActivePanel(Panel.Login);
        }
    }

    private void SetActivePanel(Panel panel)
    {
        _loginPanel.gameObject.SetActive(panel == Panel.Login);
        _mainPanel.gameObject.SetActive(panel == Panel.Menu);
        _lobbyPanel.gameObject.SetActive(panel == Panel.Room);
        _roomPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("접속에 성공했다! (OnConnectedToMaster)");
        SetActivePanel(Panel.Menu);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"접속이 끊겼다. cause : {cause} \n OnDisconnected");
        SetActivePanel(Panel.Login);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공 \n OnCreatedRoom");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"방 생성 실패, 사유 : {message} \n OnCreateRoomFailed");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공 \n OnJoinedRoom");
        SetActivePanel(Panel.Room);
    }

    // TODO: RoomPanel 구현해야함

}

