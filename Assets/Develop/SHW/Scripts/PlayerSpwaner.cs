using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpwaner : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpawnPointManager _spawnPointManager;
    public GameObject map;

    // KMS 사용 가능한 스폰 포인트 목록
    private List<Vector3> _availableSpawnPoints = new List<Vector3>();

    private void Start()
    {
        map.SetActive(true);
        // 스폰 포인트 로드
        if (_spawnPointManager.spawnPoints.Count == 0)
        {
            _spawnPointManager.LoadSpawnPoints();
        }

        // 사용 가능한 스폰 포인트 초기화
        _availableSpawnPoints = new List<Vector3>(_spawnPointManager.spawnPoints);

        // 캐릭터 ID 동기화를 기다린 후 스폰
        StartCoroutine(WaitForCharacterIdAndSpawn());
    }

    private IEnumerator WaitForCharacterIdAndSpawn()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character"))
        {
            Debug.Log("캐릭터 ID를 기다리는 중");
            yield return null; // 다음 프레임 대기
        }

        Debug.Log($"캐릭터 ID를 찾음: {PhotonNetwork.LocalPlayer.CustomProperties["Character"]}");
        SpawnPlayer();
    }

    /// <summary>
    ///  캐릭터 선택 소환 및 랜덤 스폰.
    /// </summary>
    public void SpawnPlayer()
    {
        // 스폰 위치 설정
        Vector3 spawnPoint = GetRandomSpawnPoint();

        // 캐릭터 ID 가져오기
        int characterId = GetCharacterId();

        // 캐릭터 프리팹 이름 설정
        string prefabName = GetCharacterPrefabName(characterId);

        // 캐릭터 생성
        PhotonNetwork.Instantiate(prefabName, spawnPoint, Quaternion.identity);
    }

    /// <summary>
    /// 스폰 포인터 매니저의 위치를 랜덤하게 배정하는 메서드.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomSpawnPoint()
    {
        // 사용 가능한 스폰 포인트가 없으면 오류 처리
        if (_availableSpawnPoints.Count == 0)
        {
            Debug.LogError("스폰 포인트가 부족합니다.");
            return Vector3.zero;
        }

        // 랜덤한 스폰 포인트 선택
        int randomIndex = Random.Range(0, _availableSpawnPoints.Count);
        Vector3 selectedSpawnPoint = _availableSpawnPoints[randomIndex];

        Debug.Log($"랜덤 위치는 {selectedSpawnPoint}");

        // 선택된 스폰 포인트를 리스트에서 제거하여 중복 방지
        _availableSpawnPoints.RemoveAt(randomIndex);

        return selectedSpawnPoint;
    }

    private int GetCharacterId()
    {
        // Photon의 Custom Properties에서 캐릭터 ID 가져오기
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Character", out object characterId))
        {
            return (int)characterId;
        }

        // 기본값 (0)
        Debug.LogWarning("캐릭터 ID가 설정되지 않았습니다. 기본값(0)을 사용합니다.");
        return 0;
    }

    private string GetCharacterPrefabName(int characterId)
    {
        // 캐릭터 ID에 따라 프리팹 이름 반환
        switch (characterId)
        {
            case 0:
                return "Player";        // Player 프리팹
            case 1:
                return "PlayerAdult";   // PlayerAdult 프리팹
            case 2:
                return "PlayerGirl";    // PlayerGirl 프리팹
            default:
                Debug.LogWarning("잘못된 캐릭터 ID입니다. 기본 캐릭터를 스폰합니다.");
                return "Player"; // 기본 캐릭터 프리팹
        }
    }

    #region 기본 플레이어 스폰
    /*
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

        // 랜덤 스폰
        
        //int randomNum = Random.Range(0, spawnPoints.Length);
        //for (int i = 0; i == spawnPoints.Length; i++)
        //{

        //}

        // 프로퍼티 설정이 완료되면 주석 해제해서 사용
        // PhotonNetwork.LocalPlayer.GetTeam(out charNum);

        // 스폰 캐릭터 테스트용
        // num 자리에 charNum을 넣어서 캐릭터 할당
        if (num == 0)
        {
            PhotonNetwork.Instantiate("PlayerAdult", spawnPoint, Quaternion.identity);
        }
        else if (num == 1)
        {
            PhotonNetwork.Instantiate("PlayerGirl", spawnPoint, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
        }
    }
     */
    #endregion
}
