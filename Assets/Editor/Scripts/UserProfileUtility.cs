using System.IO;
using UnityEditor;
using UnityEngine;

namespace ChessMonsterTactics
{
    public class UserProfileUtility
    {
        private static string PATH = Application.persistentDataPath + "/Profiles/";

        [MenuItem("Tools/User Profile/Delete Default Profile")]
        public static void DeleteSaveFile()
        {
            string defaultProfile = Path.Combine(PATH, "default"); 
            if (File.Exists(defaultProfile))
            {
                File.Delete(defaultProfile);
                Debug.Log("Default profile deleted");
            }
        }

        [MenuItem("Tools/User Profile/Show In Explorer")]
        public static void OpenSaveFileDirectory()
        {
            System.Diagnostics.Process.Start(PATH);
        }
    }
}
