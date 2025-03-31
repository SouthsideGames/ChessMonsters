using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ChessMonsterTactics
{
    [CustomPropertyDrawer(typeof(UIDAttribute))]
    public class UIDPropertyDrawer : PropertyDrawer
    {
        // {project_root}/Cache/uid
        private static string CachePath => Path.Combine(Directory.GetCurrentDirectory(), "EditorCache");
        private static string CacheName => "uid";

        private static HashSet<ushort> _cache;
        private bool _initialized = false;

        private void Initialize()
        {
            LoadCache();
            _initialized = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && position.Contains(e.mousePosition)) {
            
                GenericMenu context = new GenericMenu ();
            
                context.AddItem (new GUIContent ("Regenerate UID"), property.isExpanded, () => {
                    if (_cache.Contains((ushort)property.intValue))
                        _cache.Remove((ushort)property.intValue);

                    GenerateUniqueIdentifier(property);
                });
            
                context.ShowAsContext ();
            }
            
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "UID Attribute should only used with property of type Int32/ushort!", MessageType.Error);
                return;
            }

            if (!_initialized)
                Initialize();

            /*
            *   NOTE:
            *    If the cache is empty, and there's 2 or more assets with the same uid
            *    there will be 2 or more assets with the same uid that might cause a bug
            *    It's strongly advised that anyone shouldn't add/modify an asset that has uid
            *    property outside of Unity editor.
            */
            if (property.intValue <= 0)
            {
                GenerateUniqueIdentifier(property);
            }
            else if (!_cache.Contains((ushort)property.intValue))
            {
                _cache.Add((ushort)property.intValue);
                SaveCache();
            }

            EditorGUI.LabelField(position, string.Format("ID: {0:D5}", property.intValue), EditorStyles.boldLabel);
        }

        private void GenerateUniqueIdentifier(SerializedProperty property)
        {
            ushort id;
            do
            {
                id = (ushort)Random.Range(1, ushort.MaxValue);
            }
            while (_cache.Contains(id));

            property.intValue = id;
            property.serializedObject.ApplyModifiedProperties();

            _cache.Add(id);
            SaveCache();
        }
        
        private void LoadCache()
        {
            if (_cache != null)
                return;

            _cache = new HashSet<ushort>();

            if (!Directory.Exists(CachePath))
            {
                return;
            }

            string filePath = Path.Combine(CachePath, CacheName);
            if (!File.Exists(filePath))
            {
                return;
            }

            using FileStream stream = File.OpenRead(filePath);
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 128);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (ushort.TryParse(line, out ushort result))
                {
                    _cache.Add(result);
                }
            }
        }

        private void SaveCache()
        {
            if (!Directory.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }

            string filePath = Path.Combine(CachePath, CacheName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            using FileStream stream = File.OpenWrite(filePath);
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 128);
            foreach (ushort v in _cache)
                writer.WriteLine(v);
        }
    }
}
