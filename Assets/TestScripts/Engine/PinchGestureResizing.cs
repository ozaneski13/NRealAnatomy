using System.Collections;
using UnityEngine;

namespace NRKernal
{
    public class PinchGestureResizing : MonoBehaviour
    {
        [SerializeField] private GameObject resizablePrefab = null;

        [SerializeField] private Vector3 minScale = new Vector3(1, 1, 1);
        [SerializeField] private Vector3 maxScale = new Vector3(5, 5, 5);

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;
        private HandEnum defaultHandEnum;

        private float newMagni = 0f;
        private float oldMagni = 0f;

        private IEnumerator resizeToDefault = null;

        private Vector3 startingPos;

        private bool canResize = false;

        private void SetResize(bool set)
        {
            canResize = set;
        }

        private void Awake()
        {
            RegisterToEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void RegisterToEvents()
        {
            HandTrackingExamplePanel.Instance.canResize += SetResize;
        }

        private void UnregisterFromEvents()
        {
            HandTrackingExamplePanel.Instance.canResize -= SetResize;
        }

        private void Start()
        {
            startingPos = resizablePrefab.transform.position;
        }

        private void Update()
        {
            if (NRInput.Hands.GetHandState(rightHandEnum).isTracked)
                defaultHandEnum = rightHandEnum;
            else if (NRInput.Hands.GetHandState(leftHandEnum).isTracked)
                defaultHandEnum = leftHandEnum;
            else
                return;

            if (canResize)
                ResizeGO(NRInput.Hands.GetHandState(defaultHandEnum));
        }

        private void ResizeGO(HandState handState)
        {
            if (handState.currentGesture == HandGesture.Grab)
                return;

            if (handState.isTracked)
            {
                if (resizeToDefault != null)
                {
                    StopCoroutine(resizeToDefault);
                    resizeToDefault = null;
                }

                oldMagni = newMagni;

                newMagni = HandStateUtility.GetPinchStrength(handState);


                if (Mathf.Abs(oldMagni - newMagni) < 0.002f)
                    return;

                if (newMagni > oldMagni && resizablePrefab.transform.localScale.magnitude < maxScale.magnitude)
                    resizablePrefab.transform.localScale =
                        Vector3.Lerp(resizablePrefab.transform.localScale, resizablePrefab.transform.localScale * 2, Time.deltaTime * 2);

                else if (newMagni < oldMagni && resizablePrefab.transform.localScale.magnitude > minScale.magnitude)
                    resizablePrefab.transform.localScale =
                        Vector3.Lerp(resizablePrefab.transform.localScale, resizablePrefab.transform.localScale / 2, Time.deltaTime * 3);

                if (resizablePrefab.transform.localScale.magnitude > maxScale.magnitude)
                    resizablePrefab.transform.localScale = maxScale;
            }

            else
            {
                if (resizeToDefault == null)
                {
                    resizeToDefault = ResizeToDefault();
                    StartCoroutine(resizeToDefault);
                }

                newMagni = 0f;
                oldMagni = 0f;

                return;
            }
        }

        private IEnumerator ResizeToDefault()
        {
            yield return new WaitForSeconds(5f);

            while (resizablePrefab.transform.localScale.magnitude > minScale.magnitude)
            {
                resizablePrefab.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSeconds(0.1f);
            }

            resizeToDefault = null;
        }
    }
}