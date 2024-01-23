using UnityEngine;

public class XROriginReferences : MonoBehaviour
{
    public static XROriginReferences Instance { get; private set; }

    [SerializeField] private Transform xrOriginHead, 
        xrOriginLeftHand, 
        xrOriginRightHand,
        xrOriginLeftHandAnimator, 
        xrOriginRightHandAnimator;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    
    public Transform GetXrOriginHead()
    {
        return xrOriginHead;
    }
    
    public Transform GetXrOriginLeftHand()
    {
        return xrOriginLeftHand;
    }
    
    public Transform GetXrOriginRightHand()
    {
        return xrOriginRightHand;
    }
    public Transform GetXrOriginLeftHandAnimator()
    {
        return xrOriginLeftHandAnimator;
    }
    public Transform GetXrOriginRightHandAnimator()
    {
        return xrOriginRightHandAnimator;
    }
}

