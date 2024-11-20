using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Player : MonoBehaviour
{
    [SerializeField] WaterBomb waterBombPrefab;

    private Vector3 _offset;

    private void Start()
    {
        _offset = new Vector3(0, 0.5f, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(waterBombPrefab, transform.position + _offset, Quaternion.identity);
        }
    }
}
