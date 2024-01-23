using System.Collections;
using UnityEngine;

public class ParticlesObject : MonoBehaviour
{
    public ParticleSystem particleSys; // Reference to the particle system
    private float thresholdAngle = 65f; // Angle at which particles start to come out
    private float particleEmissionRate = 200f; // Number of particles emitted per second
    private ParticleSystem.EmissionModule emissionModule; // Reference to the particle system's emission module

    private bool pickedUp = false;

    void Start()
    {
        emissionModule = particleSys.emission;
        emissionModule.rateOverTime = 0f;
    }

    void FixedUpdate()
    {
        // Get the angle of the bag relative to the ground
        float angle = Vector3.Angle(transform.up, Vector3.up);
        // Check if the angle is greater than the threshold angle
        if (angle > thresholdAngle)
        {
            if (pickedUp){
                GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>().SetHapticVibration(0.3f,0.3f);
            }
            emissionModule.rateOverTime = particleEmissionRate;
        }
        else if (angle <= thresholdAngle)
        {
            emissionModule.rateOverTime = 0f;
        }
    }

    public void PickUp(){
        pickedUp = true;
    }
    public void LetGo(){
        pickedUp = false;
    }
}