using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace NRKernal
{
    public class ExplodePrefab : MonoBehaviour
    {
        [SerializeField] private float explodeDuration = 3f;
        [SerializeField] private float durationBetweenCommands = 1f;

        private Transform[] childPrefabs = null;

        private bool isExploded = false;
        public bool IsExploded => isExploded;

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;
        private HandEnum defaultHandEnum = HandEnum.RightHand;

        private IEnumerator explodeRoutine = null;
        private IEnumerator assembleRoutine = null;

        private bool canFragment = false;

        private void Awake()
        {
            RegisterToEvents();

            childPrefabs = gameObject.GetComponentsInChildren<Transform>();
            childPrefabs = childPrefabs.Where(child => child.tag == "ChildPrefab").ToArray();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void Update()
        {
            if (NRInput.Hands.GetHandState(rightHandEnum).isTracked)
                defaultHandEnum = rightHandEnum;
            else if (NRInput.Hands.GetHandState(leftHandEnum).isTracked)
                defaultHandEnum = leftHandEnum;
            else
                return;

            if (NRInput.Hands.GetHandState(defaultHandEnum).currentGesture == HandGesture.OpenHand && canFragment)
            {
                if (!isExploded)
                    Explode();
                else
                    Assamble();
            }

            else if(!canFragment)
                Assamble();
        }

        private void SetExplode(bool set)
        {
            canFragment = set;
        }

        private void RegisterToEvents()
        {
            HandTrackingExamplePanel.Instance.canFragment += SetExplode;
            HandTrackingExamplePanel.Instance.assambleImmediate += AssambleImmediate;
            HandTrackingExamplePanel.Instance.explodeImmediate += ExplodeImmediate;
        }

        private void UnregisterFromEvents()
        {
            HandTrackingExamplePanel.Instance.canFragment -= SetExplode;
            HandTrackingExamplePanel.Instance.assambleImmediate -= AssambleImmediate;
            HandTrackingExamplePanel.Instance.explodeImmediate -= ExplodeImmediate;
        }

        private void Explode()
        {
            explodeRoutine = ExplodeRoutine();
            StartCoroutine(explodeRoutine);
        }

        private void Assamble()
        {
            assembleRoutine = AssambleRoutine();
            StartCoroutine(assembleRoutine);
        }

        private void AssambleImmediate()
        {
            for (int i = 0; i < childPrefabs.Count(); i++)
                childPrefabs[i].DOLocalMove(childPrefabs[i].GetComponent<PositionHolder>().StartingPos, explodeDuration);

            isExploded = false;
        }

        private void ExplodeImmediate()
        {
            for (int i = 0; i < childPrefabs.Count(); i++)
                childPrefabs[i].DOLocalMove(childPrefabs[i].GetComponent<PositionHolder>().FragmentationPos, explodeDuration);

            isExploded = true;
        }

        private IEnumerator AssambleRoutine()
        {
            yield return new WaitForSeconds(durationBetweenCommands);

            for (int i = 0; i < childPrefabs.Count(); i++)
                childPrefabs[i].DOLocalMove(childPrefabs[i].GetComponent<PositionHolder>().StartingPos, explodeDuration);

            isExploded = false;
        }

        private IEnumerator ExplodeRoutine()
        {
            yield return new WaitForSeconds(durationBetweenCommands);

            for (int i = 0; i < childPrefabs.Count(); i++)
                childPrefabs[i].DOLocalMove(childPrefabs[i].GetComponent<PositionHolder>().FragmentationPos, explodeDuration);

            isExploded = true;
        }
    }
}