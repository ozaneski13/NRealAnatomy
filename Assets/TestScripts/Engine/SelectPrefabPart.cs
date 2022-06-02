using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NRKernal
{
    public class SelectPrefabPart : MonoBehaviour
    {
        [SerializeField] private float timeBeforeNewSelection = 2f;

        [SerializeField] private Material transparentMaterial;
        [SerializeField] private Material selectionMaterial;

        [SerializeField] private NRPointerRaycaster nrPointerRaycasterRight = null;
        [SerializeField] private NRPointerRaycaster nrPointerRaycasterLeft = null;

        private NRPointerRaycaster defaultRaycaster = null;

        private HandEnum rightHandEnum = HandEnum.RightHand;
        private HandEnum leftHandEnum = HandEnum.LeftHand;
        private HandEnum defaultHandEnum = HandEnum.RightHand;

        private MaterialPropertyBlock materialPropertyBlock;

        private List<MeshRenderer> raycastedPrefabs;

        private IEnumerator materialRoutine = null;

        private bool canSelect = false;
        private bool canStart = true;

        private void SetSelect(bool set)
        {
            canSelect = set;

            if(canSelect)
                raycastedPrefabs = new List<MeshRenderer>();
            if (!canSelect)
                ResetMaterials();
        }

        private void Awake()
        {
            RegisterToEvents();

            raycastedPrefabs = new List<MeshRenderer>();

            defaultRaycaster = nrPointerRaycasterRight;
            materialRoutine = MaterialRoutine();

            StartCoroutine(materialRoutine);
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();

            StopCoroutine(materialRoutine);
        }

        private void RegisterToEvents()
        {
            HandTrackingExamplePanel.Instance.canSelect += SetSelect;
        }

        private void UnregisterFromEvents()
        {
            HandTrackingExamplePanel.Instance.canSelect -= SetSelect;
        }

        void Update()
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
        }

        private IEnumerator MaterialRoutine()
        {
            while (true)
            {
                if (NRInput.Hands.GetHandState(defaultHandEnum).currentGesture == HandGesture.Victory && canSelect)
                {
                    RaycastResult rayResult = defaultRaycaster.FirstRaycastResult();

                    if (rayResult.isValid && rayResult.gameObject != null)
                    {
                        MeshRenderer meshRenderer = rayResult.gameObject.GetComponent<MeshRenderer>();

                        if (meshRenderer != null && canStart)
                        {
                            if (!raycastedPrefabs.Contains(meshRenderer))
                                raycastedPrefabs.Add(meshRenderer);

                            canStart = false;

                            materialPropertyBlock = new MaterialPropertyBlock();

                            if (meshRenderer.material.color == selectionMaterial.color)
                            {
                                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                                materialPropertyBlock.SetColor("_Color", transparentMaterial.color);
                                meshRenderer.SetPropertyBlock(materialPropertyBlock);
                            }

                            else if (meshRenderer.material.color == transparentMaterial.color)
                            {
                                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                                materialPropertyBlock.SetColor("_Color", selectionMaterial.color);
                                meshRenderer.SetPropertyBlock(materialPropertyBlock);
                            }

                            yield return new WaitForSeconds(timeBeforeNewSelection);

                            canStart = true;
                        }
                    }
                }

                yield return null;
            }
        }

        private void ResetMaterials()
        {
            foreach(MeshRenderer mr in raycastedPrefabs)
            {
                materialPropertyBlock = new MaterialPropertyBlock();

                mr.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetColor("_Color", transparentMaterial.color);
                mr.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
}