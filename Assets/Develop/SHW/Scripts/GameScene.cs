using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실제 게임 플레이에 필수적으로 포함시켜 줄 게임 씬 스크립트
/// (241126) 현재 플레이어의 스폰만을 담당하고 있으며 추가적인 구현이 필요합니다.
/// </summary>
public class GameScene : MonoBehaviour
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

        // 플레이어 넘버 지정
        playerNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // 약간의 딜레이 후 게임 시작
        StartCoroutine(StartDelayRoutine());
    }

    IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(1f);
        TestGameStart();
    }

    public void TestGameStart()
    {
        Debug.Log("게임 시작");

        // 테스트용 게임 시작 부분
        spawner.PlayerSpawn(playerNum);
    }
}
