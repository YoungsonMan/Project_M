using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpwaner : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpawnPointManager _spawnPointManager;
    public GameObject map;

    // KMS 사용 가능한 스폰 포인트 인덱스
    private List<int> _shuffledIndices = new List<int>();

    private void Start()
    {
        map.SetActive(true);

        // 스폰 포인터 리스트 갱신.
        _spawnPointManager.LoadSpawnPoints();

        // 마스터만 스폰 포인터 리스트 인덱스 섞고 플레이어들에게 인덱스 전달.
        if (PhotonNetwork.IsMasterClient)
        {
            ShuffleSpawnIndices();
            AssignSpawnIndicesToPlayers();
        }

        // 캐릭터 ID 동기화를 기다린 후 스폰
        StartCoroutine(WaitForCharacterIdAndSpawn());
    }

    /// <summary>
    /// 스폰 포인트의 인덱스만 섞기
    /// </summary>
    private void ShuffleSpawnIndices()
    {
        // 맵 변경시 전에 있던 값들 초기화.
        _shuffledIndices.Clear();
        for (int i = 0; i < _spawnPointManager.spawnPoints.Count; i++)
        {
            _shuffledIndices.Add(i);
        }

        // Fisher-Yates 셔플(무작위 배정 알고리즘)
        for (int i = _shuffledIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = _shuffledIndices[i];
            _shuffledIndices[i] = _shuffledIndices[randomIndex];
            _shuffledIndices[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 각 플레이어에게 섞인 스폰 인덱스 할당
    /// </summary>
    private void AssignSpawnIndicesToPlayers()
    {
        var players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length && i < _shuffledIndices.Count; i++)
        {
            players[i].SetSpawnIndex(_shuffledIndices[i]);
        }

        Debug.Log("스폰 인덱스가 모든 플레이어에게 할당되었습니다.");
    }

    private IEnumerator WaitForCharacterIdAndSpawn()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(CustomProperty.CHARACTER))
        {
            Debug.Log("캐릭터 ID를 기다리는 중");
            yield return null;
        }

        Debug.Log($"캐릭터 ID를 찾음: {PhotonNetwork.LocalPlayer.CustomProperties[CustomProperty.CHARACTER]}");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey(CustomProperty.SPAWNPOINT))
        {
            Debug.Log($"스폰 인덱스 갱신됨: {targetPlayer.GetSpawnIndex()}");
            SpawnPlayer();
        }
    }

    /// <summary>
    ///  캐릭터 선택 소환 및 랜덤 스폰.
    /// </summary>
    public void SpawnPlayer()
    {
        int spawnIndex = PhotonNetwork.LocalPlayer.GetSpawnIndex();

        if (spawnIndex < 0 || spawnIndex >= _spawnPointManager.spawnPoints.Count)
        {
            Debug.LogError("잘못된 스폰 인덱스입니다!");
            return;
        }

        Vector3 spawnPoint = _spawnPointManager.spawnPoints[spawnIndex];
        Debug.Log($"스폰 위치: {spawnPoint}");

        // 캐릭터 ID 가져오기
        int characterId = GetCharacterId();

        // 캐릭터 프리팹 이름 설정
        string prefabName = GetCharacterPrefabName(characterId);

        // 캐릭터 생성
        PhotonNetwork.Instantiate(prefabName, spawnPoint, Quaternion.identity);
    }

    private int GetCharacterId()
    {
        // Photon의 Custom Properties에서 캐릭터 ID 가져오기
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomProperty.CHARACTER, out object characterId))
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
