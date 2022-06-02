/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using UnityEngine;

namespace NRKernal
{
    /// <summary> Controls the HelloAR example. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/controller")]
    public class HelloMRControllerTest : MonoBehaviour
    {
        /// <summary> A model to place when a raycast from a user touch hits a plane. </summary>
        [SerializeField] private GameObject videoPlanePrefab;
        private GameObject videoPlane = null;

        public Action<GameObject> onPlaneCreated { get; set; }

        /// <summary> Updates this object. </summary>
        void Update()
        {
            // If the player doesn't click the trigger button, we are done with this update.
            if (!NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                return;
            }

            // Get controller laser origin.
            var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
            Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);

            RaycastHit hitResult;
            if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
            {
                if (hitResult.collider.gameObject != null && hitResult.collider.gameObject.GetComponent<NRTrackableBehaviour>() != null)
                {
                    var behaviour = hitResult.collider.gameObject.GetComponent<NRTrackableBehaviour>();
                    if (behaviour.Trackable.GetTrackableType() != TrackableType.TRACKABLE_PLANE)
                    {
                        return;
                    }

                    // Instantiate Andy model at the hit point / compensate for the hit point rotation.
                    if (videoPlane == null)
                    {
                        videoPlane = Instantiate(videoPlanePrefab, hitResult.point, Quaternion.identity, behaviour.transform);
                        onPlaneCreated?.Invoke(videoPlane);
                    }

                    else
                        videoPlane.transform.localPosition = hitResult.point;
                }
            }
        }
    }
}
