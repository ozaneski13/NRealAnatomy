using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PanelStartingAnim : MonoBehaviour
{
    [SerializeField] private float timeToMove = 5f;
    [SerializeField] private Vector3 destinationRot = Vector3.zero;

    private IEnumerator startAnim = null;

    private void Awake()
    {
        startAnim = StartAnim();
        StartCoroutine(startAnim);
    }

    private void OnDestroy()
    {
        StopCoroutine(startAnim);
    }

    private IEnumerator StartAnim()
    {
        gameObject.transform.DORotate(destinationRot, timeToMove);
        
        yield return null;
    }
}