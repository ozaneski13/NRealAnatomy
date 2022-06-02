using UnityEngine;

public class PositionHolder : MonoBehaviour
{
    [SerializeField] private Vector3 fragmentationPos = Vector3.zero;
    public Vector3 FragmentationPos => fragmentationPos;

    [SerializeField] private Vector3 fragmentationRot = Vector3.zero;
    public Vector3 FragmentationRot => fragmentationRot;

    [SerializeField] private Vector3 startingPos = Vector3.zero;
    public Vector3 StartingPos => startingPos;

    [SerializeField] private Vector3 startingRot = Vector3.zero;
    public Vector3 StartingRot => startingRot;
}