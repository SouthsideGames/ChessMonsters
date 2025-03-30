using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace ChessMonsterTactics
{
    public class BackButton : MonoBehaviour, IPointerClickHandler
    {
        private bool _pressed;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_pressed)
                return;
            
            _pressed = true;
            
            RectTransform rt = (RectTransform)transform;
            rt.DOPunchPosition(new Vector3(-rt.rect.width * 0.15f, 0), 0.125f)
                .OnComplete(() => {
                    Backstack.PopBackstack();
                    _pressed = false;
                });
        }

#if UNITY_ANDROID
        private void LayeUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Backstack.PopBackstack();
            }
        }
#endif
    }
}
