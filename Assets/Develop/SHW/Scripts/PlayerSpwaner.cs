using Photon.Pun;
using UnityEngine;

public class PlayerSpwaner : MonoBehaviourPun
{
    private SpawnPointManager spawnPointManager;
    private Vector3 spawnPoint;

    private int charNum;

    public void PlayerSpawn(int num)
    {
        // 플레이어 스폰 포인트 설정
        // 플레이어 아이디를 찾아서 
        // 아이디에 할당되는 번호와 스폰 포인트 일치
        spawnPointManager = GameObject.Find("MapContainer").GetComponent<SpawnPointManager>();
        spawnPoint = spawnPointManager.spawnPoints[num];

        // 리소스의 폴더안쪽에 들었다면 폴더의 주소로 작성 (예 : GameObject/Player)
        // PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);

        // 프로퍼티 설정이 완료되면 주석 해제해서 사용
        // PhotonNetwork.LocalPlayer.GetTeam(out charNum);

        // 스폰 캐릭터 테스트용
        // num 자리에 charNum을 넣어서 캐릭터 할당
        if (num == 0)
        {
            PhotonNetwork.Instantiate("PlayerAdult", spawnPoint, Quaternion.identity);
        }
        else if (num == 2)
        {
            PhotonNetwork.Instantiate("PlayerGirl", spawnPoint, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
        }
    }
}
