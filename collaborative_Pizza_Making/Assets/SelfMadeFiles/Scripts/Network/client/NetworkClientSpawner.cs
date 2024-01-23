using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UIElements;
using Transform = log4net.Util.Transform;
using TMPro;
using System.Xml;

public class NetworkClientSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject networkClient;
    public GameObject ipAddress;
    public Vector3[] PositionsToSpawn;
    public NetworkVariable<int> _NumberofPlayer = new NetworkVariable<int>(0);
    
    public override void OnNetworkSpawn()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) // Replace 'YourKey' with the desired key
        {
            StartHost();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        if (PositionsToSpawn[_NumberofPlayer.Value] != null)
        {
            Instantiate(networkClient, PositionsToSpawn[_NumberofPlayer.Value], Quaternion.identity).SpawnAsPlayerObject(clientId);

            SpawnPlayerClientRpc(PositionsToSpawn[_NumberofPlayer.Value], Quaternion.identity, clientId);
            _NumberofPlayer.Value++;
        }
        else
        {
            Debug.Log("instantiated without coordination");
            Instantiate(networkClient).SpawnAsPlayerObject(clientId);
            _NumberofPlayer.Value++;
        }
    }
    
    [ClientRpc]
    private void SpawnPlayerClientRpc(Vector3 position, Quaternion rotation, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            GameObject XRrig = GameObject.FindGameObjectsWithTag("XRrig")[0];
            XRrig.transform.position = position;
        }

    }

    
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton is not null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    [ContextMenu("Start Host")]
    public void StartHost()
    {
        //string serverIp;
        if (NetworkManager.Singleton is not null)
        {   
            //string ipAddressText = ipAddress.GetComponent<TMP_InputField>().text;
            // Path to the XML file
            string filePath = Application.dataPath + "/config.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {   xmlDoc.Load(filePath);
                XmlNode serverIpNode = xmlDoc.SelectSingleNode("Settings/ServerIp");
                NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address = serverIpNode.InnerText;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading XML: " + e.Message);
            }
            // Call the function you want to activate
            NetworkManager.Singleton.StartHost();
        }
    }

    [ContextMenu("Start Client")]
    public void StartClient()
    {
        if (NetworkManager.Singleton is not null)
        {
            string ipAddressText = ipAddress.GetComponent<TMP_InputField>().text;
            NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address= ipAddressText ;
            NetworkManager.Singleton.StartClient();
        }
    }
}
