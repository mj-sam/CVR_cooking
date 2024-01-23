using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractionManager : XRInteractionManager
{
    public override void SelectEnter(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        interactable.transform.gameObject.GetComponent<GrabbedItem>().Grab(interactor);
        base.SelectEnter(interactor, interactable);
    }

    public override void SelectExit(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        interactable.transform.gameObject.GetComponent<GrabbedItem>().Release();
        base.SelectExit(interactor, interactable);
    }


    public void ForceDrop(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        base.SelectExit(interactor, interactable);
        string handpath = "VR_Player(Clone)";
        if (GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>().isServer){
            handpath = handpath + "Server";
        }
        handpath = handpath + "/XR Origin/Camera Offset/" + interactor.transform.gameObject.name;
        GameObject.Find(handpath).GetComponent<SphereCollider>().enabled = false;
    }
}
