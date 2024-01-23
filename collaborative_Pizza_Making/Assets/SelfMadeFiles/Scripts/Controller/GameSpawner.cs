using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameSpawner : MonoBehaviour
{
    // Array to hold prefabs
    public GameObject doughBallPrefab;
    public GameObject pizzaPrefab;
    public GameObject sausageSlicePrefab;
    public GameObject sausageSliceGhostPrefab;
    public GameObject pepperSlicePrefab;

    public GameObject pepperSliceGhostPrefab;

    //===================================
    public GameObject _doughBall;
    public GameObject _pizza;
    private GameObject _pizzaDough; // Reference to the "dough" child object
    private GameObject _pizzaCrust; // Reference to the "crust" child object
    private GameObject _saucePlane; // Reference to the "crust" child object

    private GameObject _pizzaSet;
    //private GameObject SausageSlices{};

//===================
    // Method to get properties
    public void SetPizza(GameObject pizza)
    {
        _pizzaSet = pizza;
    }

    public GameObject GetPizza()
    {
        return _pizzaSet;
    }

    // public GameObject GetCrust()
    // {
    //     
    // }

//===================================
    public GameObject SpawnGhostTopping(int SpawnID, Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {   GameObject newSlice;
        if (SpawnID == 1)
        {
            newSlice = Instantiate(sausageSliceGhostPrefab, spawnPos, spawnOrientation);
        } else  {
            newSlice = Instantiate(pepperSliceGhostPrefab, spawnPos, spawnOrientation);
        }
        newSlice.transform.localScale = spawnScale;
        return newSlice;
    }
    
    public GameObject SpawnTopping(int SpawnID, Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {   GameObject newSlice;
        if (SpawnID == 1)
        {
            newSlice = Instantiate(sausageSlicePrefab, spawnPos, spawnOrientation);
        } else  {
            newSlice = Instantiate(pepperSlicePrefab, spawnPos, spawnOrientation);
        }
        newSlice.transform.localScale = spawnScale;
        return newSlice;
    }
    public GameObject SpawnSausageSlice(Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {
        GameObject newSlice;
        newSlice = Instantiate(sausageSlicePrefab, spawnPos, spawnOrientation);
        newSlice.transform.localScale = spawnScale;
        return newSlice;
    }
    public GameObject SpawnBellSlice(Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {
        GameObject newSlice;
        newSlice = Instantiate(pepperSlicePrefab, spawnPos, spawnOrientation);
        newSlice.transform.localScale = spawnScale;
        return newSlice;
    }
    
    public GameObject SpawnSausageGhostSlice(Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {
        GameObject newSlice;
        newSlice = Instantiate(doughBallPrefab, spawnPos, spawnOrientation);
        newSlice.transform.localScale = spawnScale;
        return newSlice;
    }

    public GameObject SpawnBellGhostSlice(Vector3 spawnPos, Quaternion spawnOrientation, Vector3 spawnScale)
    {
        GameObject newSlice;
        newSlice = Instantiate(doughBallPrefab, spawnPos, spawnOrientation);
        newSlice.transform.localScale = spawnScale;
        return newSlice;
    }

    public void Despawn(GameObject obj)
    {
        
        obj.transform.position = new Vector3(1000,0,0);
    }
    public void SpawnDoughBall(Vector3 spawnPosition, Quaternion orientation)
    { 
        _doughBall = Instantiate(doughBallPrefab, spawnPosition, orientation);
    }
    public void DeSpawnDoughBall()
    { 
        _doughBall.SetActive(false);
        //Debug.Log("Dough ball disappeared");       
    }
    public GameObject SpawnPizza(Vector3 spawnPosition, Quaternion orientation)
    { 
        _pizza = Instantiate(pizzaPrefab, spawnPosition, orientation);
        _pizzaCrust = _pizza.transform.Find("PizzaCrust").gameObject;
        _pizzaDough = _pizza.transform.Find("PizzaDough").gameObject;
        _pizzaDough.GetComponent<DoughToCrust>().pizza = _pizza;
        _saucePlane = _pizzaCrust.transform.Find("SaucePlane").gameObject;
        
        _pizzaDough.transform.GetComponent<Renderer>().enabled = true;
        _pizzaCrust.transform.GetComponent<Renderer>().enabled = false;
        //_pizzaCrust.SetActive(false);
        return _pizza;
    }
    public void ConvertDoughToCrust()
    {
        _pizzaDough.transform.GetComponent<Renderer>().enabled = false;
        _pizzaCrust.transform.GetComponent<Renderer>().enabled = true;
        _pizzaDough.SetActive(false);
        _pizzaCrust.SetActive(true);
        Debug.Log("Dough ball ");
        Debug.Log("Crust ball appeared");       
    }
}