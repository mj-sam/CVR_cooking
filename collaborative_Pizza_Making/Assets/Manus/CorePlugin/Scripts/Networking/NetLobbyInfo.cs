using System.Net;
using Lidgren.Network;

namespace Manus.Networking
{
	/// <summary>
	/// Sample Lobby Info
	/// </summary>
	[System.Serializable]
	public class NetLobbyInfo
	{
		public static double s_HostRefresh = 60;
		public static double s_HostTimeout = 30;

		public long hostID = 0;
		public IPEndPoint internalHostEndpoint = new IPEndPoint(IPAddress.None, 0);
		public IPEndPoint externalHostEndpoint = new IPEndPoint(IPAddress.None, 0);

		public byte status = 0;
		public string name = "";
		public byte players = 0;
		public byte maxPlayers = 0;

#if MASTERSERVER
		public double m_RegisterTime;
		public NetLobbyInfo(ref NetIncomingMessage _Msg)
		{
			m_HostID = _Msg.ReadInt64();
			m_InternalHostEndpoint = _Msg.ReadIPEndPoint();
			_Msg.ReadIPEndPoint(); //Discard Data
			m_ExternalHostEndpoint = _Msg.SenderEndPoint;

			m_Status = _Msg.ReadByte();
			m_Name = _Msg.ReadString();
			m_Players = _Msg.ReadByte();
			m_MaxPlayers = _Msg.ReadByte();
		}
		public void GiveData(ref NetOutgoingMessage _Msg)
		{
			_Msg.Write(m_HostID);
			_Msg.Write(m_InternalHostEndpoint);
			_Msg.Write(m_ExternalHostEndpoint);

			_Msg.Write(m_Status);
			_Msg.Write(m_Name);
			_Msg.Write(m_Players);
			_Msg.Write(m_MaxPlayers);
		}
#else
		public NetLobbyInfo()
		{

		}

		/// <summary>
		/// Read from Net Message.
		/// </summary>
		/// <param name="p_Msg"></param>
		public NetLobbyInfo(ref NetIncomingMessage p_Msg)
		{
			hostID = p_Msg.ReadInt64();
			internalHostEndpoint = p_Msg.ReadIPEndPoint();
			externalHostEndpoint = p_Msg.ReadIPEndPoint();

			status = p_Msg.ReadByte();
			name = p_Msg.ReadString();
			players = p_Msg.ReadByte();
			maxPlayers = p_Msg.ReadByte();
		}

		/// <summary>
		/// Write into Net Message.
		/// </summary>
		/// <param name="p_Msg"></param>
		public void GiveData(ref NetOutgoingMessage p_Msg)
		{
			p_Msg.Write(hostID);
			p_Msg.Write(internalHostEndpoint);
			p_Msg.Write(externalHostEndpoint);

			p_Msg.Write(status);
			p_Msg.Write(name);
			p_Msg.Write(players);
			p_Msg.Write(maxPlayers);
		}
		public int m_Ping; //https://msdn.microsoft.com/en-us/library/system.net.networkinformation.ping(v=vs.110).aspx
#endif
	}
}
