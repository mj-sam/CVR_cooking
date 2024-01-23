using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Bowl : MonoBehaviour
{
    private GameSpawner objectSpawner;
    
    // Represents the two levels of flour in the flour and water bowl
    public GameObject Flour1;
    public GameObject Flour2;
    
    // Represents the two levels of water in the flour and water bowl
    public GameObject Water1;
    public GameObject Water2;
    
    // Represents the cheese in the cheese bowl
    public GameObject Cheese1;
    public GameObject Cheese2;

    // Measure how much flour/ water/ cheese particles are put into the bowl
    private int GramsFlour = 0;
    private int GramsWater = 0;
    private int GramsCheese = 0;
    // Check if flour and water bowl is filled
    private bool flourPresent = false;
    private bool waterPresent = false;
    
    // Doughball will spawn in when bowl is filled with water and flower and is kneaded
    public GameObject DoughBallPrefab;
    private int amountOfKneads = 0;
    private XRGrabInteractable grabInteractable;
    public BowlType bowlType;
    private TaskProgression taskProgression;
    // How many particles should collide with the bowl before bowl is filled
    private int fillThreshold = 60;
    
    // Different type of bowls
    public enum BowlType{
        TomatoBowl, //Not used
        DoughBowl,
        CheeseBowl
    }

    private void Start(){
        grabInteractable = GetComponent<XRGrabInteractable>();
        taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        objectSpawner = GameObject.Find("TaskProgression").GetComponent<GameSpawner>();
    }

    // Particles from water/ flour/ cheese grating collide with bowl and get counted, if there is enough the bowl is filled
    private void OnParticleCollision(GameObject other)
    {
        // if (!taskProgression.isServer) return;
        if (bowlType == BowlType.DoughBowl)
        {
            if (other.CompareTag("Flour") && flourPresent == false)
            {
                GramsFlour++;
                if (GramsFlour > fillThreshold && GramsFlour < fillThreshold*2){
                    //BroadcastRemoteMethod("setFlour1", true);
                    setFlour1(true);
                }
                else if (GramsFlour > fillThreshold*2){
                    flourPresent = true;
                    //BroadcastRemoteMethod("setFlour2", true);
                    setFlour2(true);
                    //TaskProgression (used to update instruction text)
                    taskProgression.flourFilled = true;
                    taskProgression.UpdateInstruction();
                }
            }
            if (other.CompareTag("Water") && waterPresent == false)
            {
                GramsWater++;
                if (GramsWater > fillThreshold && GramsWater < fillThreshold*2){
                    //BroadcastRemoteMethod("setWater1", true);
                    setWater1(true);
                }
                else if (GramsWater > fillThreshold*2){
                    waterPresent = true;
                    //BroadcastRemoteMethod("setWater1", false);
                    //BroadcastRemoteMethod("setWater2", true);
                    setWater1(false);
                    setWater2(true);
                    //TaskProgression (used to update instruction text)
                    taskProgression.waterFilled = true;
                    taskProgression.UpdateInstruction();
                }
            }
        }
        else if (bowlType == BowlType.CheeseBowl)
        {
            if (other.CompareTag("CheeseParticle"))
            {
                GramsCheese++;
                if (GramsCheese > fillThreshold && GramsCheese < fillThreshold*2){
                    //BroadcastRemoteMethod("setCheese1", true);
                    setCheese1(true);
                }
                else if (GramsCheese > fillThreshold*2){
                    //BroadcastRemoteMethod("setCheese2", true);
                    setCheese2(true);
                    //TaskProgression (used to update instruction text)
                    taskProgression.cheeseGrated = true;
                    taskProgression.UpdateInstruction();
                }
            }
        }
    }

    // If enough water and flour present, the user can knead the dough by grabbing the bowl a couple of times
    public void KneadDough(){
        if (bowlType != BowlType.DoughBowl) return;
        if (waterPresent == true && flourPresent == true){
            amountOfKneads += 1;
            
        }
        // Spawn in doughball prefab when kneaded enough
        if (amountOfKneads > 3){
            //BroadcastRemoteMethod("ResetBowl");
            ResetBowl();
            
            //GameObject dough = 
            objectSpawner.SpawnDoughBall(this.transform.position + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
            //GameObject pizza =
            //objectSpawner.SpawnPizza(this.transform.position + new Vector3(-10000f,0.1f,0f), Quaternion.identity);
            //, new Vector3(0.14f,0.14f,0.14f));
            //objectSpawner._pizza.GetComponent<Rigidbody>().useGravity = false;
            //objectSpawner._pizza.GetComponent<Rigidbody>().isKinematic = true;
            //TaskProgression (used to update instruction text)
            taskProgression.doughKneaded = true;
            taskProgression.UpdateInstruction();
        }
    }
    
    
    private void ResetBowl(){
        Water2.SetActive(false);
        Flour1.SetActive(false);
        Flour2.SetActive(false);
        waterPresent = false;
        flourPresent = false;
        amountOfKneads = 0;
        GramsFlour = 0;
        GramsWater = 0;
    }

    
    private void setWater1(bool set){
        Water1.SetActive(set);
    }
    
    private void setWater2(bool set){
        Water2.SetActive(set);
    }
    
    private void setFlour1(bool set){
        Flour1.SetActive(set);
    }
    
    private void setFlour2(bool set){
        Flour2.SetActive(set);
    }
    
    private void setCheese1(bool set){
        Cheese1.SetActive(set);
    }
    
    private void setCheese2(bool set){
        Cheese2.SetActive(set);
    }
}
