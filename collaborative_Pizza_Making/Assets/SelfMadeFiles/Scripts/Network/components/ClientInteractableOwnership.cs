using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace SelfMadeFiles.Scripts.Network.components
{

    [RequireComponent(typeof(XRBaseInteractable)), RequireComponent(typeof(NetworkObject))]
    public class ClientInteractableOwnership : NetworkBehaviour
    {
        private XRBaseInteractable _interactable;

        private void Awake()
        {
            _interactable = GetComponent<XRBaseInteractable>();
            if (_interactable == null)
            {
                enabled = false;
            }
        }
        IEnumerator SetInteractionLayerTemporarily()
        {
            _interactable.interactionLayers = 0;
            Debug.Log("myVariable is now false");
    
            yield return new WaitForSeconds(1);
            _interactable.interactionLayers = LayerMask.GetMask("Default");
            // Log to demonstrate the change back
            Debug.Log("myVariable is now true");
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _interactable.selectEntered.AddListener(ChangeOwnership);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _interactable.selectEntered.RemoveListener(ChangeOwnership);
        }

        private void ChangeOwnership(SelectEnterEventArgs args)
        {
            if (IsOwner)
            {
                return;
            }
            ChangeOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
            
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeOwnershipServerRpc(ulong clientId)
        {
            NetworkObject.ChangeOwnership(clientId);
            ChangeChildrenOwnership( NetworkObject.gameObject,clientId);
            ForceDropClientRpc();
        }
        private void ChangeChildrenOwnership(GameObject obj, ulong newOwnerId)
        {
            foreach (Transform child in obj.transform)
            {
                NetworkObject childNetworkObject = child.GetComponent<NetworkObject>();
                if (childNetworkObject != null)
                {
                    childNetworkObject.ChangeOwnership(newOwnerId);
                }

                // Recursively change ownership of grandchildren (if any)
                ChangeChildrenOwnership(child.gameObject, newOwnerId);
            }
        }
        
        [ClientRpc]
        private void ForceDropClientRpc()
        {
            if (IsOwner)
            {
                return;
            }
            if (_interactable.isSelected)
            {
                SetInteractionLayerTemporarily();
                //_interactable.selectMode
                // // Get the interactor that is currently interacting with this object
                XRBaseInteractor interactor = _interactable.selectingInteractor;
                // //SetVariableFalseTemporarily(interactor);
                // interactor.selectExited
                //
                if (interactor.isSelectActive)
                {
                    _interactable.interactionManager.SelectExit(interactor, _interactable);
                       
                }
            }
        }
    }
}
