using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool clap;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Method to trigger animations
    public void SetRunning(bool isRunning)
    {
        animator.SetBool("IsFinished", isRunning);
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
