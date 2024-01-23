using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;  
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;  
using UnityEngine.SceneManagement;  

public class ChangeScene : MonoBehaviour
{
    public void SceneGame() {
        Disconnect();
        Cleanup();
        SceneManager.LoadScene("Game");  
    }  
    public void SceneTutorial()
    {
        SceneManager.LoadScene("Tutorial");  
    }
    public void SceneMenu()
    {
        Disconnect();
        Cleanup();
        SceneManager.LoadScene("Menu");  
    }
    public void Disconnect()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
    public void Cleanup()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
}
