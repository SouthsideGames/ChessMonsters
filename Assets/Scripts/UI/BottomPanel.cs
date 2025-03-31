using DG.Tweening;
using UnityEngine;

namespace ChessMonsterTactics.UI
{
    public class BottomPanel : UIPanel
    {
        public override YieldInstruction Show(bool useTransition = true)
        {
            if (!useTransition)
            {
                RectTransform.position = Vector2.zero;
                gameObject.SetActive(true);
                return null;
            }

            return RectTransform.DOAnchorPos(Vector2.zero, 0.15f)
                .WaitForCompletion();
        }

        public override YieldInstruction Hide(bool useTransition = true)
        {
            float height = RectTransform.rect.height;
            Vector2 targetPos = new Vector2(0, -height);

            if (!useTransition)
            {
                RectTransform.anchoredPosition = targetPos;
                gameObject.SetActive(true);
                return null;
            }

            return RectTransform.DOAnchorPos(targetPos, 0.15f)
                .WaitForCompletion();
        }
    }
}
