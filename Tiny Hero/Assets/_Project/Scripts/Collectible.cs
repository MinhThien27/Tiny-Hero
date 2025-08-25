using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class Collectible : Entity
{
    [SerializeField] private GameObject collectVFX;
    [SerializeField] private int score = 10;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collectVFX != null)
            {
                Instantiate(collectVFX, transform.position, Quaternion.identity);
            }

            GameManager.Instance?.AddScore(score);

            Destroy(gameObject);
        }
    }
}
