using UnityEngine;
using TMPro;

namespace Fireball.UI
{
    public class DamagePopup : MonoBehaviour
    {
        // Using TMP_Text handles both TextMeshPro (3D) and TextMeshProUGUI (UI)
        [SerializeField] private TMP_Text _textMesh;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _fadeSpeed = 3f;
        [SerializeField] private float _lifeTime = 1f;

        private Color _textColor;
        private float _timer;
        private bool _initialized = false;

        private void Awake()
        {
            // Auto-find component if not assigned
            if (_textMesh == null) _textMesh = GetComponent<TMP_Text>();
            if (_textMesh == null) _textMesh = GetComponentInChildren<TMP_Text>();
        }

        public void Setup(float damageAmount)
        {
            if (_textMesh == null) Awake();
            
            if (_textMesh != null)
            {
                _textMesh.SetText(Mathf.CeilToInt(damageAmount).ToString());
                _textColor = _textMesh.color;
                _timer = _lifeTime;
                _initialized = true;
            }
            else
            {
                Debug.LogWarning($"DamagePopup: No TMP_Text found on {gameObject.name}");
            }
        }

        private void Update()
        {
            if (!_initialized || _textMesh == null) return;

            // Move up
            transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

            // Face camera
            if (Camera.main != null)
            {
                transform.forward = Camera.main.transform.forward;
            }
            else
            {
                // Fallback if no MainCamera tagged camera is found
                transform.LookAt(transform.position + Vector3.back);
            }

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
