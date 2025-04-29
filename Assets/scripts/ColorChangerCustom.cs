using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.Samples
{
    public class ColorChangerCustom : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("El material cuyo color queremos cambiar")]
        private Material _targetMaterial;

        private Color _savedColor;
        private float _lastHue = 0f;

        public void NextColor()
        {
            _lastHue = (_lastHue + 0.3f) % 1f;
            Color newColor = Color.HSVToRGB(_lastHue, 0.8f, 0.8f);
            _targetMaterial.color = newColor;
        }

        public void Save()
        {
            _savedColor = _targetMaterial.color;
        }

        public void Revert()
        {
            _targetMaterial.color = _savedColor;
        }

        protected virtual void Start()
        {
            this.AssertField(_targetMaterial, nameof(_targetMaterial));
            _savedColor = _targetMaterial.color;
        }
    }
}