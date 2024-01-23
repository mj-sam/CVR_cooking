namespace Manus
{
	namespace Networking
	{
		namespace Message
		{
			/// <summary>
			/// Custom message types should start with ID's LARGER than CustomMessage.
			/// All ID's before the CustomMessage are reserved for future development!
			/// </summary>
			public enum Type : ushort
			{
				IDInitialize, /*!< Initialize ID message */

				ObjectsInitialize, /*!< Initialize objects message */
				ObjectsUpdate, /*!< Update objects message */

				ObjectCreate, /*!< Create an object message */
				ObjectDestroy, /*!< Destroy an object message */
				ObjectChangeOwner, /*<! Change an object's owner message */

				GrabbableObjectSync, /*<! Grabbable Object sync message */

				CustomMessage = 1000, /*<! all custom messages IDs should come after this message! */
			}
		}
	}
}
