using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Chat;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    SoundManager soundManager = SoundManager.Instance;

    private ChatClient _chatClient;
    private string _userName;
    private string _currentChannelName;

    public TMP_InputField inputFieldChat;   // ChatInputField 0 
    public TMP_Text currentChannelText;     // ChatDisplay
    public TMP_Text outputText;             // ChatDisplay


    private bool _isTyping = false;
    public int InputSelected;

    // Use this for initialization
    private void OnEnable()
    {
        Application.runInBackground = true;

        // 닉네임설정
        _userName = PhotonNetwork.LocalPlayer.NickName;
        _currentChannelName = "Channel 001";

        ClearChatMessage();

        _chatClient = new ChatClient(this);

        // true 가 아닌 경우 어플이 백그라운드로 갈 때 연결 끊김
        _chatClient.UseBackgroundWorkerForSending = true;
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(_userName));
        AddLine(string.Format("Try CONNECTION", _userName));
    }
    private void OnDisable()
    {
        ClearChatMessage();
        _chatClient.Disconnect();

    }
    // 포톤 공식 홈페이지에도 기술되어 있는 내용
    // chatClient.Service() 를 Update 에서 호출하던지
    // 필요에 따라 chatClient.Service() 를 반드시 호출 해야한다
    private void Update()
    {
        ChatOn();
        TabInputField();
        OnEnterSend();
        _chatClient.Service();

       // if (Input.GetKeyDown(KeyCode.P))
       // {
       //     Debug.Log("P버튼누르면 나오는 로그");
       //     ClearChatMessage();
       // }
    }

    /// <summary>
    /// TabInputField
    /// Int 변수로 InputField 하나씩지정해서 탭키 누르면 ++ 되고
    /// 최대 수치를 넘어가면 처음으로 돌아가도록
    /// </summary>
    public void TabInputField()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InputSelected++;
            if (InputSelected > 0)
                InputSelected = 0;
            SelectInputField();
        }
    }
    /// <summary>
    /// 마우스로 클릭해서 하면 정해주는
    /// </summary>
    public void SelectInputField()
    {
        switch (InputSelected)
        {
            case 0:
                inputFieldChat.Select();
                break;
        }
    }
    public void ChatSelected() => InputSelected = 0;



    #region NoNeedToShowAlltheTime
    // 현재 채팅 상태를 출력해줄 UI.Text
    public void AddLine(string lineString)
    {
        outputText.text += lineString + "\r\n";
    }

    // 어플리케이션이 종료되었을 때 호출
    public void OnApplicationQuit()
    {
        if (_chatClient != null)
        {
            _chatClient.Disconnect();
        }
    }

    // DebugLevel 에 정의 된 enum 타입에 따라 메세지를 출력한다
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    // 서버에 연결을 성공함
    public void OnConnected()
    {
        AddLine("Connected to Server (ChatManager).");

        // 지정한 채널명으로 접속
        _chatClient.Subscribe(new string[] { _currentChannelName }, 0);
        // (이름, 이전기록을 최대 ___개 까지  ) 뒤에 숫자 
        // (이름, 10) 기록된거 10개를 보여줌, 0이면 0개
    }

    // 서버와의 연결이 끊어짐
    public void OnDisconnected()
    {
        AddLine("Disconnected from server (ChatManager).");

    }

    // 현재 클라이언트의 상태를 출력
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        AddLine(string.Format("Enter Channel ({0})", string.Join(",", channels)));
    }

    public void OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("Exit Channel ({0})", string.Join(",", channels)));
    }

    // Update() 의 chatClient.Service() 가
    // 매 호출 시 OnGetMessages 를 호출한다.
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(_currentChannelName))
        {
            // update text
            this.ShowChannel(_currentChannelName);
        }
    }
    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = this._chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        this._currentChannelName = channelName;
        // 보여질 TMP_TEXT (display? panel? 만들어둔 UI)에 연결
        // 채널에 저장 된 모든 채팅 메세지를 불러온다.
        // 유저 이름과 채팅 내용이 한꺼번에 불러와진다.
        this.currentChannelText.text = channel.ToStringMessages();
        Debug.Log("ShowChannel: " + _currentChannelName);
    }
    public void ClearChatMessage()
    {
        currentChannelText.text = "";
        // 그냥 보여주는 창을 한번 비움

        /*
         * 위에 ClearChatMessage(string channelName)
        //  ChatChannel channel = new ChatChannel(_currentChannelName);
        //  ChatChannel channel = null;
        //  this._currentChannelName = channel.Name;

      //  if(string.IsNullOrEmpty(channelName))
      //  {
      //      return;
      //  }
      //
      //  ChatChannel channel = null;
      //  bool found = this._chatClient.TryGetChannel(channelName, out channel);
      //  if (!found)
      //  {
      //      Debug.Log("ShowChannel failed to find channel: " + channelName);
      //      return;
      //  }
      //
      //  this._currentChannelName = channelName;
      //
      //  channel.ClearMessages();
        */

    }



    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    // 귓속말 메소드
    // TODO 나중에 하게된다면 해보기
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);
    }

    /// <summary>
    /// 인스펙터의 InputField 에서 입력받은 메세지를 보낼 때 사용
    /// </summary>
    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Debug.Log("엔터치면채팅나가기");
            this.SendChatMessage(this.inputFieldChat.text);
            this.inputFieldChat.text = "";
            
        }
    }
    /// <summary>
    /// 위에 함수 버튼용
    /// </summary>
    public void OnEnterSendButton()
    {
        Debug.Log("엔터치면채팅나가기");
        this.SendChatMessage(this.inputFieldChat.text);
        this.inputFieldChat.text = "";
        // soundManager.PlaySFX(SoundManager.E_SFX.BOMB_EXPLOSION);
    }
    

    // 입력한 채팅을 서버로 전송한다.
    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }
        this._chatClient.PublishMessage(_currentChannelName, inputLine);
    }

    // 채팅입력창에 아무것도 없이 엔터 눌리면 채팅입력 ON.
    private void ChatOn()
    {
        if (inputFieldChat.text == "" && Input.GetKey(KeyCode.Return))
        {
            // soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
            inputFieldChat.Select();
        }
    }

}
