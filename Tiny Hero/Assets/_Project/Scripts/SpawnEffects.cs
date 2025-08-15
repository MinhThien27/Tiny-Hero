using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpawnEffect : MonoBehaviour
{
    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private float animationDuration = 1.0f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
        if (spawnVFX)
        {
            Instantiate(spawnVFX, transform.position, Quaternion.identity);
        }

        GetComponent<AudioSource>().Play();   
    }
}
