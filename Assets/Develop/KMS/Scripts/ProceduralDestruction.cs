using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDestruction : MonoBehaviour, IExplosionInteractable
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

    /// <summary>
    /// 테스트용 메서드
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            DestroyObject();
        }
    }

    private void Start()
    {
        // Player 태그를 가진 모든 캐릭터의 Collider를 캐싱
        if (playerColliders == null)
        {
            CachePlayerColliders();
        }
    }

    /// <summary>
    /// Player 태그를 가진 모든 Collider를 캐싱
    /// </summary>
    private void CachePlayerColliders()
    {
        playerColliders = new List<Collider>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Collider collider = player.GetComponent<Collider>();
            if (collider != null)
            {
                playerColliders.Add(collider);
            }
        }

        if (playerColliders.Count == 0)
        {
            Debug.LogWarning("Player 태그를 가진 Collider를 찾지 못했습니다.");
        }
        else
        {
            foreach (Collider collider in playerColliders)
                Debug.Log($"Player 태그 : {collider.name}");
        }
    }

    /// <summary>
    /// 원본 오브젝트가 사라지고 부서진 파편이 폭발로 날아갈 메서드.
    /// 네트워크 동작시 해당 부분을 Rpc로 변환해서 동작해야한다.
    /// </summary>
    public void DestroyObject()
    {
        List<GameObject> fragments = CreateFragments();

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

                foreach (Collider playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(fragmentCollider, playerCollider);
                }
            }
        }

        // 아이템 생성 확률에 따라 스폰
        SpawnItem();

        Destroy(gameObject);
    }

    /// <summary>
    /// 부서진 파편 생성 메서드.
    /// </summary>
    private List<GameObject> CreateFragments()
    {
        List<GameObject> fragments = new List<GameObject>();

        for (int i = 0; i < fragmentsCount; i++)
        {
            // 무작위 위치 지정하여 조각생성.
            //      Random.insideUnitSphere = 반경 1을 갖는 구 안의 임의의 지점을 반환합니다.
            Vector3 randomPos = transform.position + Random.insideUnitSphere * 0.5f;
            GameObject fragment = Instantiate(fragmentPrefab, randomPos, Random.rotation); //Rpc로 변경 예정.
            
            // 조각의 크기를 무작위로 조정(다양성 부여)
            float randomScale = Random.Range(0.1f, 0.4f);
            fragment.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // 조각에 물리적 속성 추가
            Rigidbody rb = fragment.AddComponent<Rigidbody>();
            rb.mass = 0.1f;

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
    private void SpawnItem()
    {
        // 랜덤 확률로 아이템 생성
        if (Random.value <= itemSpawnChance)
        {
            // 아이템 프리팹 중 하나를 랜덤 선택
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject item = Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);

            Debug.Log("아이템 생성: " + item.name);
        }
    }

    public bool Interact()
    {
        DestroyObject();
        return false;
    }
}
