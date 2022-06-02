using UnityEngine;

namespace NRKernal
{
    public class RotatePrefab : MonoBehaviour
    {
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private float turningRate = 5f;

        private bool canRotate = false;
        private Quaternion startingPos;

        private void SetRotate(bool set)
        {
            canRotate = set;
        }

        private void Awake()
        {
            RegisterToEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void Start()
        {
            startingPos = prefab.transform.rotation;
        }

        void Update()
        {
            if (canRotate)
                prefab.transform.RotateAround(prefab.transform.position, Vector3.up, turningRate);
        }

        private void RegisterToEvents()
        {
            HandTrackingExamplePanel.Instance.canRotate += SetRotate;
        }

        private void UnregisterFromEvents()
        {
            HandTrackingExamplePanel.Instance.canRotate -= SetRotate;
        }
    }
}