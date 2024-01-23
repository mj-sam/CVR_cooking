using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manus.Utility
{
    public static class TransformExtension
    {
        /// <summary>
        /// Finds a child transform of a given transform recursively with the given criteria.
        /// </summary>
        /// <param name="p_Transform">Transform to start searching the children of.</param>
        /// <param name="p_ChildCriteria">The name criteria the child must have.</param>
        /// <returns>Returns NULL if not found.</returns>
        public static Transform FindDeepChildCriteria(Transform p_Transform, string[] p_ChildCriteria)
        {
            if (p_Transform == null)
                return null;

            foreach (Transform t_Child in p_Transform)
            {
                if (StringContainsCriteria(t_Child.name, p_ChildCriteria))
                {
                    return t_Child;
                }

                if (t_Child.childCount > 0)
                {
                    Transform t_FoundTransform = FindDeepChildCriteria(t_Child, p_ChildCriteria);
                    if (t_FoundTransform != null)
                    {
                        return t_FoundTransform;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a string contains each of the criterea.
        /// </summary>
        /// <param name="p_String">The string to check.</param>
        /// <param name="p_Criteria">The Criteria of the string.</param>
        /// <returns>True if it contains all of the criteria.</returns>
        public static bool StringContainsCriteria(string p_String, string[] p_Criteria)
        {
            int t_Count = 0;
            foreach (var t_Crit in p_Criteria)
            {
                if (p_String.ToUpper().Contains(t_Crit.ToUpper()))
                {
                    t_Count++;
                }
            }
            return t_Count == p_Criteria.Length;
        }

        /// <summary>
        /// Finds a child transform of a given transform recursively, using a breadth-first search.
        /// </summary>
        /// <param name="p_Transform">The Transform that the children should be searched for.</param>
        /// <param name="p_Name">The name of the (grand)child to be found.</param>
        /// <returns>The child Transform with the given name if it was found, and null otherwise.</returns>
        public static Transform FindDeepChild(this Transform p_Transform, string p_Name)
        {
            // Look for a direct child called name.
            Transform t_Result = p_Transform.Find(p_Name);
            if (t_Result != null)
            {
                return t_Result;
            }

            // No direct child called name found. Do a search for each child.
            foreach (Transform t_Child in p_Transform)
            {
                t_Result = t_Child.FindDeepChild(p_Name);

                if (t_Result != null)
                {
                    return t_Result;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the hierarchical string of this Transform.
        /// </summary>
        /// <param name="p_Transform"></param>
        /// <returns></returns>
        public static string GetPath(this Transform p_Transform)
        {
            if (p_Transform.parent == null)
                return "/" + p_Transform.name;
            return p_Transform.parent.GetPath() + "/" + p_Transform.name;
        }

        /// <summary>
        /// Finds a transform of a given transform recursively, using a breadth-first search.
        /// Unlike FindDeepChild this function includes the Root in its search, and allows for multiple name matches.
        /// </summary>
        /// <param name="p_Root">The Transform that should be searched in</param>
        /// <param name="p_CandidateNames">The possible names.</param>
        /// <returns>The Transform with the given name if it was found, and null otherwise.</returns>
        internal static Transform FindChildByRecursion(this Transform p_Root, List<string> p_CandidateNames)
        {
            if (p_Root == null)
                return null;

            //	Could in fact be the root...
            if (p_CandidateNames.Any(w => p_Root.name.ToLower().Contains(w)))
                return p_Root;

            //	... or one of the siblings...
            foreach (Transform t_Trans in p_Root)
                if (p_CandidateNames.Any(w => t_Trans.name.ToLower().Contains(w)))
                    return t_Trans;

            //	... or kids.
            foreach (Transform t_Child in p_Root)
            {
                var t_Result = t_Child.FindChildByRecursion(p_CandidateNames);
                if (t_Result != null)
                    return t_Result;
            }
            return null;
        }
    }
}
