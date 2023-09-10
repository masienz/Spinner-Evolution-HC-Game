using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Destroyer : MonoBehaviour
{
    public string destroyTag = "TransformGate";
    public GameObject followTarget;

    [Header("Destroy Animation")]
    public float animDuration = 1;
    public Ease destroyEase;

    private Vector3 followDistance;

    private void Awake()
    {
        followDistance = transform.position - followTarget.transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = followTarget.transform.position + followDistance;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(destroyTag))
		{
            col.transform.DOScale(Vector3.one * 0.01f, animDuration).SetEase(destroyEase);

            Destroy(col.gameObject, animDuration + 1);
        }
    }
}
