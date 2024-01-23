using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit;

// Keeping track of what tasks have been performed and what tasks may be performed next
// Updates the instruction text as well
public class TaskProgression : MonoBehaviour
{
    // Does this user or the other use haptics?
    public SetupNumber setup;
    public enum SetupNumber{
        NoHaptic,
        ServerHaptic,
        ClientHaptic
    }
    [System.NonSerialized] public bool LocalHaptic = false;
    [System.NonSerialized] public bool OtherHaptic = false;
    public GameObject cheeseGrating;
    [NonSerialized] public bool isServer = false;
    //Variables for checking the progression
    // start here
    //[SynchronizableField] 
    [System.NonSerialized] public bool cheeseGrated = false;
    [System.NonSerialized] public bool doughKneaded = false;
    [System.NonSerialized] public bool waterFilled = false;
    [System.NonSerialized] public bool flourFilled = false;
    [System.NonSerialized] public bool doughSpread = false;
    [System.NonSerialized] public bool tomatoSpread = false;
    [System.NonSerialized] public bool sausagePizzaDone = false;
    [System.NonSerialized] public bool bellPepperPizzaDone = false;
    [System.NonSerialized] public bool doughPlaced = false;
    [System.NonSerialized] public bool pizzaFinished = false;
    [System.NonSerialized] public bool ovenOpen = false;
    [System.NonSerialized] public bool pizzaInOven = false;
    [System.NonSerialized] public bool pizzaBaked = false;
    [System.NonSerialized] public bool finished = false;
    private int bellPepperCut = 0;
    private int sausageCut = 0;
    private int sausageOnPizza = 0;
    private int bellPepperOnPizza = 0;


    [System.NonSerialized] public GameObject pizza;
    [System.NonSerialized] public GameObject canvas;
    [System.NonSerialized] public GameObject VRcamera;
    [System.NonSerialized] public bool twoPlayers = false;
    [System.NonSerialized] public bool timerRunning = false;

    // Text to show on the instruction board
    public TMP_Text instructionText;  //private  
    //private Spawner spawner;
    public GameObject InteractablesSingleUser;
    public GameObject InteractablesMultiUser;
    private float pizzaBakeTimer = 10f;
    //[SynchronizableField] 
    private float taskTimer = 0f;
    public int sessionID;
    public bool test;
    public bool SingleUser;

    private uint TotalBytesPerSecondSent, PeekLoadSent, PeekLoadReceived, TotalBytesPerSecondReceived = 0;
    private int networkUpdateCounter = 0;
    private bool final = false;
    private string filename = "";
    private void Start(){
        //spawner = GameObject.Find("NetworkManager").GetComponent<Spawner>();
        filename = Application.dataPath + "/test.csv";
        //instructionText.text = "";
    }

    public void setObjects(GameObject canv, TMP_Text canvtext, GameObject camer){
        instructionText = canvtext;
        canvas = canv;
        VRcamera = camer;
        instructionText.text = "Mix Water and Flour in Bowl";
    }

    public void UpdateInstruction(){
        // call update instruction for all the players
        //BroadcastRemoteMethod("UpdateInstructionText");
        UpdateInstructionText();
    }

    //[SynchronizableMethod]
    public void UpdateInstructionText(){
        if (final) return;
        if (flourFilled && waterFilled){
            instructionText.text = "Knead the Dough";
        }
        if (doughKneaded){
            instructionText.text = "Place Dough on Indicator";
        }
        if (doughPlaced){
            instructionText.text = "Spread Dough with Two Hands using Rolling Pin";
        }
        if (doughSpread){
            instructionText.text = "Spread Sauce on Pizza with Spoon";
        }
        if (tomatoSpread && (sausageCut < 4 || bellPepperCut < 4)){
            instructionText.text = "Cut Pepper = " + Math.Min(bellPepperCut,4) + "of 4 Cut Sausage = " + Math.Min(sausageCut,4) + " of 4";   // \n
        }
        if (tomatoSpread && (sausageCut >= 4 && bellPepperCut >= 4)){
            instructionText.text = "Place 4 Slices of each Topping on Pizza";
        }
        if (pizzaFinished && !ovenOpen){ 
            instructionText.text = "Open Oven with Button";   // This one DOES HAPPEN
        }
        if (pizzaFinished && ovenOpen){ //sans!  DOES NOT
            instructionText.text = "Put Pizza in Oven with Pizza Shovel";
        }
        if (pizzaInOven && ovenOpen){ //sans!  DOES NOT
            instructionText.text = "Close Oven";
        }
        if (pizzaInOven && !ovenOpen){  //DOES HAPPEN
            instructionText.text = "Baking " + pizzaBakeTimer.ToString("F1");
        }
        if (pizzaBakeTimer <= 0){
            instructionText.text = "Place Pizza on Plate with Pizza Shovel";
            if (!pizzaBaked)
            {
                //BroadcastRemoteMethod("BakePizza");
                BakePizza();
            }
            pizzaBaked = true;
        }
        if (finished){
            if (!SingleUser){
                GameObject interactables = GameObject.Find("InteractablesMultiUser");
                for (int i = 0; i < interactables.transform.childCount; i++)
                {
                    // Disable each child object
                    if (!interactables.transform.GetChild(i).CompareTag("DinnerPlate")){
                        interactables.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                cheeseGrating.SetActive(true);
            }
            instructionText.text = "Grate the block of cheese";
        }
        if (cheeseGrated && !final && finished){
            instructionText.text = "Well Done!  Time: " + taskTimer.ToString("F1") + " Seconds";   // \n
            // Debug.LogWarning("------------------------------------------------");
            // Debug.LogWarning("Session ID:" + sessionID.ToString());
            // Debug.LogWarning("Clearance Time:" + taskTimer.ToString("F2"));
            // Debug.LogWarning("Average Network:" + ((TotalBytesPerSecondSent+TotalBytesPerSecondReceived)/2/networkUpdateCounter).ToString("F2"));
            // Debug.LogWarning("Peak Network:" + (Math.Max(PeekLoadSent,PeekLoadReceived)).ToString());
            // Debug.LogWarning("Average FPS:" + (totalFrameCount/taskTimer).ToString("F2"));
            // Debug.LogWarning("Minimum FPS:" + minFps.ToString("F2"));
            // Debug.LogWarning("------------------------------------------------");
            final = true;
            // if (!test) InvokeRemoteMethod("WriteCSV", 65535, MeasurementsToString());
        }
    }


    //[SynchronizableMethod]
    public void WriteCSV(string input)
    {
        TextWriter tw = new StreamWriter(filename, true);
        // tw.WriteLine("SessionID; Clearance_Time; Average_Network; Peak_Network; Average_FPS; Minimum_FPS"; "Haptic"; "Server");
        tw.WriteLine(MeasurementsToString());
        tw.WriteLine(input);
        tw.Close();
    }

    public string MeasurementsToString(){
        string haptic;
        if (SetupNumber.ClientHaptic == setup){
            haptic = "Client";
        }
        else if (SetupNumber.ServerHaptic == setup){
            haptic = "Server";
        }
        else{
            haptic = "None";
        }
        string server;
        if (isServer){
            server = "Server";
        }
        else server = "Client";
        return sessionID.ToString() + ";" +
            taskTimer.ToString("F2") + ";" +
            (TotalBytesPerSecondSent+TotalBytesPerSecondReceived/2/networkUpdateCounter).ToString("F2") + ";" +
            (Math.Max(PeekLoadSent,PeekLoadReceived)).ToString() + ";" +
            (totalFrameCount/taskTimer).ToString("F2") + ";" +
            minFps.ToString("F2") + ";" +
            haptic + ";" +
            server;
    }


    //[SynchronizableMethod]
    private void BakePizza(){
        BakedPizzaVisual[] bakeScripts = pizza.GetComponentsInChildren<BakedPizzaVisual>();
        foreach (BakedPizzaVisual bakeScript in bakeScripts)
        {
            bakeScript.Bake();
        }
        pizzaBakeTimer = 0;
        SetHapticVibration(0.3f,0.3f);
    }

    int totalFrameCount = 0;
    int frameCount = 0;
    double dt = 0.0;
    double fps = 0.0;
    double minFps = 9999;
    double updateRate = 5.0;

    private void FixedUpdate(){
        if (pizzaBakeTimer > 0 && pizzaInOven && !ovenOpen){
            pizzaBakeTimer -= Time.deltaTime;
            instructionText.text = "Baking " + pizzaBakeTimer.ToString("F1");
            if (pizzaBakeTimer <= 0) UpdateInstruction();
        }


        if (canvas != null)
        canvas.transform.rotation = Quaternion.LookRotation((canvas.transform.position - VRcamera.transform.position).normalized);

        // timer starts when first item is picked up by either person, timer stops when pizza is placed on plate (task is done)
        if (timerRunning && !final){
            // frameCount += 1;
            // dt += Time.deltaTime;
            // if (dt > 1.0/updateRate){
            //     fps = frameCount / dt;
            //     totalFrameCount += frameCount;
            //     frameCount = 0;
            //     dt -= 1.0/updateRate;
            //     // Debug.LogWarning(fps.ToString());
            //     minFps = Math.Min(minFps, fps);
            // }
            if (isServer) taskTimer += Time.deltaTime;
        }        
    }

    public void SetHapticVibration(float Lvalue, float Rvalue){
        if (!LocalHaptic) return;
        PlayerVRController[] pc = FindObjectsOfType<PlayerVRController>();
        foreach(PlayerVRController player in pc){
            player.SetHapticVibration(Lvalue, Rvalue);
        }
    }

    public void BellPepperCut(){
        bellPepperCut++;
        UpdateInstruction();
    }
    public void SausageCut(){
        sausageCut++;
        UpdateInstruction();
    }
    public void PlaceSausage(){
        sausageOnPizza++;
        if (sausageOnPizza >= 4){
            sausagePizzaDone = true;
            if (bellPepperPizzaDone){
                pizzaFinished = true;
                UpdateInstruction();
            } 
        }
    }
    public void PlaceBellPepper(){
        bellPepperOnPizza++;
        if (bellPepperOnPizza >= 4){
            bellPepperPizzaDone = true;
            if (sausagePizzaDone){
                pizzaFinished = true;
                UpdateInstruction();
            }
        }
    }

    /*
    public void SpawnedObject(){
        foreach((GameObject, Guid, string) obj in spawner.SpawnedObjects){
            if (obj.Item1.transform.Find("SaucePlane"))
                GameObject.Find("Spoon").GetComponent<NetworkTomatoDrawer>()._pizza = obj.Item1.transform.Find("SaucePlane").GetComponent<TomatoDisplay>();
        }
    }
    */
    public void NetworkData(uint newBytesPerSecondSent, uint newPeekLoadSent, uint newBytesPerSecondReceived, uint newPeekLoadReceived){
        PeekLoadReceived = Math.Max(PeekLoadReceived, newPeekLoadReceived);
        PeekLoadSent = Math.Max(PeekLoadSent, newPeekLoadSent);
        TotalBytesPerSecondReceived += (uint)newBytesPerSecondReceived/1000;
        TotalBytesPerSecondSent += newBytesPerSecondSent;
        networkUpdateCounter += 1;
    }

    public void SetServerTrue(){
        isServer = true;
    }


}
