using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

namespace NRKernal
{
    public class ExplodeHuman : MonoBehaviour
    {
        [SerializeField] private float explodeDuration = 3f;
        [SerializeField] private float durationBetweenCommands = 1f;

        private Transform[] childPrefabs = null;

        private bool isExploded = false;
        public bool IsExploded => isExploded;

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;

        private IEnumerator updateRoutine = null;

        private bool inRoutine = false;

        public Action exploded;
        public Action assembled;

        private void Awake()
        {
            childPrefabs = gameObject.GetComponentsInChildren<Transform>();
            childPrefabs = childPrefabs.Where(child => child.GetComponent<InfoPanel>() != null).ToArray();

            RegisterToEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void RegisterToEvents()
        {
            AnatomyPanel.Instance.assembleImmediate += Assemble;
            AnatomyPanel.Instance.explodeImmediate += Explode;
        }

        private void UnregisterFromEvents()
        {
            if (AnatomyPanel.Instance != null)
            {
                AnatomyPanel.Instance.assembleImmediate -= Assemble;
                AnatomyPanel.Instance.explodeImmediate -= Explode;
            }
        }

        private void Update()
        {
            if (NRInput.Hands.GetHandState(rightHandEnum).isTracked && NRInput.Hands.GetHandState(leftHandEnum).isTracked)
                if (NRInput.Hands.GetHandState(rightHandEnum).currentGesture == HandGesture.OpenHand &&
                    NRInput.Hands.GetHandState(leftHandEnum).currentGesture == HandGesture.OpenHand &&
                    !inRoutine) 
                {
                    updateRoutine = UpdateRoutine();
                    StartCoroutine(updateRoutine);
                }
        }

        private void Explode()
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            for (int i = 0; i < childPrefabs.Count(); i++)
            {
                childPrefabs[i].DOLocalMove(childPrefabs[i].GetComponent<PositionHolder>().FragmentationPos, explodeDuration);
                childPrefabs[i].DORotate(childPrefabs[i].GetComponent<PositionHolder>().FragmentationRot, explodeDuration);
            }

            isExploded = true;
            exploded?.Invoke();
        }

        private void Assemble()
        {
            for (int i = 0; i < childPrefabs.Count(); i++)
            {
                childPrefabs[i].DOLocalMove(childPrefabs[i].GetComponent<PositionHolder>().StartingPos, explodeDuration);
                childPrefabs[i].DORotate(childPrefabs[i].GetComponent<PositionHolder>().StartingRot, explodeDuration);
            }

            isExploded = false;
            assembled?.Invoke();
        }

        private IEnumerator UpdateRoutine()
        {
            inRoutine = true;

            yield return new WaitForSeconds(durationBetweenCommands);

            if (NRInput.Hands.GetHandState(rightHandEnum).currentGesture == HandGesture.OpenHand &&
                    NRInput.Hands.GetHandState(leftHandEnum).currentGesture == HandGesture.OpenHand)
            {
                if (!isExploded) Explode();
                else Assemble();
            }

            inRoutine = false;
        }
    }
}