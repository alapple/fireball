using UnityEngine;

namespace Fireball.UI
{
    public class DamagePopupManager : MonoBehaviour
    {
        public static DamagePopupManager Instance { get; private set; }

        [SerializeField] private GameObject _popupPrefab;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CreatePopup(Vector3 position, float damageAmount)
        {
            if (_popupPrefab == null)
            {
                Debug.LogWarning("DamagePopupManager: Popup Prefab is not assigned!");
                return;
            }

            GameObject popupObj = Instantiate(_popupPrefab, position, Quaternion.identity);
            DamagePopup popup = popupObj.GetComponent<DamagePopup>();
            if (popup != null)
            {
                popup.Setup(damageAmount);
            }
        }
    }
}
