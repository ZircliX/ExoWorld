using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Player.PlayerHUD
{
    public class CompasControllerUI : MonoBehaviour
    {
        public enum CompassMode
        {
            Wrapping,  // Wraps infinitely
            Clamped,   // Stops at limits
            Centered   // Always centered (player rotation only affects arrow)
        }
        
        [SerializeField] private Transform player;
        [SerializeField] private RectTransform directionsContent;
        
        [Space]
        [SerializeField] private float pixelsPerDegree = 3.02f;
        [SerializeField] private float northOffsetPixels = -270f;
        [SerializeField] private float wrapBoundary = 545f;
        
        [Space]
        [SerializeField] private CompassMode mode = CompassMode.Wrapping;
        [SerializeField] private float clampDistance = 250f;
        [SerializeField] private float degreesVisible = 90f;
        [SerializeField] private float centerPos = 66.6f;
        
        [Space]
        [SerializeField] private bool smoothMovement = true;
        [SerializeField] private float smoothSpeed = 5f;
        
        private float calibrationOffset;
        private float targetXOffset;
        private float currentXOffset;

        private void Start()
        {
            CalibrateCompass();
            targetXOffset = currentXOffset;
        }

        private void LateUpdate()
        {
            UpdateCompass();
        }

        private void CalibrateCompass()
        {
            if (player == null) return;
            calibrationOffset = player.eulerAngles.y - 270 / pixelsPerDegree;
        }

        private void UpdateCompass()
        {
            if (player == null || directionsContent == null)
                return;

            // Calculate rotation relative to calibration
            float playerRotation = player.eulerAngles.y - calibrationOffset;
    
            // Convert rotation to pixels and ADD the north offset
            float xOffset = (-playerRotation * pixelsPerDegree) + northOffsetPixels;

            switch (mode)
            {
                case CompassMode.Wrapping:
                    xOffset = WrapPosition(xOffset);
                    break;

                case CompassMode.Clamped:
                    float minClamp = (centerPos + northOffsetPixels) - clampDistance;
                    float maxClamp = (centerPos + northOffsetPixels) + clampDistance;
                    xOffset = Mathf.Clamp(xOffset, minClamp, maxClamp);
                    break;

                case CompassMode.Centered:
                    float centeredBase = centerPos + northOffsetPixels;
                    float offset = xOffset - centeredBase;
                    offset = Mathf.Clamp(offset, -clampDistance, clampDistance);
                    xOffset = centeredBase + offset;
                    break;
            }

            Vector3 currentPos = directionsContent.anchoredPosition;
            directionsContent.anchoredPosition = new Vector3(xOffset, currentPos.y, currentPos.z);
        }

        private float WrapPosition(float position)
        {
            float fullRange = wrapBoundary * 2; // 1090 pixels total range
    
            // Normalize to range [-wrapBoundary, +wrapBoundary]
            while (position > wrapBoundary)
                position -= fullRange;
            while (position < -wrapBoundary)
                position += fullRange;
    
            return position;
        }

        public void Recalibrate() => CalibrateCompass();
        public void SetPixelsPerDegree(float value) => pixelsPerDegree = value;
        public void SetMode(CompassMode newMode) => mode = newMode;
        public void SetClampDistance(float distance) => clampDistance = distance;
        public void SetCenterPos(float pos) => centerPos = pos;
    }
}