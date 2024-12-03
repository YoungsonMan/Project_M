using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDestruction : MonoBehaviourPun, IExplosionInteractable
{
    [Header("부서지는 벽 세팅")]
    public int fragmentsCount = 10;         // 부서지는 조각 갯수
    public float explosionForce = 30f;      // 폭발범위
    public float explosionRadius = 5f;      // 폭발반경
    public GameObject fragmentPrefab;       // 부서진 조각
    public Transform parentContainer;       // 부서진 조각들을 담을 곳

    private List<Collider> playerColliders; // Player 태그를 가진 모든 캐릭터의 Collider 리스트

    [Header("아이템 스폰 설정")]
    public GameObject[] itemPrefabs;        // 생성될 아이템 프리팹
    public float itemSpawnChance = 0.3f;    // 아이템 생성 확률

    [Header("물줄기 추가 진행 여부")]
    public bool isContinue;

    /// <summary>
    /// 원본 오브젝트가 사라지고 부서진 파편이 폭발로 날아갈 동작을 실행할 RPC메서드.
    /// </summary>
    public void DestroyObject()
    {
        if (!photonView || photonView.ViewID == 0)
        {
            Debug.LogError($"DestroyObject 호출 중 PhotonView가 유효하지 않음. {name} 오브젝트 확인 필요.");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(DestroyObjectRPC), RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    /// 원본 오브젝트가 사라지고 부서진 파편이 폭발로 날아갈 메서드.
    /// 네트워크 동작시 해당 부분을 Rpc로 변환해서 동작해야한다.
    /// </summary>
    [PunRPC]
    private void DestroyObjectRPC(PhotonMessageInfo info)
    {
        if (!this || !gameObject)
        {
            Debug.LogWarning($"DestroyObjectRPC 호출 실패: 오브젝트가 이미 삭제됨 {name}");
            return;
        }

        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        Debug.Log($"DestroyObjectRPC 호출 지연 시간: {lag}초");

        List<GameObject> fragments = CreateFragments(lag);

        foreach(var fragment in fragments)
        {
            // 파편들에게 랜덤 방향으로 폭발력 적용.
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            
            if(rb)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // 모든 Player와의 충돌 무시
            Collider fragmentCollider = fragment.GetComponent<Collider>();
            if (fragmentCollider != null)
            {
                Debug.Log("동작");

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    Collider playerCollider = player.GetComponent<Collider>();
                    if (playerCollider != null)
                    {
                        Physics.IgnoreCollision(fragmentCollider, playerCollider);
                    }
                }
            }
        }

        // 아이템 생성 확률에 따라 스폰
        // 생성 또한 권한 있는 플레이어만 생성하고 다른 플레이어에게 알린다.
        if(PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(SpawnItemRPC), RpcTarget.AllBuffered, info.SentServerTime);


        // 네트워크에서 벽 삭제
        if (PhotonNetwork.IsMasterClient || photonView.IsMine)
            PhotonNetwork.Destroy(gameObject); 
    }

    /// <summary>
    /// 부서진 파편 생성 메서드.
    /// </summary>
    private List<GameObject> CreateFragments(float lag)
    {
        List<GameObject> fragments = new List<GameObject>();

        for (int i = 0; i < fragmentsCount; i++)
        {
            // 무작위 위치 지정하여 조각생성.
            //      Random.insideUnitSphere = 반경 1을 갖는 구 안의 임의의 지점을 반환합니다.
            Vector3 randomPos = transform.position + Random.insideUnitSphere * 0.5f;
            GameObject fragment = Instantiate(fragmentPrefab, randomPos, Random.rotation); //Rpc로 변경 예정.
            
            // 조각의 크기를 무작위로 조정(다양성 부여)
            float randomScale = Random.Range(0.5f, 1f);
            
            if(fragment.name == "Fragment_Wood(Clone)")
                randomScale = Random.Range(0.1f, 0.3f);
            fragment.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // 조각에 물리적 속성 추가
            Rigidbody rb = fragment.AddComponent<Rigidbody>();
            rb.mass = 0.1f;

            // 지연 시간만큼 파편의 위치를 보정
            if (rb)
            {
                Vector3 direction = (randomPos - transform.position).normalized;
                rb.position += direction * lag * explosionForce * Time.deltaTime;
            }

            // 부모 오브젝트 설정
            if (parentContainer)
            {
                fragment.transform.SetParent(parentContainer);
            }

            // 생성된 조각을 리스트로 반환.
            fragments.Add(fragment);
        }

        return fragments;
    }

    /// <summary>
    /// 벽이 부서질시 생성되는 스폰 아이템
    /// </summary>
    /// 
    [PunRPC]
    private void SpawnItemRPC(double sentServerTime)
    {
        float lag = Mathf.Abs((float)(PhotonNetwork.Time - sentServerTime));
        Debug.Log($"SpawnItemRPC 호출 지연 시간: {lag}초");

        // 마스터 클라이언트에서만 실행
        if (!PhotonNetwork.IsMasterClient) return;

        // 랜덤 확률로 아이템 생성
        if (Random.value <= itemSpawnChance)
        {
            // 아이템 프리팹 중 하나를 랜덤 선택.
            // 아이템이 생생될때 방의 오브젝트로 생성하기.
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            Vector3 spawnPosition = transform.position + Vector3.down * lag;
            if (transform.name == "stun_hammer_head_lvl3_LOD1")
                spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);

            GameObject item = PhotonNetwork.InstantiateRoomObject($"Item/{itemPrefabs[randomIndex].name}", spawnPosition, Quaternion.identity);

            if(parentContainer)
            {
                item.transform.SetParent(parentContainer);
            }

            Debug.Log("아이템 생성: " + item.name);
        }
    }

    public bool Interact()
    {
        DestroyObject();
        return isContinue;
    }
}
