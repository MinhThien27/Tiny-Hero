using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    [SerializeField] float fadeDuration = 0.5f;
    MeshRenderer[] renderers;
    Material[] materials;
    float startTime;

    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        materials = new Material[renderers.Length];

        for(int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
        }

        startTime = Time.time;
    }

    private void Update()
    {
        float t = (Time.time - startTime) / fadeDuration;
        Color color;

        for(int i = 0; i < materials.Length; i++)
        {
            color = materials[i].color;
            color.a = Mathf.Lerp(0.5f, 0f, t);
            materials[i].color = color;
        }
    }
}
