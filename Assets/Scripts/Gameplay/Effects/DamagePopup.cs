using UnityEngine;
using DG.Tweening;
using TMPro;

namespace ChessMonsterTactics.Gameplay
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public static void Show(Vector2 pos, int value, float delay = 0)
        {
            DamagePopup prefab = Instantiate(UISettings.GetInstance().DamagePopup.Prefab);
            prefab.Set(pos, value, delay);
        }

        private void Set(Vector2 pos, int value, float delay)
        {
            _text.SetText(value.ToString());
            transform.position = pos;

            Vector2 dir = GetRandomDirection();

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.Append(transform.DOMove(pos + (dir * 0.8f), UISettings.GetInstance().DamagePopup.Transition).SetEase(Ease.OutCubic));
            sequence.Join(transform.DOScale(1.1f, UISettings.GetInstance().DamagePopup.Transition).SetEase(Ease.OutCubic));
            sequence.Join(_text.DOFade(0, UISettings.GetInstance().DamagePopup.Transition).SetEase(Ease.InCubic));

            sequence.OnComplete(() => Destroy(gameObject, 0.5f));
        }

        private Vector2 GetRandomDirection()
        {
            return new Vector2(Random.Range(-1, 1), 1).normalized;
        }
    }
}
