using UnityEngine;
using TMPro;

namespace Fireball.UI
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textMesh;
        [SerializeField] private float _moveSpeed = 1.2f;
        [SerializeField] private float _fadeSpeed = 2f;
        [SerializeField] private float _lifeTime = 0.7f;
        
        [SerializeField] private AnimationCurve _scaleCurve = new AnimationCurve(
            new Keyframe(0f, 0.4f), 
            new Keyframe(0.15f, 1.4f), 
            new Keyframe(0.3f, 1f),
            new Keyframe(1f, 0.7f)
        );

        private Color _textColor;
        private float _timer;
        private float _maxLifetime;
        private bool _initialized = false;
        private Vector3 _baseScale;
        private Vector3 _moveDirection;

        private void Awake()
        {
            if (_textMesh == null) _textMesh = GetComponent<TMP_Text>();
            if (_textMesh == null) _textMesh = GetComponentInChildren<TMP_Text>();
            
            _baseScale = transform.localScale;
            if (_baseScale.sqrMagnitude < 0.001f) _baseScale = Vector3.one * 0.4f; 

            // Add random horizontal drift so they spread out
            float randomX = Random.Range(-0.8f, 0.8f);
            float randomZ = Random.Range(-0.8f, 0.8f);
            _moveDirection = new Vector3(randomX, 1f, randomZ).normalized;
        }

        public void Setup(float damageAmount)
        {
            if (_textMesh == null) Awake();
            
            if (_textMesh != null)
            {
                // Better formatting: if it's < 1, show 1. If it's > 1, show integer.
                int displayDamage = Mathf.Max(1, Mathf.RoundToInt(damageAmount));
                _textMesh.SetText(displayDamage.ToString());
                
                _textColor = _textMesh.color;
                _timer = _lifeTime;
                _maxLifetime = _lifeTime;
                _initialized = true;
                
                // Slightly randomize start position
                transform.position += Random.insideUnitSphere * 0.15f;
            }
        }

        private void Update()
        {
            if (!_initialized || _textMesh == null) return;

            // Move in the randomized direction (up + side drift)
            transform.position += _moveDirection * _moveSpeed * Time.deltaTime;

            // Look AT camera
            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                                 Camera.main.transform.rotation * Vector3.up);
            }

            // Animation scaling
            float agePercent = 1f - (_timer / _maxLifetime);
            transform.localScale = _baseScale * _scaleCurve.Evaluate(agePercent);

            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                // Fade out
                _textColor.a -= _fadeSpeed * Time.deltaTime;
                _textMesh.color = _textColor;

                if (_textColor.a <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
