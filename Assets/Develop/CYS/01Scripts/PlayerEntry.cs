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
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerEntry : BaseUI
{
    [Header("한글")]
    [SerializeField] TMP_FontAsset kFont;
    [Header("텍스트관련")]
    [SerializeField] TMP_Text _nameText;
   // [SerializeField] TMP_Text _readyText;
    [SerializeField] TMP_Text _readyPopText;
    [Header("레디버튼관련")]
  //  [SerializeField] Button _readyButton;
  //  [SerializeField] GameObject _readyButtonText;
  //  [SerializeField] GameObject _readyTextBox;

    // 팀관련
    [Header("팀관련")]
    public int TeamNumber; // 여기서 값설정해서 플레이어한테
    // KMS 색상 관련 배열 및 표시 이미지.
    [SerializeField] public Color[] _teamColors;           // 팀 색상 배열
    [SerializeField] public RawImage _teamColorIndicator;  // 선택된 팀 색상 표시
    

    // 캐릭터관련
    [Header("캐릭터 관련")]
    [SerializeField] Texture[] _charTexture;
    [SerializeField] RawImage _charRawImage;
    GameObject _playerImage;
    // public int charNumber;

    // KMS Local Player의 레퍼런스
    private Player _player;
    private readonly Color readyColor = new Color(1, 0.8f, 0, 1);
    private readonly Color notReadyColor = Color.white;

    private void Update()
    {   // 테스트 끝나면 지우기
        if (Input.GetKeyDown(KeyCode.T))
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
        _nameText.fontSizeMin = 14;
        _nameText.fontSize = 22;
        _nameText.fontSizeMax = 58;
        _teamColorIndicator = GetUI<RawImage>("TeamColorBox");


        // 레디버튼 레디 (스타트버튼옆)
        // _readyTextBox = GetUI("ReadyTextBox");
        // _readyText = GetUI<TMP_Text>("ReadyText");
       // _readyText.font = kFont;
       // _readyText.fontSizeMin = 14;
       // _readyText.fontSize = 22;
       // _readyText.fontSizeMax = 58;
        // 레디텍스트박스 밑에 레디버튼 (평상시 흰색글씨에 레디하면 노랑색되기위한구조)++처음에 만들고수정하다보니이렇게됨
        // 구조조정하려다가 망할뻔해서 일단 그냥 두기로함.
      //  _readyButton = GetUI<Button>("ReadyButton");
      //  _readyButtonText = GetUI("ReadyButtonText");
      //  GetUI<TMP_Text>("ReadyButtonText").font = kFont;
      //  GetUI<TMP_Text>("ReadyButtonText").fontSizeMin = 14;
      //  GetUI<TMP_Text>("ReadyButtonText").fontSize = 36;
      //  GetUI<TMP_Text>("ReadyButtonText").fontSizeMax = 72;
      //  _readyButton.onClick.AddListener(Ready);

        // 레디하면 플레이어 위에 나오는 READY텍스트
        _readyPopText = GetUI<TMP_Text>("ReadyPopText");
        _readyPopText.font = kFont;


        // _playerImage = GetUI("PlayerImage");
        // _charRawImage = (RawImage)_playerImage.GetComponent<RawImage>();
        // _charRawImage.texture = _charTexture[charNumber];
    }

    public void SetPlayer(Player player)
    {
        // KMS 플레이어 가져오기.
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

            // KMS 방장 자리에 있다가 다시 들어가도 색이 변경이 안되었던 부분 수정.
            _nameText.color = Color.white;
        }

        // KMS Ready상태 갱신 메서드. 
        {
            //if (player.GetReady())
            //{
            //    _readyText.text = "Ready";
            //    _readyText.color = new Color(1, .8f, 0, 1);
            //    // _readyButton.transition.;
            //    _readyPopText.text = "READY";
            //}
            //else
            //{
            //    _readyText.text = "Ready";
            //    _readyText.color = Color.white;
            //    _readyPopText.text = "";
            //}

            UpdateReadyState();
        }

        //KMS UpdateCharacter()를 이용하여 플레이어 정보 갱신.
        {
            //// 캐릭터 갱신
            //PhotonNetwork.LocalPlayer.GetCharacter();
            //_charRawImage.texture = _charTexture[PhotonNetwork.LocalPlayer.GetCharacter()];

            UpdateCharacter(player.GetCharacter());
            UpdateTeam(player.GetTeam());
        }
    }

    /// <summary>
    /// KMS 준비 상태 업데이트
    /// </summary>
    public void UpdateReadyState()
    {
        if (_player.GetReady())
        {
           // _readyText.text = "Ready";
           // _readyText.color = readyColor;
            _readyPopText.text = "READY";
        }
        else
        {
           // _readyText.text = "Ready";
           // _readyText.color = notReadyColor;
            _readyPopText.text = "";
        }
    }

    /// <summary>
    /// KMS 캐릭터 선택 정보 업데이트
    /// </summary>
    public void UpdateCharacter(int characterId)
    {
        if (_charRawImage == null || _charTexture == null)
        {
            Debug.LogWarning("캐릭터 이미지가 할당되지 않음");
            return;
        }

        if (characterId >= 0 && characterId < _charTexture.Length)
        {
            _charRawImage.texture = _charTexture[characterId];

            // 색상 초기화 (알파 값을 1로 설정)
             _charRawImage.color = new Color(1, 1, 1, 1); // 흰색, 불투명

            Debug.Log($"업데이트 된 캐릭터 이미지: {_charRawImage.texture.name}, Character ID: {characterId}");
        }
        else
        {
            Debug.LogWarning($"실패한 character ID: {characterId}. 업데이트가 안됨.");
        }
    }

    /// <summary>
    /// KMS 팀 정보 업데이트
    /// </summary>
    /// <param name="teamNumber">팀 번호</param>
    public void UpdateTeam(int teamNumber)
    {
        if (teamNumber >= 0 && teamNumber < _teamColors.Length)
        {
            _teamColorIndicator.color = _teamColors[teamNumber];
        }
    }

    public void SetEmpty()
    {
        _readyPopText.text = "";
      //  _readyText.text = "";
        _nameText.text = "None";
        // KMS 비어있는 이름 공간의 색상을 흰색으로 갱신.
        if (_nameText != null) _nameText.color = Color.white;
       // if (_readyButton != null) _readyButton.gameObject.SetActive(false);
       // if (_readyTextBox != null) _readyTextBox.SetActive(false);

        // KMS 플레이어 칸이 비어지게 되었을시 해당 칸의 이미지 삭제.
        if (_charRawImage != null)
        {
            _charRawImage.texture = null;
            _charRawImage.color = new Color(0, 0, 0, 0); // 완전히 투명
        }
        if (_teamColorIndicator != null) _teamColorIndicator.color = new Color(0, .61f, 1f, 1f);
    }

    public void Ready()
    {
        // !레디 -> 레디 || 레디 -> !레디 
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        ready = !ready;

        PhotonNetwork.LocalPlayer.SetReady(ready);
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        // KMS 레디 부분갱신.
        {
            UpdateReadyState();

            if (ready)
            {
                //PhotonNetwork.LocalPlayer.SetReady(true);
                //_readyText.text = "Ready";
                Debug.Log($"준비상태: {ready}");
             //   _readyButtonText.SetActive(false);
            }
            else
            {
                //PhotonNetwork.LocalPlayer.SetReady(false);
                //_readyText.text = "";
              //  _readyButtonText.SetActive(true);
            }
        }
    }


    /// <summary>
    /// KMS 팀 색상 선택 처리 버튼(색상선택 버튼에서 호출 해주세요.)
    /// </summary>
    /// <param name="teamNumber">선택한 팀 번호</param>
    private void OnTeamColorSelected(int teamNumber)
    {
        PhotonNetwork.LocalPlayer.SetTeam(teamNumber);

        UpdateTeam(teamNumber);

        Debug.Log($"플레이어 {_player.NickName} 팀 번호: {teamNumber}, 색상: {_teamColors[teamNumber]}");
    }
}
