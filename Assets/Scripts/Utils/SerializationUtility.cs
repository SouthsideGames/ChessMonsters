using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ChessMonsterTactics
{
    internal static class SerializationUtility
    {
        /// <summary>
        /// Serialize an object to json
        /// </summary>
        /// <param name="o">target object</param>
        public static string SerializeToJson(object o)
        {
            string json = JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return json;
        }

        /// <summary>
        /// Deserialize object from json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Serialize an object to a file
        /// </summary>
        /// <param name="o">target object</param>
        /// <param name="file">file name</param>
        /// <param name="path">relative path</param>
        public static void WriteJson(object o, string file, string path)
        {
            string fullPath = GetPath(file, path);
            File.WriteAllText(fullPath, SerializeToJson(o));
            Debug.LogFormat("File \"{0}\" written successfully.", fullPath);
        }

        /// <summary>
        /// Deserialize an object from a file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">file name</param>
        /// <param name="path">relative path</param>
        /// <returns></returns>
        public static T ReadJson<T>(string file, string path)
        {
            string fullPath = GetPath(file, path);
            string json = File.ReadAllText(fullPath);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogErrorFormat("Unable to read from \"{0}\". File is empty.", fullPath);
                return default;
            }

            T result = JsonConvert.DeserializeObject<T>(json);

            if (result == null)
            {
                Debug.LogErrorFormat("Unable to read from \"{0}\". Error while deserializing.", fullPath);
                return default;
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool IsFileExists(string file, string path)
        {
            return File.Exists(GetPath(file, path));
        }

        private static string GetPath(string file, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Path.Combine(path, file);
        }
    }
}
