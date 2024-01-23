using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowlNetwork : NetworkBehaviour
{
    private GameSpawner objectSpawner;
    //syncronization behaviour
    public NetworkVariable<bool> flourPresent= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> waterPresent= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

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
    
    
    // Doughball will spawn in when bowl is filled with water and flower and is kneaded
    //private int amountOfKneads = 4;
    private XRGrabInteractable grabInteractable;
    public BowlType bowlType;
    //private TaskProgression _taskProgression;
    private NetworkTaskProgression _networkTaskProgression;

    private NetworkGameSpawner _networkSpawner; 
    // How many particles should collide with the bowl before bowl is filled
    private readonly int _fillThreshold = 60;
    
    // Different type of bowls
    public enum BowlType{
        TomatoBowl,
        DoughBowl,
        CheeseBowl
    }

    private void Start(){
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        //_taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        
        objectSpawner = GameObject.Find("TaskProgression").GetComponent<GameSpawner>();
        _networkSpawner = GameObject.Find("TaskProgression").GetComponent<NetworkGameSpawner>();
        if (GameObject.Find("SaucePlane") && bowlType != BowlType.CheeseBowl)
            GameObject.Find("Spoon").GetComponent<NetworkTomatoDrawer>()._pizzaDisplay = GameObject.Find("SaucePlane").GetComponent<TomatoDisplay>();
    }

    // Particles from water/ flour/ cheese grating collide with bowl and get counted, if there is enough the bowl is filled
    private void OnParticleCollision(GameObject other)
    {
        // if (!taskProgression.isServer) return;
        if (bowlType == BowlType.DoughBowl)
        {
            if (other.CompareTag("Flour") && flourPresent.Value == false)
            {
                GramsFlour++;
                if (GramsFlour > _fillThreshold && GramsFlour < _fillThreshold*2){
                    setFlour1ServerRpc(true);
                }
                else if (GramsFlour > _fillThreshold*2){
                    //flourPresent = true;
                    setflourPresentServerRpc(true);
                    setFlour2ServerRpc(true);
                    //TaskProgression (used to update instruction text)
                    //_taskProgression.flourFilled = true;
                    _networkTaskProgression.flourFilledServerRpc(true);
                    //_networkTaskProgression.UpdateInstruction();
                }
            }
            if (other.CompareTag("Water") && waterPresent.Value == false)
            {
                GramsWater++;
                if (GramsWater > _fillThreshold && GramsWater < _fillThreshold*2){
                    setWater1ServerRpc(true);
                }
                else if (GramsWater > _fillThreshold*2){
                    //waterPresent = true;
                    setWaterPresentServerRpc(true);
                    setWater1ServerRpc(false);
                    setWater2ServerRpc(true);
                    //TaskProgression (used to update instruction text)
                    //_taskProgression.waterFilled = true;
                    //_taskProgression.UpdateInstruction();
                    _networkTaskProgression.waterFilledServerRpc(true);
                    //_networkTaskProgression.UpdateInstruction();
                }
            }
        }
        // else if (bowlType == BowlType.CheeseBowl)
        // {
        //     if (other.CompareTag("CheeseParticle"))
        //     {
        //         GramsCheese++;
        //         if (GramsCheese > _fillThreshold && GramsCheese < _fillThreshold*2){
        //             setCheese1ServerRpc(true);
        //         }
        //         else if (GramsCheese > _fillThreshold*2){
        //             setCheese2ServerRpc(true);
        //             //_taskProgression.cheeseGrated = true;
        //             //_taskProgression.UpdateInstruction();
        //             _networkTaskProgression.cheeseGratedServerRpc(true);
        //             _networkTaskProgression.UpdateInstruction();
        //         }
        //     }
        // }
    }

    // If enough water and flour present, the user can knead the dough by grabbing the bowl a couple of times
    [ServerRpc(RequireOwnership = false)]
    public void KneadDoughServerRpc(){
        if (bowlType != BowlType.DoughBowl) return;
        if (_networkTaskProgression.doughKneaded.Value) return;
        if (waterPresent.Value == true && flourPresent.Value == true){
            _networkTaskProgression.kneadCounter.Value++;
        }
        
        // Spawn in doughball prefab when kneaded enough
        if (_networkTaskProgression.kneadCounter.Value > 3){
            ResetBowlServerRpc();
            _networkSpawner.SpawnDoughBallServerRpc(
                this.transform.position + new Vector3(0f, 0.1f, 0f), 
                Quaternion.identity);
            //objectSpawner.SpawnDoughBall(, );
            //_taskProgression.doughKneaded = true;
            //_taskProgression.UpdateInstruction();
            _networkTaskProgression.doughKneadedServerRpc(true);// = true;
            //_networkTaskProgression.UpdateInstruction();
        }
        
        
        KneadDoughClientRpc();
    }
    [ClientRpc]
    public void KneadDoughClientRpc(){
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void setWaterPresentServerRpc(bool value)
    {
        waterPresent.Value = value;
    }
    [ServerRpc(RequireOwnership = false)]
    private void setflourPresentServerRpc(bool value)
    {
        flourPresent.Value = value;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ResetBowlServerRpc()
    {
        waterPresent.Value = false;
        flourPresent.Value = false;
        _networkTaskProgression.kneadCounter.Value = 0;
        ResetBowlServerClientRpc();
    }
    
    [ClientRpc]
    private void ResetBowlServerClientRpc(){
        Water1.SetActive(false);
        Water2.SetActive(false);
        Flour1.SetActive(false);
        Flour2.SetActive(false);
        
        GramsFlour = 0;
        GramsWater = 0;
    }

    
    // set water level
    [ServerRpc(RequireOwnership = false)]
    private void setWater1ServerRpc(bool set){
        //Debug.Log("Water level 1 is called");
        setWater1ClientRpc(set);
    }
    [ClientRpc]
    private void setWater1ClientRpc(bool set)
    {   
        //Debug.Log("Water level 1 is send to run");
        Water1.SetActive(set);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void setWater2ServerRpc(bool set)
    {
        setWater2ClientRpc(set);
    }

    [ClientRpc]
    private void setWater2ClientRpc(bool set)
    {
        Water2.SetActive(set);
    }
    //==============================================
    // set flour level
    [ServerRpc(RequireOwnership = false)]
    private void setFlour1ServerRpc(bool set){
        setFlour1ClientRpc(set);
    }
    [ClientRpc]
    private void setFlour1ClientRpc(bool set){
        Flour1.SetActive(set);
    }
    //
    [ServerRpc(RequireOwnership = false)]
    private void setFlour2ServerRpc(bool set){
        setFlour2ClientRpc(set);
    }
    [ClientRpc]
    private void setFlour2ClientRpc(bool set){
        Flour2.SetActive(set);
    }
    //==============================================
    // set Cheese level
    [ServerRpc(RequireOwnership = false)]
    private void setCheese1ServerRpc(bool set){
        setCheese1ClientRpc(set);
    }
    [ClientRpc]
    private void setCheese1ClientRpc(bool set){
        Cheese1.SetActive(set);
    }
    [ServerRpc(RequireOwnership = false)]
    private void setCheese2ServerRpc(bool set){
        setCheese2ClientRpc(set);
    }
    [ClientRpc]
    private void setCheese2ClientRpc(bool set){
        Cheese2.SetActive(set);
    }
}
