using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    internal class GameServerManager : GameManager<GameServerManager>
    {
        private static readonly int PLAYER_LENGTH = 2;
        private static readonly Vector2Int NULL_CELL = new Vector2Int(-1, -1);

        private GameClientManager _client;
        private PlayerSlot[] _slots;
        private AttackSolver _attackSolver;

        private ServerState _currentState;
        private NetworkVariable<ushort> _currentTurn;
        private NetworkVariable<bool> _inputEnabled;
        private NetworkVariable<bool> _activeAbilityEnabled;
        private NetworkVariable<Vector2Int> _selected; // Selected cell
        private NetworkVariable<Vector2Int> _targeted; // Targeted cell
        private HashSet<Vector2Int> _possibleMoves;

        public PlayerSlot[] Slots { get => _slots; }
        public PlayerSlot CurrentSlot { get => _slots[_currentTurn.Value]; }
        public Vector2Int SelectedCell { get => _selected.Value; }
        public Vector2Int TargetedCell { get => _targeted.Value; }
        public bool EnableInput { get => _inputEnabled.Value; }
        public bool ActiveAbilityEnabled { get => _activeAbilityEnabled.Value; }

        private void Awake()
        {
            _selected = new NetworkVariable<Vector2Int>(NULL_CELL);
            _targeted = new NetworkVariable<Vector2Int>(NULL_CELL);
            _activeAbilityEnabled = new NetworkVariable<bool>(false);
            _currentTurn = new NetworkVariable<ushort>(0);
            _inputEnabled = new NetworkVariable<bool>(true);
            _possibleMoves = new HashSet<Vector2Int>();

            _client = GetComponent<GameClientManager>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsServer)
                return;

            SwitchState(new ServerState_WaitForPlayer(this, _client));
            _attackSolver = new AttackSolver();
            _currentTurn.Value = 1;
            // WaitForPlayersRpc();
        }

        protected override void Update()
        {               
            base.Update();

            if (!IsServer)
                return;

            if (_currentState != null)
                _currentState.OnUpdate();
        }

        public void SwitchState(ServerState state)
        {
            if (!IsServer)
                return;

            if (_currentState != null)
                _currentState.OnExit();

            _currentState = state;
            _currentState.OnEnter();

            // Debug.Log(_currentState);
        }

        [Rpc(SendTo.Server)]
        public void NextTurnRpc()
        {
            if (!IsServer)
                return;

            _currentTurn.Value = (ushort)(_currentTurn.Value == 1 ? 0 : 1);
        }

        [Rpc(SendTo.Server)]
        public void SetInputEnabledRpc(bool enabled)
        {
            _inputEnabled.Value = enabled;
        }

        [Rpc(SendTo.Server)]
        public void ProcessClickRpc(ulong localId, Vector2Int cellPos)
        {
            // If not the user's current turn, return
            if (localId != CurrentSlot.LocalId)
                return;

            if (!IsServer || _inputEnabled.Value == false)
                return;

            if (IsSelectingPiece() && _possibleMoves.Contains(cellPos))
            {
                _targeted.Value = cellPos;

                HandleMovementRpc(cellPos);
                SwitchState(new ServerState_TransitionNextTurn(this, _client));
                return;
            }

            _targeted.Value = NULL_CELL;
            _selected.Value = NULL_CELL;
            _possibleMoves.Clear();

            Monster monster = Chessboard.GetMonster(cellPos);
            if (monster == null || monster.Owner != localId)
                return;

            _selected.Value = cellPos; // NOTE: Mirror the get position
            Monster selectedMonster = Chessboard.GetMonster(_selected.Value);
            foreach (Vector2Int pos in monster.MovementSolver.CheckForValidMovement(cellPos, Chessboard))
            {
                Chessboard.Node n = Chessboard.GetNode(pos);
                if (n == null)
                    continue;

                MovementType moveType = MovementType.Normal;
                if (n.Monster != null && n.Monster.Owner != selectedMonster.Owner)
                {
                    moveType = MovementType.Attack;
                }

                _possibleMoves.Add(pos);
                _client.ShowMoveHighlightsRpc(selectedMonster.Owner, pos, moveType);
            }
        }

        [Rpc(SendTo.Server)]
        public void RegisterClientToServerRpc(ulong localId, UserProfile profile)
        {
            Debug.Log("Registering player. Local ID: " + localId);

            if (_slots == null)
                _slots = new PlayerSlot[PLAYER_LENGTH];

            if (_slots[0] == null)
            {
                _slots[0] = new PlayerSlot(localId, profile, BoardPosition.Bottom);
                SetClientBoardPositionRpc(localId, BoardPosition.Bottom);

                Debug.LogFormat("Player {0} registered!", localId);
            }
            else if (_slots[1] == null)
            {
                _slots[1] = new PlayerSlot(localId, profile, BoardPosition.Top);
                SetClientBoardPositionRpc(localId, BoardPosition.Top);

                Debug.LogFormat("Player {0} registered!", localId);
            }

            for (int i = 0; i < _slots.Length; i++)
            {
                SetLocalSlotRpc(_slots[i], i);
            }
        }

        [Rpc(SendTo.Server)]
        public void ClearPlayerSlotsRpc()
        {
            if (_slots == null)
            {
                _slots = new PlayerSlot[2];
                return;
            }

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = null;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetClientBoardPositionRpc(ulong id, BoardPosition position)
        {
            if (_client != null && _client.ClientID == id)
            {
                _client.SetBoardPositionRpc(position);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetLocalSlotRpc(PlayerSlot slots, int index)
        {
            if (_slots == null)
                _slots = new PlayerSlot[2];

            _slots[index] = slots;
        }

        public bool IsSelectingPiece()
        {
            return _selected.Value != NULL_CELL;
        }

        [Rpc(SendTo.Server)]
        private void HandleMovementRpc(Vector2Int to)
        {
            Vector2Int current = _selected.Value;
            _selected.Value = NULL_CELL;

            Chessboard.Node target = Chessboard.GetNode(to);
            if (target.Monster != null)
            {
                Attack(current, to);
                return;
            }

            // Moves the current monster to the target slot
            MovePiece(current, to);

            // Moves the current monster to the targeted position
            _client.MovePieceRpc(target.MonsterId, to);
        }

        private void Attack(Vector2Int attackerPos, Vector2Int targetPos)
        {
            Chessboard.Node attacker = Chessboard.GetNode(attackerPos);
            Chessboard.Node target = Chessboard.GetNode(targetPos);
            
            AttackData attack = _attackSolver.Calculate(attacker.Monster, target.Monster);

            // Reduce the monster health
            target.Monster.TakeDamage(attack.TotalDamage);
            target.Monster.AbilityController.TryEvaluateTriggerCondition(AbilityTrigger.OnReceiveDamage);
            attacker.Monster.AbilityController.TryEvaluateTriggerCondition(AbilityTrigger.OnAttack);

            // Perform the attack on client-side
            _client.PerformAttackRpc(attacker.MonsterId, target.MonsterId, attack);

            if (target.Monster.CurrentHealth <= 0)
            {
                DestroyMonster(target);
                MovePiece(attackerPos, targetPos);
            }
        }

        private void MovePiece(Vector2Int from, Vector2Int to)
        {
            Chessboard.Node fromNode = Chessboard.GetNode(from);
            Chessboard.Node toNode = Chessboard.GetNode(to);

            if (fromNode.Monster == null)
            {
                Debug.LogWarningFormat("The node {0} does not contains piece.", from);
                return;
            }

            toNode.SetMonster(fromNode.Monster, true);
            toNode.Monster.Moved = true;

            fromNode.SetMonster(null);
        }

        [Rpc(SendTo.Server)]
        public void TurnStartedRpc()
        {
            foreach (Monster monster in Chessboard.Monsters)
            {
                if (monster.Owner == CurrentSlot.LocalId)
                    monster.OnTurnStarted();
            }

            _client.TurnStartedRpc(CurrentSlot.LocalId);
        }

        [Rpc(SendTo.Server)]
        public void TurnEndedRpc()
        {
            foreach (Monster monster in Chessboard.Monsters)
            {
                if (monster.Owner == CurrentSlot.LocalId)
                    monster.OnTurnPassed();
            }

            _client.TurnEndedRpc(CurrentSlot.LocalId);
        }

        [Rpc(SendTo.Server)]
        public void ToggleActiveAbilityRpc(bool active)
        {
            _activeAbilityEnabled.Value = active;
#if UNITY_EDITOR
            Debug.Log(GetSelectedMonsterId() + " active ability " + (active ? "enabled" : "disabled"));
#endif
        }

        /// <summary>
        /// Get the selected piece/monster id
        /// </summary>
        /// <returns></returns>
        public ushort GetSelectedMonsterId()
        {
            Chessboard.Node node = Chessboard.GetNode(_selected.Value);
            if (node != null && node.Monster != null)
                return node.MonsterId;

            return 0;
        }

        /// <summary>
        /// Get the currently targetted piece/monster id that being attacked or moved to
        /// </summary>
        /// <returns></returns>
        public ushort GetTargetedMonsterId()
        {
            Chessboard.Node node = Chessboard.GetNode(_targeted.Value);
            if (node != null && node.Monster != null)
                return node.MonsterId;

            return 0;
        }

        public abstract class ServerState
        {
            protected GameServerManager Server { get; private set; }
            protected GameClientManager Client { get; private set; }

            public ServerState() { }

            public ServerState(GameServerManager server, GameClientManager client)
            {
                Server = server;
                Client = client;
            }

            public abstract void OnEnter();
            public abstract void OnUpdate();
            public abstract void OnExit();
        }

        public class ServerState_WaitForPlayer : ServerState
        {
            public ServerState_WaitForPlayer(GameServerManager server, GameClientManager client) 
                : base(server, client) { }

            public override void OnEnter()
            {

            }

            public override void OnUpdate()
            {
                if (Server.Slots != null && Server.Slots[0] != null && Server.Slots[1] != null)
                {
                    Server.SwitchState(new ServerState_InitializePlayers(Server, Client));
                }

#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.P))
                {
                    ulong localId = Server.NetworkManager.LocalClientId;

                    if (Server.Slots != null && Server.Slots[0] != null)
                    {
                        // The first slot already filled by client, swap it
                        PlayerSlot client = Server.Slots[0];

                        Server.ClearPlayerSlotsRpc();
                        Server.RegisterClientToServerRpc(localId, Game.CurrentProfile);
                        Server.RegisterClientToServerRpc(client.LocalId, client.Profile);

                        Server.SwitchState(new ServerState_InitializePlayers(Server, Client));
                        return;
                    }

                    for (int i = 0; i < PLAYER_LENGTH; i++)
                    {
                        Server.RegisterClientToServerRpc(localId, Game.CurrentProfile);
                    }
                    
                    Server.SwitchState(new ServerState_InitializePlayers(Server, Client));
                    return;
                }
#endif
            }

            public override void OnExit()
            {
                
            }
        }

        public class ServerState_InitializePlayers : ServerState
        {
            private float _waitTime = 1.0f;

            public ServerState_InitializePlayers(GameServerManager server, GameClientManager client) 
                : base(server, client) { }

            public override void OnEnter()
            {
                foreach (PlayerSlot slot in Server.Slots)
                {
                    // Spawn pieces on server and client
                    foreach (MonsterBuilder builder in SpawnPieces(slot.LocalId, slot.Profile.TeamConfig, slot.BoardPosition))
                    {
                        Client.SpawnPieceRpc(builder);
                    }
                }

                Client.DispatchEventRpc(GameEvents.EVT_INITIALIZE_PLAYERS);
            }

            public override void OnUpdate()
            {
                _waitTime -= Time.deltaTime;
                if (_waitTime <= 0)
                {
                    Server.SwitchState(new ServerState_TransitionNextTurn(Server, Client));
                }
            }

            public override void OnExit()
            {
                
            }

            private IEnumerable<MonsterBuilder> SpawnPieces(ulong localId, TeamConfig config, BoardPosition position)
            {
                int index = 0;
                switch (position)
                {
                    case BoardPosition.Bottom:
                        int height = config.Pieces.Length / Server.Chessboard.Width;
                        for (int y = height - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < Server.Chessboard.Width; x++)
                            {
                                ushort resId = config.Pieces[index];
                                MonsterBuilder builder = new MonsterBuilder(0, resId)
                                    .SetOwner(localId)
                                    .SetCellPos(new Vector2Int(x, y))
                                    .SetBoardPos(BoardPosition.Bottom)
                                    .SetResourceId(resId);

                                builder.Build(Server.Chessboard);
                                yield return builder;
                    
                                index++;
                            }
                        }
                        break;
                    case BoardPosition.Top:
                        for (int y = Server.Chessboard.Height - 2; y < Server.Chessboard.Height; y++)
                        {
                            for (int x = 0; x < Server.Chessboard.Width; x++)
                            {
                                ushort resId = config.Pieces[index];
                                MonsterBuilder builder = new MonsterBuilder(0, resId)
                                    .SetOwner(localId)
                                    .SetCellPos(new Vector2Int(x, y))
                                    .SetBoardPos(BoardPosition.Top)
                                    .SetResourceId(resId);

                                builder.Build(Server.Chessboard);
                                yield return builder;

                                index++;
                            }
                        }
                        break;
                }
            }
        }

        public class ServerState_BeginTurn : ServerState
        {
            private float _turnTimer;

            public ServerState_BeginTurn(GameServerManager server, GameClientManager client) 
                : base(server, client) { }

            public override void OnEnter()
            {
                // TODO: Show switch player animation
                Server.TurnStartedRpc();
                Server.SetInputEnabledRpc(true);
                Client.DispatchEventRpc(GameEvents.EVT_TURN_BEGIN);

                _turnTimer = GameSettings.GetInstance().TurnDuration;
            }

            public override void OnUpdate()
            {
                _turnTimer -= Time.deltaTime;
                if (_turnTimer <= 0)
                {
                    // Next Turn
                    Server.SwitchState(new ServerState_TransitionNextTurn(Server, Client));
                }
            }

            public override void OnExit()
            {
                Server.SetInputEnabledRpc(false);
                Server.TurnEndedRpc();
            }
        }

        public class ServerState_TransitionNextTurn : ServerState
        {
            private float _transitionTime;

            public ServerState_TransitionNextTurn(GameServerManager server, GameClientManager client) 
                : base(server, client) { }

            public override void OnEnter()
            {
                Server.NextTurnRpc();
                Client.DispatchEventRpc(GameEvents.EVT_TURN_TRANSITION);
                _transitionTime = GameSettings.GetInstance().TurnTransition;
            }

            public override void OnUpdate()
            {
                _transitionTime -= Time.deltaTime;
                if (_transitionTime <= 0)
                {
                    Server.SwitchState(new ServerState_BeginTurn(Server, Client));
                }
            }

            public override void OnExit()
            {
            }
        }

        [System.Serializable]
        public class PlayerSlot : INetworkSerializable
        {
            private ulong _localId;
            private UserProfile _profile;
            private BoardPosition _position;

            public ulong LocalId { get => _localId; }
            public UserProfile Profile { get => _profile; }
            public BoardPosition BoardPosition { get => _position; }

            public PlayerSlot() { }

            public PlayerSlot(ulong localId, UserProfile profile, BoardPosition position)
            {
                _localId = localId;
                _profile = profile;
                _position = position; 
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                if (serializer.IsWriter)
                {
                    serializer.GetFastBufferWriter().WriteValueSafe(_localId);
                    serializer.GetFastBufferWriter().WriteValueSafe(_profile);
                    serializer.GetFastBufferWriter().WriteValueSafe(_position);
                }
                else
                {
                    serializer.GetFastBufferReader().ReadValueSafe(out _localId);
                    serializer.GetFastBufferReader().ReadValueSafe(out _profile);
                    serializer.GetFastBufferReader().ReadValueSafe(out _position);
                }
            }
        }
    }
}
