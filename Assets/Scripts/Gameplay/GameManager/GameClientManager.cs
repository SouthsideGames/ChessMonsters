using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

namespace ChessMonsterTactics.Gameplay
{
    internal class GameClientManager : GameManager<GameClientManager>
    {
        [SerializeField] private GameServerManager _server;

        private GameObject _highlightPrefab;
        private List<GameObject> _moveHighlights;
        private Dictionary<string, System.Action> _events;

        private BoardPosition _boardPosition;

        public ulong ClientID { get => NetworkManager.LocalClientId; }

        public event System.Action<Monster> OnSelectPiece;
        public event System.Action OnDeselectPiece;

        // TODO: Use custom event system
        public event System.Action<Monster> OnMonsterTakeDamage;

        private void Awake()
        {
            _events = new Dictionary<string, System.Action>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
                return;

            _highlightPrefab = GameSettings.GetInstance().Gameplay.HighlightPrefab;
            _moveHighlights = new List<GameObject>();

            _server.RegisterClientToServerRpc(ClientID, Game.CurrentProfile);
        }

        protected override void HandleClicks(Vector2 worldPos)
        {
            if (IsServer)
                return;
            
            Vector2Int cellPos = Chessboard.WorldToCell(worldPos);                
            if (!Chessboard.IsWithinBounds(cellPos))
                return;

            ClearMoveHighlightsRpc();
            Chessboard.Node node = Chessboard.GetNode(cellPos);

            _server.ProcessClickRpc(ClientID, _boardPosition == BoardPosition.Top ? Chessboard.MirrorY(cellPos) : cellPos);

            if (node != null && node.Monster != null)
                OnSelectPiece?.Invoke(node.Monster);
            else
                OnDeselectPiece?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SetBoardPositionRpc(BoardPosition boardPosition)
        {
            if (_boardPosition != BoardPosition.None)
                return;

            _boardPosition = boardPosition;
            Debug.LogFormat("ID: {0}, BoardPosition: {1}", ClientID, _boardPosition);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnPieceRpc(MonsterBuilder builder)
        {
            Vector2Int cellPos = builder.CellPos;
            if (_boardPosition == BoardPosition.Top)
                cellPos = Chessboard.MirrorY(builder.CellPos); 

            builder.SetCellPos(cellPos)
                .Build(Chessboard);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void MovePieceRpc(ushort pieceId, Vector2Int to)
        {
            Vector2Int cellPos = Chessboard.GetCellPosition(pieceId);
            Vector2Int targetPos = _boardPosition == BoardPosition.Top ? Chessboard.MirrorY(to) : to;

            Chessboard.Node fromNode = Chessboard.GetNode(cellPos);
            Chessboard.Node toNode = Chessboard.GetNode(targetPos);

            Monster monster = fromNode.Monster;
            if (monster == null)
            {
                Debug.LogWarningFormat("The node {0} does not contains piece.", cellPos);
                return;
            }

            Sequence sequence = DOTween.Sequence();
            sequence.Append(MoveMonster(monster, toNode.Position));
            sequence.OnComplete(() => {
                toNode.SetMonster(monster);
                toNode.Monster.Moved = true;

                fromNode.SetMonster(null);
            });
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PerformAttackRpc(ushort attackerId, ushort targetId, AttackData attackData)
        {
            /*
            *   NOTE:
            *       The damage and the defense value should only be calculated by the server.
            */

            Chessboard.Node attacker = Chessboard.GetNode(attackerId);
            Chessboard.Node target = Chessboard.GetNode(targetId);

            /*
            *   TODO:
            *       Animate sequences for attack animations, and send rpc callback to server
            *       when the attack completed.
            */

            attacker.Monster.SetSpriteSortOrder(Monster.SortingOrder.Front);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(MoveMonster(attacker.Monster, target.Position));
            sequence.AppendCallback(() => {

                // TODO: Use active ability
                target.Monster.TakeDamage(attackData.TotalDamage);
                target.Monster.AbilityController.TryEvaluateTriggerCondition(AbilityTrigger.OnReceiveDamage);
                attacker.Monster.AbilityController.TryEvaluateTriggerCondition(AbilityTrigger.OnAttack);

                DamagePopup.Show(target.Position, attackData.TotalDamage);
                
                OnMonsterTakeDamage?.Invoke(target.Monster);

                // TODO: Create an event system to support parameters
                // DispatchEventRpc(GameEvents.EVT_PIECE_TAKE_DAMAGE);

                if (target.Monster.CurrentHealth <= 0)
                {
                    // Monster died
                    DestroyMonster(target);
                    target.SetMonster(attacker.Monster);
                    attacker.SetMonster(null);
                }
                else
                {
                    MoveMonster(attacker.Monster, attacker.Position);
                }
            });
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ShowMoveHighlightsRpc(ulong ownerId, Vector2Int cellPos, MovementType movementType)
        {
            if (ownerId != ClientID)
                return;

            Color c;
            switch (movementType)
            {
                default:
                case MovementType.Normal:
                    c = UISettings.GetInstance().MoveHighlightNormalColor;
                    break;
                case MovementType.Attack:
                    c = UISettings.GetInstance().MoveHighlightAttackColor;
                    break;
            }

            SpriteRenderer clone = Instantiate(_highlightPrefab.GetComponent<SpriteRenderer>(), Chessboard.transform);
            Vector2Int targetPos = _boardPosition == BoardPosition.Top ? Chessboard.MirrorY(cellPos) : cellPos;
            Chessboard.Node node = Chessboard.GetNode(targetPos);
            clone.transform.position = node.Position;
            clone.color = c;

            _moveHighlights.Add(clone.gameObject);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ClearMoveHighlightsRpc()
        {            
            for (int i = _moveHighlights.Count - 1; i >= 0; i--)
            {
                Destroy(_moveHighlights[i]);
            }

            _moveHighlights.Clear();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DispatchEventRpc(string evt)
        {
            if (!_events.ContainsKey(evt))
                return;
            
            _events[evt]?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ToggleActiveAbilityRpc(bool active)
        {
            Chessboard.Node node = Chessboard.GetNode(_server.SelectedCell);
            if (node != null && node.Monster != null)
            {
                _server.ToggleActiveAbilityRpc(active);
            }
        }

        /// <summary>
        /// Called by the server
        /// </summary>
        /// <param name="currentTurn"></param>
        [Rpc(SendTo.ClientsAndHost)]
        public void TurnStartedRpc(ulong currentTurn)
        {
            foreach (Monster monster in Chessboard.Monsters)
            {
                if (monster.Owner == currentTurn)
                    monster.OnTurnStarted();
            }
        }

        /// <summary>
        /// Called by the server
        /// </summary>
        /// <param name="currentTurn"></param>
        [Rpc(SendTo.ClientsAndHost)]
        public void TurnEndedRpc(ulong currentTurn)
        {
            foreach (Monster monster in Chessboard.Monsters)
            {
                if (monster.Owner == currentTurn)
                    monster.OnTurnPassed();
            }
        }

        private Tween MoveMonster(Monster monster, Vector2 pos)
        {
            return monster.transform.DOMove(pos, 0.75f).SetEase(Ease.InBack);
        }

        public void RegisterEvent(string evt, System.Action listener)
        {
            if (IsServer)
                return;

            if (!_events.TryGetValue(evt, out System.Action act))
            {
                _events.Add(evt, () => {});
            }

            _events[evt] += listener;
        }

        public void UnregisterEvent(string evt, System.Action listener)
        {
            if (IsServer)
                return;

            if (!_events.ContainsKey(evt))
                return;

            _events[evt] -= listener;
        }
    }
}
