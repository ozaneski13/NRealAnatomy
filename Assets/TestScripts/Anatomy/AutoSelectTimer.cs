using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NRKernal
{
    public class AutoSelectTimer : MonoBehaviour
    {
        [SerializeField] private float timeToAutoClick = 0.5f;
        [SerializeField] private float timeForeNextDetection = 2f;
        [SerializeField] private float victoryThresholdTime = 0.5f;

        [SerializeField] private NRPointerRaycaster nrPointerRaycasterRight;
        [SerializeField] private NRPointerRaycaster nrPointerRaycasterLeft;
        private NRPointerRaycaster defaultRaycaster = null;

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;

        private IEnumerator timerRoutine = null;
        private IEnumerator immediateRoutine = null;

        private bool isImmediateDone = true;

        private IEnumerator TimerRoutine()
        {
            RaycastResult rayResult;

            Toggle firstTg = null;
            Toggle secondTg = null;

            bool isSecond = false;

            while (true)
            {
                rayResult = defaultRaycaster.FirstRaycastResult();

                if (rayResult.isValid && rayResult.gameObject != null)
                    if (rayResult.gameObject.tag == "Button")
                    {
                        if (rayResult.gameObject.GetComponentInParent<Toggle>() != null)
                        {
                            if (!isSecond)
                                firstTg = rayResult.gameObject.GetComponentInParent<Toggle>();
                            else
                                secondTg = rayResult.gameObject.GetComponentInParent<Toggle>();

                            isSecond = !isSecond;
                        }
                    }

                yield return new WaitForSeconds(timeToAutoClick);

                if (secondTg != null && firstTg == secondTg)
                {
                    AnatomyPanel.Instance.AutoToggle(secondTg);

                    if (!secondTg.isOn)
                    {
                        secondTg.onValueChanged.Invoke(true);
                        secondTg.isOn = true;
                        secondTg.Select();
                    }

                    else
                    {
                        secondTg.onValueChanged.Invoke(false);
                        secondTg.isOn = false;
                    }
                }

                if(secondTg != null)
                {
                    firstTg = secondTg;
                    secondTg = null;
                    isSecond = true;
                }

                yield return new WaitForSeconds(timeForeNextDetection);
            }
        }

        private void Awake()
        {
            defaultRaycaster = nrPointerRaycasterRight;

            timerRoutine = TimerRoutine();
            StartCoroutine(timerRoutine);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if (NRInput.Hands.GetHandState(rightHandEnum).isTracked)
                defaultRaycaster = nrPointerRaycasterRight;
            else if (NRInput.Hands.GetHandState(leftHandEnum).isTracked)
                defaultRaycaster = nrPointerRaycasterLeft;
            else return;

            if (NRInput.Hands.GetHandState(rightHandEnum).currentGesture == HandGesture.Point ||
                NRInput.Hands.GetHandState(leftHandEnum).currentGesture == HandGesture.Point)
                SelectImmediately();
        }

        private void SelectImmediately()
        {
            if (isImmediateDone)
            {
                immediateRoutine = ImmediateRoutine();
                StartCoroutine(immediateRoutine);
            }
        }

        private IEnumerator ImmediateRoutine()
        {
            isImmediateDone = false;

            RaycastResult rayResult;
            Toggle tg;

            rayResult = defaultRaycaster.FirstRaycastResult();

            if (rayResult.isValid && rayResult.gameObject != null)
                if (rayResult.gameObject.tag == "Button")
                {
                    tg = rayResult.gameObject.GetComponentInParent<Toggle>();

                    if (tg != null)
                    {
                        yield return new WaitForSeconds(victoryThresholdTime);

                        AnatomyPanel.Instance.AutoToggle(tg);

                        if (!tg.isOn)
                        {
                            tg.onValueChanged.Invoke(true);
                            tg.isOn = true;
                            tg.Select();
                        }

                        else
                        {
                            tg.onValueChanged.Invoke(false);
                            tg.isOn = false;
                        }
                    }
                }

            isImmediateDone = true;
        }
    }
}