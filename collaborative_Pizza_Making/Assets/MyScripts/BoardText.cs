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
using System.IO;

public class BoardText : MonoBehaviour
{
    private NetworkTaskProgression task_progression;
    private TMP_Text instruction_text;

    // Start is called before the first frame update
    void Start()
    {   
        //connect task progression to the general one
        //task_progression = GameObject.Find("TaskProgression").GetComponent<TaskProgression>();
        task_progression = GameObject.Find("TaskProgression").GetComponent<NetworkTaskProgression>();
        instruction_text = GameObject.Find("Tasks").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {   
        // we have to consider removing it because in every frame it update the text
        //task_progression.UpdateInstruction();
        //instruction_text.text = task_progression.instructionText.text; //getting console errors
         
    }
}
