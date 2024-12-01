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
    // 팀 색상관련
    [SerializeField] private Color[] _teamColors;           // 팀색상 배열
    [SerializeField] private RawImage _teamColorIndicator;  // 선택된 팀색상 표시

    // 캐릭터관련
    [Header("캐릭터 관련")]
    [SerializeField] Texture[] _charTexture;
    [SerializeField] RawImage _charRawImage;
    GameObject _playerImage;
    public int charNumber;
    // Local Player reference
    private Player _player;
    private readonly Color readyColor = new Color(1, 0.8f, 0, 1);
    private readonly Color notReadyColor = Color.white;

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

        charNumber = PhotonNetwork.LocalPlayer.GetCharacter();
        // _playerImage = GetUI("PlayerImage");
        // _charRawImage = (RawImage)_playerImage.GetComponent<RawImage>();
        // _charRawImage.texture = _charTexture[charNumber];
    }

    public void SetPlayer(Player player)
    {
        // 플레이어 가져오기
        _player = player;
        if (player.IsMasterClient)
        {            
            _nameText.text = $"방장\n{player.NickName}";
            _nameText.color = new Color(1, .8f, 0, 1);
            // 일단 "MASTER" 글씨, 추후 이미지라던가 의논후 변경
        }
        else
        {
            _nameText.text = player.NickName;
            // 방장 자리에 있다가 다시 들어가면 색이 변경 안되는 부분 색을 다시 지정하기.
            _nameText.color = notReadyColor;
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
        {
            UpdateReadyState();
        }
        // UpdateCharcter()를 이용해 플레이어 정보 갱신
        {
            UpdateCharacter(player.GetCharacter());
            UpdateTeam(player.GetTeam());
        }
       // if (charNumber != player.GetCharacter())
       // {
       //     UpdateCharacter(player.GetCharacter());
       // }
        
    }
    private void UpdateReadyState()
    {
        if (_player.GetReady())
        {
            _readyText.text = "Ready";
            _readyText.color = readyColor;
            _readyPopText.text = "READY";
        }
        else
        {
            _readyText.text = "Ready";
            _readyText.color = notReadyColor;
            _readyPopText.text = "";
        }
    }
    public void SetEmpty()
    {
        _readyPopText.text = "";
        _readyText.text = "";
        _nameText.text = "None";
        // 비어있는 이름 공간의 색상을 흰색으로 갱신하기
        if(_nameText != null) _nameText.color = Color.white;
        if(_readyButton != null) _readyButton.gameObject.SetActive(false);
        if(_readyTextBox != null) _readyTextBox.SetActive(false);
        // 플레이어 칸이 비어지게 됐으면, 해당 칸의 이미지 삭제하기
        if(_charRawImage != null)
        {
            _charRawImage.texture = null;
            _charRawImage.color = new Color(0, 0, 0, 0);  // 4번째 랏에, 0을 주어서 transparency까지 0으로
        }
        if (_teamColorIndicator != null) _teamColorIndicator.color = Color.clear;
        
    }

    public void Ready()
    {
        // !레디 -> 레디 || 레디 -> !레디 
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        ready = !ready;

        PhotonNetwork.LocalPlayer.SetReady(ready);
        UpdateReadyState();
        if (ready)
        {
            // PhotonNetwork.LocalPlayer.SetReady(true);
            // _readyText.text = "Ready";
            Debug.Log($"준비상태: {ready}");
            _readyButtonText.SetActive(false);
        }
        else
        {
            // PhotonNetwork.LocalPlayer.SetReady(false);
            // _readyText.text = "";
            _readyButtonText.SetActive(true);
        }
    }

    /// <summary>
    /// 캐릭터 선택 정보 업데이트
    /// </summary>
    /// <param name="characterID"></param>
    public void UpdateCharacter(int characterID)
    {
        if (_charRawImage == null || _charTexture == null)
        {
            Debug.LogWarning("캐릭터 이미지가 할당되지 않았습니다.");
            return;
        }
        if (characterID >= 0 && characterID < _charTexture.Length)
        {
            _charRawImage.texture = _charTexture[characterID];
            // 색상 초기화하기 (알파값 1로 설정)
            _charRawImage.color = new Color(1, 1, 1, 1); // 흰색, 불투명
            Debug.Log($"업데이트된 캐릭터 이미지: {_charRawImage.texture.name}, Character ID: {characterID}");
        }
        else 
        {
            Debug.LogWarning($"실패한 characterID: {characterID}, 없데이트가 안됐습니다.");
        }

    }


    /// <summary>
    /// 팀정보 업데이트
    /// </summary>
    /// <param name="teamNumber">팀번호</param>
    public void UpdateTeam(int teamNumber)
    {
        if (teamNumber >= 0 && teamNumber < _teamColors.Length)
        {
            _teamColorIndicator.color = _teamColors[teamNumber];
        }
    }
    public void OnTeamColorSelected(int teamNumber)
    {
        PhotonNetwork.LocalPlayer.SetTeam(teamNumber);
        UpdateTeam(teamNumber);
        Debug.Log($"플레이어 {_player.NickName}, 팀번호: {teamNumber}, 색상: {teamNumber}");
    }
}
