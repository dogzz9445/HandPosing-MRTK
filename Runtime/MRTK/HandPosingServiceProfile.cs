using Microsoft.MixedReality.Toolkit;
using UnityEngine;

namespace HandPosing.MRTK
{
    [MixedRealityServiceProfile(typeof(IHandPosingService))]
    [CreateAssetMenu(fileName ="HandPosingServiceProfile", menuName = "HandPosing/MRTK/Hand Posing Service Configuration Profile")]
    public class HandPosingServiceProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private GameObject _trackHandLeftPrefab = null;
        [SerializeField]
        private GameObject _trackHandRightPrefab = null;
        [SerializeField]
        private GameObject _controllerHandLeftPrefab = null;
        [SerializeField]
        private GameObject _controllerHandRightPrefab = null;

        public GameObject TrackHandLeftPrefab { get => _trackHandLeftPrefab; set => _trackHandLeftPrefab = value; }
        public GameObject TrackHandRightPrefab { get => _trackHandRightPrefab; set => _trackHandRightPrefab = value; }
        public GameObject ControllerHandLeftPrefab { get => _controllerHandLeftPrefab; set => _controllerHandLeftPrefab = value; }
        public GameObject ControllerHandRightPrefab { get => _controllerHandRightPrefab; set => _controllerHandRightPrefab = value; }
    }
}
