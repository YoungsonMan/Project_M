using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class EnterableObject : MonoBehaviourPun
{
    [SerializeField] List<GameObject> _innerObjs;

    private int _waterbombLayer;

    private void Awake()
    {
        _innerObjs = new List<GameObject>();
        _waterbombLayer = LayerMask.NameToLayer("WaterBomb");
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform root = GetRoot(other.transform);
        RendererCache rendererCache = root.gameObject.GetComponent<RendererCache>();
        if (rendererCache == null)
            return;

        _innerObjs.Add(root.gameObject);

        List<Renderer> renderers = rendererCache.Cache;

        if (renderers != null)
            Conceal(renderers);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform root = GetRoot(other.transform);
        RendererCache rendererCache = root.gameObject.GetComponent<RendererCache>();
        if (rendererCache == null)
            return;

        _innerObjs.Remove(root.gameObject);

        List<Renderer> renderers = rendererCache.Cache;

        if (renderers != null)
            Reveal(renderers);
    }

    private void OnDestroy()
    {
        if (_innerObjs.Count == 0)
            return;

        foreach (GameObject obj in _innerObjs)
        {
            List<Renderer> renderers = obj.GetComponent<RendererCache>().Cache;
            if (renderers != null && obj.layer != _waterbombLayer)
                Reveal(renderers);

            IExplosionInteractable interactable = obj.GetComponent<IExplosionInteractable>();
            interactable?.Interact();
        }
    }

    private Transform GetRoot(Transform target)
    {
        Transform curTransform = target;
        while (curTransform.parent != null)
        {
            curTransform = curTransform.parent;
        }

        return curTransform;
    }

    private void Conceal(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    private void Reveal(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }
}
