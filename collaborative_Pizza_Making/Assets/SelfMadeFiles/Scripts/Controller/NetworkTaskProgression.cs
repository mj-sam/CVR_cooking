using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.IO;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

// Keeping track of what tasks have been performed and what tasks may be performed next
// Updates the instruction text as well
public class NetworkTaskProgression : NetworkBehaviour
{ 
    // Variable to set visually
    public TMP_Text instructionText; // Text to show on the instruction board
    public GameObject canvas;
    public GameObject VRcamera;
    public GameObject pizza;
    public GameObject cheeseGrating;

    public GameObject  ExperimentNameInput;
    public GameObject  ExperimentSettingInput;
    private String ExperimentName = "";
    private String ExperimentSetting = "";
    // ===== ---- Variable to check progression ----=======///
    private double initialTimer = 0;
    private double doughKneadedTimer = 0;
    private double doughSpreadTimer = 0;
    private double waterFilledTimer = 0;
    private double flourFilledTimer = 0;
    private double tomatoSpreadTimer = 0;
    private double sausagePizzaDoneTimer = 0;
    private double bellPepperPizzaDoneTimer = 0;
    private double doughPlacedTimer = 0;
    private double pizzaBakedTimer = 0;
    private double pizzafinishedTimer = 0;
    private double gamefinishedTimer = 0;

    private bool CanWrite = true;
    //--------- public variable----------
    public NetworkVariable<bool> gameStarted= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> doughKneaded= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> doughSpread= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> waterFilled= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> flourFilled= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> tomatoSpread= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> sausagePizzaDone= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> bellPepperPizzaDone= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> doughPlaced= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> pizzaFinished= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> ovenOpen= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> pizzaInOven= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> pizzaBaked= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> finished= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> twoPlayers= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> timerRunning= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> cheeseGrated= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    //=====------- private Variable ----=====//
    public NetworkVariable<int> kneadCounter = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> bellPepperCut= new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> sausageCut= new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> sausageOnPizza= new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> bellPepperOnPizza= new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<float> pizzaBakeTimer= new NetworkVariable<float>(10f, NetworkVariableReadPermission.Everyone);
    //==================------ NPC ------------- 
    //public GameObject John;
    //public GameObject Megan;
    //public GameObject Ruth;
    
    //private Animator MeganAnimator;
    //private Animator JohnAnimator;
    //private Animator RuthAnimator;

    //--------------------------------------------------------------
    [System.NonSerialized] public bool LocalHaptic = false;
    [System.NonSerialized] public bool OtherHaptic = false;
    [System.NonSerialized] public bool isServer;
    
    // Task Identifier
    private float taskTimer = 0f;
    public int sessionID;
    //public bool test;
    //public bool SingleUser;
    
    // Does this user or the other use haptics?
    public SetupNumber setup;
    public enum SetupNumber{
        NoHaptic,
        ServerHaptic,
        ClientHaptic
    }
    //------------===== Track Objective =====-----------------------
    private uint TotalBytesPerSecondSent, PeekLoadSent, PeekLoadReceived, TotalBytesPerSecondReceived = 0;
    private int networkUpdateCounter = 0;
    private bool final = false;
    private string filename = "performance_final.csv";
    //private string filePath = ;
    //====================================================================
    //------------------ RPC METHODS -------------------------------------
    private void SaveData()
    {
        Debug.Log("SAVE CALLED ========1111111111111111111111!==========");
        ExperimentName = ExperimentNameInput.GetComponent<TMP_InputField>().text.ToString();
        ExperimentSetting = ExperimentSettingInput.GetComponent<TMP_InputField>().text.ToString();
        string data = $"{ExperimentName},{ExperimentSetting},{doughKneadedTimer.ToString()},{doughSpreadTimer.ToString()},{waterFilledTimer.ToString()},{flourFilledTimer.ToString()},{tomatoSpreadTimer.ToString()},{sausagePizzaDoneTimer.ToString()},{bellPepperPizzaDoneTimer.ToString()},{doughPlacedTimer.ToString()},{pizzafinishedTimer.ToString()},{pizzaBakedTimer.ToString()},{gamefinishedTimer.ToString()}\n"; // add all variables with a newline

        //string data = $"{ExperimentName},{ExperimentSetting},{doughKneadedTimer},{doughSpreadTimer},{waterFilledTimer},{flourFilledTimer},{tomatoSpreadTimer},{sausagePizzaDoneTimer},{bellPepperPizzaDoneTimer},{doughPlacedTimer},{pizzafinishedTimer},{pizzaBakedTimer},{gamefinishedTimer}"; // add all variables

        string filePath = Application.persistentDataPath + "/" + filename;

        // Check if file doesn't exist, then create it
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }

        // Append data to the file
        File.AppendAllText(filePath, data);
    }
    
    [ClientRpc]
    public void ruthPointClientRpc()
    {
        //RuthAnimator.SetTrigger("Point");
    }
    [ServerRpc(RequireOwnership = false)]
    public void cheeseGratedServerRpc(bool value)
    {
        cheeseGrated.Value = value;
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void startGameServerRpc()
    {
        gameStarted.Value = true;
        initialTimer = Time.time;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void doughKneadedServerRpc(bool value)
    {
        doughKneaded.Value = value;
        doughKneadedTimer = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void waterFilledServerRpc(bool value)
    {
        SaveData();
        waterFilled.Value = value;
        waterFilledTimer  = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void flourFilledServerRpc(bool value)
    {
        flourFilled.Value = value;
        flourFilledTimer  = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void doughSpreadServerRpc(bool value)
    {
        doughSpread.Value = value;
        doughSpreadTimer = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void tomatoSpreadServerRpc(bool value)
    {
        tomatoSpread.Value = value;
        tomatoSpreadTimer = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void sausagePizzaDoneServerRpc(bool value)
    {
        sausagePizzaDone.Value = value;
        sausagePizzaDoneTimer  = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void bellPepperPizzaDoneServerRpc(bool value)
    {
        bellPepperPizzaDone.Value = value;
        bellPepperPizzaDoneTimer = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void doughPlacedServerRpc(bool value)
    {
        doughPlaced.Value = value;
        doughPlacedTimer  = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void pizzaFinishedServerRpc(bool value)
    {
        pizzaFinished.Value = value;
        pizzafinishedTimer  = Time.time - initialTimer;
        //SaveData();
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void GameFinishedServerRpc(bool value)
    {
        finished.Value = value;
        gamefinishedTimer  = Time.time - initialTimer;
        if(CanWrite && (isServer || IsHost)) SaveData();
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ovenOpenServerRpc(bool value)
    {
        ovenOpen.Value = value;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void pizzaInOvenServerRpc(bool value)
    {
        pizzaInOven.Value = value;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void pizzaBakedServerRpc(bool value)
    {
        pizzaBaked.Value = value;
        pizzaBakedTimer = Time.time - initialTimer;
        UpdateInstruction();
        UpdateInstructionClientRpc();
        ruthPointClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void finishedServerRpc(bool value)
    {
        // MeganAnimator.SetBool("IsFinished", true);
        // JohnAnimator.SetBool("IsFinished", true);
        // RuthAnimator.SetBool("IsFinished", true);
        finished.Value = value;
        GameFinishedServerRpc(true);
        UpdateInstruction();
        UpdateInstructionClientRpc();
        //finishedClientRpc();
    }

    [ClientRpc]
    public void finishedClientRpc()
    {
        // MeganAnimator.SetBool("IsFinished", true);
        // JohnAnimator.SetBool("IsFinished", true);
        // RuthAnimator.SetBool("IsFinished", true);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void timerRunningServerRpc(bool value)
    {
        timerRunning.Value = value;
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }

    [ClientRpc]
    public void UpdateInstructionClientRpc()
    {
        UpdateInstruction();
    }
    //-------------- TOPING MANAGER ---------------------------
    private float _lastPizzaBakeTimer = 10;
    [ServerRpc(RequireOwnership = false)]
    public void pizzaBakeDecreaseTimerServerRpc(float value)
    {
        if( (pizzaBakeTimer.Value - _lastPizzaBakeTimer) >= 1.0)
        {   pizzaBakeTimer.Value = _lastPizzaBakeTimer;
            UpdateInstruction();
            UpdateInstructionClientRpc();
        }
        _lastPizzaBakeTimer -= value;
    }
    [ServerRpc(RequireOwnership = false)]
    public void pizzaBakeTimerServerRpc(float value)
    {
        pizzaBakeTimer.Value = value;
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void bellPepperCutServerRpc(int value)
    {
        bellPepperCut.Value = value;
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void sausageCutServerRpc(int value)
    {
        sausageCut.Value = value;
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void sausageOnPizzaServerRpc(int value)
    {
        sausageOnPizza.Value = value;
        if (!pizzaFinished.Value && sausageOnPizza.Value >= 3 && bellPepperOnPizza.Value >= 3)
        {
            pizzaFinishedServerRpc(true);
        }
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void bellPepperOnPizzaServerRpc(int value)
    {
        bellPepperOnPizza.Value = value;
        if (!pizzaFinished.Value && sausageOnPizza.Value >= 3 && bellPepperOnPizza.Value >= 3)
        {
            pizzaFinishedServerRpc(true);
        }
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void KneadingServerRpc()
    {
        kneadCounter.Value++;
        UpdateInstruction();
        UpdateInstructionClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void BakePizzaServerRpc(){
        BakedPizzaVisual[] bakeScripts = pizza.GetComponentsInChildren<BakedPizzaVisual>();
        foreach (BakedPizzaVisual bakeScript in bakeScripts)
        {
            bakeScript.Bake();
        }
        //pizzaBakeTimerServerRpc(0);
        SetHapticVibration(0.3f,0.3f);
        BakePizzaClientRpc();
    }
    [ClientRpc]
    private void BakePizzaClientRpc(){
        BakedPizzaVisual[] bakeScripts = pizza.GetComponentsInChildren<BakedPizzaVisual>();
        foreach (BakedPizzaVisual bakeScript in bakeScripts)
        {
            bakeScript.Bake();
        }
        SetHapticVibration(0.3f,0.3f);
    }
    //====================================================================
    
    private void Awake()
    {
        isServer = IsServer;
    }

    private void Start(){
        //filename = Application.persistentDataPath + "/" + filename;
        setObjects(canvas, instructionText, VRcamera);
        
        // JohnAnimator = John.GetComponent<Animator>();
        // MeganAnimator = Megan.GetComponent<Animator>();
        // RuthAnimator = Ruth.GetComponent<Animator>();
    }

    public void setObjects(GameObject canv, TMP_Text canvtext, GameObject camer){
        instructionText = canvtext;
        canvas = canv;
        VRcamera = camer;
        instructionText.text = "Push The Button to Start the Game";
    }

    public void UpdateInstruction(){
        if (final) return;
        if (gameStarted.Value)
        {
            instructionText.text = "Mix Water and Flour in Bowl";
        }
        if (flourFilled.Value && waterFilled.Value){
            instructionText.text = "Knead the Dough";
        }
        if (doughKneaded.Value){
            instructionText.text = "Place Dough on Indicator";
        }
        if (doughPlaced.Value){
            instructionText.text = "Spread Dough with Two Hands using Rolling Pin";
        }
        if (doughSpread.Value){
            instructionText.text = "Spread Sauce on Pizza with Spoon";
        }
        if (tomatoSpread.Value && (sausageCut.Value < 4 || bellPepperCut.Value < 4)){
            instructionText.text = "Cut Pepper = " + Math.Min(bellPepperCut.Value,4) + "of 4 Cut Sausage = " + Math.Min(sausageCut.Value,4) + " of 4";   // \n
        }
        if (tomatoSpread.Value && (sausageCut.Value >= 4 && bellPepperCut.Value >= 4)){
            instructionText.text = "Place 4 Slices of each Topping on Pizza";
        }
        if (pizzaFinished.Value && !ovenOpen.Value){ 
            instructionText.text = "Open Oven with Button";   // This one DOES HAPPEN
        }
        if (pizzaFinished.Value && ovenOpen.Value){ //sans!  DOES NOT
            instructionText.text = "Put Pizza in Oven with Pizza Shovel";
        }
        if (pizzaInOven.Value && ovenOpen.Value){ //sans!  DOES NOT
            instructionText.text = "Close Oven";
        }
        if (pizzaInOven.Value && !ovenOpen.Value){  //DOES HAPPEN
            instructionText.text = "Baking " + pizzaBakeTimer.Value.ToString("F1");
        }
        if (pizzaBakeTimer.Value <= 0){
            instructionText.text = "Place Pizza on Plate with Pizza Shovel";
            if (!pizzaBaked.Value)
            {
                //BroadcastRemoteMethod("BakePizza");
                BakePizzaServerRpc();
            }
            pizzaBaked.Value = true; // TODO
        }
        if (finished.Value){
            // if (!SingleUser){
            //     GameObject interactables = GameObject.Find("InteractablesMultiUser");
            //     for (int i = 0; i < interactables.transform.childCount; i++)
            //     {
            //         // Disable each child object
            //         if (!interactables.transform.GetChild(i).CompareTag("DinnerPlate")){
            //             interactables.transform.GetChild(i).gameObject.SetActive(false);
            //         }
            //     }
            //     cheeseGrating.SetActive(true);
            // }
            instructionText.text = "Well Done , Thank you for your contribution!";
        }
        if (cheeseGrated.Value && !final && finished.Value){
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


   

    int totalFrameCount = 0;
    int frameCount = 0;
    double dt = 0.0;
    double fps = 0.0;
    double minFps = 9999;
    double updateRate = 5.0;
    private float tempPizzaBakeTimer = 1;
    private void FixedUpdate(){
        if (pizzaBakeTimer.Value > 0 && pizzaInOven.Value && !ovenOpen.Value)
        {
            tempPizzaBakeTimer -= Time.deltaTime;
            if (tempPizzaBakeTimer <= 0)
            {   pizzaBakeDecreaseTimerServerRpc(1);
                tempPizzaBakeTimer = 1;
                instructionText.text = "Baking " + pizzaBakeTimer.Value.ToString("F1");
            }
        }
        if (doughKneaded.Value)
        {
            
        }


        if (canvas != null) canvas.transform.rotation = Quaternion.LookRotation((canvas.transform.position - VRcamera.transform.position).normalized);

        // timer starts when first item is picked up by either person, timer stops when pizza is placed on plate (task is done)
        if (timerRunning.Value && !final){
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
    public void BellPepperCut()
    {
        bellPepperCutServerRpc(bellPepperCut.Value + 1);
        UpdateInstruction();
    }
    public void SausageCut(){
        sausageCutServerRpc(sausageCut.Value + 1);
        UpdateInstruction();
    }
    public void PlaceSausage(){
        sausageOnPizzaServerRpc(sausageOnPizza.Value + 1);
        if (sausageOnPizza.Value >= 4){
            //sausagePizzaDone = true;
            sausagePizzaDoneServerRpc(true);
            if (bellPepperPizzaDone.Value){
                //pizzaFinished = true;
                pizzaFinishedServerRpc(true);
                UpdateInstruction();
            } 
        }
    }
    public void PlaceBellPepper(){
        //bellPepperOnPizza++;
        bellPepperOnPizzaServerRpc(bellPepperOnPizza.Value+1);
        if (bellPepperOnPizza.Value >= 4){
            //bellPepperPizzaDone = true;
            bellPepperPizzaDoneServerRpc(true);
            if (sausagePizzaDone.Value){
                //pizzaFinished = true;
                pizzaFinishedServerRpc(true);
                UpdateInstruction();
            }
        }
    }
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
