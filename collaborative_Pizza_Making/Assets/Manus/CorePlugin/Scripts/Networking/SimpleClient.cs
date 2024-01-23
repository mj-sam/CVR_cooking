using LidNet = Lidgren.Network;

namespace Manus.Networking
{
	/// <summary>
	/// A sample Client implementation which can receive details about hosted games and utilizes the SimpleInterface.
	/// </summary>
	public class SimpleClient : Client
	{
		public SimpleLobbyBrowser lobbyBrowser;
		public SimpleInterface simpleInterface;

		/// <summary>
		/// Initializes the Client with the basic message types
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID, this must be unique and is used for matchmaking</param>
		public SimpleClient(NetworkManager p_Man, string p_AppID) : base(p_Man, p_AppID)
		{

		}

		/// <summary>
		/// Function called when a connection is made to a server.
		/// Call the On Connected in the interface
		/// </summary>
		/// <param name="p_Reason">A string explaining what happened</param>
		protected override void OnConnected(string p_Reason)
		{
			base.OnConnected(p_Reason);
			simpleInterface?.OnConnected();
		}

		/// <summary>
		/// Function called when the client disconnects.
		/// Call the On Disconnected in the interface
		/// </summary>
		/// <param name="p_Reason">The reason for disconnection</param>
		protected override void OnDisconnected(string p_Reason)
		{
			base.OnDisconnected(p_Reason);
			simpleInterface?.OnDisconnected();
		}

		/// <summary>
		/// Function called when the client receives a discovery message from a server.
		/// The simple implementation reads a NetLobbyInfo data structure.
		/// The returned object is saved to the Discovered Servers list.
		/// </summary>
		/// <param name="p_Msg"></param>
		/// <returns>Server information</returns>
		protected override object OnReceiveDiscoveryMessage(LidNet.NetIncomingMessage p_Msg)
		{
			NetLobbyInfo t_Info = new NetLobbyInfo(ref p_Msg);
			t_Info.externalHostEndpoint = p_Msg.SenderEndPoint;
			if (lobbyBrowser) lobbyBrowser.shouldUpdate = true;
			return t_Info;
		}
	}
}
