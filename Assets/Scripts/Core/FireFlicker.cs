using UnityEngine;

namespace Fireball.VFX
{
    public class FireFlicker : MonoBehaviour
    {
        [SerializeField] private Light fireLight;
        [SerializeField] private float minIntensity = 0.8f;
        [SerializeField] private float maxIntensity = 1.2f;
        [SerializeField] private float flickerSpeed = 0.1f;

        private float baseIntensity;

        private void Start()
        {
            if (fireLight == null) fireLight = GetComponent<Light>();
            if (fireLight != null) baseIntensity = fireLight.intensity;
        }

        private void Update()
        {
            if (fireLight == null) return;
            
            float noise = Mathf.PerlinNoise(Time.time * (1f / flickerSpeed), 0);
            fireLight.intensity = baseIntensity * Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}
