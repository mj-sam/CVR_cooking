//using NetcodeVRTemplate.XR;

using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;

namespace SelfMadeFiles.Scripts.Network.client
{
    public class NetworkClient : NetworkBehaviour
    {
        [SerializeField] private Transform clientHead, clientLeftHand, clientRightHand;
        [SerializeField] private Animator leftHandAnimator, rightHandAnimator;
        private Transform _xrOriginHead, _xrOriginLeftHand, _xrOriginRightHand;
        private bool _isXrOriginInitialized;

        private void Awake()
        {
            _isXrOriginInitialized = false;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                foreach (var item in GetComponentsInChildren<Renderer>())
                {
                    item.enabled = false;
                }

                InitializeXrOrigin();
            }
        }

        private void InitializeXrOrigin()
        {
            var xrOriginReferences = XROriginReferences.Instance;
            _xrOriginHead = xrOriginReferences.GetXrOriginHead();
            _xrOriginLeftHand = xrOriginReferences.GetXrOriginLeftHand();
            _xrOriginRightHand = xrOriginReferences.GetXrOriginRightHand();
            _isXrOriginInitialized = true;
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (!_isXrOriginInitialized)
            {
                InitializeXrOrigin();
                return;
            }

            LocalPlayer();
        }

        private void LocalPlayer()
        {
            try
            {
                MapPosition(clientHead, _xrOriginHead);
                MapPosition(clientLeftHand, _xrOriginLeftHand);
                MapPosition(clientRightHand, _xrOriginRightHand);
                UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
                UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
            }
            catch
            {
                _isXrOriginInitialized = false;
            }
        }

        private void MapPosition(Transform target, Transform rigTransform)
        {
            target.position = rigTransform.position;
            target.rotation = rigTransform.rotation;
        }

        private static readonly int Trigger = Animator.StringToHash("Trigger");
        private static readonly int Grip = Animator.StringToHash("Grip");
        
        private void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue))
            {
                handAnimator.SetFloat(Trigger, triggerValue);
            }
            else
            {
                handAnimator.SetFloat(Trigger, 0);
            }

            if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out var gripValue))
            {
                handAnimator.SetFloat(Grip, gripValue);
            }
            else
            {
                handAnimator.SetFloat(Grip, 0);
            }
        }
    }
}