using Unity.Netcode;
using UnityEngine;

namespace ChessMonsterTactics
{
    public abstract class InstancedNetworkBehaviour<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T _currentInstance;
        public static T Current => _currentInstance;

        public override void OnNetworkSpawn()
        {
            if (_currentInstance != null)
            {
                Debug.LogWarning("Multiple instance of " + typeof(T) + " found.");
                Destroy(this.gameObject);
                return;
            }

            _currentInstance = gameObject.GetComponent<T>();
        }

        public override void OnNetworkDespawn()
        {
            _currentInstance = null;
        }
    }
}
