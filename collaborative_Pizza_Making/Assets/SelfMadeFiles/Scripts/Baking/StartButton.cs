using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

//using Alteruna;

public class StartButton : NetworkBehaviour
{
    public GameObject parrentButton;
    public GameObject button;
    bool isPressed = false;
    private bool canPush = true;
    public GameObject[] objectsToDeactivate;
    private NetworkTaskProgression _networkTaskProgression;

    private void Start(){
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        Freeze();
    }
    
    // When an object collides with button, the button is pressed
    private void OnTriggerEnter(Collider other) {
        if (!isPressed && other.CompareTag("Hand"))
        {
            _networkTaskProgression.startGameServerRpc();
        }
    }

    // When an object leaves the button collider, the button is released
    private void OnTriggerExit(Collider other) {
        button.transform.localPosition = new Vector3(0,0.015f,0);
        isPressed = false;
        canPush = false;
        unfreezeServerRpc();
        makeItDisapearServerRpc();
    }
    
    //============ Freeze All =====================
    //[ServerRpc(RequireOwnership = false)]
    public void Freeze()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                XRGrabInteractable interactable = obj.GetComponent<XRGrabInteractable>();
                if (interactable != null)
                {
                    interactable.interactionLayers =  0;   
                }
            }
        }

        //FreezeClientRpc();
    }
    // [ClientRpc]
    // public void FreezeClientRpc()
    // {
    //     foreach (GameObject obj in objectsToDeactivate)
    //     {
    //         if (obj != null)
    //         {
    //             Debug.Log("deactivate Client");
    //             obj.SetActive(false); // Deactivate the object
    //         }
    //     }
    // }
    //=================== Unfreeze All ======================
    [ServerRpc(RequireOwnership = false)]
    public void unfreezeServerRpc()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                XRGrabInteractable interactable = obj.GetComponent<XRGrabInteractable>();
                if (interactable != null)
                {
                    interactable.interactionLayers = LayerMask.GetMask("Default");    
                }
            }
        }
        unfreezeClientRpc();
    }
    [ClientRpc]
    public void unfreezeClientRpc()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                XRGrabInteractable interactable = obj.GetComponent<XRGrabInteractable>();
                if (interactable != null)
                {
                    interactable.interactionLayers = LayerMask.GetMask("Default");    
                }
            }
        }
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void makeItDisapearServerRpc()
    {
        parrentButton.SetActive(false);
        makeItDisapearClientRpc();
    }
    [ClientRpc]
    public void makeItDisapearClientRpc()
    {
        parrentButton.SetActive(false);
    }
}