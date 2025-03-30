using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ChessMonsterTactics
{
    public class ResourceManifestUpdater
    {
        /*
        *   TODO: Use automated asset processor to update the manifest
        */

        [MenuItem("Tools/Update Resource Manifest")]
        public static void UpdateResourceManifest()
        {
            List<string> resources = new List<string>();

            DirectoryInfo resFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "Resources"));
            foreach (DirectoryInfo dir in resFolder.GetDirectories("*.*", SearchOption.AllDirectories))
            {
                foreach (FileInfo file in dir.GetFiles("*.asset"))
                {
                    string relPath = GetRelativePath(file.FullName);
                    GameResource asset = AssetDatabase.LoadAssetAtPath<GameResource>(relPath);
                    if (asset != null)
                    {
                        string path = GetResourcesPath(relPath);
                        resources.Add(string.Format("{0};{1}", asset.Id, path));
                    }
                }
            }

            File.WriteAllLines(resFolder.FullName + string.Format("/{0}.txt", ResourceDatabase.RESOURCE_MANIFEST_PATH), resources);
            AssetDatabase.Refresh();
        }

        private static string GetRelativePath(string path)
        {
            int index = path.IndexOf("Assets");
            return path.Substring(index, path.Length - index);
        }

        private static string GetResourcesPath(string path)
        {
            return path.Replace("Assets\\Resources\\", "").Replace(".asset", "");
        }
    }
}
