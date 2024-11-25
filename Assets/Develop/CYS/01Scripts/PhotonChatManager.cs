using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;

    bool isConnected;

    [SerializeField] GameObject _chatPanel;
    string _privateReceiver = "";
    string _currentChat;
    [SerializeField] TMP_InputField _chatInputField;
    [SerializeField] TMP_Text _chatDisplay;

    [SerializeField] string userID;
    private void Start()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userID));
        chatClient.Subscribe(new string[] { "channelA", "channelB" });
        chatClient.PublishMessage("channelA", "So Long, and Thanks for All the Fish!");
        Debug.Log("Connecting");
    }
    private void Update()
    {
        if (isConnected)
        {
            chatClient.Service();
        }
        if (_chatInputField.text != "" && Input.GetKey(KeyCode.Return))
        {
           // SubmitPublicChatOnClick();
           // SubmitPrivateChatOnClick();
        }
    }
    public void OnConnected()
    {
        Debug.Log("연결되었습니다.");
        isConnected = true;
        // SubToChatOnClick();
        // chatClient.Subscribe(new string[] { "RegionChannel"} );
    }
    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnConnected()
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        _chatPanel.SetActive(true);
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    


}
