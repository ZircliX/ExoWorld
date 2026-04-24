using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class MissionWaypoint : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 2f;
        [SerializeField] private float minAlpha = 0.3f;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private bool clampToScreen = true;
        [SerializeField] private float screenBorder = 50f;
        private string waypointId;
        private Camera mainCamera;
        private RectTransform rectTransform;
        public Transform target;
        private bool shouldUpdateDominator = true;

        public string WaypointId => waypointId;

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (target == null || mainCamera == null) return;

            // Convertir la position monde de la target en position écran
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

            // Placer le RectTransform du waypoint à la position écran
            rectTransform.position = screenPos;

            float distance = Vector3.Distance(mainCamera.transform.position, target.position);

            if (distance > maxDistance && shouldUpdateDominator)
            {
                icon.gameObject.SetActive(false);
                return;
            }

            if (shouldUpdateDominator)
                icon.gameObject.SetActive(true);

            // Scale based on distance (closer = larger)
            float scale = Mathf.Lerp(minScale, maxScale, 1 - (distance / maxDistance));
            rectTransform.localScale = Vector3.one * scale;

            // Alpha based on distance (closer = more opaque)
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, 1 - (distance / maxDistance));
            Color color = icon.color;
            color.a = alpha;
            icon.color = color;

            // Clamp à l'écran si demandé
            if (screenPos.z > 0)
            {
                if (clampToScreen)
                {
                    screenPos.x = Mathf.Clamp(screenPos.x, screenBorder, Screen.width - screenBorder);
                    screenPos.y = Mathf.Clamp(screenPos.y, screenBorder, Screen.height - screenBorder);
                }
                rectTransform.position = screenPos;
            }
            else if (shouldUpdateDominator)
            {
                icon.gameObject.SetActive(false);
            }
        }

        public void HideWaypoint()
        {
            if (icon != null)
            {
                icon.gameObject.SetActive(false);
                shouldUpdateDominator = false;
            }
        }

        public void ShowWaypoint()
        {
            if (icon != null)
            {
                icon.gameObject.SetActive(true);
                shouldUpdateDominator = true;
            }
        }

        public void SetTarget(Transform t)
        {
            target = t;
            waypointId = target.name;
            MissionWaypointRegistry.Register(this);
        }
    }
}