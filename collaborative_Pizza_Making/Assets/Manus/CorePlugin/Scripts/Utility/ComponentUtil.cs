using UnityEngine;

namespace Manus.Utility
{
	public static class ComponentUtil
	{
		/// <summary>
		/// Tries to find component of the supplied type on the gameobject.
		/// If no component can be found, it will try to find or instantiate the component in the scene using the <see cref="FindOrInstantiateComponent{T}"/> method.
		/// </summary>
		public static T GetFindOrInstantiateComponent<T>(GameObject p_GameObject) where T : Component
		{
			T t_Component = p_GameObject.GetComponent<T>();
			return t_Component ? t_Component : FindOrInstantiateComponent<T>();
		}

		/// <summary>
		/// Find a component of the supplied type in the scene.
		/// If no component can be found, a gameobject will be instantiated with the component attached.
		/// This attached component will be returned.
		/// </summary>
		public static T FindOrInstantiateComponent<T>() where T : Component
		{
			T t_Component = Object.FindObjectOfType<T>();
			return t_Component ? t_Component : new GameObject(typeof(T).Name).AddComponent<T>();
		}

		/// <summary>
		/// Find a component of the supplied type attached to the supplied gameobject.
		/// If no component can be found, a gameobject will be added to the supplied gameobject.
		/// This attached component will be returned.
		/// </summary>
		public static T GetOrAddComponent<T>(this GameObject p_GameObject) where T : Component
		{
			T t_Component = p_GameObject.GetComponent<T>();
			return t_Component ? t_Component : p_GameObject.AddComponent<T>();
		}
	}
}
