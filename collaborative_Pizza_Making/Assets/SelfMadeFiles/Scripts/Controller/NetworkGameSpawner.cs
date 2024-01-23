using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;


public class NetworkGameSpawner : NetworkBehaviour
{
    // Variables to hold prefabs
    public GameObject doughBallPrefab;
    public GameObject pizzaPrefab;
    public GameObject pizzaDoughBallPrefab;
    public GameObject pizzaCrustPrefab;
    public GameObject sausageSlicePrefab;
    public GameObject sausageSliceGhostPrefab;
    public GameObject pepperSlicePrefab;
    public GameObject pepperSliceGhostPrefab;

    //===================================
    // Do not need preValue in unity !!!!!!!!!!!!!! just shown public for debug
    public GameObject _doughBall;
    public GameObject _pizzaDoughBall;
    public GameObject _pizzaCrust;
    
    // Private Variables 
    private NetworkObject _doughBallNetwork;    
    private NetworkObject _pizzaNetwork;
    private GameObject _pizzaDough; // Reference to the "dough" child object
    private GameObject _pizzaDoughNetwork;
    //private GameObject _pizzaCrust; // Reference to the "crust" child object
    private GameObject _pizzaCrustNetwork;
    private GameObject _saucePlane; // Reference to the "crust" child object
    private GameObject _saucePlaneNetwork;

    private GameObject _pizzaSet;
    //private GameObject SausageSlices{};
    private Vector3 _pizzaDoughBallPosition;
    private NetworkTaskProgression _networkTaskProgression;
    //===================
    // Method to get properties
    public void Start()
    {
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();

    }

    public void SetPizza(GameObject pizza)
    {
        _pizzaSet = pizza;
    }

    public GameObject GetPizza()
    {
        return _pizzaSet;
    }
    //===================================
    public GameObject SpawnGhostTopping(int SpawnID, Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {   
        GameObject newSlice;
        if (SpawnID == 1)
        {   //instantiate the slice locally
            newSlice = Instantiate(sausageSliceGhostPrefab, spawnPos, spawnOrientation);
            //instantiate it over the network
            newSlice.GetComponent<NetworkObject>().Spawn();
        } else  {
            //instantiate the slice locally
            newSlice = Instantiate(pepperSliceGhostPrefab, spawnPos, spawnOrientation);
            //instantiate it over the network
            newSlice.GetComponent<NetworkObject>().Spawn();
        }
        newSlice.transform.localScale = spawnScale;
        return newSlice;
    }
    
    
    
    public void Despawn(GameObject obj)
    {
        obj.transform.position = new Vector3(1000,0,0);
    }
    

    public void DeSpawnDoughBall()
    {
        DeSpawnDoughBallServerRpc();
        Debug.Log("Dough ball disappeared");       
    }
    public void SpawnPizza(Vector3 spawnPosition, Quaternion orientation)
    { 
        SpawnPizzaDoughBallServerRpc(spawnPosition, orientation);    
    }

    public void ConvertDoughToCrust()
    {
        ConvertDoughToCrustServerRpc();
    }
   
    //==========================================================================
    //============= Method that has to be synced across network ===============
    //==========================================================================
    
    
    // =================   Spawn the Dough ball ==============================
    //Spawn the Dough ball on network after kneading
    [ServerRpc(RequireOwnership = false)]
    public void SpawnDoughBallServerRpc(Vector3 spawnPosition, Quaternion orientation)
    {
        if (_doughBall == null && !_networkTaskProgression.doughKneaded.Value)
        {
            _doughBall = Instantiate(doughBallPrefab, spawnPosition, orientation);
            NetworkObject networkDoughBall = _doughBall.GetComponent<NetworkObject>();
            networkDoughBall.Spawn();
            // pass the network Id to client so they can find it there
            SpawnDoughBallClientRpc(networkDoughBall.NetworkObjectId);   
        }
    }
    [ClientRpc]
    public void SpawnDoughBallClientRpc(ulong objectId)
    {
        // find the spawned dough ball
        NetworkObject networkObject = GetNetworkObjectById(objectId);
        if (networkObject != null)
        {
            _doughBall = networkObject.gameObject;
        }
    }
    
    // Despawn the doghball
    [ServerRpc(RequireOwnership = false)]
    public void DeSpawnDoughBallServerRpc()
    {
        
        try{
            _doughBall.GetComponent<NetworkObject>().Despawn();
        }
        catch (NotServerException ex)
        {
            Debug.LogError("Attempted to despawn an object: " + ex.Message);
        }
    }
    
    //---------------------------------------------------------------------------   
    //======================= Convert Pizza DoughBall to Crust ========================    
    [ServerRpc(RequireOwnership = false)]
    private void ConvertDoughToCrustServerRpc()
    {
        
        //Spawn the pizza Crust
        SpawnPizzaCrustServerRpc(
            _pizzaDoughBallPosition, 
            Quaternion.identity);
        
        //Despawn the Pizza Dough ball
        DeSpawnPizzaDoughBallServerRpc();
        
    }
    [ClientRpc]
    public void ConvertDoughToCrustClientRpc()
    {
        //Despawn the Pizza Dough ball
        _pizzaDough.SetActive(false);
        
        //Spawn the pizza Crust
        _pizzaCrust.SetActive(true);
    }
    
    //========================== Pizza Doughball ================================
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPizzaDoughBallServerRpc(Vector3 spawnPosition, Quaternion orientation)
    {
        if (_pizzaDoughBall.GetComponent<DoughToCrust>().pizza == null)
        {
            _pizzaDoughBall = Instantiate(pizzaDoughBallPrefab, spawnPosition, orientation);
            NetworkObject networkDoughBall = _pizzaDoughBall.GetComponent<NetworkObject>();
            networkDoughBall.Spawn();
            _pizzaDoughBall.GetComponent<DoughToCrust>().pizza = _pizzaDoughBall; // test for bug later
            SpawnPizzaDoughBallClientRpc(networkDoughBall.NetworkObjectId);
            _pizzaDoughBallPosition = _pizzaDoughBall.transform.position; // to prevent null access after object despawned   
        }
    }

    [ClientRpc]
    public void SpawnPizzaDoughBallClientRpc(ulong pizzaObjectId)
    {
        // find the spawned pizza on clients
        NetworkObject networkObject = GetNetworkObjectById(pizzaObjectId);
        if (networkObject != null)
        {
            _pizzaDoughBall = networkObject.gameObject;
            _pizzaDoughBallPosition = _pizzaDoughBall.transform.position; // to prevent null access after object despawned
            _pizzaDoughBall.GetComponent<DoughToCrust>().pizza = _pizzaDoughBall; // also had to be set on clients
        }
        else
        {
            Debug.Log("could not find the local Pizza Dough Ball object");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void DeSpawnPizzaDoughBallServerRpc()
    {
        NetworkObject networkDoughBall = _pizzaDoughBall.GetComponent<NetworkObject>();
        //networkDoughBall.Despawn();
        // keep the object on network but deactivate it
        networkDoughBall.gameObject.SetActive(false);
        DeSpawnPizzaDoughBallClientRpc();
    }
    [ClientRpc]
    public void DeSpawnPizzaDoughBallClientRpc()
    {
        NetworkObject networkDoughBall = _pizzaDoughBall.GetComponent<NetworkObject>();
        networkDoughBall.gameObject.SetActive(false);
    }
    
    //========================== Pizza Crust ======================
    //Spawn the pizza
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPizzaCrustServerRpc(Vector3 spawnPosition, Quaternion orientation)
    {
        if (!_networkTaskProgression.doughSpread.Value)
        {
            _networkTaskProgression.doughSpreadServerRpc(true);// = true;
            _pizzaCrust = Instantiate(pizzaCrustPrefab, spawnPosition, orientation);
            _networkTaskProgression.pizza = _pizzaCrust;
            NetworkObject networkPizzaCrust = _pizzaCrust.GetComponent<NetworkObject>();
            networkPizzaCrust.Spawn();
            //networkPizzaCrust.SpawnWithOwnership();
            _saucePlane = _pizzaCrust.transform.Find("SaucePlane").gameObject;
            // on the server
            GameObject.Find("Spoon").GetComponent<NetworkTomatoDrawer>()._pizzaDisplay = GameObject.Find("SaucePlane").GetComponent<TomatoDisplay>();
        
            //GameObject.Find("Spoon").GetComponent<NetworkTomatoDrawer>()._pizza = GameObject.Find("SaucePlane").GetComponent<TomatoDisplay>();
        
            SpawnPizzaCrustClientRpc(networkPizzaCrust.NetworkObjectId);    
        }
        
    }

    [ClientRpc]
    public void SpawnPizzaCrustClientRpc(ulong pizzaObjectId)
    {
        // find the spawned pizza on clients
        NetworkObject networkObject = GetNetworkObjectById(pizzaObjectId);
        if (networkObject != null)
        {
            _pizzaCrust = networkObject.gameObject;
            _networkTaskProgression.pizza = _pizzaCrust;
            // sync with server
            _saucePlane = _pizzaCrust.transform.Find("SaucePlane").gameObject;
            
            // maybe use direct sause plane instead : TODO
            GameObject.Find("Spoon").GetComponent<NetworkTomatoDrawer>()._pizzaDisplay = _saucePlane.GetComponent<TomatoDisplay>();
        }
        else
        {
            Debug.Log("could not find the local Pizza CRUST object");
        }
    }
    
    //----------------------------------------------------------------------------------------
    //============================== Pizza Topping ===========================================
    
    [ServerRpc(RequireOwnership = false)] 
    public void SpawnToppingServerRpc(int SpawnID, Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {
        GameObject newSlice;
        if (SpawnID == 1)
        {
            //intantiate locally
            newSlice = Instantiate(sausageSlicePrefab, spawnPos, spawnOrientation);
            // make them to look like they have been cut from
            // the piece
            newSlice.transform.LookAt(spawnPos);
            newSlice.transform.Rotate(Vector3.back, 90f);
            newSlice.GetComponent<Topping>().pizza = _pizzaCrust;
            //tell others about this intantce 
            newSlice.GetComponent<NetworkObject>().Spawn();
        } else  {
            //intantiate locally
            newSlice = Instantiate(pepperSlicePrefab, spawnPos, spawnOrientation);
            // make them to look like they have been cut from
            // the piece
            newSlice.transform.LookAt(spawnPos);
            newSlice.transform.Rotate(Vector3.back, 90f);
            newSlice.GetComponent<Topping>().pizza = _pizzaCrust;
            //tell others about this intantce 
            newSlice.GetComponent<NetworkObject>().Spawn();
        }
        newSlice.transform.localScale = spawnScale;
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void ChangeToppingServerRpc(ulong spawnableID, ulong PizzaObjID, Vector3 position, Quaternion  rotation, Vector3 scale)
    {
        NetworkObject toppingNetworkObject = GetNetworkObjectById(spawnableID);
        GameObject toppingGameObject = toppingNetworkObject.gameObject;
         
        NetworkObject PizzaNetworkObj = GetNetworkObjectById(PizzaObjID);
        GameObject foundPizza = PizzaNetworkObj.gameObject;
        
        // Change transform properties
        toppingGameObject.transform.position = position;
        toppingGameObject.transform.rotation = rotation;
        
        //toppingGameObject.transform.localScale = scale;
        toppingGameObject.transform.SetParent(foundPizza.transform);
        
        //_networkSpawner.SpawnToppingServerRpc();
        //ChangeToppingClientRpc(spawnableID, PizzaObjID); 
    }
    [ClientRpc]
    public void ChangeToppingClientRpc(ulong spawnableID, ulong PizzaObjID)
    {
        NetworkObject toppingNetworkObject = GetNetworkObjectById(spawnableID);
        GameObject toppingGameObject = toppingNetworkObject.gameObject;
        
        NetworkObject PizzaNetworkObj = GetNetworkObjectById(PizzaObjID);
        GameObject foundPizza = PizzaNetworkObj.gameObject;
        Rigidbody _rigidbody = toppingGameObject.GetComponent<Rigidbody>();
        
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
    }

    private NetworkObject GetNetworkObjectById(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            return networkObject;
        }
        return null;
    }
    
}