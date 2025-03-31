using UnityEngine;

namespace ChessMonsterTactics
{
    public class UIPanel : MonoBehaviour
    {
        private RectTransform _rt;

        protected RectTransform RectTransform { get => _rt; } 

        protected virtual void Awake()
        {
            if (_rt == null)
                _rt = (RectTransform)transform;
        }
        
        public virtual YieldInstruction Show(bool useTransition = true)
        {
            gameObject.SetActive(true);
            return new YieldInstruction();
        }

        public virtual YieldInstruction Hide(bool useTransition = true)
        {
            gameObject.SetActive(false);
            return new YieldInstruction();
        }
    }
}
