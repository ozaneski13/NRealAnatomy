using UnityEngine;

namespace NRKernal
{
    public class GrabbableHuman : MonoBehaviour
    {
        [SerializeField] private Vector3 collidierSize = Vector3.zero;
        [SerializeField] private Vector3 colliderCenter = Vector3.zero;

        [SerializeField] private GameObject mainGameObject = null;

        private bool canGrab = false;
        private bool alreadyInited = false;

        private Transform startingTransform;

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;
        private HandEnum defaultHandEnum = HandEnum.RightHand;

        private NRGrabbableObject nrGrabbableObject = null;

        private void Awake()
        {
            startingTransform = mainGameObject.transform;

            RegisterToEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void RegisterToEvents()
        {
            AnatomyPanel.Instance.canGrab += setGrab;
        }

        private void UnregisterFromEvents()
        {
            if (AnatomyPanel.Instance != null)
                AnatomyPanel.Instance.canGrab -= setGrab;
        }

        void Update()
        {
            if (NRInput.Hands.GetHandState(rightHandEnum).isTracked)
                defaultHandEnum = rightHandEnum;
            else if (NRInput.Hands.GetHandState(leftHandEnum).isTracked)
                defaultHandEnum = leftHandEnum;
            else
                return;

            if (alreadyInited)
                if (NRInput.Hands.GetHandState(defaultHandEnum).currentGesture != HandGesture.Grab ||
                    !NRInput.Hands.GetHandState(leftHandEnum).isTracked ||
                    !NRInput.Hands.GetHandState(rightHandEnum).isTracked)
                    StopPrefab();

            if (canGrab && !alreadyInited)
                MakeGrabbable();
            else if (!canGrab && alreadyInited)
                DestroyGrabbable();
        }

        private void setGrab(bool set) => canGrab = set;

        private void MakeGrabbable()
        {
            Rigidbody rb = mainGameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.drag = 0.05f;
            rb.angularDrag = 0.05f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            float xSize = mainGameObject.transform.localScale.x / (mainGameObject.transform.localScale.x / collidierSize.x);
            float ySize = mainGameObject.transform.localScale.y / (mainGameObject.transform.localScale.y / collidierSize.y);
            float zSize = mainGameObject.transform.localScale.z / (mainGameObject.transform.localScale.z / collidierSize.z);
            BoxCollider col = mainGameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(xSize, ySize, zSize);
            col.center = colliderCenter;

            nrGrabbableObject = mainGameObject.AddComponent<NRGrabbableObject>();

            alreadyInited = true;
        }

        private void DestroyGrabbable()
        {
            Destroy(mainGameObject.GetComponent<NRGrabbableObject>());
            Destroy(mainGameObject.GetComponent<Rigidbody>());
            Destroy(mainGameObject.GetComponent<Collider>());
           
            alreadyInited = false;
        }

        private void StopPrefab()
        {
            Rigidbody rb = mainGameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.Sleep();
                rb.WakeUp();
            }
        }
    }
}