using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PizzaShovel : NetworkBehaviour
{
    private TaskProgression _taskProgression;
    private NetworkTaskProgression _networkTaskProgression;
    private GameSpawner _spawner;
    private NetworkGameSpawner _networkGameSpawner;
    private GameObject pizza;
    private NetworkVariable<bool> pizzaPickedUp= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    private ulong pizzaObjectId;
    private ulong shovelObjectId;
    private ulong dinnerObjectId;
    private Transform ShovelTransform;
    private void Start(){
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();

        _spawner = GameObject.Find("TaskProgression").GetComponent<GameSpawner>();
        _networkGameSpawner = GameObject.Find("TaskProgression").GetComponent<NetworkGameSpawner>();
        ShovelTransform = GetComponent<Transform>();
    }

    // Pick up pizza with the pizza shovel if either
    // - pizza is ready to be put into the oven
    // - pizza is baked and ready to be removed from the oven
    private void OnTriggerEnter(Collider other)
    {
        //--------------------------------------------
        // If pizza is ready to be placed in the oven, the pizza shovel can pick it up by colliding with it
        // Same if pizza is baked and can be removed from the oven
        //
        if (!pizzaPickedUp.Value &&
            other.CompareTag("Pizza") && 
            _networkTaskProgression.pizzaFinished.Value && 
            (!_networkTaskProgression.pizzaInOven.Value || (_networkTaskProgression.pizzaBaked.Value && _networkTaskProgression.ovenOpen.Value))){
            _networkTaskProgression.SetHapticVibration(0.3f,0.3f);
            pizzaObjectId = other.GetComponent<NetworkObject>().NetworkObjectId;
            shovelObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;
            //pizza = other.gameObject;
            SetPizzaPickedUpServerRpc(true);
            PickUpPizzaServerRpc(shovelObjectId, pizzaObjectId);
        }
        if (_networkTaskProgression.pizzaInOven.Value) SetPizzaPickedUpServerRpc(false);
        //---------------------------------------------
        // Drop the pizza to the plate
        // If the pizza is baked the pizza can be transferred from the pizza shovel to the dinner plate
        if (_networkTaskProgression.pizzaBaked.Value && other.CompareTag("DinnerPlate"))
        {
            dinnerObjectId = other.GetComponent<NetworkObject>().NetworkObjectId;
            if (dinnerObjectId != null)
            {
                DropPizzaToPlateServerRpc(dinnerObjectId, _networkGameSpawner._pizzaCrust.GetComponent<NetworkObject>().NetworkObjectId);
                _networkTaskProgression.finishedServerRpc(true); // = true;
                _networkTaskProgression.UpdateInstruction();
            }
        }
    }
    
    

    [ServerRpc(RequireOwnership = false)]
    public void DropPizzaToPlateServerRpc(ulong dinnerObjectId, ulong PizzaObjID)
    {
        pizza.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        NetworkObject dinnerObj = FindNetworkObject(dinnerObjectId);
        NetworkObject PizzaObj = FindNetworkObject(PizzaObjID);

        PizzaObj.gameObject.transform.SetParent(dinnerObj.gameObject.transform);
        PizzaObj.gameObject.transform.localPosition = new Vector3(0,0.0453f,0);
        //PizzaObj.gameObject.transform.localScale = new Vector3(1,1,1);
        PizzaObj.gameObject.transform.localRotation = Quaternion.identity;//oventrayObj.gameObject.transform.rotation;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void PickUpPizzaServerRpc(ulong ShovelObjID, ulong PizzaObjID)
    {
        
        NetworkObject ShovelObj = FindNetworkObject(ShovelObjID);
        NetworkObject PizzaObj = FindNetworkObject(PizzaObjID);
        pizza = PizzaObj.gameObject;
        // Reparent the GameObject
        pizza.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        pizza.transform.SetParent(ShovelObj.gameObject.transform);
        pizza.transform.localPosition = new  Vector3(-0.070f,0,0.4f);//Vector3(-2.0652f, 0.969717026f, -0.683157444f);//
        pizza.transform.rotation = Quaternion.identity;//ShovelObj.gameObject.transform.rotation;
        pizza.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        pizza.GetComponent<Rigidbody>().useGravity = false;
        pizza.GetComponent<Rigidbody>().isKinematic = true;
        pizza.GetComponent<MeshCollider>().enabled = false;
        
        // Disable all colliders of the pizza
        MeshCollider[] colliders = GetComponentsInChildren<MeshCollider>();
        foreach (MeshCollider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                collider.convex = true;
                collider.isTrigger = true;
            }
        }
        pizzaPickedUp.Value = true;
        
    }

    private void FixedUpdate()
    {
        if (IsServer && pizzaPickedUp.Value && pizza != null)
        {
            pizza.transform.rotation = ShovelTransform.rotation;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPizzaPickedUpServerRpc(bool value)
    {
        pizzaPickedUp.Value = value;
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
