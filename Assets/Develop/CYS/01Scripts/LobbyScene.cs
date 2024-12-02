using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room } // 메인메뉴에 로비가 같이있어서 음..???

    [SerializeField] LoginPanel _loginPanel;
    // [SerializeField] MainPanel _mainPanel;
    [SerializeField] LobbyPanel _lobbyPanel;
    [SerializeField] RoomPanel _roomPanel;


    // ChatFunction
    public GameObject _chatContent;
    public TMP_InputField _chatInputField;

    PhotonView _photonView;

    GameObject _chatDisplay;

    string _userName;

    TMP_Text _roomChatDisplay;

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
        else if (PhotonNetwork.InLobby)  // 이거 비활성화 해도 될지도.. 레디관련 테스트해봐야함
        {
            SetActivePanel(Panel.Lobby);
        }
        else
        {
            SetActivePanel(Panel.Login);
        }

        // From ChatManager
        // PhotonNetwork.ConnectUsingSettings(); 
        // 위 함수는 connects to a dedicated server that provides rooms, matchmaking, and communication
        // 지금 상황에서는 바로 방으로 연결되버려서 쓸 수 없음.
        _chatDisplay = _chatContent.transform.GetChild(0).gameObject;
        _photonView = GetComponent<PhotonView>();
        Debug.Log("ChatManager테스트 디버그@Start");

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _chatInputField.isFocused == false)
        {
            _chatInputField.ActivateInputField();
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

        _userName = PhotonNetwork.LocalPlayer.NickName;
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

        //Chat 관련 FromChatManager
        AddChatMessage($"{PhotonNetwork.LocalPlayer.NickName} has joined");

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
        
        // ClearChatMessages();
    }
    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장 성공");
        SetActivePanel(Panel.Lobby);
        ClearChatMessages();
    }
    /// <summary>
    /// 벙프로퍼티 업데이트
    /// </summary>
    /// <param name="changedProperty"></param>
    public override void OnRoomPropertiesUpdate(Hashtable changedProperty)
    {
        // 현재 참여한 방의 프로퍼티가 업데이트시 호출됨
        _roomPanel.UpdateRoomProperty(changedProperty);
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


    // 채팅관련
    private void ChatOn()
    {
        if (_chatInputField.text == "" && Input.GetKey(KeyCode.Return))
        {
            _chatInputField.Select();
        }
    }
    public void OnEndEditEvent()
    {
       if (_chatInputField.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("채팅엔터 테스트");
            string strMessage = _userName + " : " + _chatInputField.text;

            // target 받는이 모두에게 inputField에 적힌대로 
            _photonView.RPC("RPC_Chat", RpcTarget.All, strMessage);
            _chatInputField.text = "";
       }
    }
    public void OnEndEditEventButton()
    {
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        Debug.Log("채팅버튼 테스트");
        string strMessage = _userName + " : " + _chatInputField.text;

        // target 받는이 모두에게 inputField에 적힌대로 
        _photonView.RPC("RPC_Chat", RpcTarget.All, strMessage);
        _chatInputField.text = "";
        // }
    }


    // From ChatManager 채팅관련 함수들
    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(_chatDisplay, _chatContent.transform);
        goText.GetComponent<TextMeshProUGUI>().text = message;
        _chatDisplay.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
    [PunRPC]
    void RPC_Chat(string message)
    {
        AddChatMessage(message);
        
    }
    // 본인나갔다오는거로 다 채팅이 지워지면 안되니 RPC를 하면 안될거 같음.
    void ClearChatMessages()
    {
        // 들어올때 한번 클리어하고 게임시작할때도 한번해야됨

        // 뒤에 클론있는애들만 지워야됨
        string objName = "RoomChatDisplay";
        foreach (Transform child in _chatContent.transform)
        {   
            if (child.name != objName)
            {
                Destroy(child.gameObject);
            }
        }
    }


}

