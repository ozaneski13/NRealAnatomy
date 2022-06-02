using UnityEngine;

namespace NRKernal
{
    public class RotateHuman : MonoBehaviour
    {
        [SerializeField] private ExplodeHuman explodeHuman = null;

        [SerializeField] private GameObject parentPrefab = null;
        [SerializeField] private GameObject childPrefab = null;

        [SerializeField] private float turningRate = 5f;

        private bool canRotate = false;
        private Quaternion startingPos;

        private bool isExploded = false;

        private void SetRotate(bool set) => canRotate = set;

        private GameObject tempObject = null;

        private void Awake()
        {
            tempObject = childPrefab;
            RegisterToEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void Start()
        {
            startingPos = parentPrefab.transform.rotation;
        }

        private void Update()
        {
            if (canRotate)
            {
                tempObject = childPrefab;

                if (isExploded) tempObject = parentPrefab;

                parentPrefab.transform.RotateAround(tempObject.transform.position, Vector3.up, turningRate);
            }
        }

        private void RegisterToEvents()
        {
            AnatomyPanel.Instance.canRotate += SetRotate;

            explodeHuman.exploded += () => isExploded = true;
            explodeHuman.assembled += () => isExploded = false;
        }

        private void UnregisterFromEvents()
        {
            AnatomyPanel.Instance.canRotate -= SetRotate;

            explodeHuman.exploded -= () => isExploded = true;
            explodeHuman.assembled -= () => isExploded = false;
        }
    }
}