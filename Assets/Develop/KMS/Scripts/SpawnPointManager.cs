using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    private List<Vector3> spawnPoints = new List<Vector3>();


    public List<Vector3> GetSpawnPoints()
    {
        if (spawnPoints.Count == 0)
        {
            // "SpawnPoint" 태그를 가진 오브젝트에서 스폰 위치를 검색
            GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (GameObject spawnObject in spawnObjects)
            {
                Vector3 pos = new Vector3(
                    spawnObject.transform.position.x, 
                    0, 
                    spawnObject.transform.position.z);
                spawnPoints.Add(pos);
            }
        }

        return spawnPoints;
    }
}
