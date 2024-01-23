using UnityEngine;

namespace Manus.Networking
{
	/// <summary>
	/// A sample NetworkManager implementation which can be used to host or join a game with client/server specific implementations.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Networking/Simple Network Manager")]
	public class SimpleNetworkManager : NetworkManager
	{
		/// <summary>
		/// Casts the Peer to a SimpleServer.
		/// Returns NULL if not a SimpleServer.
		/// </summary>
		public SimpleServer server
		{
			get
			{
				return m_Peer as SimpleServer;
			}
		}

		/// <summary>
		/// Instantiates a SimpleServer when called by the NetworkManager. (Host & Server functions)
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID</param>
		/// <param name="p_Port">The Port</param>
		/// <returns>Returns the created Server</returns>
		public override Server CreateServer(NetworkManager p_Man, string p_AppID, int p_Port)
		{
			return new SimpleServer(p_Man, p_AppID, p_Port);
		}

		/// <summary>
		/// Instantiates a SimpleClient when called by the NetworkManager. (Client functions)
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID</param>
		/// <returns>Returns the created Server</returns>
		public override Client CreateClient(NetworkManager p_Man, string p_AppID)
		{
			return new SimpleClient(p_Man, p_AppID);
		}
	}
}
