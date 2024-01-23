using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartHost : MonoBehaviour
{
    public GameObject NetworkClient;
    // Start is called before the first frame update
    void Start()
    {
        //string serverIp;
        if (NetworkManager.Singleton is not null)
        {   
            NetworkManager.Singleton.StartHost();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
