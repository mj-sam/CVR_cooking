using Unity.Netcode.Components;
using UnityEngine;

namespace SelfMadeFiles.Scripts.Network.components
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}