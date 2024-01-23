using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Interaction;

public class SliceToppings : MonoBehaviour
{
    private GameObject _instTopping;
    public float delay = 0.04f;
    private bool _canCut = true;
    private TaskProgression _taskProgression;
    private NetworkTaskProgression _networkTaskProgression;

    private GameSpawner _spawner;
    private NetworkGameSpawner _NetworkSpawner;

    private bool _pickedUp = false;

    private bool _aboveCuttingBoard = false;

    private void Start(){
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();

        _spawner = GameObject.Find("TaskProgression").GetComponent<GameSpawner>();
        _NetworkSpawner = GameObject.Find("TaskProgression").GetComponent<NetworkGameSpawner>();
    }

    // If knife collides with predefined collider of sausage or bellpepper a prefab of the topping is spawned in
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CuttingBoard")){
            _aboveCuttingBoard = true;
        }
        if (!_pickedUp || !_aboveCuttingBoard) return;
        Vector3 spawnSize;
        float spawnDistance;
        if (_canCut && other.CompareTag("Sausage")){
            //_taskProgression.SausageCut();
            _networkTaskProgression.SausageCut();
            spawnSize = new Vector3(0.05f,0.002f,0.05f);
            spawnDistance = 0.03f;
            
            _canCut = false;
            StartCoroutine(CutDelayCoroutine());

            Vector3 hitPos = other.transform.position;
            // Vector3 direction = other.transform.TransformDirection(spawnDirection);
            Vector3 direction = Vector3.right;

            hitPos -= direction * spawnDistance;
        
            // Network Spawn sausage
            _NetworkSpawner.SpawnToppingServerRpc(1,hitPos,Quaternion.identity,spawnSize);
            
            
            //_instTopping = _spawner.SpawnSausageSlice( hitPos, Quaternion.identity, spawnSize);
            //_instTopping.transform.LookAt(hitPos);
            //_instTopping.transform.Rotate(Vector3.back, 90f);
            _networkTaskProgression.SetHapticVibration(0.3f,0.3f);
        }
        else if (_canCut && other.CompareTag("BellPepper")){
            //_taskProgression.BellPepperCut();
            _networkTaskProgression.BellPepperCut();
            spawnSize = new Vector3(0.0018f,0.0018f,0.0018f);
            spawnDistance = 0.12f;
            
            _canCut = false;
            StartCoroutine(CutDelayCoroutine());

            Vector3 hitPos = other.transform.position;
            // Vector3 direction = other.transform.TransformDirection(spawnDirection);
            Vector3 direction = Vector3.right;

            hitPos -= direction * spawnDistance;
        
            _NetworkSpawner.SpawnToppingServerRpc(0,hitPos,Quaternion.identity,spawnSize);

            //_instTopping = _spawner.SpawnBellSlice(hitPos, Quaternion.identity, spawnSize);
            //_instTopping.transform.LookAt(hitPos);
            //_instTopping.transform.Rotate(Vector3.back, 90f);
            _networkTaskProgression.SetHapticVibration(0.3f,0.3f);
            
            
        }
        else return;
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("CuttingBoard")){
            _aboveCuttingBoard = false;
        }
    }

    // The knife can only cut again after a small delay
    private IEnumerator CutDelayCoroutine()
    {
        yield return new WaitForSeconds(delay);
        _canCut = true;
    }

    public void PickUp(){
        _pickedUp = true;
    }

    public void LetGo(){
        _pickedUp = false;
    }
}