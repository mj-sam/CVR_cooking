using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Alteruna;
using UnityEngine.XR.Interaction.Toolkit;
using Manus.Interaction;

public class GrabbedItem : MonoBehaviour
{
    public bool otherUserPickedUp = false;
    private NetworkTaskProgression _networkTaskProgression;
    public IXRSelectInteractor interactor;
    public GameObject hapticInteractor;


    public void Start(){
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        //_taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
    }

    public void Grab(IXRSelectInteractor hand){
        interactor = hand;
        if (otherUserPickedUp){
            //InvokeRemoteMethod("ForceDrop", 65535);
            otherUserPickedUp = false;
        }
    }

    public void GrabHaptic(GameObject hapticHand){
        /*
        hapticInteractor = hapticHand;
        InvokeRemoteMethod("setOtherUserPickedUp", 65535, true);
        if (otherUserPickedUp){
            InvokeRemoteMethod("ForceDrop", 65535);
            otherUserPickedUp = false;
        }
        */
    }

    public void Release(){
        //InvokeRemoteMethod("setOtherUserPickedUp", 65535, false);
    }

    //[SynchronizableMethod]
    public void setOtherUserPickedUp(bool set){
        otherUserPickedUp = set;
    }

    //[SynchronizableMethod]
    public void ForceDrop(){
        GameObject.Find("XR Interaction Manager").GetComponent<InteractionManager>().ForceDrop(interactor, this.gameObject.GetComponent<XRGrabInteractable>());
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    //[SynchronizableMethod]
    public void ForceDropHaptic(){
        hapticInteractor.GetComponent<HandGrabInteraction>().Release();
        hapticInteractor.GetComponent<SphereCollider>().enabled = false;
    }


    // When an item is picked up, all colliders (without isTrigger) should be disabled
    // Otherwise when moving around an item the user would accidentally bump against other objects
    //[SynchronizableMethod]
    public void DisableAllColliders()
    {
        //BroadcastRemoteMethod("NetworkDisableColliders");
        NetworkDisableColliders();
        GameObject.Find("TaskProgression").GetComponent<TaskProgression>().timerRunning = true;
    }

    // enable the colliders again when the object is dropped
    
    //[SynchronizableMethod]
    public void EnableAllColliders()
    {
        if (!otherUserPickedUp) {
            //BroadcastRemoteMethod("NetworkEnableColliders");
            NetworkEnableColliders();
        }
    }
    

    //[SynchronizableMethod]
    public void NetworkDisableColliders(){
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                collider.enabled = false;
            }
        }
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        /*
        if (GetComponent<TransformSynchronizable>())
            GetComponent<TransformSynchronizable>().enabled = true;
        */
    }

    //[SynchronizableMethod]
    public void NetworkEnableColliders(){
        if (otherUserPickedUp) return;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        /*
        if (GetComponent<TransformSynchronizable>())
            GetComponent<TransformSynchronizable>().enabled = false;
        */
    }

}
