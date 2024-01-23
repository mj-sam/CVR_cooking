using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manus.Networking
{
	/// <summary>
	/// A sample Lobby Browser to display how one might want to show lobbies.
	/// </summary>
	[AddComponentMenu("Manus/Networking/Simple Lobby Browser")]
	public class SimpleLobbyBrowser : MonoBehaviour
	{
		public NetworkManager manager;

		public GameObject genericLobbyGameButton;
		public RectTransform lobbyGameListRect;
		public RectTransform highlightRect;

		public bool shouldUpdate = false;

		/// <summary>
		/// The currently selected Lobby
		/// </summary>
		public NetLobbyInfo selectedLobby
		{
			get
			{
				return m_SelectedLobby;
			}
		}

		List<NetLobbyInfo> m_LobbyList = null;
		NetLobbyInfo m_SelectedLobby = null;

		/// <summary>
		/// The update function checks if the lobby should be updated
		/// </summary>
		private void Update()
		{
			if(shouldUpdate)
			{
				shouldUpdate = false;
				Client t_Client = manager.GetClient();
				if (t_Client != null)
				{
					List<NetLobbyInfo> t_Info = new List<NetLobbyInfo>();
					var t_DS = t_Client.discoveredServers;
					foreach (var t_Serv in t_DS)
					{
						t_Info.Add(t_Serv.Value as NetLobbyInfo);
					}

					UpdateBrowser(t_Info);
				}
			}
		}

		/// <summary>
		/// Updates the list of lobbies in the interface.
		/// </summary>
		/// <param name="p_LobbyList">List of lobbies</param>
		public void UpdateBrowser(List<NetLobbyInfo> p_LobbyList)
		{
			highlightRect.gameObject.SetActive(false);
			m_LobbyList = p_LobbyList;
			GameObject t_LGObj;
			for (int i = 0; i < lobbyGameListRect.childCount; i++)
			{
				t_LGObj = lobbyGameListRect.GetChild(i).gameObject;
				if (t_LGObj && t_LGObj.gameObject.activeSelf) Destroy(t_LGObj);
			}

			for (int i = 0; i < m_LobbyList.Count; i++)
			{
				var t_GObj = Instantiate(genericLobbyGameButton);
				t_GObj.SetActive(true);
				var t_RTrans = t_GObj.GetComponent<RectTransform>();
				t_RTrans.SetParent(lobbyGameListRect);
				t_RTrans.localPosition = Vector3.zero;
				t_RTrans.anchorMax = new Vector2(1.0f, 0.05f * (20f - i));
				t_RTrans.anchorMin = new Vector2(0.0f, 0.05f * (19f - i));
				t_RTrans.localScale = Vector3.one;
				t_RTrans.anchoredPosition = Vector3.zero;
				t_RTrans.sizeDelta = Vector2.zero;

				var t_Button = t_GObj.GetComponent<Button>();
				t_Button.onClick = new Button.ButtonClickedEvent();

				var t_Item = m_LobbyList[i];
				long t_HostKey = m_LobbyList[i].hostID;
				t_Button.onClick.AddListener(delegate ()
				{
					highlightRect.gameObject.SetActive(true);
					highlightRect.anchorMin = t_RTrans.anchorMin;
					highlightRect.anchorMax = t_RTrans.anchorMax;
					highlightRect.anchoredPosition = t_RTrans.anchoredPosition;
					highlightRect.sizeDelta = t_RTrans.sizeDelta;
					m_SelectedLobby = t_Item;
				});

				var t_Text = t_RTrans.GetComponentInChildren<Text>();
				t_Text.text = m_LobbyList[i].status.ToString() + " | "
					+ m_LobbyList[i].name + " | "
					+ m_LobbyList[i].players.ToString() + "/"
					+ m_LobbyList[i].maxPlayers.ToString() + " | "
					+ m_LobbyList[i].m_Ping;
			}
		}
	}
}
