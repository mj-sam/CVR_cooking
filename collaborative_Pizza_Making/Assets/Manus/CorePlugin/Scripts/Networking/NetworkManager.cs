using System.Collections.Generic;
using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
	class NetworkEditor
	{
		static NetworkEditor()
		{
			UnityEditor.EditorApplication.projectChanged += OnProjectChanged;
			UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange _State)
		{
			OnProjectChanged();
		}

		static void OnProjectChanged()
		{
			NetObject[] t_AllObjects = NetworkManager.GetPrefabs();
			int t_ID = int.MinValue;
			for (int i = 0; i < t_AllObjects.Length; i++)
			{
				t_ID++;
				t_AllObjects[i].m_PrefabID = t_ID;
				UnityEditor.EditorUtility.SetDirty(t_AllObjects[i]);
			}
			//Debug.Log("Regenerated Prefab IDs");
		}
	}
#endif

	/// <summary>
	/// The basic NetworkManager Implementation.
	/// There should only be one NetworkManager in a project.
	/// This class should be inherited from in order to use your own Client and Server implementation.
	/// This class handles generic functions such as Hosting, Joining, Object spawning and removing.
	/// </summary>
	[DisallowMultipleComponent]
	public class NetworkManager : MonoBehaviour
	{
		private static NetworkManager s_Instance = null;

		/// <summary>
		/// Get the NetworkManager
		/// </summary>
		public static NetworkManager instance
		{
			get
			{
				if (s_Instance) return s_Instance;
				Debug.LogError("Network Manager not initialized!");
				return null;
			}
		}

		/// <summary>
		/// The different statuses the NetworkManager can have.
		/// </summary>
		public enum Status
		{
			Inactive,
			Peer,
			Server,
			Client,
		}

		/// <summary>
		/// The current status of the NetworkManager.
		/// </summary>
		public Status status
		{
			get
			{
				return m_Status;
			}
		}

		protected Status m_Status;

		/// <summary>
		/// This variable defines which applications it can find and be found by.
		/// It should be unique for every project made with the NetworkManager.
		/// </summary>
		public string appIdentifier = "MyApp";

		/// <summary>
		/// The port on which the NetworkManager creates the Server at.
		/// </summary>
		public int port = 6969;

		protected Peer m_Peer = null;

		/// <summary>
		/// The player prefab that is spawned when a player joins.
		/// </summary>
		public NetObject playerPrefab;

		/// <summary>
		/// The world position the Players are spawned in as.
		/// </summary>
		public Vector3 playerSpawnPostion = Vector3.zero;

		/// <summary>
		/// The world rotation the Players are spawned in as.
		/// </summary>
		public Vector3 playerSpawnRotation = Vector3.zero;

		NetObjectManager m_NetObjectManager = null;

		/// <summary>
		/// Gets all registered NetObject Prefabs.
		/// </summary>
		/// <returns>Returns all registered NetObject Prefabs.</returns>
		public static NetObject[] GetPrefabs()
		{
			return Resources.LoadAll<NetObject>("");
			//return Resources.FindObjectsOfTypeAll(typeof(NetObject)) as NetObject[];
		}

		/// <summary>
		/// On awake gathers all the NetObjects in the scene and adds them to the NetObjectManager
		/// </summary>
		private void Awake()
		{
			s_Instance = this;
			m_NetObjectManager = new NetObjectManager();

			var t_Scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene(); //improve later
			InitializeNetObjectsOnSceneLoaded(t_Scene);
		}

		/// <summary>
		/// Called by Unity.
		/// </summary>
		void Start()
		{
			NetObject[] t_AllObjects = GetPrefabs();
			for (int i = 0; i < t_AllObjects.Length; i++)
			{
				Debug.Log(t_AllObjects[i].name + " ~ " + t_AllObjects[i].m_PrefabID.ToString());
			}
		}

		/// <summary>
		/// Gathers all the NetObjects in the scene and adds them to the NetObjectManager
		/// </summary>
		void InitializeNetObjectsOnSceneLoaded(UnityEngine.SceneManagement.Scene p_Scene)
		{
			GameObject[] t_Objs = p_Scene.GetRootGameObjects();

			//Sort - good nuff?
			System.Array.Sort(t_Objs, new System.Comparison<GameObject>((i1, i2)
				=> i2.GetInstanceID().CompareTo(i1.GetInstanceID())));

			for (int i = 0; i < t_Objs.Length; i++)
			{
				NetObject[] t_NetObjs = t_Objs[i].GetComponentsInChildren<NetObject>(true);
				m_NetObjectManager.AddNetObjects(t_NetObjs);
			}
		}

		/// <summary>
		/// Called by Unity.
		/// </summary>
		void Update()
		{
			if (m_Peer == null) return;

			m_Peer.HandleIncomingMessages();
			m_Peer.Update(Time.deltaTime);
		}

		/// <summary>
		/// Called by Unity.
		/// </summary>
		private void OnDestroy()
		{
			if (m_Peer != null)
			{
				m_Peer.Shutdown();
				m_Peer = null;
			}
		}

		/// <summary>
		/// Function called when creating a server, this function needs to be overridden to use a custom Server implementation!
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID</param>
		/// <param name="p_Port">The Port</param>
		/// <returns>Returns the created Server</returns>
		public virtual Server CreateServer(NetworkManager p_Man, string p_AppID, int p_Port)
		{
			return new Server(p_Man, p_AppID, p_Port);
		}

		/// <summary>
		/// Function called when creating a client, this function needs to be overridden to use a custom Client implementation!
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID</param>
		/// <returns>Returns the created Server</returns>
		public virtual Client CreateClient(NetworkManager p_Man, string p_AppID)
		{
			return new Client(p_Man, p_AppID);
		}

		/// <summary>
		/// Creates a server and spawns the Player Prefab for this game instance.
		/// </summary>
		public void Host()
		{
			m_Peer = CreateServer(this, appIdentifier, port);
			m_Peer.SetNetObjectManager(m_NetObjectManager);
			m_Peer.Start();
			OnServerStart();
			SpawnObject(playerPrefab, playerSpawnPostion, Quaternion.Euler(playerSpawnRotation));
		}

		/// <summary>
		/// Creates a server and does NOT spawn a Player Prefab for this game instance.
		/// </summary>
		public void Server()
		{
			m_Peer = CreateServer(this, appIdentifier, port);
			m_Peer.SetNetObjectManager(m_NetObjectManager);
			m_Peer.Start();
			OnServerStart();
		}

		/// <summary>
		/// Creates a client for this game instance.
		/// </summary>
		public void Client()
		{
			m_Peer = CreateClient(this, appIdentifier);
			m_Peer.SetNetObjectManager(m_NetObjectManager);
			m_Peer.Start();
			OnClientStart();
		}

		/// <summary>
		/// If the Client is running, find servers on the LAN.
		/// </summary>
		public void DiscoverLan()
		{
			if (m_Status != Status.Client) return;

			(m_Peer as Client).SendDiscoverySignal(port);
		}

		/// <summary>
		/// If there is a Client available, connect to a Server at a given address and port.
		/// </summary>
		/// <param name="p_Host">IP Address of the Server</param>
		/// <param name="p_Port">Port of the Server</param>
		public void Connect(string p_Host, int p_Port)
		{
			if (m_Status != Status.Client) return;

			(m_Peer as Client).Connect(p_Host, p_Port);
		}

		/// <summary>
		/// If there is a Client available, connect to a given Remote End Point.
		/// This function should be used in combination with the results of a LAN Discovery.
		/// </summary>
		/// <param name="p_RemoteEndPoint">The Server's Remote End Point</param>
		public void Connect(System.Net.IPEndPoint p_RemoteEndPoint)
		{
			if (m_Status != Status.Client) return;

			(m_Peer as Client).Connect(p_RemoteEndPoint);
		}

		/// <summary>
		/// Returns the Client, if any is available.
		/// Returns NULL if no Client is available.
		/// </summary>
		/// <returns>A Client or NULL</returns>
		public Client GetClient()
		{
			return m_Peer as Client;
		}

		/// <summary>
		/// Returns the Server, if any is available.
		/// Returns NULL if no Server is available.
		/// </summary>
		/// <returns>A Server or NULL</returns>
		public Server GetServer()
		{
			return m_Peer as Server;
		}

		/// <summary>
		/// Returns the Peer, if any is available.
		/// Returns NULL if no Peer available.
		/// </summary>
		/// <returns>A Peer or NULL</returns>
		public Peer GetPeer()
		{
			return m_Peer;
		}

		/// <summary>
		/// This function is called when the server is started.
		/// </summary>
		void OnServerStart()
		{
			m_Status = Status.Server;
			m_NetObjectManager.EnableNetObjectsOnServer();
		}

		/// <summary>
		/// This function is called when the client is started.
		/// </summary>
		void OnClientStart()
		{
			m_Status = Status.Client;
		}

		/// <summary>
		/// Call this function when an object needs to be created.
		/// Make sure this NetObject is in the resources folder!
		/// The return will be NULL if used as a client!
		/// </summary>
		/// <param name="p_Obj">The NetObject to spawn.</param>
		/// <param name="p_Position">The initial position of the spawned object.</param>
		/// <param name="p_Rotation">The initial rotation of the spawned object.</param>
		/// <returns>NULL if NetObject not found or not instantly created.</returns>
		public NetObject SpawnObject(NetObject p_Obj, Vector3 p_Position, Quaternion p_Rotation)
		{
			if (m_Peer == null) return null;
			return m_Peer.CreateObject(p_Obj, p_Position, p_Rotation);
		}

		/// <summary>
		/// Call this function when an object needs to be destroyed.
		/// </summary>
		/// <param name="p_Obj">The NetObject to destroy</param>
		public void DestroyObject(NetObject p_Obj)
		{
			if (m_Peer == null) return;
			m_Peer.DestroyObject(p_Obj);
		}

		/// <summary>
		/// Call this function when you want to take control of an object.
		/// </summary>
		/// <param name="p_Obj">The NetObject to take control of</param>
		public void TakeControlOfObject(NetObject p_Obj)
		{
			if (m_Peer == null) return;
			m_Peer.TakeControlOfObject(p_Obj);
		}

		/// <summary>
		/// Call this function when you want to release control of an object.
		/// </summary>
		/// <param name="p_Obj">The NetObject to release control of</param>
		public void ReleaseControlOfObject(NetObject p_Obj)
		{
			if (m_Peer == null) return;
			m_Peer.ReleaseControlOfObject(p_Obj);
		}

		/// <summary>
		/// Get the NetObject with a given NetID.
		/// </summary>
		/// <param name="p_NetID">NetID to get</param>
		/// <returns></returns>
		public NetObject GetNetObject(long p_NetID)
		{
			if (m_NetObjectManager == null) return null;
			return m_NetObjectManager.GetNetObject(p_NetID);
		}

		/// <summary>
		/// Returns all the NetObjects.
		/// </summary>
		/// <returns>A List of NetObjects</returns>
		public List<NetObject> GetNetObjects()
		{
			if (m_NetObjectManager == null) return null;
			return m_NetObjectManager.GetNetObjects();
		}

		/// <summary>
		/// Returns all the NetObjects that a given player controls.
		/// </summary>
		/// <param name="p_PlayerNetID">The controller's ID</param>
		/// <returns>A List of NetObjects</returns>
		public List<NetObject> GetNetObjectsControlledByPlayer(long p_PlayerNetID)
		{
			if (m_NetObjectManager == null) return null;
			return m_NetObjectManager.GetNetObjectsControlledByPlayer(p_PlayerNetID);
		}

		/// <summary>
		/// Call this function when you want to send a message.
		/// </summary>
		/// <param name="p_Type">The Message Type</param>
		/// <param name="p_Msg">The Message</param>
		public void SendNetMessage(Message.Type p_Type, LidNet.NetBuffer p_Msg)
		{
			if (m_Peer == null) return;
			m_Peer.SendMessage(p_Type, p_Msg);
		}

		/// <summary>
		/// Call this function when you want to send a message.
		/// </summary>
		/// <param name="p_Type">The Message Type</param>
		/// <param name="p_Msg">The Message</param>
		public void SendNetMessage(ushort p_Type, LidNet.NetBuffer p_Msg)
		{
			if (m_Peer == null) return;
			m_Peer.SendMessage(p_Type, p_Msg);
		}
	}
}
