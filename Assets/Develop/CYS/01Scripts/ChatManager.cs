using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public GameObject _chatContent;
    public TMP_InputField _chatInputField;

    PhotonView _photonView;

    GameObject _chatDisplay;

    string _userName;
    // 이게 결국 로비씬에서 다 이루어질 기능들이니까
    // 그냥 LobbyScene : MonoBehaviourPunCallbacks 에서 작동해도 되지 않으려나

    private void OnEnable()
    {
        
    }
    void Start()
    {
       // PhotonNetwork.ConnectUsingSettings();
        _chatDisplay = _chatContent.transform.GetChild(0).gameObject;
        _photonView = GetComponent<PhotonView>();
        Debug.Log("ChatManager테스트 디버그@Start");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _chatInputField.isFocused == false)
        {
            _chatInputField.ActivateInputField();
        }
    }
    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
      
        int _randomKey = Random.Range(0, 100);
        _userName = "user" + _randomKey;
      
        PhotonNetwork.LocalPlayer.NickName = _userName;
        PhotonNetwork.JoinOrCreateRoom("Room1", options, null);
      
        Debug.Log("OnConnectedToMatser 연결 테스트");
   
        // 이 함수이미 LobbyScene에서 써서 굳이 여기서 안해도 필요하다 싶은거 거기다 이식하기
    }

    // 일단 룸으로 되있는거 같은데 로비로 바꿀수 있으면 바꾸기
    public override void OnJoinedRoom()
    {
        AddChatMessage("connect user : " + PhotonNetwork.LocalPlayer.NickName);

        // 룸 들어가는거도 룸에 만약 들어가서 하게하면..
    }
    public void OnEndEditEvent()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("채팅엔터 테스트");
            string strMessage = _userName + " : " + _chatInputField.text;

            // target 받는이 모두에게 inputField에 적힌대로 
            _photonView.RPC("RPC_Chat", RpcTarget.All, strMessage);
            _chatInputField.text = "";
        }
    }
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
}
