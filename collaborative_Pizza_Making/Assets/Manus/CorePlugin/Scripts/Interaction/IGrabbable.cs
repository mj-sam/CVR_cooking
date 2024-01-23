
namespace Manus.Interaction
{
	/// <summary>
	/// This interface is required for objects to be grabbable.
	/// This interface must be used in combination with a MonoBehaviour in order for it to actually function.
	/// It allows for specific behaviour when an object is being grabbed or released.
	/// </summary>
	public interface IGrabbable
	{
		/// <summary>
		/// Called when this starts getting grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		void OnGrabbedStart(GrabbedObject p_Object);

		/// <summary>
		/// Called when this stops being grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		void OnGrabbedEnd(GrabbedObject p_Object);

		/// <summary>
		/// Called when a new grabber starts grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the added grabber</param>
		void OnAddedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info);

		/// <summary>
		/// Called when a grabber stops grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the removed grabber</param>
		void OnRemovedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info);

		/// <summary>
		/// Called every FixedUpdate when this is grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		void OnGrabbedFixedUpdate(GrabbedObject p_Object);
	}
}
