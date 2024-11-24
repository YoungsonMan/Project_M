using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject wallPrefab;       // 고정된 벽
    public List<GameObject> obstaclePrefabs;   // 파괴 가능한 장애물
    public GameObject spawnPointPrefab; // 플레이어 스폰 지점
    public GameObject groundPlane;      // 바닥 Plane
    public Vector3 offset;              // 맵 전체 위치 조정

    private int[,] mapData = {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 3, 0, 2, 0, 2, 0, 2, 0, 2, 0, 3, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 1, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 1, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 1, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 3, 0, 2, 0, 2, 0, 2, 0, 2, 0, 3, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
    };

    private List<Vector3> spawnPoints = new List<Vector3>(); // 스폰 위치 리스트
    private GameObject mapContainer; // 맵의 모든 오브젝트를 담는 컨테이너

    /// <summary>
    /// 플레이어가 스폰되는 위치를 반환하는 메서드.
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetSpawnPoints()
    {
        return spawnPoints;
    }

    /// <summary>
    /// 맵 데이터에 의하여 생성하도록 하는 메서드.
    /// </summary>
    public void GenerateMap()
    {
        ClearMap();
        CreateGroundPlane(); // 바닥 Plane 생성

        // 맵 컨테이너 생성
        mapContainer = new GameObject("MapContainer");
        mapContainer.transform.parent = transform;

        for (int z = 0; z < mapData.GetLength(0); z++)
        {
            // Z축 기준으로 맵 컨테이너에 생성
            GameObject zParent = new GameObject($"Z_{z}");
            zParent.transform.parent = mapContainer.transform;

            for (int x = 0; x < mapData.GetLength(1); x++)
            {
                Vector3 position = new Vector3(x + offset.x, offset.y, -z + offset.z);
                Vector3 spawnPos = new Vector3(x + offset.x, -1f + offset.y, -z + offset.z);
                GameObject tileObject = null;

                switch (mapData[z, x])
                {
                    case 1: // 벽
                        tileObject = Instantiate(wallPrefab, position, Quaternion.identity, zParent.transform);
                        break;
                    case 2: // 파괴 가능한 장애물
                        if (obstaclePrefabs != null && obstaclePrefabs.Count > 0)
                        {
                            GameObject randomObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
                            tileObject = Instantiate(randomObstacle, position, Quaternion.identity, zParent.transform);
                        }
                        break;
                    case 3: // 스폰 지점
                        tileObject = Instantiate(spawnPointPrefab, spawnPos, Quaternion.identity, zParent.transform);
                        spawnPoints.Add(position);
                        break;
                }

                if (tileObject != null)
                {
                    tileObject.name = $"Tile_x({x})_z({z})";
                }
            }
        }
    }

    /// <summary>
    /// 맵 생성시 바닥을 Plane으로 크기를 조정해서 생성하는 메서드.
    /// </summary>
    public void CreateGroundPlane()
    {
        Vector3 planePosition = new Vector3
            (
                (mapData.GetLength(1) - 1) / 2f + offset.x,
                offset.y, 
                -(mapData.GetLength(0) - 1) / 2f + offset.z
            );

        GameObject ground = Instantiate(groundPlane, planePosition, Quaternion.identity, transform);
        ground.name = "GroundPlane";

        ground.transform.localScale = new Vector3(mapData.GetLength(1) / 10f, 1, mapData.GetLength(0) / 10f);
    }

    /// <summary>
    /// 만들어진 맵을 삭제할수 있도록 한 메서드.
    /// </summary>
    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // Destroy는 다음 프레임에서 제거.(Play 모드에서만 동작)
            // DestroyImmediate는 객체를 즉시 제거할 때 사용.(Play 모드와 에디터 모드 둘 다 동작)
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}


#region Test Map Data
/*
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 3, 0, 0, 2, 2, 2, 2, 0, 0, 3, 0, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 0, 0, 2, 2, 0, 2, 0, 0, 2, 2, 0, 1 },
        { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 },
        { 1, 0, 2, 0, 2, 0, 0, 0, 2, 0, 2, 0, 1 },
        { 1, 2, 1, 2, 1, 0, 0, 0, 1, 2, 1, 2, 1 },
        { 1, 2, 2, 2, 2, 0, 0, 0, 2, 2, 2, 2, 1 },
        { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 },
        { 1, 0, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 3, 0, 0, 2, 2, 2, 2, 0, 0, 3, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
 */
#endregion

#region Map01
/*
    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 3, 0, 2, 0, 2, 0, 2, 0, 2, 0, 3, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 1, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 1, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 1, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 1 },
        { 1, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 1 },
        { 1, 3, 0, 2, 0, 2, 0, 2, 0, 2, 0, 3, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }

 */
#endregion