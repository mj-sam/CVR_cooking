using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrackedPoseDriver = UnityEngine.SpatialTracking.TrackedPoseDriver;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using Manus.Hand;
using Manus.Haptics;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Manus.Hermes.Glove;
using Manus.Utility;
using Hermes.Protocol;


public class PlayerVRController : MonoBehaviour
{
    private Avatar _avatar;
    //[System.NonSerialized] public bool LocalHaptic = false;
    //[System.NonSerialized] public bool OtherHaptic = false;
    [SerializeField] private Transform head;
    [SerializeField] private Material transparent;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject canvas;
    //[SerializeField] private GameObject canvasHaptic;
    [SerializeField] private TMP_Text instructionText;
    //[SerializeField] private TMP_Text instructionTextHaptic;
    [SerializeField] private GameObject rController, lController;
    [SerializeField] private GameObject rControllerPresence, lControllerPresence;
    [SerializeField] private GameObject rControllerAnimator, lControllerAnimator;

    //[SerializeField] private GameObject rHaptic, lHaptic;
    private Manus.Hermes.Glove.Data rHand_data, lHand_data;
    //[SerializeField] private GameObject rHapticPresence, lHapticPresence;
    //[SerializeField] private GameObject rHapticAnimator, lHapticAnimator;
    [SerializeField] private int playerSelfLayer;
    public InputActionProperty RgrabActionProperty, LgrabActionProperty;
    private float RlastSelectInput, LlastSelectInput = 0f;
    //private List<List<List<float>>> gloveData = new List<List<List<float>>>();
    private TaskProgression _taskProgression;
    private NetworkTaskProgression _networkTaskProgression;

    private double _TOLERANCE = 0.001;
    private string _handpath;
    private SphereCollider _LeftHandSphereCollider;
    private SphereCollider _RightHandSphereCollider;
    
    
    private void Awake() {
        _avatar = GetComponent<Avatar>();
    }
    private void Start() {
        _taskProgression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        _networkTaskProgression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();

        gameObject.tag = "Player";
        // to make sure we label the server
        // if I am not the server the other one is the server
        //&& _avatar.IsOwner
        //&& !_avatar.IsOwner
        if ((_networkTaskProgression.isServer ) ||
            (!_networkTaskProgression.isServer )){
            this.gameObject.name = this.gameObject.name + "Server";
        }

        //_avatar.IsOwner
        if (true){
            // the head is transparent for the owner but it is visible by the other player
            foreach(Transform child in head)
                child.gameObject.GetComponent<MeshRenderer>().material = transparent;
            
            // set canvas to show instruction only for the owner player
            _networkTaskProgression.setObjects(canvas, instructionText, _camera.gameObject);  //sin barritas de comentarios
            
            // syncronize the player controller
            //rController.GetComponent<TransformSynchronizable>().enabled = true;
            //lController.GetComponent<TransformSynchronizable>().enabled = true;
            rControllerPresence.SetActive(true);
            lControllerPresence.SetActive(true);
        }
        else
        {
            _taskProgression.twoPlayers = true;
            _camera.gameObject.SetActive(false);
            canvas.SetActive(false);
            rController.GetComponent<ActionBasedController>().enableInputTracking = false;
            lController.GetComponent<ActionBasedController>().enableInputTracking = false;
            rControllerPresence.GetComponent<HandPresence>().enabled = false;
            lControllerPresence.GetComponent<HandPresence>().enabled = false;
            rController.GetComponent<SphereCollider>().enabled = false;
            lController.GetComponent<SphereCollider>().enabled = false;
        }
        _handpath = GetHandPath();
        _LeftHandSphereCollider = GameObject.Find(_handpath + "LeftHand Controller").GetComponent<SphereCollider>();
        _RightHandSphereCollider = GameObject.Find(_handpath + "RightHand Controller").GetComponent<SphereCollider>();
    }

    public void SetHapticVibration(float Lvalue, float Rvalue){
        //avatar.IsMe
        if (true) return;
        //rHapticAnimator.GetComponent<HandHaptics>().SetHapticVibration(Rvalue);
        //lHapticAnimator.GetComponent<HandHaptics>().SetHapticVibration(Lvalue);
    }

    private void FixedUpdate() {
        //I am not the owner or there is only one player just return
        //if (!_avatar.IsOwner || !_taskProgression.twoPlayers) return;
        
        // set head to camera orientation and position
        var transform1 = _camera.transform;
        head.localPosition = transform1.localPosition;
        head.rotation = transform1.rotation;
        // update hand gesture if it had changed
        float newRGrabValue = RgrabActionProperty.action.ReadValue<float>();
        float newLGrabValue = LgrabActionProperty.action.ReadValue<float>();
        // if change is bigger than the tolerance
        if ( Math.Abs(newRGrabValue - RlastSelectInput) > _TOLERANCE || 
             Math.Abs(newLGrabValue - LlastSelectInput) > _TOLERANCE){
            // InvokeRemoteMethod("NetworkControllerAnimation", 
            //     65535, // target all user except self
            //     newRGrabValue, 
            //     newLGrabValue);
            NetworkControllerAnimation(newRGrabValue, newLGrabValue);
            RlastSelectInput = newRGrabValue;
            LlastSelectInput = newLGrabValue;

            // why ?
            if (RlastSelectInput < 0.25){
                _RightHandSphereCollider.enabled = true;
            }
            if (LlastSelectInput < 0.25){
                _LeftHandSphereCollider.enabled = true;
            }
        }
    }
    public string GetHandPath(){
        //_avatar.IsOwner
        if(true)
        {
            return this.gameObject.name + "/XR Origin/Camera Offset/";
        }
        return null;
    }
    //[SynchronizableMethod]
    private void NetworkControllerAnimation(float RgripValue, float LgripValue){
        //_avatar.IsOwner
        if (true){
            rControllerAnimator.GetComponent<Animator>().SetFloat("Grip", RgripValue);
            lControllerAnimator.GetComponent<Animator>().SetFloat("Grip", LgripValue);
        }
    }
}
