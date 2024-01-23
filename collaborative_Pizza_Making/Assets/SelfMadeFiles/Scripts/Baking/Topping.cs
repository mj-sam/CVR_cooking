using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Manus.Interaction;
using System;
using SelfMadeFiles.Scripts.Network.components;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;

//using Alteruna;

public class Topping : NetworkBehaviour
{
    private RaycastHit _touch;
    // prefab of the type of topping
    public int spawnableID;
    public GameObject pizza; // pizza is set uppon initialization to the crust
    [SerializeField] private ToppingType toppingType;
    
   
    private Vector3 _scale;
    private Vector3 _sausageGhostScale;
    private Vector3 _pepperGhostScale;
    
    // Indicator that shows where the topping will be placed when the user will release the topping
    public GameObject objectIndicator;
    private float height = 0.0001f;
    private bool pickedUp = false;
    
    private TaskProgression _taskProgression;
    private NetworkTaskProgression _networkTaskProgression;

    private GameSpawner _spawner;
    private bool _droppable= false;
    private NetworkGameSpawner _networkSpawner;
    private ulong _toppingObjectId;
    private ulong _pizzaObjectID;
    enum ToppingType{
        BellPepper,
        Sausage
    }
    

    private void Start(){
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        _spawner = GameObject.Find("TaskProgression").GetComponent<GameSpawner>();
        _networkSpawner = GameObject.Find("TaskProgression").GetComponent<NetworkGameSpawner>();
        _toppingObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;
        
        // select an appropriate scale
        if (toppingType == ToppingType.Sausage)
            _scale = new Vector3(0.05f,0.002f,0.05f);
        else if (toppingType == ToppingType.BellPepper)
            _scale = new Vector3(0.0018f,0.0018f,0.0018f);
        // assign pizza if called before creating pitza it will generate an error
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) return;
        //--------------------------------------------
        // If pizza is ready to be placed in the oven, the pizza shovel can pick it up by colliding with it
        // Same if pizza is baked and can be removed from the oven
        //
        if (pickedUp && other.CompareTag("DropableArea"))
        {
            _droppable = true;
        }
    } 
    private void OnTriggerExit(Collider other)
    {
        //--------------------------------------------
        // If pizza is ready to be placed in the oven, the pizza shovel can pick it up by colliding with it
        // Same if pizza is baked and can be removed from the oven
        //
        if(!IsOwner) return;
        if (pickedUp && other.CompareTag("DropableArea"))
        {
            _droppable = false;
        }
        if (objectIndicator != null){
            Destroy(objectIndicator);
        }
    } 
    void Update(){
        // Rays casted by the held topping should only collide with the tomato plane
        if(!IsOwner) return;
        if(!_droppable) return; // prevents placing on the crust
        int mask = Physics.AllLayers;
        mask &= ~11;
        if (!pickedUp || _networkTaskProgression.tomatoSpread.Value == false) return;
        if (Physics.Raycast(this.transform.position, 
                Vector3.down,
                out _touch, 0.16f, mask) && 
            _touch.transform.CompareTag("TomatoSaucePizza"))
        {
            TomatoDisplay _pizza = _touch.transform.GetComponent<TomatoDisplay>();
            //_touch.
            //int x = (int)(_touch.textureCoord.x * _pizza.textureSize.x);
            //int y = (int)(_touch.textureCoord.y * _pizza.textureSize.y);
            //float dist = new Vector2(x-_pizza.textureSize.x/2, y-_pizza.textureSize.y/2).magnitude;
            
            // Toppings can only be placed close enough to the middle
            if (objectIndicator != null){
               _spawner.Despawn(objectIndicator);
               Destroy(objectIndicator);
            }
            // Create an indicator object showing where the held topping would be placed when released
            if (objectIndicator == null){
                objectIndicator = _spawner.SpawnGhostTopping(spawnableID, 
                    new Vector3(transform.position.x,_touch.transform.position.y+height, transform.position.z), 
                    Quaternion.identity,
                    _scale);
                //objectIndicator.transform.position = new Vector3(transform.position.x,_touch.transform.position.y+height, transform.position.z);
                //objectIndicator.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0f);
            }
        }
    }

    public void PickUp(){
        pickedUp = true;
    }

    // When a topping is released in a location where it can be put on the pizza, it will be placed on the pizza
    public void LetGo(){
        if(!IsOwner) return;
        pickedUp = false;
        if (objectIndicator != null){
            
            if (toppingType == ToppingType.Sausage){
                _networkTaskProgression.PlaceSausage();
            }
            else{
                _networkTaskProgression.PlaceBellPepper();
            }
            
            _networkTaskProgression.SetHapticVibration(0.3f,0.3f);
            if (_networkSpawner._pizzaCrust != null)
            {
                _pizzaObjectID = _networkSpawner._pizzaCrust.GetComponent<NetworkObject>().NetworkObjectId;
                if(IsOwner)
                {
                    ChangeToppingServerRpc(_toppingObjectId, _pizzaObjectID, objectIndicator.transform.position,
                        objectIndicator.transform.rotation, _scale);
                }
                
                // TODO CHECK IF IT WORKS :
                // try
                // {
                //     Destroy(this.GetComponent<ClientInteractableOwnership>());
                //     Destroy(this.GetComponent<XRGrabInteractable>());
                //     Destroy(this.GetComponent<ThrowableObject>());
                //     Destroy(this.GetComponent<GrabbableObject>());
                //     Destroy(this.GetComponent<Rigidbody>());    
                // }
                // catch (NotServerException ex)
                // {
                //     Debug.LogError("Attempted to delete a NetworkObject instance: " + ex.Message);
                // }
                _spawner.Despawn(objectIndicator);
                Destroy(objectIndicator);
                //RemoveTopping();
            }
            else
            {
                pizza = GameObject.Find("");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeToppingServerRpc(ulong spawnableID, ulong PizzaObjID, Vector3 position, Quaternion  rotation, Vector3 scale)
    {
        NetworkObject ToppingObj = FindNetworkObject(spawnableID);
        NetworkObject PizzaObj = FindNetworkObject(PizzaObjID);
        GameObject foundPizza = PizzaObj.gameObject;
        GameObject toppingGameObject = ToppingObj.gameObject;
        Rigidbody _rigidbody = toppingGameObject.GetComponent<Rigidbody>();
        BoxCollider _boxCollider = toppingGameObject.GetComponent<BoxCollider>();
        MeshCollider _meshCollider = toppingGameObject.GetComponent<MeshCollider>();
        
        // Change transform properties
        toppingGameObject.transform.position = position;
        toppingGameObject.transform.rotation = rotation;
        try
        {
            toppingGameObject.transform.SetParent(foundPizza.transform);
            Destroy(ToppingObj.GetComponent<ClientInteractableOwnership>());
            Destroy(ToppingObj.GetComponent<XRGrabInteractable>());
            Destroy(ToppingObj.GetComponent<ThrowableObject>());
            Destroy(ToppingObj.GetComponent<GrabbableObject>());
            Destroy(ToppingObj.GetComponent<Rigidbody>());
        }
        catch (NotServerException ex)
        {
            Debug.LogError("Attempted to reparent a NetworkObject on a non-server instance: " + ex.Message);
        }
        // Modify Rigidbody
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
        //
        //_networkSpawner.SpawnToppingServerRpc();
        ChangeToppingClientRpc(spawnableID, PizzaObjID);

    }
    [ClientRpc]
    public void ChangeToppingClientRpc(ulong spawnableID, ulong PizzaObjID)
    {
        NetworkObject ToppingObj = FindNetworkObject(spawnableID);
        NetworkObject PizzaObj = FindNetworkObject(PizzaObjID);
        Rigidbody _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
        try
        {
            Destroy(ToppingObj.GetComponent<ClientInteractableOwnership>());
            Destroy(ToppingObj.GetComponent<XRGrabInteractable>());
            Destroy(ToppingObj.GetComponent<ThrowableObject>());
            Destroy(ToppingObj.GetComponent<GrabbableObject>());
            Destroy(ToppingObj.GetComponent<Rigidbody>());    
        }
        catch (NotServerException ex)
        {
            Debug.LogError("Attempted to delete a NetworkObject instance: " + ex.Message);
        }
        
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
