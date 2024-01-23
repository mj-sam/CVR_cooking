using System;
using System.Collections;
using System.Collections.Generic;
using GlmSharp;
using UnityEngine;

public class DoughToCrust : MonoBehaviour
{
    public GameObject pizza;
    private NetworkTaskProgression _networkTaskProgression;
    
    // Start is called before the first frame update
    public void Start()
    {
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
    }

    public void EnterRoller()
    {
        //Debug.Log("ENTERED CRUST");
        if (pizza != null)
        {
            // it is odd to use the task progression after checking pizza
            // it is because in the last version we had pizza crust and ball in one componnent
            // but in the muliplayer version we had to deparent them due to the NOG constraint in handling parents
            _networkTaskProgression.GetComponent<DeformDough>().EnterRollerServerRpc();
        }
    }

    public void ExitRoller()
    {
        //Debug.Log("EXIT CRUST");
        if (pizza != null)
        {
            _networkTaskProgression.GetComponent<DeformDough>().ExitRollerServerRpc();
        }
    }
    
}
