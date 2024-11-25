using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpwaner : MonoBehaviourPun
{
    private SpawnPointManager spawnPointManager;
    private List<Vector3> spawnPoints = new List<Vector3>();
    private Vector3 spawnPoint;

    public void PlayerSpawn(int num)
    {
        // 플레이어 스폰 포인트 설정
        // 플레이어 아이디를 찾아서 
        // 아이디에 할당되는 번호와 스폰 포인트 일치
        spawnPointManager = GameObject.Find("MapContainer").GetComponent<SpawnPointManager>();
        spawnPoints = spawnPointManager.spawnPoints;
        // int num = photonView.Owner.GetPlayerNumber();
        //Debug.Log($"{photonView.ViewID}");
        Debug.Log($"{num}");
         spawnPoint = spawnPoints[num];
        

        // 리소스의 폴더안쪽에 들었다면 폴더의 주소로 작성 (예 : GameObject/Player)
        PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }
}
