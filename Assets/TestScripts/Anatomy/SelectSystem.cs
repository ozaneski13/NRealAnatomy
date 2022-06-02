using UnityEngine;
using UnityEngine.EventSystems;

namespace NRKernal
{
    public class SelectSystem : MonoBehaviour
    {
        [SerializeField] private NRPointerRaycaster nrPointerRaycasterRight = null;
        [SerializeField] private NRPointerRaycaster nrPointerRaycasterLeft = null;

        private NRPointerRaycaster defaultRaycaster = null;

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;
        private HandEnum defaultHandEnum = HandEnum.RightHand;

        private bool canSelect = false;

        private void Awake()
        {
            RegisterToEvents();

            defaultRaycaster = nrPointerRaycasterRight;
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void RegisterToEvents()
        {
            AnatomyPanel.Instance.canSelect += SetSelect;
        }

        private void UnregisterFromEvents()
        {
            AnatomyPanel.Instance.canSelect -= SetSelect;
        }

        private void Update()
        {
            if (NRInput.Hands.GetHandState(rightHandEnum).isTracked)
            {
                defaultRaycaster = nrPointerRaycasterRight;
                defaultHandEnum = rightHandEnum;
            }

            else if (NRInput.Hands.GetHandState(leftHandEnum).isTracked)
            {
                defaultRaycaster = nrPointerRaycasterLeft;
                defaultHandEnum = leftHandEnum;
            }

            else return;

            if (NRInput.Hands.GetHandState(defaultHandEnum).currentGesture == HandGesture.Victory && canSelect)
            {
                RaycastResult rayResult = defaultRaycaster.FirstRaycastResult();

                if (rayResult.isValid && rayResult.gameObject != null)
                    if (rayResult.gameObject.GetComponent<InfoPanel>() != null)
                        rayResult.gameObject.GetComponent<InfoPanel>().ShowInfo();
            }

        }

        private void SetSelect(bool set) => canSelect = set;
    }
}