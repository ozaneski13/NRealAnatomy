using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal
{
    public class InfoPanel : MonoBehaviour
    {
        [SerializeField] private ExplodeHuman explodeHuman = null;

        [SerializeField] private GridLayoutGroup gridLayoutGroup = null;
        [SerializeField] private Text panelText = null;

        [SerializeField] private int disappearSpeed = 2;

        [SerializeField] private float durationBetweenSelection = 2f;

        private int topBorder;

        private bool isShowed = false;
        private bool canChangeActivity = true;

        private IEnumerator infoRoutine = null;

        private void Awake()
        {
            topBorder = gridLayoutGroup.padding.top / -2;

            RegisterToEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        private void RegisterToEvents()
        {
            AnatomyPanel.Instance.disableInfos += Disable;
            explodeHuman.assembled += Disable;
        }

        private void UnregisterFromEvents()
        {
            if (AnatomyPanel.Instance != null)
                AnatomyPanel.Instance.disableInfos -= Disable;

            explodeHuman.assembled -= Disable;
        }

        private IEnumerator InfoRoutine()
        {
            if (!canChangeActivity)
                yield return 0;

            canChangeActivity = false;

            if (isShowed)
            {
                panelText.enabled = false;

                while(true)
                {
                    gridLayoutGroup.padding.top -= disappearSpeed;

                    if (gridLayoutGroup.padding.top <= topBorder * -2)
                    {
                        gridLayoutGroup.padding.top = topBorder * -2;
                        break;
                    }

                    yield return null;
                }
            }

            else if (!isShowed)
            {
                while (true)
                {
                    gridLayoutGroup.padding.top += disappearSpeed;

                    LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.gameObject.GetComponent<RectTransform>());

                    if (gridLayoutGroup.padding.top >= topBorder)
                    {
                        gridLayoutGroup.padding.top = topBorder;
                        break;
                    }

                    yield return null;
                }

                panelText.enabled = true;
            }

            isShowed = !isShowed;

            yield return new WaitForSeconds(durationBetweenSelection);

            canChangeActivity = true;
        }

        public void ShowInfo()
        {
            if (!canChangeActivity)
                return;

            infoRoutine = InfoRoutine();
            StartCoroutine(infoRoutine);
        }

        private void Disable()
        {
            isShowed = false;

            gridLayoutGroup.padding.top = topBorder * -2;
            panelText.enabled = false;
        }
    }
}