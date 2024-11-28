using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterableObject : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        Transform root = GetRoot(other.transform);
        List<Renderer> renderers = root.GetComponent<RendererCache>().Cache;

        if (renderers != null)
            Conceal(renderers);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform root = GetRoot(other.transform);
        List<Renderer> renderers = root.GetComponent<RendererCache>().Cache;

        if (renderers != null)
            Reveal(renderers);
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
