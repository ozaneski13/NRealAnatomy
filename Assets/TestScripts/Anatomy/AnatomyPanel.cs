using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal
{
    public class AnatomyPanel : MonoBehaviour
    {
        private static AnatomyPanel _instance;
        public static AnatomyPanel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<AnatomyPanel>();

                return _instance;
            }
        }

        [SerializeField] private GameObject parentPrefab = null;
        [SerializeField] private GameObject childPrefab = null;

        [SerializeField] private SwitchToggle pinchSwitch = null;
        [SerializeField] private SwitchToggle rotateSwitch = null;
        [SerializeField] private SwitchToggle grabSwitch = null;
        [SerializeField] private SwitchToggle selectSwitch = null;

        private Toggle pinchToggle = null;
        private Toggle rotateToggle = null;
        private Toggle grabToggle = null;
        private Toggle selectToggle = null;

        [SerializeField] private Vector3 parentInitialPos = Vector3.zero;
        [SerializeField] private Vector3 childInitialPos = Vector3.zero;

        public Action<bool> canRotate;
        public Action<bool> canGrab;
        public Action<bool> canResize;
        public Action<bool> canSelect;

        public Action assembleImmediate;
        public Action explodeImmediate;

        public Action disableInfos;

        private void Awake()
        {
            RegisterToSwitches();
            InitializeToggles();
        }

        private void OnDestroy()
        {
            UnregisterFromSwitches();
        }

        private void InitializeToggles()
        {
            pinchToggle = pinchSwitch.gameObject.GetComponent<Toggle>();
            rotateToggle = rotateSwitch.gameObject.GetComponent<Toggle>();
            grabToggle = grabSwitch.gameObject.GetComponent<Toggle>();
            selectToggle = selectSwitch.gameObject.GetComponent<Toggle>();
        }

        private void RegisterToSwitches()
        {
            pinchSwitch.OnToggleChanged += CheckToggles;
            rotateSwitch.OnToggleChanged += CheckToggles;
            grabSwitch.OnToggleChanged += CheckToggles;
            selectSwitch.OnToggleChanged += CheckToggles;
        }

        private void UnregisterFromSwitches()
        {
            pinchSwitch.OnToggleChanged -= CheckToggles;
            rotateSwitch.OnToggleChanged -= CheckToggles;
            grabSwitch.OnToggleChanged -= CheckToggles;
            selectSwitch.OnToggleChanged -= CheckToggles;
        }

        public void AutoToggle(Toggle tg)
        {
            CheckToggles(tg);
        }

        private void CheckToggles(Toggle toggle)
        {
            if (toggle == pinchToggle)
            {
                if (pinchToggle.isOn)
                {
                    rotateToggle.isOn = false;
                    grabToggle.isOn = false;
                    selectToggle.isOn = false;

                    canSelect?.Invoke(false);
                    canRotate?.Invoke(false);
                    canGrab?.Invoke(false);
                    canResize?.Invoke(true);
                }

                else
                    canResize?.Invoke(false);
            }

            if (toggle == rotateToggle)
            {
                if (rotateToggle.isOn)
                {
                    pinchToggle.isOn = false;
                    grabToggle.isOn = false;

                    canResize?.Invoke(false);
                    canGrab?.Invoke(false);
                    canRotate?.Invoke(true);
                }

                else
                    canRotate?.Invoke(false);
            }

            if (toggle == grabToggle)
            {
                if (grabToggle.isOn)
                {
                    rotateToggle.isOn = false;
                    pinchToggle.isOn = false;
                    selectToggle.isOn = false;

                    canSelect?.Invoke(false);
                    canResize?.Invoke(false);
                    canRotate?.Invoke(false);
                    canGrab?.Invoke(true);
                }

                else
                    canGrab?.Invoke(false);
            }

            if (toggle == selectToggle)
            {
                if (selectToggle.isOn)
                {
                    rotateToggle.isOn = false;
                    pinchToggle.isOn = false;
                    grabToggle.isOn = false;

                    canResize?.Invoke(false);
                    canRotate?.Invoke(false);
                    canGrab?.Invoke(false);
                    canSelect?.Invoke(true);

                    explodeImmediate?.Invoke();
                }

                else
                {
                    canSelect?.Invoke(false);
                    disableInfos?.Invoke();
                }
            }
        }

        public void ResetPrefab()
        {
            canSelect?.Invoke(false);
            selectToggle.isOn = false;

            canResize?.Invoke(false);
            pinchToggle.isOn = false;

            canRotate?.Invoke(false);
            rotateToggle.isOn = false;

            canGrab?.Invoke(false);
            grabToggle.isOn = false;

            assembleImmediate?.Invoke();

            Rigidbody rb = parentPrefab.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.Sleep();
                rb.WakeUp();
            }

            parentPrefab.transform.position = parentInitialPos;
            childPrefab.transform.localPosition = childInitialPos;
            childPrefab.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            parentPrefab.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            PositionHolder[] ph = parentPrefab.GetComponentsInChildren<PositionHolder>();

            foreach(PositionHolder phs in ph)
            {
                phs.transform.localPosition = phs.StartingPos;

                if (phs.gameObject.name == "Skin")
                    phs.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                else
                    phs.transform.localRotation = Quaternion.Euler(new Vector3(phs.transform.localRotation.x, 0, phs.transform.localRotation.z));//Quaternion.Euler(phs.StartingRot);
            }
        }
    }
}