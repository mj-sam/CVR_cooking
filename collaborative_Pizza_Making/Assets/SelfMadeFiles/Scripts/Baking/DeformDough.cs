using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(InputData))]
public class DeformDough : NetworkBehaviour
{
    private InputData _inputData;
    private NetworkTaskProgression _networkTaskProgression;
    private NetworkGameSpawner _networkSpawner;
    
    public NetworkVariable<int> _enter = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> _out= new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    
    [ServerRpc (RequireOwnership = false)]
    public void EnterRollerServerRpc()
    {
        _enter.Value++;
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ExitRollerServerRpc()
    {
        _out.Value++;
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void DeactivateServerRpc()
    {
        _out.Value = 0;
        _enter.Value = 0;
    }
    protected virtual void Start()
    {
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        _networkSpawner = GameObject.Find("TaskProgression").GetComponent<NetworkGameSpawner>();
        _inputData = GetComponent<InputData>();
    }
    
    private void FixedUpdate()
    {
        if (_out.Value >2 && _enter.Value > 2)
        {
            DeformMesh();
        }
    }
    
    /// <summary> Actually deform the mesh </summary>
    protected void DeformMesh()
    {
        Debug.Log("Deform Mesh run");
        //======================
        Vector3 lhandVelocity;
        Vector3 rhandVelocity;
        //======================
        Vector3 _posDough;
        Vector3 _posLeftHand;
        Vector3 _posRightHand; 
        //======================
        // check if the velocity is correct
        _inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity);
        lhandVelocity = leftVelocity - new Vector3(0,leftVelocity.y,0);
        _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity);
        rhandVelocity = rightVelocity - new Vector3(0,rightVelocity.y,0);
        //======================
        // Right Hand
        _inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPosition);
        // Left Hand
        _inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPosition);
        //======================
        // If the hands are moving in the horizontal direction enough
        if (((rhandVelocity.magnitude > 0.15f && (rightVelocity.y * rightVelocity.y) < 0.04f) ||
             (lhandVelocity.magnitude > 0.15f && (leftVelocity.y * leftVelocity.y) < 0.04f))&& !_networkTaskProgression.doughSpread.Value)
        {
            Debug.Log("start transforming");
            _networkSpawner.ConvertDoughToCrust();
            FinishDough();
            DeactivateServerRpc();
        }
    }

    private void FinishDough()
    {
        _networkTaskProgression.SetHapticVibration(1f, 1f);
        gameObject.layer = LayerMask.NameToLayer("Default");
        
        _networkTaskProgression.doughSpreadServerRpc(true);// = true;
        
        // make tomato drawable
        Transform crust = transform.Find("PizzaCrust");
        if (crust != null)
        {
            //Debug.Log("Crust found");
            Transform plane = crust.Find("SaucePlane");
            if (plane != null)
            {   //Debug.Log("Suase plane found");
                plane.GetComponent<TomatoDisplay>().drawable = true;
            }
        }
        
    }

    
}