using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script that makes the hands have colliders, objects can be moved by touch and grabbing instead of only grabbing



// NOT used in final version




public class CollidingHands : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    public Renderer nonPhysicalHand;
    private float showNonPhysicalHandDistance = 0.05f;
    private Collider[] handColliders;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handColliders = GetComponentsInChildren<Collider>();
    }

    public void EnableHandCollider()
    {
        foreach(var item in handColliders){
            item.enabled = true;
        }
    }

    public void EnableHandColliderDelay(float delay){
        Invoke("EnableHandCollider", delay);
    }

    public void DisableHandCollider()
    {
        CancelInvoke();
        foreach(var item in handColliders){
            item.enabled = false;
        }
    }

    private void Update(){
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > showNonPhysicalHandDistance){
            nonPhysicalHand.enabled = true;
        }
        else{
            nonPhysicalHand.enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = (target.position - transform.position)/Time.fixedDeltaTime;

        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angelInDegree, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegree = angelInDegree * rotationAxis;

        rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }
}
