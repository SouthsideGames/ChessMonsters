using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChessMonsterTactics
{
    /*
    *   This script contains all the initialization codes
    */
    internal class Game
    {
        private const string DEFAULT_PROFILE = "default";

#region Singleton
        private static Game Instance { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            Instance = new Game();
            Instance.OnInit();
        }
#endregion

        public static UserProfile CurrentProfile { get; private set; }

        private void OnInit()
        {
            // TODO:
            // Initialize services
            // Initialize dependencies
            // Sign-in

            ResourceDatabase.Initialize();

            InitializeNetworkManager();
            LoadUserProfile(DEFAULT_PROFILE, true);
        }

        private void InitializeNetworkManager()
        {
            /*
            *   TODO:
            *    Use settings asset to reference the prefab so that it's not loaded by string ref  
            */
            NetworkManager prefab = Resources.Load<NetworkManager>("Imp/NetworkManager");
            Object.Instantiate(prefab);

#if UNITY_EDITOR
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "MainMenu")
                return;
                
            DebugLoginHelper.NextSceneName = sceneName;
            SceneManager.LoadScene("DebugLoginScene");
#endif
        }

        public static void SaveUserProfile()
        {
            string path = Application.persistentDataPath + "\\profiles";
            SerializationUtility.WriteJson(CurrentProfile, CurrentProfile.UserName, path);
        }

        public static void LoadUserProfile(string profileName, bool createIfNotExists = false)
        {
            if (string.IsNullOrEmpty(profileName))
            {
                Debug.LogError("Invalid profile name: " + profileName);
                return;
            }

            string path = Application.persistentDataPath + "\\profiles";
            if (!SerializationUtility.IsFileExists(profileName, path) && createIfNotExists)
            {
                // TODO: Give initial pieces
                CurrentProfile = CreateUserProfile(profileName);
                SaveUserProfile();

                Debug.Log("Created new user profile.");
                return;
            }

            // Load locally
            CurrentProfile = SerializationUtility.ReadJson<UserProfile>(profileName, path);
            Debug.LogFormat("Profile {0} loaded.", profileName);
        }
    
        private static UserProfile CreateUserProfile(string username)
        {
            UserProfile profile = new UserProfile(username);

            MonsterData pawn = ResourceDatabase.Load<MonsterData>("Vine Pawn");
            MonsterData rook = ResourceDatabase.Load<MonsterData>("Stone Rook");
            MonsterData knight = ResourceDatabase.Load<MonsterData>("Inferno Knight");
            MonsterData bishop = ResourceDatabase.Load<MonsterData>("Aqua Bishop");
            MonsterData queen = ResourceDatabase.Load<MonsterData>("Mystic Queen");
            MonsterData king = ResourceDatabase.Load<MonsterData>("Guardian King");

            profile.Inventory.AddItem(pawn, 0, 8);
            profile.Inventory.AddItem(rook, 0, 4);
            profile.Inventory.AddItem(knight, 0, 4);
            profile.Inventory.AddItem(bishop, 0, 4);
            profile.Inventory.AddItem(queen, 0, 4);
            profile.Inventory.AddItem(king, 0, 4);
            
            return profile;
        }
    }
}
