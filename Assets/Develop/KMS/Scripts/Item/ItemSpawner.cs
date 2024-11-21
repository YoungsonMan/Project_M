using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;    // 아이텝 프리펩 배열
    public Transform[] spawnPoints;     // 아이템 스폰 위치 배열
    public float spawnInterval = 10f;   // 스폰 간격.

    private void Start()
    {
        InvokeRepeating(nameof(SpawnItem), 0, spawnInterval);
    }

    private void SpawnItem()
    {
        // 랜덤한 아이템과 스폰 위치 선택
        int randomItemIndex = Random.Range(0, itemPrefabs.Length);
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);

        // 아이템 생성
        Instantiate(itemPrefabs[randomItemIndex], spawnPoints[randomSpawnIndex].position, Quaternion.identity);
    }
}
