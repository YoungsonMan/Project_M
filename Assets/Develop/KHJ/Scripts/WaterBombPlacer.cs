using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WaterBombPlacer : MonoBehaviourPun
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
    [SerializeField] private int _curBombCount;

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
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space) && _waterBombPool != null && _curBombCount < _playerStatus.bombCount)
            {
                photonView.RPC(nameof(PlaceBomb), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void PlaceBomb(PhotonMessageInfo info)
    {
        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

        // Get waterbomb from pool
        WaterBomb waterBomb = _waterBombPool.Get();
        if (waterBomb == null)
            return;

        waterBomb.Lag = lag;
        if (waterBomb.SetLocation(transform.position))
        {
            waterBomb.Range = (int)_playerStatus.power;
        }
    }

    #region ObjectPool Callbacks
    private WaterBomb CreateWaterBomb()
    {
        _curBombCount++;
        WaterBomb waterBomb = Instantiate(_waterBombPrefab, new Vector3(-50, 0, -50), Quaternion.identity); // setting zone
        waterBomb.ObjectPool = _waterBombPool;
        return waterBomb;
    }

    private void OnGetFromPool(WaterBomb pooledObject)
    {
        _curBombCount++;
        pooledObject.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(WaterBomb pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
        _curBombCount--;
    }

    private void OnDestroyPooledObject(WaterBomb pooledObject) => Destroy(pooledObject.gameObject);
    #endregion
}
