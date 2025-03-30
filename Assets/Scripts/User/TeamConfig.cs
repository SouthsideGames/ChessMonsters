using Newtonsoft.Json;
using Unity.Netcode;

namespace ChessMonsterTactics
{
    [System.Serializable]
    internal class TeamConfig
    {
        [JsonProperty] private ushort[] _pieces;

        [JsonIgnore] public ushort[] Pieces { get => _pieces; }

        public TeamConfig(int teamSize = 16)
        {
            _pieces = new ushort[teamSize];
        }

        public bool Validate()
        {
            for (int i = 0; i < _pieces.Length; i++)
            {
                if (_pieces[i] <= 0)
                    return false;
            }

            return true;
        }
    }
}
