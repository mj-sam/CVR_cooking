using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking
{
	/// <summary>
	/// This is an example of a Server implementation.
	/// It has lobby information, which it sends upon request.
	/// </summary>
	public class SimpleServer : Server
	{
		public NetLobbyInfo lobbyInfo;

		/// <summary>
		/// Initializes the Server with the basic message types and port connection
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID, this must be unique and is used for matchmaking</param>
		/// <param name="p_Port">Port the server should listen on</param>
		public SimpleServer(NetworkManager p_Man, string p_AppID, int p_Port) : base(p_Man, p_AppID, p_Port)
		{
			lobbyInfo = new NetLobbyInfo();
		}

		/// <summary>
		/// Function called when the server receives a discovery message from a client, they want information about the server.
		/// Writes the NetLobbyInfo to a message which the SimpleClient can receive and read.
		/// </summary>
		/// <param name="p_Msg"></param>
		protected override void OnRequestServerInfo(LidNet.NetOutgoingMessage p_Msg)
		{
			lobbyInfo.GiveData(ref p_Msg);
		}
	}
}
