using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room } // 메인메뉴에 로비가 같이있어서 음..???

    [SerializeField] LoginPanel _loginPanel;
    // [SerializeField] MainPanel _mainPanel;
    [SerializeField] LobbyPanel _lobbyPanel;
    [SerializeField] RoomPanel _roomPanel;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.InRoom == true)
        {
            SetActivePanel(Panel.Room);
        }
        else if (PhotonNetwork.IsConnected)
        {
           // PhotonNetwork.InLobby;
            SetActivePanel(Panel.Lobby);
        }
        else if (PhotonNetwork.InLobby)
        {
            SetActivePanel(Panel.Lobby);
        }
        else
        {
            SetActivePanel(Panel.Login);
        }
    }

    private void SetActivePanel(Panel panel)
    {
        _loginPanel.gameObject.SetActive(panel == Panel.Login);
        //_mainPanel.gameObject.SetActive(panel == Panel.Menu);
        _lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
        _roomPanel.gameObject.SetActive(panel == Panel.Room);
        // 껏다켰따하는구조라 같이돌면 서로 돌면서 서로 꺼줌

    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("접속에 성공했다! (OnConnectedToMaster)");
       // SetActivePanel(Panel.Menu);
        SetActivePanel(Panel.Lobby);
        PhotonNetwork.JoinLobby();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"접속이 끊겼다. cause : {cause} \n OnDisconnected");
        SetActivePanel(Panel.Login);
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 성공");
         SetActivePanel(Panel.Lobby);
        // 같이 입장해야되서 일단 이런식으로 되면안됨
    }
   // public override void OnLeftLobby()
   // {
   //     Debug.Log("로비 퇴장 성공");
   //     _lobbyPanel.ClearRoomEntries();
   //     SetActivePanel(Panel.Menu);
   // }
   // 로비가 메인씬이라 딱히 필요가 없음.


    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공 \n OnJoinedRoom");
        SetActivePanel(Panel.Room);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장 성공");
        SetActivePanel(Panel.Lobby);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방의 목록이 변경이 있는 경우 서버에서 보내는 정보들
        // 주의사항
        // 1. 처음 로비 입장 시 : 모든 방 목록을 전달
        // 2. 입장 중 방 목록이 변경되는 경우 : 변경된 방 목록만 전달
        _lobbyPanel.UpdateRoomList(roomList);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _roomPanel.EnterPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _roomPanel.ExitPlayer(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        _roomPanel.UpdatePlayerProperty(targetPlayer, changedProps);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"{newMasterClient.NickName} 플레이어가 방장이 되었습니다.");

        // PhotonNetwork.SetMasterClient(); 방장주는 기능도 있음 (방장만 할 수 있음)
    }
    // TODO: RoomPanel 구현해야함
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공 \n OnCreatedRoom");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"방 생성 실패, 사유 : {message} \n OnCreateRoomFailed");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"방 입장 실패, 사유 : {message}");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"랜덤 매칭 실패, 사유 : {message}");
    }

}

