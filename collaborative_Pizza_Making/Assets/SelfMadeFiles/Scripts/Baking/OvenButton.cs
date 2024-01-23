using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
//using Alteruna;

public class OvenButton : NetworkBehaviour
{
    public GameObject button;
    bool isPressed = false;
    private GameObject ovenDoor;
    private bool canPush = true;
    private NetworkVariable<float> ovenSpeed = new NetworkVariable<float>(-50, NetworkVariableReadPermission.Everyone);
    private TaskProgression _taskprogression;
    private NetworkTaskProgression _networkTaskProgression;
   //private JointMotor _jm;

    private void Start(){
        ovenDoor = GameObject.Find("OvenDoor");
        _taskprogression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        

    }

    // When an object collides with button, the button is pressed
    private void OnTriggerEnter(Collider other) {
        if (!isPressed && canPush && other.CompareTag("Hand"))
        {
            JointMotor jm = ovenDoor.GetComponent<HingeJoint>().motor;
            if ((jm.targetVelocity < 0 && !_networkTaskProgression.ovenOpen.Value) || (jm.targetVelocity > 0 && _networkTaskProgression.ovenOpen.Value)){
                OvenSpeedToggleServerRpc();
                button.transform.localPosition = new Vector3(0,0.005f,0);
                isPressed = true;
                GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>().SetHapticVibration(0.3f,0.3f);
            }
        }
    }

    // When an object leaves the button collider, the button is released
    private void OnTriggerExit(Collider other) {
        button.transform.localPosition = new Vector3(0,0.015f,0);
        isPressed = false;
        //canPush = false;
        //StartCoroutine(ButtonPressDelayCoroutine());
    }

    // Button can only be pressed again after some delay
    private IEnumerator ButtonPressDelayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        canPush = true;
    }

    // Check if oven door is open or closed, used in taskprogression to check if the pizza is in the oven for long enough
    private void FixedUpdate(){
        if (IsClient) //
        {
            if (ovenDoor.transform.rotation.x < 0.2f)
            {
                //_taskprogression.ovenOpen = false; 
                _networkTaskProgression.ovenOpenServerRpc(false);
            } else {
                //_taskprogression.ovenOpen = true;
                _networkTaskProgression.ovenOpenServerRpc(true); 
            }
        }
        JointMotor _jm = ovenDoor.GetComponent<HingeJoint>().motor;
        _jm.targetVelocity = ovenSpeed.Value;
        ovenDoor.GetComponent<HingeJoint>().motor = _jm;
    }

    [ServerRpc(RequireOwnership = false)] 
    public void OvenSpeedToggleServerRpc()
    {
        ovenSpeed.Value *= -1;
    }
}