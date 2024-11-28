using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererCache : MonoBehaviour
{
    [SerializeField] private List<Renderer> _cache;

    private int waterbombLayer;

    public List<Renderer> Cache { get { return _cache; } }

    private void Awake()
    {
        _cache = new List<Renderer>();
        waterbombLayer = LayerMask.NameToLayer("WaterBomb");
    }

    private void Start()
    {
        CacheRenderers(transform);
    }

    private void CacheRenderers(Transform transform)
    {
        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null && gameObject.activeSelf)
            _cache.Add(renderer);

        // Ignore waterbomb's effect renderer
        if (transform.gameObject.layer == waterbombLayer && _cache.Count == 1)
            return;

        if (transform.childCount == 0)
            return;

        for (int i = 0; i < transform.childCount; i++)
        {
            CacheRenderers(transform.GetChild(i));
        }
    }
}
