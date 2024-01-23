using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Hand;
using Manus.Hand.Gesture;

namespace Manus.Hand
{
    public class GestureLogger : MonoBehaviour
    {
        [SerializeField]
        private List<GestureSimple> m_LoggedGestures;

        private Hand m_Hand;

        void Start()
        {
            m_Hand = GetComponent<Hand>();
        }

        void FixedUpdate()
        {
            foreach (GestureSimple t_Gesture in m_LoggedGestures)
            {
                if (t_Gesture != null)
                {
                    if (t_Gesture.Evaluate(m_Hand))
                    {
                        Debug.Log(t_Gesture + " detected");
                    }
                    else
                    {
                        Debug.Log(t_Gesture + "not detected");
                    }
                }
            }
        }
    }
}