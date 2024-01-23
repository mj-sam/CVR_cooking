using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkKenimatic : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
    }
}