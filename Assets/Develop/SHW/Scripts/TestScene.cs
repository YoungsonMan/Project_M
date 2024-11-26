using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoom";
    private PlayerSpwaner spawner;
    private int playerNum;

    private void Start()
    {
        spawner = GetComponent<PlayerSpwaner>();

        // 닉네임도 일단 대충 랜덤 부여
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        // 시작하자마자 접속할거니깐
        // PhotonNetwork.ConnectUsingSettings();

        StartCoroutine(StartDelayRoutine());
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    playerNum = newPlayer.ActorNumber;
    //    // spawner.PlayerSpawn(playerNum);
    //}

    // 그냥 서버말고 마스터 서버에 접속시키자
    //public override void OnConnectedToMaster()
    //{
    //    RoomOptions options = new RoomOptions();
    //    options.MaxPlayers = 8;
    //    options.IsVisible = false;

    //    // 룸 이름, 옵션, 로비타입을 적어주자
    //    // 로비는 건너 뛸거니 TypedLobby.Default 로 설정 (null로 두면 오류)
    //    PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    //}

    //public override void OnJoinedRoom()
    //{
    //    // StartCoroutine(StartDelayRoutine());
    //    TestGameStart();
    //}

    // 들어오자마자 시작하면 오류가 있을 수 있으니 네트워크 정리를 해줄 시간 벌어주기
    IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(1f);
        TestGameStart();
    }

    public void TestGameStart()
    {
        Debug.Log("게임 시작");

        // 테스트용 게임 시작 부분
         spawner.PlayerSpawn(PhotonNetwork.LocalPlayer.ActorNumber-1);
    }

}
