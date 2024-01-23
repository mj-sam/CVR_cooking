using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
//using Alteruna;


// Handles how objects should be rotaded and positioned when grabbed with both hands
public class DoubleXRGrabInteractable : XRGrabInteractable
{

    [SerializeField] private Transform secondAttachTransform;
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        // Grabbed with one hand -> this script acts as normal XRGrabInteractable
        if (interactorsSelecting.Count == 1){
            base.ProcessInteractable(updatePhase);
            // this.gameObject.GetComponent<PizzaDeformer>().GrabbedWithTwoHands(false);
        }
        // Grabbed with both hands -> own implementation for object position and rotation
        else if (interactorsSelecting.Count == 2 &&
            updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            // this.gameObject.GetComponent<PizzaDeformer>().GrabbedWithTwoHands(true);
            ProcessDoubleGrip();
        }
    }

    private void ProcessDoubleGrip()
    {
        Transform firstAttach = GetAttachTransform(null);
        Transform firstHand = interactorsSelecting[0].transform;
        Transform secondAttach = secondAttachTransform;
        Transform secondHand = interactorsSelecting[1].transform;
 
        Vector3 directionBetweenHands = secondHand.position - firstHand.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionBetweenHands, firstHand.up);

        Vector3 worldDirectionFromHandleToBase = transform.position - firstAttach.position;
        Vector3 localDirectionFromHandleToBase = transform.InverseTransformDirection(worldDirectionFromHandleToBase);
        Vector3 targetPosition = firstHand.position + targetRotation * localDirectionFromHandleToBase;

        transform.SetPositionAndRotation(targetPosition, targetRotation);
    }

    protected override void Grab()
    {
        if (interactorsSelecting.Count == 1){
            base.Grab();
        }
    }

    protected override void Drop()
    {
        if (!isSelected){
            base.Drop();
            this.gameObject.GetComponent<GrabbedItem>().EnableAllColliders();
        }
    }

}
