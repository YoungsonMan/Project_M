using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : BaseUI
{
    [SerializeField] Image _roomMap;
    [SerializeField] TMP_Text _roomTitle;
    [SerializeField] TMP_Text _roomCapacity;
    [SerializeField] Button _roomJoinButton;
    // RoomStatus - Availability to join (waiting / started)
    // RoomInfo, protected bool isOpen = true; (IsOpen)


    // 위에 이것도 그냥 '들어갈 수 있는지' 에 관한건데
    // not open 이면 못들어가는데 로비에 룸리스트에는 영향x
    // not open 이면 램덤매치입장 안됨. 시작하면 !open. 으로 처리할 수 있을거 같음

    // RoomSetting - Availability to join (private / public) 
    // RoomInfo, protected bool isVisible (IsVisible) ?? 이거 쓸 수 있을거 같은디
    // IsVisible 이거는, 그냥 lobby리스트에 안나오는거 같음.

    // 그래서 위에 둘다 방에 들어갈 수 있냐/마냐 니까
    // 그냥 IsOpen이면 표시를 다른식으로해서 해도 되지않으려나
    // RoomStatus, waiting => isOpen || started => !isOpen
    // RoomSetting, public => isOpen || private => !isOpen

    private void OnEnable()
    {
        Init();
    }
    public void SetRoomInfo(RoomInfo info)
    {
        _roomTitle.text = info.Name;
        _roomCapacity.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        _roomJoinButton.interactable = info.PlayerCount < info.MaxPlayers;
    }
    private void Init()
    {
        _roomTitle = GetUI<TMP_Text>("RoomTitle");
        _roomCapacity = GetUI<TMP_Text>("RoomCapacity");
        _roomJoinButton = GetUI<Button>("RoomJoinButton");
        _roomJoinButton.onClick.AddListener(JoinRoom);
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(_roomTitle.text);
    }

}
