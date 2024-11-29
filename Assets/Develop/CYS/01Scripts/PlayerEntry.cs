using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
//using System.Drawing;
using TMPro;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.EventSystems;

public class PlayerEntry : BaseUI
{
    [SerializeField] TMP_FontAsset kFont;
    // [SerializeField] GameObject _playerImage; ("PlayerImage") 나중에 
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _readyText;
    [SerializeField] Button _readyButton;
    [SerializeField] GameObject _readyButtonText;
    [SerializeField] GameObject _readyTextBox;
    [SerializeField] TMP_Text _readyPopText;

    // 팀관련
    public int TeamNumber; // 여기서 값설정해서 플레이어한테
    [SerializeField] GameObject _teamChoicePanel;
    [SerializeField] GameObject[] _teamButtons;

    private void Update()
    {   // 테스트 끝나면 지우기
        if(Input.GetKeyDown(KeyCode.T))
        {
           
            Debug.Log($"선택하신 팀번호: {PhotonNetwork.LocalPlayer} from PlayerEntryScript");
        }
    }

    private void OnEnable()
    {
        Init();        
    }
    private void Init()
    {
        _nameText = GetUI<TMP_Text>("PlayerNameText");
        _nameText.font = kFont;
        _readyText = GetUI<TMP_Text>("ReadyText");
        _readyText.font = kFont;
        _readyButton = GetUI<Button>("ReadyButton");
        _readyButtonText = GetUI("ReadyButtonText");
        GetUI<TMP_Text>("ReadyButtonText").font = kFont;
        _readyButton.onClick.AddListener(Ready);
        _readyTextBox = GetUI("ReadyTextBox");
        _readyPopText = GetUI<TMP_Text>("ReadyPopText");
        _readyPopText.font = kFont;
        GetUI<TMP_Text>("ReadyButtonText").font = kFont;
      //  _teamChoicePanel = GetUI("TeamChoicePanel");

      //  _teamButtons = GetChildren(_teamChoicePanel);
    }
    public GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }
        return children;
    }


    public void SetPlayer(Player player)
    {
        if(player.IsMasterClient)
        {
            
            _nameText.text = $"방장\n{player.NickName}";
            _nameText.color = new Color(1, .8f, 0, 1);
            // 일단 "MASTER" 글씨, 추후 이미지라던가 의논후 변경
        }
        else
        {
            _nameText.text = player.NickName;
        }


        _readyButton.gameObject.SetActive(true);
        _readyButton.interactable = player == PhotonNetwork.LocalPlayer;
        // 플레이어가 본인이지 확인 -> 레디버튼 player =isLocal 도 가능
        // _readyButton.interactable = player == PhotonNetwork.LocalPlayer;
        // 팀선택패널 본인인지 확인


        // 내버튼만 활성화 / 다른사람꺼는 비활성화
        if (_readyButton.interactable)
        {
            _readyTextBox.SetActive(true);
        }
        else
        {
            _readyTextBox.SetActive(false);
            _readyPopText.text ="";
        }

        if (player.GetReady())
        {
            _readyText.text = "Ready";
            _readyText.color = new Color(1, .8f, 0, 1);
            // _readyButton.transition.;
            _readyPopText.text = "READY";
        }
        else
        {
            _readyText.text = "Ready";
            _readyText.color = Color.white;
            _readyPopText.text = "";
        }
        // 팀관련
      //  GetUI<Button>("Team1").onClick.AddListener(SelectTeam);
      //  GetUI<Button>("Team2").onClick.AddListener(SelectTeam);
      //  GetUI<Button>("Team3").onClick.AddListener(SelectTeam);
      //  GetUI<Button>("Team4").onClick.AddListener(SelectTeam);
      //  GetUI<Button>("Team5").onClick.AddListener(SelectTeam);
      //  GetUI<Button>("Team6").onClick.AddListener(SelectTeam);
      //  GetUI<Button>("Team7").onClick.AddListener(SelectTeam);
      //  GetUI<Button>("Team8").onClick.AddListener(SelectTeam);
        // 레디버튼 장동하는것처럼 팀 작동하게하기
    }
  //  public void SelectTeam()
  //  {
  //      string SelectedTeam = EventSystem.current.currentSelectedGameObject.name;
  //      Debug.Log($"{SelectedTeam} is selected.");
  //
  //      // SelectedTeam 이름이 누른 버튼과 동일하면 그버튼에 맞는 팀 넘버를 부여
  //      switch (SelectedTeam)
  //      {
  //          case "Team1":
  //              TeamNumber = 1;
  //              break;
  //          case "Team2":
  //              TeamNumber = 2;
  //              break;
  //          case "Team3":
  //              TeamNumber = 3;
  //              break;
  //          case "Team4":
  //              TeamNumber = 4;
  //              break;
  //          case "Team5":
  //              TeamNumber = 5;
  //              break;
  //          case "Team6":
  //              TeamNumber = 6;
  //              break;
  //          case "Team7":
  //              TeamNumber = 7;
  //              break;
  //          case "Team8":
  //              TeamNumber = 8;
  //              break;
  //      }
  //      PhotonNetwork.LocalPlayer.SetTeam(TeamNumber);
  //      Debug.Log($"선택하신 팀번호: {PhotonNetwork.LocalPlayer.GetTeam(TeamNumber)}");
  //      // Debug.Log($"선택하신 팀번호: {PhotonNetwork.LocalPlayer.GetTeam(TeamNumber)}");
  //  }

    public void SetEmpty()
    {
        _readyPopText.text = "";
        _readyText.text = "";
        _nameText.text = "None";
        _readyButton.gameObject.SetActive(false);
        
    }



    public void Ready()
    {
        // !레디 -> 레디 || 레디 -> !레디 
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        ready = !ready;

        PhotonNetwork.LocalPlayer.SetReady(ready);
        if (ready)
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
            _readyText.text = "Ready";
            Debug.Log($"준비상태: {ready}");
            _readyButtonText.SetActive(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(false);
            _readyText.text = "";
            _readyButtonText.SetActive(true);
        }
    }

}
