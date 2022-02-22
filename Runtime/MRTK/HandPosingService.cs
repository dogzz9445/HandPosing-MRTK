using HandPosing.OVRIntegration;
using HandPosing.OVRIntegration.GrabEngine;
using HandPosing.TrackingData;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace HandPosing.MRTK
{
    [MixedRealityExtensionService(
        (SupportedPlatforms)(-1),
        "Hand Posing Service",
        "MRTK/Profiles/DefaultHandPosingServiceProfile.asset",
        "HandPosing.MRTK",
        true)]
    public class HandPosingService : BaseExtensionService, IHandPosingService, IMixedRealityExtensionService
    {
        private HandPosingServiceProfile _handPosingServiceProfile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public HandPosingService(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority, profile)
        {
            _handPosingServiceProfile = (HandPosingServiceProfile)profile;
        }

        public GameObject HandPosingServiceRoot { get; private set; }
        public GameObject TrackHandLeftPrefab { get; set; }
        public GameObject TrackHandRightPrefab { get; set; }
        public GameObject ControllerHandLeftPrefab { get; set; }
        public GameObject ControllerHandRightPrefab { get; set; }
        public OVRCameraRig RefOVRCameraRig { get; set; }

        private bool _isInitilized = false;

        public override void Initialize()
        {
            base.Initialize();
            TrackHandLeftPrefab = _handPosingServiceProfile.TrackHandLeftPrefab;
            TrackHandRightPrefab = _handPosingServiceProfile.TrackHandRightPrefab;
            ControllerHandLeftPrefab = _handPosingServiceProfile.ControllerHandLeftPrefab;
            ControllerHandRightPrefab = _handPosingServiceProfile.ControllerHandRightPrefab;
        }

        public override void Enable()
        {
            base.Enable();
            HandPosingServiceRoot = new GameObject("Hand Posing Service");

            _isInitilized = CreateHandPosingService(HandPosingServiceRoot);
            if (_isInitilized == false)
            {
                UnityEngine.Object.Destroy(HandPosingServiceRoot);
                HandPosingServiceRoot = null;
            }
        }

        public override void Disable()
        {
            if (HandPosingServiceRoot != null)
            {
                _isInitilized = false;
                UnityEngine.Object.Destroy(HandPosingServiceRoot);
                HandPosingServiceRoot = null;
            }
            base.Disable();
        }

        public override void Update()
        {
            base.Update();

            if (_isInitilized == false)
            {
                HandPosingServiceRoot = new GameObject("Hand Posing Service");
                _isInitilized = CreateHandPosingService(HandPosingServiceRoot);

                if (_isInitilized == false)
                {
                    UnityEngine.Object.Destroy(HandPosingServiceRoot);
                    HandPosingServiceRoot = null;
                }
            }
        }

        private bool CreateHandPosingService(GameObject parent)
        {
            var RefOVRCameraRig = GameObject.FindObjectOfType<OVRCameraRig>();

            if (RefOVRCameraRig == null)
            {
                Debug.LogError("Null reference exception occurs 'RefOVRCameraRig'");
                return false;
            }
            if (TrackHandLeftPrefab == null)
            {
                Debug.LogError("Null reference exception occurs 'TrackHandLeftPrfeab'");
                return false;
            }
            if (TrackHandRightPrefab == null)
            {
                Debug.LogError("Null reference exception occurs 'TrackHandRightPrefab'");
                return false;
            }
            if (ControllerHandLeftPrefab == null)
            {
                Debug.LogError("Null reference exception occurs 'ControllerHandLeftPrefab'");
                return false;
            }
            if (ControllerHandRightPrefab == null)
            {
                Debug.LogError("Null reference exception occurs 'ControllerHandRightPrefab'");
                return false;
            }

            OVRHand leftHand = null;
            OVRHand rightHand = null;
            OVRSkeleton leftSkeleton = null;
            OVRSkeleton rightSkeleton = null;
            Transform leftHandAnchor = null;
            Transform rightHandAnchor = null;

            var ovrHands = RefOVRCameraRig.GetComponentsInChildren<OVRHand>();

            foreach (var ovrHand in ovrHands)
            {
                // Manage Hand skeleton data
                var skeletonDataProvider = ovrHand as OVRSkeleton.IOVRSkeletonDataProvider;
                var skeletonType = skeletonDataProvider.GetSkeletonType();

                var ovrSkeleton = ovrHand.GetComponent<OVRSkeleton>();
                if (ovrSkeleton == null)
                {
                    continue;
                }

                switch (skeletonType)
                {
                    case OVRSkeleton.SkeletonType.HandLeft:
                        leftHand = ovrHand;
                        leftSkeleton = ovrSkeleton;
                        leftHandAnchor = leftHand.gameObject.transform.parent.Find("LeftControllerAnchor");
                        if (leftHandAnchor == null)
                        {
                            leftHandAnchor = new GameObject("LeftControllerAnchor").transform;
                            leftHandAnchor.parent = leftHand.gameObject.transform.parent;
                        }
                        break;
                    case OVRSkeleton.SkeletonType.HandRight:
                        rightHand = ovrHand;
                        rightSkeleton = ovrSkeleton;
                        rightHandAnchor = rightHand.gameObject.transform.parent.Find("RightControllerAnchor");
                        if (rightHandAnchor == null)
                        {
                            rightHandAnchor = new GameObject("RightControllerAnchor").transform;
                            rightHandAnchor.parent = rightHand.gameObject.transform.parent;
                        }
                        break;
                }
            }

            if (leftHand == null)
            {
                Debug.LogError("Null reference exception occurs 'leftHand'");
                return false;
            }
            if (rightHand == null)
            {
                Debug.LogError("Null reference exception occurs 'rightHand'");
                return false;
            }
            if (leftSkeleton == null)
            {
                Debug.LogError("Null reference exception occurs 'leftSkeleton'");
                return false;
            }
            if (rightSkeleton == null)
            {
                Debug.LogError("Null reference exception occurs 'rightSkeleton'");
                return false;
            }
            if (leftHandAnchor == null)
            {
                Debug.LogError("Null reference exception occurs 'leftHandAnchor'");
                return false;
            }
            if (rightHandAnchor == null)
            {
                Debug.LogError("Null reference exception occurs 'rightHandAnchor'");
                return false;
            }

            var TrackHandLeft = GameObject.Instantiate(TrackHandLeftPrefab, parent.transform);
            TrackHandLeft.SetActive(false);
            var TrackHandRight = GameObject.Instantiate(TrackHandRightPrefab, parent.transform);
            TrackHandRight.SetActive(false);
            var ControllerHandLeft = GameObject.Instantiate(ControllerHandLeftPrefab, parent.transform);
            var ControllerHandRight = GameObject.Instantiate(ControllerHandRightPrefab, parent.transform);

            var leftTipsTrigger = leftHand.gameObject.GetComponent<TipsTriggersOVR>();
            if (leftTipsTrigger == null)
            {
                Debug.LogError("Null reference exception occurs 'leftTipsTrigger'");
                return false;
            }
            leftTipsTrigger.Grabber = TrackHandLeft.GetComponent<GrabberHybridOVR>();

            var rightTipsTrigger = rightHand.gameObject.GetComponent<TipsTriggersOVR>();
            if (rightTipsTrigger == null)
            {
                Debug.LogError("Null reference exception occurs 'rightTipsTrigger'");
                return false;
            }
            rightTipsTrigger.Grabber = TrackHandRight.GetComponent<GrabberHybridOVR>();

            var trackHandLeftHandPuppet = TrackHandLeft.GetComponent<HandPuppet>();
            if (trackHandLeftHandPuppet == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandLeftHandPuppet'");
                return false;
            }
            trackHandLeftHandPuppet.Skeleton = leftHand.GetComponent<ExtrapolationTrackingCleaner>();
            trackHandLeftHandPuppet.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();
            trackHandLeftHandPuppet.ControllerAnchor = leftHandAnchor;

            var trackHandRightHandPuppet = TrackHandRight.GetComponent<HandPuppet>();
            if (trackHandRightHandPuppet == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandRightHandPuppet'");
                return false;
            }
            trackHandRightHandPuppet.Skeleton = rightHand.GetComponent<ExtrapolationTrackingCleaner>();
            trackHandRightHandPuppet.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();
            trackHandRightHandPuppet.ControllerAnchor = rightHandAnchor;

            var controllerHandLeftHandPuppet = ControllerHandLeft.GetComponent<HandPuppet>();
            if (controllerHandLeftHandPuppet == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandLeftHandPuppet'");
                return false;
            }
            controllerHandLeftHandPuppet.Skeleton = leftHand.GetComponent<ExtrapolationTrackingCleaner>();
            controllerHandLeftHandPuppet.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();
            controllerHandLeftHandPuppet.ControllerAnchor = leftHandAnchor;

            var controllerHandRightHandPuppet = ControllerHandRight.GetComponent<HandPuppet>();
            if (controllerHandRightHandPuppet == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandRightHandPuppet'");
                return false;
            }
            controllerHandRightHandPuppet.Skeleton = rightHand.GetComponent<ExtrapolationTrackingCleaner>();
            controllerHandRightHandPuppet.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();
            controllerHandRightHandPuppet.ControllerAnchor = rightHandAnchor;

            var trackHandLeftGrabberHybridOVR = TrackHandLeft.GetComponent<GrabberHybridOVR>();
            if (trackHandLeftGrabberHybridOVR == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandLeftGrabberHybridOVR'");
                return false;
            }
            trackHandLeftGrabberHybridOVR.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();

            var trackHandRightGrabberHybridOVR = TrackHandRight.GetComponent<GrabberHybridOVR>();
            if (trackHandRightGrabberHybridOVR == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandRightGrabberHybridOVR'");
                return false;
            }
            trackHandRightGrabberHybridOVR.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();

            var controllerHandLeftGrabberHybridOVR = ControllerHandLeft.GetComponent<GrabberHybridOVR>();
            if (controllerHandLeftGrabberHybridOVR == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandLeftGrabberHybridOVR'");
                return false;
            }
            controllerHandLeftGrabberHybridOVR.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();

            var controllerHandRightGrabberHybridOVR = ControllerHandRight.GetComponent<GrabberHybridOVR>();
            if (controllerHandRightGrabberHybridOVR == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandRightGrabberHybridOVR'");
                return false;
            }
            controllerHandRightGrabberHybridOVR.UpdateNotifier = RefOVRCameraRig.GetComponent<UpdateNotifierOVR>();

            var trackHandLeftPinchTriggerFlex = TrackHandLeft.GetComponent<PinchTriggerFlex>();
            if (trackHandLeftPinchTriggerFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandLeftPinchTriggerFlex'");
                return false;
            }
            trackHandLeftPinchTriggerFlex.FlexHand = leftHand;

            var trackHandRightPinchTriggerFlex = TrackHandRight.GetComponent<PinchTriggerFlex>();
            if (trackHandRightPinchTriggerFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandRightPinchTriggerFlex'");
                return false;
            }
            trackHandRightPinchTriggerFlex.FlexHand = rightHand;

            var controllerHandLeftPinchTriggerFlex = ControllerHandLeft.GetComponent<PinchTriggerFlex>();
            if (controllerHandLeftPinchTriggerFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandLeftPinchTriggerFlex'");
                return false;
            }
            controllerHandLeftPinchTriggerFlex.FlexHand = leftHand;

            var controllerHandRightPinchTriggerFlex = ControllerHandRight.GetComponent<PinchTriggerFlex>();
            if (controllerHandRightPinchTriggerFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandRightPinchTriggerFlex'");
                return false;
            }
            controllerHandRightPinchTriggerFlex.FlexHand = rightHand;

            var trackHandLeftSphereGrabFlex = TrackHandLeft.GetComponent<SphereGrabFlex>();
            if (trackHandLeftSphereGrabFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandLeftSphereGrabFlex'");
                return false;
            }
            trackHandLeftSphereGrabFlex.FlexHand = leftHand;
            trackHandLeftSphereGrabFlex.Skeleton = leftSkeleton;

            var trackHandRightSphereGrabFlex = TrackHandRight.GetComponent<SphereGrabFlex>();
            if (trackHandRightSphereGrabFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'trackHandRightSphereGrabFlex'");
                return false;
            }
            trackHandRightSphereGrabFlex.FlexHand = rightHand;
            trackHandRightSphereGrabFlex.Skeleton = rightSkeleton;

            var controllerHandLeftSphereGrabFlex = ControllerHandLeft.GetComponent<SphereGrabFlex>();
            if (controllerHandLeftSphereGrabFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandLeftSphereGrabFlex'");
                return false;
            }
            controllerHandLeftSphereGrabFlex.FlexHand = leftHand;
            controllerHandLeftSphereGrabFlex.Skeleton = leftSkeleton;

            var controllerHandRightSphereGrabFlex = ControllerHandRight.GetComponent<SphereGrabFlex>();
            if (controllerHandRightSphereGrabFlex == null)
            {
                Debug.LogError("Null reference exception occurs 'controllerHandRightSphereGrabFlex'");
                return false;
            }
            controllerHandRightSphereGrabFlex.FlexHand = rightHand;
            controllerHandRightSphereGrabFlex.Skeleton = rightSkeleton;

            return true;
        }
    }

}
