using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaDeformer : MonoBehaviour
{
    // Rolling pin can only deform the doughball when the ball is placed on the indicator (ballIsPlaced variable)
    public bool ballIsPlaced = false;
    
    // Rolling pin can only deform the doughball when it is grabbed with both hands
    private GameObject currentObject;

    private int hands = 0;

    // calling Activate on the DoughBall object makes it deformable by the rolling pin
    protected void OnTriggerEnter(Collider other)
    {
        if (this.CompareTag("RollingPin") && other.CompareTag("DoughBall") && ballIsPlaced)
        {
            currentObject = other.gameObject;
            //Debug.Log("Hands:");
            //Debug.Log(hands);
            if (hands == 2){
                currentObject.GetComponent<DoughToCrust>().EnterRoller();
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (this.CompareTag("RollingPin") && other.CompareTag("DoughBall") && ballIsPlaced)
        {
            if (currentObject != null){
                //currentObject.GetComponent<DeformDough>().Deactivate();
                currentObject.GetComponent<DoughToCrust>().ExitRoller();
            }
            currentObject = null;
            
        }
    }

    public void PickUp(){
        hands += 1;
        //if (hands == 2 && currentObject != null) currentObject.GetComponent<DeformDough>().Activate();
    }
    public void LetGo(){
        hands -= 1;
    }
}