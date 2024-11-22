using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WaterBombPlacer : MonoBehaviour
{
    [Header("WaterBomb Object Pool")]
    [Tooltip("WaterBomb prefab to place")]
    [SerializeField] private WaterBomb _waterBombPrefab;
    [Tooltip("Throw an exception if we try to return an exisitng item, already in the pool")]
    [SerializeField] private bool _collectionCheck = true;
    [Tooltip("Initial pool size")]
    [SerializeField] private int _defaultCapacity = 8;
    [Tooltip("Maximum pool size")]
    [SerializeField] private int _maxPoolSize = 8;

    private ObjectPool<WaterBomb> _waterBombPool;
    private PlayerStatus _playerStatus;

    private void Awake()
    {
        _waterBombPool = new ObjectPool<WaterBomb>(CreateWaterBomb,
            OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
            _collectionCheck, _defaultCapacity, _maxPoolSize);
    }

    private void Start()
    {
        _playerStatus = GetComponent<PlayerStatus>();

        for (int i = 0; i < _maxPoolSize; i++)
        {
            CreateWaterBomb();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _waterBombPool != null && _waterBombPool.CountAll == 0)
        {
            // Get waterbomb from pool
            WaterBomb waterBomb = _waterBombPool.Get();
            if (waterBomb == null)
                return;

            waterBomb.Range = (int)_playerStatus.power;
            waterBomb.SetLocation(transform.position);
        }
    }

    #region ObjectPool Callbacks
    private WaterBomb CreateWaterBomb()
    {
        WaterBomb waterBomb = Instantiate(_waterBombPrefab);
        waterBomb.ObjectPool = _waterBombPool;
        return waterBomb;
    }

    private void OnGetFromPool(WaterBomb pooledObject) => pooledObject.gameObject.SetActive(true);
    private void OnReleaseToPool(WaterBomb pooledObject) => pooledObject.gameObject.SetActive(false);
    private void OnDestroyPooledObject(WaterBomb pooledObject) => Destroy(pooledObject.gameObject);
    #endregion
}
