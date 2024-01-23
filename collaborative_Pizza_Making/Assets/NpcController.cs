using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NpcController : NetworkBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Method to trigger animations
    public void SetRunning(bool isRunning)
    {
        animator.SetBool("IsRunning", isRunning);
    }
    
    // Example usage
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetRunning(true);
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            SetRunning(false);
        }
    }
}
