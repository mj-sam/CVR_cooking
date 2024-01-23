using System;
using System.Collections.Generic;
using System.Linq;

namespace Manus.Utility
{
	public static class TemplateExtension
	{
		/// <summary>
		/// Performs actions on a collection of sources.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="p_Source"></param>
		/// <param name="p_Action"></param>
		public static void ForEach<T>(this IEnumerable<T> p_Source, Action<T> p_Action)
		{
			if (p_Source == null)
			{
				throw new ArgumentException("Argument cannot be null.", "source");
			}

			foreach (T value in p_Source)
			{
				p_Action(value);
			}
		}

		/// <summary>
		/// Resize the list, use the default value initializer if none specified.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="p_List"></param>
		/// <param name="p_Size"></param>
		/// <param name="p_Value"></param>
		public static void Resize<T>(this List<T> p_List, int p_Size, T p_Value = default)
		{
			int t_Cnt = p_List.Count;
			if (t_Cnt == p_Size) return;
			if (p_Size < t_Cnt)
			{
				p_List.RemoveRange(p_Size, t_Cnt - p_Size);
				return;
			}
			if (p_Size > p_List.Capacity)
			{
				p_List.Capacity = p_Size;
			}
			p_List.AddRange(Enumerable.Repeat(p_Value, p_Size - t_Cnt));
		}

		/// <summary>
		/// Make the list at least this size or larger.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="p_List"></param>
		/// <param name="p_Size"></param>
		public static void Upsize<T>(this List<T> p_List, int p_Size, T p_Value = default)
		{
			int t_Cnt = p_List.Count;
			if (t_Cnt >= p_Size) return;
			if (p_Size > p_List.Capacity)
			{
				p_List.Capacity = p_Size;
			}
			p_List.AddRange(Enumerable.Repeat(p_Value, p_Size - t_Cnt));
		}

		/// <summary>
		/// Make the list this size or smaller.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="p_List"></param>
		/// <param name="p_Size"></param>
		public static void Downsize<T>(this List<T> p_List, int p_Size)
		{
			int t_Cnt = p_List.Count;
			if (p_Size >= t_Cnt) return;
			p_List.RemoveRange(p_Size, t_Cnt - p_Size);
		}
	}
}
