using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
//using Alteruna;

// When the doughball is picked up, this indicator is shown
// Here the douhghball can be placed to start the spreading of the dough
public class DoughIndicator : MonoBehaviour
{
    //public GameObject pizzaClone;
    private TaskProgression _taskProgression;
    private GameSpawner _spawner;
    //private NetworkGameSpawner _networkSpawner
    private NetworkGameSpawner _networkSpawner;
    private NetworkTaskProgression _networkTaskProgression;

    private GameObject _pizza;
    private GameObject _doughBall;
    private GameObject _indicatorBall;
    private Vector3 _position1;
    private Vector3 _position2;

    //private Spawner spawner;
    //public bool flattened = false;

    void Start()
    {
        _indicatorBall = GameObject.Find("BallIndicator");
        _indicatorBall.GetComponent<MeshRenderer>().enabled = false;
        //_spawner = GameObject.Find("TaskProgression").GetComponent<Spawner>();
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();

        
        _spawner = GameObject.Find("TaskProgression").GetComponent<GameSpawner>();
        _networkSpawner = GameObject.Find("TaskProgression").GetComponent<NetworkGameSpawner>();
    }


    public void PickUpBall(){
        _indicatorBall.GetComponent<MeshRenderer>().enabled = true;
    }

    public void LetGoBall(){
        _indicatorBall.GetComponent<MeshRenderer>().enabled = false;
        // If doughball is released close enough to the indicator, it snaps to that location and spreading can be started
        _position1 = transform.TransformPoint(_networkSpawner._doughBall.transform.position);
        _position2 = transform.TransformPoint(_indicatorBall.transform.position);
        Debug.Log((_position1 - _position2).magnitude);
        if ((_position1 - _position2).magnitude < 0.02f){
            //_spawner.DeSpawnDoughBall();
            _networkSpawner.DeSpawnDoughBall();
            _networkSpawner.SpawnPizza(_indicatorBall.transform.position, Quaternion.identity);
            //_pizza = _spawner.SpawnPizza(_indicatorBall.transform.position, Quaternion.identity);
            //_spawner.SetPizza(_pizza);
            
            //Debug.Log("pizza Spawned");
            
            //_taskProgression.doughPlaced = true;
            //_taskProgression.UpdateInstruction();
            //_taskProgression.SetHapticVibration(0.3f,0.3f);
            _networkTaskProgression.doughPlacedServerRpc(true);// = true;
            _networkTaskProgression.UpdateInstruction();
            _networkTaskProgression.SetHapticVibration(0.3f,0.3f);
            GameObject.Find("RollingPin").GetComponent<PizzaDeformer>().ballIsPlaced = true;
            //Debug.Log("Tommato Drawer assigned");


        }
    }
}
