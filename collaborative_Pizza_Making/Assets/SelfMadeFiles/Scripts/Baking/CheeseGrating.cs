using UnityEngine;
using UnityEngine.XR;

//make sure componnet have inputdata
[RequireComponent(typeof(InputData))]
public class CheeseGrating : MonoBehaviour
{
    public ParticleSystem particleSys;
    private float particleEmissionRate = 300f;
    private ParticleSystem.EmissionModule emissionModule;
    private float speedThreshold = 0.06f;

    private InputData _inputData;
    private float controllerVelocityDifference;
    //all the following were syncronizable
    private bool cheeseGrabbed = false;
    private bool graterGrabbed = false;
    private bool serverHeld = false;
    private bool clientHeld = false;
    private float serverHandVelocityx;
    private float serverHandVelocityy;
    private float serverHandVelocityz;
    private float clientHandVelocityx;
    private float clientHandVelocityy;
    private float clientHandVelocityz;
    //until here
    private TaskProgression taskProgression;

    void Start()
    {
        emissionModule = particleSys.emission; // Get reference to the particle system's emission module
        emissionModule.rateOverTime = 0f;
        taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _inputData = GetComponent<InputData>();
    }

    // While both the grater and the cheese keep in touch
    private void OnTriggerStay(Collider other)
    {
        bool shouldProcess = taskProgression.SingleUser ?
            (other.CompareTag("Cheese") && cheeseGrabbed && graterGrabbed) :
            (other.CompareTag("Cheese") && cheeseGrabbed && graterGrabbed && serverHeld && clientHeld);

        if (shouldProcess)
        {
            if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity) &&
                _inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity))
            {
                // Placeholder for the empty block, consider removing or adding a comment explaining the purpose.
            }

            //BroadcastRemoteMethod("HandVelocity");
            HandVelocity();

            controllerVelocityDifference = (new Vector3(serverHandVelocityx, serverHandVelocityy, serverHandVelocityz) - new Vector3(clientHandVelocityx, clientHandVelocityy, clientHandVelocityz)).magnitude;

            if (controllerVelocityDifference < speedThreshold)
            {
                emissionModule.rateOverTime = 0;
            }
            else
            {
                emissionModule.rateOverTime = Mathf.Min(220f, Mathf.Log(controllerVelocityDifference + 1) * particleEmissionRate);
            }
        }
    }

    // Stop particle emission when they do not collide
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cheese"))
        {
            emissionModule.rateOverTime = 0;
        }
    }

    //[SynchronizableMethod]
    public void HandVelocity(){
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity)){

        }
        else{
            rightVelocity = new Vector3(0,0,0);
        }
        if (taskProgression.isServer){
            serverHandVelocityx = rightVelocity.x;
            serverHandVelocityy = rightVelocity.y;
            serverHandVelocityz = rightVelocity.z;
        }
        else{
            clientHandVelocityx = rightVelocity.x;
            clientHandVelocityy = rightVelocity.y;
            clientHandVelocityz = rightVelocity.z;
        }
    }


    public void CheeseGrabbed(){
        cheeseGrabbed = true;
        Grab();
    }

    public void CheeseReleased(){
        cheeseGrabbed = false;
        Release();
    }

    public void GraterGrabbed(){
        graterGrabbed = true;
        Grab();
    }

    public void GraterReleased(){
        graterGrabbed = false;
        Release();
    }

    private void Grab(){
        if (taskProgression.isServer){
            serverHeld = true;
        }
        else{
            clientHeld = true;
        }
    }
    private void Release(){
        if (taskProgression.isServer){
            serverHeld = false;
        }
        else{
            clientHeld = false;
        }
    }
}
