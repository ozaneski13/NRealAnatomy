using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal
{
    public class HandTrackingExamplePanel : MonoBehaviour
    {
        private static HandTrackingExamplePanel _instance;
        public static HandTrackingExamplePanel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<HandTrackingExamplePanel>();

                return _instance;
            }
        }

        [SerializeField] private GameObject prefab = null;
        private ExplodePrefab explodePrefab = null;

        [SerializeField] private SwitchToggle pinchSwitch = null;
        [SerializeField] private SwitchToggle selectSwitch = null;
        [SerializeField] private SwitchToggle rotateSwitch = null;
        [SerializeField] private SwitchToggle grabSwitch = null;

        private Toggle pinchToggle = null;
        private Toggle selectToggle = null;
        private Toggle rotateToggle = null;
        private Toggle grabToggle = null;

        [SerializeField] private Vector3 initialPos = Vector3.zero;
        private Quaternion initialRot = Quaternion.identity;
        private Vector3 initialScale = Vector3.zero;

        public Action<bool> canRotate;
        public Action<bool> canSelect;
        public Action<bool> canGrab;
        public Action<bool> canResize;
        public Action<bool> canFragment;

        public Action explodeImmediate;
        public Action assambleImmediate;

        private void Awake()
        {
            explodePrefab = prefab.GetComponent<ExplodePrefab>();

            RegisterToSwitches();
            InitializeToggles();

            initialRot = prefab.transform.rotation;
            initialScale = prefab.transform.localScale;
        }

        private void OnDestroy()
        {
            UnregisterFromSwitches();
        }

        private void InitializeToggles()
        {
            pinchToggle = pinchSwitch.gameObject.GetComponent<Toggle>();
            selectToggle = selectSwitch.gameObject.GetComponent<Toggle>();
            rotateToggle = rotateSwitch.gameObject.GetComponent<Toggle>();
            grabToggle = grabSwitch.gameObject.GetComponent<Toggle>();
        }

        private void RegisterToSwitches()
        {
            pinchSwitch.OnToggleChanged += CheckToggles;
            selectSwitch.OnToggleChanged += CheckToggles;
            rotateSwitch.OnToggleChanged += CheckToggles;
            grabSwitch.OnToggleChanged += CheckToggles;
        }

        private void UnregisterFromSwitches()
        {
            pinchSwitch.OnToggleChanged -= CheckToggles;
            selectSwitch.OnToggleChanged -= CheckToggles;
            rotateSwitch.OnToggleChanged -= CheckToggles;
            grabSwitch.OnToggleChanged -= CheckToggles;
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
                    selectToggle.isOn = false;
                    rotateToggle.isOn = false;
                    grabToggle.isOn = false;

                    canRotate?.Invoke(false);
                    canSelect?.Invoke(false);
                    canGrab?.Invoke(false);
                    canResize?.Invoke(true);

                    canFragment?.Invoke(false);
                    assambleImmediate?.Invoke();
                }

                else
                    canResize?.Invoke(false);
            }

            if (toggle == selectToggle)
            {
                if (selectToggle.isOn)
                {
                    pinchToggle.isOn = false;
                    rotateToggle.isOn = false;
                    grabToggle.isOn = false;

                    canResize?.Invoke(false);
                    canRotate?.Invoke(false);
                    canGrab?.Invoke(false);

                    canSelect?.Invoke(true);
                    canFragment?.Invoke(true);

                    explodeImmediate?.Invoke();
                }

                else if (!selectToggle.isOn && explodePrefab.IsExploded)
                {
                    pinchToggle.isOn = false;
                    rotateToggle.isOn = false;
                    grabToggle.isOn = false;

                    canResize?.Invoke(false);
                    canRotate?.Invoke(false);
                    canGrab?.Invoke(false);
                    canFragment?.Invoke(true);
                    canSelect?.Invoke(true);
                }

                else
                {
                    canSelect?.Invoke(false);
                    assambleImmediate?.Invoke();
                    canFragment?.Invoke(false);
                }
            }

            if (toggle == rotateToggle)
            {
                if (rotateToggle.isOn)
                {
                    selectToggle.isOn = false;
                    pinchToggle.isOn = false;
                    grabToggle.isOn = false;

                    canResize?.Invoke(false);
                    canSelect?.Invoke(false);
                    canGrab?.Invoke(false);
                    canRotate?.Invoke(true);
                    
                    canFragment?.Invoke(false);
                    assambleImmediate?.Invoke();
                }

                else
                    canRotate?.Invoke(false);
            }

            if (toggle == grabToggle)
            {
                if (grabToggle.isOn)
                {
                    selectToggle.isOn = false;
                    rotateToggle.isOn = false;
                    pinchToggle.isOn = false;

                    canResize?.Invoke(false);
                    canRotate?.Invoke(false);
                    canSelect?.Invoke(false);
                    canGrab?.Invoke(true);

                    canFragment?.Invoke(false);
                    assambleImmediate?.Invoke();
                }

                else
                    canGrab?.Invoke(false);
            }
        }

        public void ResetPrefab()
        {
            assambleImmediate?.Invoke();

            Rigidbody rb = prefab.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.Sleep();
                rb.WakeUp();
            }

            prefab.transform.position = initialPos;
            prefab.transform.rotation = initialRot;
            prefab.transform.localScale = initialScale;
        }
    }
}