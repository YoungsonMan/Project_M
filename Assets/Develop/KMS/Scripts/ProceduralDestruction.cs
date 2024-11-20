using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDestruction : MonoBehaviour
{
    [Header("부서지는 벽 세팅")]
    public int fragmentsCount = 10;         // 부서지는 조각 갯수
    public float explosionForce = 30f;      // 폭발범위
    public float explosionRadius = 5f;      // 폭발반경
    public GameObject fragmentPrefab;       // 부서진 조각
    public Transform parentContainer;       // 부서진 조각들을 담을 곳

    /// <summary>
    /// 원본 오브젝트가 사라지고 부서진 파편이 폭발로 날아갈 메서드.
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
        }

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
}
