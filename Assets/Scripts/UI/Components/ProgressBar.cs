using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ChessMonsterTactics
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;

        [SerializeField] private float _min = 0;
        [SerializeField] private float _max = 1;
        [SerializeField] private float _value;

        private RectTransform _fillTransform;

        private void OnValidate()
        {
            if (_fillTransform == null)
                _fillTransform = (RectTransform)_fill.transform;

            UpdateGraphics();   
        }

        private void Awake()
        {
            _fillTransform = (RectTransform)_fill.transform;
        }

        public void SetValue(float value)
        {
            _value = value;
            UpdateGraphics();
        }

        public void SetNormalizedValue(float value)
        {
            _value = _max * Mathf.Clamp01(value);
        }

        public void SetMinMax(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public void SetColor(Color color)
        {
            _fill.color = color;
        }

        public void Animate(float value, float duration)
        {
            DOTween.Kill(this);
            DOTween.To(() => _value, SetValue, value, duration);
        }
        
        private void UpdateGraphics()
        {
            float p = (_value - _min) / (_max - _min);
            _fillTransform.localScale = new Vector2(Mathf.Clamp01(p), 1.0f);
        }
    }
}
