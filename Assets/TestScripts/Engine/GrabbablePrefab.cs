using System.Linq;
using UnityEngine;

namespace NRKernal
{
    public class GrabbablePrefab : MonoBehaviour
    {
        [SerializeField] private Material grabedMaterial = null;
        [SerializeField] private Material defaultMaterial = null;

        private bool canGrab = false;
        private bool alreadyInited = false;
        private bool isGrabbed = false;

        private Transform startingTransform;

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;
        private HandEnum defaultHandEnum = HandEnum.RightHand;

        private MeshRenderer[] childPrefabMeshes = null;

        private NRGrabbableObject nrGrabbableObject = null;

        private void Awake()
        {
            startingTransform = gameObject.transform;
            childPrefabMeshes = gameObject.GetComponentsInChildren<MeshRenderer>();
            childPrefabMeshes = childPrefabMeshes.Where(child => child.name == "default").ToArray();

            RegisterToEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void RegisterToEvents()
        {
            HandTrackingExamplePanel.Instance.canGrab += setGrab;
        }

        private void UnregisterFromEvents()
        {
            HandTrackingExamplePanel.Instance.canGrab -= setGrab;
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
                if (NRInput.Hands.GetHandState(defaultHandEnum).currentGesture != HandGesture.Grab)
                    StopPrefab();

            if (canGrab && !alreadyInited)
                MakeGrabbable();
            else if (!canGrab && alreadyInited)
                DestroyGrabbable();

            ChangeMaterial();
        }

        private void ChangeMaterial()
        {
            Material tempMaterial = defaultMaterial;

            if (canGrab && alreadyInited && NRInput.Hands.GetHandState(defaultHandEnum).currentGesture == HandGesture.Grab && isGrabbed)
                tempMaterial = grabedMaterial;

            foreach (MeshRenderer mesh in childPrefabMeshes)
                mesh.material = tempMaterial;
        }

        private void setGrab(bool set)
        {
            canGrab = set;
        }

        private void MakeGrabbable()
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.drag = 0.05f;
            rb.angularDrag = 0.05f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(0.1f, 0.13f, 0.3f);

            nrGrabbableObject = gameObject.AddComponent<NRGrabbableObject>();

            nrGrabbableObject.OnGrabBegan += setGrabbedTrue;
            nrGrabbableObject.OnGrabEnded += setGrabbedFalse;

            alreadyInited = true;
        }

        private void setGrabbedTrue()
        {
            isGrabbed = true;
        }

        private void setGrabbedFalse()
        {
            isGrabbed = false;
        }

        private void DestroyGrabbable()
        {
            nrGrabbableObject.OnGrabBegan -= setGrabbedTrue;
            nrGrabbableObject.OnGrabEnded -= setGrabbedFalse;

            isGrabbed = false;

            Destroy(gameObject.GetComponent<NRGrabbableObject>());
            Destroy(gameObject.GetComponent<Rigidbody>());
            Destroy(gameObject.GetComponent<Collider>());
           
            alreadyInited = false;

            ChangeMaterial();
        }

        private void StopPrefab()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.Sleep();
                rb.WakeUp();
            }
        }
    }
}