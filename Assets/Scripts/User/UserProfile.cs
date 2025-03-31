using Newtonsoft.Json;

namespace ChessMonsterTactics
{
    [System.Serializable]
    internal class UserProfile
    {
        [JsonProperty] private string _username;
        [JsonProperty] private TeamConfig _teamConfig;
        [JsonProperty] private Inventory _inventory;

        [JsonIgnore] public string UserName { get => _username; }
        [JsonIgnore] public Inventory Inventory { get => _inventory; }
        [JsonIgnore] public TeamConfig TeamConfig { get => _teamConfig; }

        public UserProfile(string username)
        {
            _username = username;
            _teamConfig = new TeamConfig();
            _inventory = new Inventory();
        }
    }
}
