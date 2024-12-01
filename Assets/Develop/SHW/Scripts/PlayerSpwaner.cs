using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerSpwaner : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpawnPointManager _spawnPointManager;
    private Vector3[] randomPoints;
    private Vector3 spawnPoint;

    private int charNum;        // 캐릭터 넘버

    private void Awake()
    {
        PlayerSpawn(PhotonNetwork.LocalPlayer.ActorNumber - 1);
    }

    public void PlayerSpawn(int num)
    {
        // 플레이어 스폰 포인트 설정
        // 플레이어 아이디를 찾아서 
        // 아이디에 할당되는 번호와 스폰 포인트 일치

        if(_spawnPointManager.spawnPoints.Count == 0)
        {
            _spawnPointManager.LoadSpawnPoints();
        }

        spawnPoint = _spawnPointManager.spawnPoints[num];

        // 프로퍼티 설정이 완료되면 주석 해제해서 사용
        charNum =  PhotonNetwork.LocalPlayer.GetCharacter();

        // 스폰 캐릭터 테스트용
        // num 자리에 charNum을 넣어서 캐릭터 할당
        if (charNum == 0)
        {
            PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
        }
        else if (charNum == 1)
        {
            PhotonNetwork.Instantiate("PlayerAdult", spawnPoint, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("PlayerGirl", spawnPoint, Quaternion.identity);
        }
    }
}
