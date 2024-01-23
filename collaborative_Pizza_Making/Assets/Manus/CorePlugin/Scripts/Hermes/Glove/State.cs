using HProt = Hermes.Protocol;

namespace Manus.Hermes.Glove
{
	/// <summary>
	/// Glove specific information, such as battery life and transmission strength.
	/// </summary>
	public class State
	{
		#region Properties
		public uint batteryPercentage { get; protected set; }
		public int transmissionStrength { get; protected set; }
		public int errorState { get; protected set; }
		public int optionState { get; protected set; }
		#endregion

		#region Fields
		HProt.Embedded.ImuStatus m_ImuStatus;
		#endregion

		public State(HProt.Hardware.ManusVR_PrimeOneGlove p_Glove)
		{
			batteryPercentage = p_Glove.BatteryPercentage;
			errorState = p_Glove.ErrorState;
			optionState = p_Glove.OptionState;
			m_ImuStatus = p_Glove.ImuStatus;
			transmissionStrength = p_Glove.TransmissionStrength;
		}

		public static implicit operator State(HProt.Hardware.ManusVR_PrimeOneGlove p_Glove)
		{
			return new State(p_Glove);
		}
	}
}
