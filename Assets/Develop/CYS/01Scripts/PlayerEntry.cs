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
    [Header ("한글")]
    [SerializeField] TMP_FontAsset kFont;
    [Header("텍스트관련")]
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _readyText;
    [SerializeField] TMP_Text _readyPopText;
    [Header("레디버튼관련")]
    [SerializeField] Button _readyButton;
    [SerializeField] GameObject _readyButtonText;
    [SerializeField] GameObject _readyTextBox;

    // 팀관련
    [Header("팀관련")]
    public int TeamNumber; // 여기서 값설정해서 플레이어한테
    [SerializeField] GameObject[] _teamButtons;

    // 캐릭터관련
    [Header("캐릭터 관련")]
    [SerializeField] Texture[] _charTexture;
    [SerializeField] RawImage _charRawImage;
    GameObject _playerImage;
    // public int charNumber;

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
        // 플레이어 이름
        _nameText = GetUI<TMP_Text>("PlayerNameText");
        _nameText.font = kFont;
        
        // 레디버튼 레디 (스타트버튼옆)
        _readyTextBox = GetUI("ReadyTextBox");
        _readyText = GetUI<TMP_Text>("ReadyText");
        _readyText.font = kFont;
            // 레디텍스트박스 밑에 레디버튼 (평상시 흰색글씨에 레디하면 노랑색되기위한구조)++처음에 만들고수정하다보니이렇게됨
            // 구조조정하려다가 망할뻔해서 일단 그냥 두기로함.
            _readyButton = GetUI<Button>("ReadyButton");
            _readyButtonText = GetUI("ReadyButtonText");
             GetUI<TMP_Text>("ReadyButtonText").font = kFont;
            _readyButton.onClick.AddListener(Ready);
        
        // 레디하면 플레이어 위에 나오는 READY텍스트
        _readyPopText = GetUI<TMP_Text>("ReadyPopText");
        _readyPopText.font = kFont;


        // _playerImage = GetUI("PlayerImage");
        // _charRawImage = (RawImage)_playerImage.GetComponent<RawImage>();
        // _charRawImage.texture = _charTexture[charNumber];
    }

    public void SetPlayer(Player player)
    {
        
        if (player.IsMasterClient)
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
        // 캐릭터 갱신
        PhotonNetwork.LocalPlayer.GetCharacter();
        _charRawImage.texture = _charTexture[PhotonNetwork.LocalPlayer.GetCharacter()];

    }
  
    public void SetEmpty()
    {
        _readyPopText.text = "";
        _readyText.text = "";
        _nameText.text = "None";
        _readyButton.gameObject.SetActive(false);
        _readyTextBox.SetActive(false);
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
