using System.Collections.Generic;
using UnityEngine;

namespace ChessMonsterTactics
{
    public static class ResourceDatabase
    {
        public const string RESOURCE_MANIFEST_PATH = "_manifest";
        
        private static Dictionary<ushort, ResourceWrapper> Db;

        public static void Initialize()
        {
            Db = new Dictionary<ushort, ResourceWrapper>();

            Debug.Log("Loading resources");

            TextAsset textAsset = Resources.Load<TextAsset>(RESOURCE_MANIFEST_PATH);
            if (textAsset == null)
            {
                Debug.LogError("Unable to find resource manifest! Please go to Tools/Update Resource Manifest to generate a new one.");
            }

            int loaded = 0;
            foreach (string entry in textAsset.text.Split('\n', '\r'))
            {
                if (string.IsNullOrEmpty(entry))
                    continue;

                string[] values = entry.Split(';');
                Object res = Resources.Load(values[1]);
                if (res == null)
                {
                    Debug.LogWarningFormat("Manifest entries contains invalid path: \"{0}\". The entry is removed. Please make sure the resource manifest is updated.", values[0]);
                    continue;
                }

                if (ushort.TryParse(values[0], out ushort id))
                {
                    Db.Add(id, new ResourceWrapper
                    {
                        Path = values[1],
                        TypeHash = HashUtility.Hash16(res.GetType().ToString()),
                        NameHash = HashUtility.Hash16(res.name)
                    });

                    Resources.UnloadAsset(res);
                    loaded++;
                }
                else
                {
                    Debug.LogWarningFormat("Game resource has an invalid id at {0}", values[1]);
                }
            }

            Debug.LogFormat("Loaded {0} resources.", loaded);
        }

        public static T Load<T>(ushort id) where T : GameResource
        {
            if (id <= 0)
            {
                Debug.LogWarning("Invalid uid.");
                return null;
            }

            if (!Db.ContainsKey(id))
                Debug.LogError("Unable to find resource with id: " + id + ". Please make sure the resource manifest is updated.");

            if (Db.TryGetValue(id, out ResourceWrapper wrapper))
            {
                return Resources.Load<T>(wrapper.Path);
            }

            return null;
        }

        public static T Load<T>(string assetName) where T : GameResource
        {
            ushort resId = Query(w => w.NameHash == HashUtility.Hash16(assetName));
            if (resId <= 0)
            {
                Debug.LogWarning("Unable to find asset with name " + assetName);
                return null;
            }

            return Load<T>(resId);
        }

        private static ushort Query(System.Func<ResourceWrapper, bool> q)
        {
            foreach (KeyValuePair<ushort, ResourceWrapper> kvp in Db)
            {
                if (q.Invoke(kvp.Value))
                {
                    return kvp.Key;
                }
            }

            return 0;
        }

        private struct ResourceWrapper
        {
            public string Path;
            public uint TypeHash;
            public uint NameHash;
        }
    }
}
