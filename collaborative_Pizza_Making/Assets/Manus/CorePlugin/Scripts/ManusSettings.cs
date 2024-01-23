using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus
{
    /// <summary>
    /// The global Settings used by the Manus Manager.
    /// </summary>
    [CreateAssetMenu(fileName = "ManusSettings", menuName = "Manus/Settings", order = 1)]
    public class ManusSettings : ScriptableObject
    {
        public Hand.Gesture.GestureBase[] gestures;
    }
}
