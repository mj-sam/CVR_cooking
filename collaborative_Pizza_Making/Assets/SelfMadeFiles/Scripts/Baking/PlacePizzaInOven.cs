using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlacePizzaInOven : NetworkBehaviour
{
    private TaskProgression _taskProgression;
    private NetworkTaskProgression _networkTaskProgression;
    private GameObject _pizza;
    private void Start(){
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        
    }

    // If pizza is ready to be put into the oven and is picked up with the pizza shovel
    // When the pizza collides with the collider in the oven, the pizza is transferred from the pizza shovel to the pizza
    private void OnTriggerEnter(Collider other)
    {
        if (_networkTaskProgression.pizzaFinished.Value &&
            _networkTaskProgression.ovenOpen.Value &&
            other.CompareTag("Pizza") && 
            !_networkTaskProgression.pizzaBaked.Value)
        {
            _pizza = other.gameObject;
            ulong pizzaObjectId = other.GetComponent<NetworkObject>().NetworkObjectId;
            ulong ovenTrayObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;
            DropPizzaOnOvenServerRpc(ovenTrayObjectId,pizzaObjectId);
            //other.gameObject.transform.SetParent(this.transform);
            //other.gameObject.transform.localPosition = new Vector3(0,0.03f,0);
            //other.gameObject.transform.rotation = this.gameObject.transform.rotation;
            //_taskProgression.pizzaInOven = true;
            //_taskProgression.pizza = other.gameObject;
            _networkTaskProgression.pizzaInOvenServerRpc(true);/// = true;
            _networkTaskProgression.pizza = other.gameObject;
        }
    }
    private void FixedUpdate()
    {
        if (IsServer && _networkTaskProgression.pizzaInOven.Value && _pizza != null)
        {
            _pizza.transform.localRotation = Quaternion.identity;
        }
        
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DropPizzaOnOvenServerRpc(ulong oventrayObjID, ulong pizzaObjID)
    {
        
        NetworkObject oventrayObj = FindNetworkObject(oventrayObjID);
        NetworkObject PizzaObj = FindNetworkObject(pizzaObjID);
        PizzaObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        PizzaObj.transform.SetParent(oventrayObj.gameObject.transform);
        PizzaObj.transform.localPosition = new Vector3(0,0,0);
        PizzaObj.transform.localRotation = Quaternion.identity;//oventrayObj.gameObject.transform.rotation;
        PizzaObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }
    
    
    private NetworkObject FindNetworkObject(ulong objectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject networkObject))
        {
            return networkObject;
        }
        return null;
    }
}
