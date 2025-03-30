using UnityEngine;

namespace ChessMonsterTactics
{
    public abstract class ScriptableSettings<T> : ScriptableObject where T : ScriptableSettings<T>
    {
        private static T Instance { get; set; }

        public static T GetInstance()
        {   
            if (Instance == null)
            {
                T t = ScriptableObject.CreateInstance<T>();
                Instance = Resources.Load<T>(t.GetAssetPath());
                Destroy(t);
            }

            return Instance;
        }

        protected abstract string GetAssetPath();
    }
}
