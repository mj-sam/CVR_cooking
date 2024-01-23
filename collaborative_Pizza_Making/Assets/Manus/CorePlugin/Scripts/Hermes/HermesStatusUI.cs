using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus
{
	public class HermesStatusUI : MonoBehaviour
	{
		private CanvasGroup m_CanvasGroup = null;

		public bool connected { get; private set; } = false;

		private void OnEnable()
		{
			m_CanvasGroup = GetComponent<CanvasGroup>();
			m_CanvasGroup.alpha = 0;
		}

		private void Update()
		{
			if (ManusManager.instance?.communicationHub?.careTaker?.Hermes != null && ManusManager.instance.communicationHub.careTaker.connected)
			{
				connected = true;
			} else
			{
				connected = false;
			}

			if (Time.time > 2f)
				m_CanvasGroup.alpha = connected ? 0f : 1f;
		}

		private void Toggle(bool p_Connected)
		{
			connected = p_Connected;
		}
	}
}