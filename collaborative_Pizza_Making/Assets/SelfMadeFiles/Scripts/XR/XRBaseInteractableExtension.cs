using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

// You can use your project's namespace or a specific namespace for extensions

public static class XRBaseInteractableExtension
{
    public static void ForceDeselect(this XRBaseInteractable interactable)
    {
        interactable.interactionManager.CancelInteractableSelection(interactable);
        Assert.IsFalse(interactable.isSelected);
    }
}

public static class XRBaseInteractorExtension
{
    public static void ForceDeselect(this XRBaseInteractor interactor)
    {
        interactor.interactionManager.CancelInteractorSelection(interactor);
        Assert.IsFalse(interactor.isSelectActive);
    }
}
