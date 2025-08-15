using DG.Tweening;
using System;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] Vector3 moveTo = Vector3.zero;
    [SerializeField] float moveTime = 1f;
    [SerializeField] Ease ease = Ease.InOutQuad;

    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        MovePlatform();
    }

    private void MovePlatform()
    {
        transform.DOMove(startPosition + moveTo, moveTime)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo); // Loop indefinitely with a Yoyo effect
    }
}
