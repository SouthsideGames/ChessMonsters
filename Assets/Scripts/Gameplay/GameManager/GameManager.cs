using Unity.Netcode;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [RequireComponent(typeof(GameClientManager))]
    internal abstract class GameManager<T> : InstancedNetworkBehaviour<T> where T : NetworkBehaviour
    {
        [SerializeField] private Chessboard _chessboard;

        private Camera _camera;

        public Camera Cam { get => _camera; }
        public Chessboard Chessboard { get => _chessboard; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _camera = Camera.main;
            _chessboard.Initialize();
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                HandleClicks(worldPos);
            }
        }

        protected virtual void HandleClicks(Vector2 worldPos)
        {

        }
    
        protected virtual void DestroyMonster(Chessboard.Node node)
        {
            Destroy(node.Monster.gameObject);
            node.SetMonster(null);
        }
    }
}
