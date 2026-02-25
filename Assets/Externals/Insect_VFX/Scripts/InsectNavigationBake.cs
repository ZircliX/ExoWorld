using UnityEditor;
using UnityEngine;

namespace Insect_VFX
{
    public static class InsectNavigationBake
    {
        public static Texture2D Bake(InsectEmitter _insectEmitter)
        {
            int bakeResolutionSize = GetBakeResolution(_insectEmitter.bakeResolutions);

            Texture2D navigationTexture = new(bakeResolutionSize, bakeResolutionSize, TextureFormat.RFloat, false);
            for (int x = 0; x < bakeResolutionSize; x++)
            {
                for (int y = 0; y < bakeResolutionSize; y++)
                {
                    Vector3 point =
                        (Vector3.right * (x * (_insectEmitter.simulationSize / bakeResolutionSize)) +
                         Vector3.forward * (y * (_insectEmitter.simulationSize / bakeResolutionSize))) +
                        (_insectEmitter.transform.position -
                         new Vector3(_insectEmitter.simulationSize, 0, _insectEmitter.simulationSize) * 0.5f +
                         (Vector3.up * (_insectEmitter.simulationHeight * 0.5f)));

                    float height = 0;
                    if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, _insectEmitter.simulationHeight))
                    {
                        height = hit.point.y;
                    }

                    float normalizedHeight = height / _insectEmitter.simulationHeight;

                    navigationTexture.SetPixel(x, y, new Color(normalizedHeight, 0, 0));
                }
            }

            navigationTexture.Apply();
            Debug.Log("Navigation map baked and stored in texture.");

            return navigationTexture;
        }

        public static float GetHeightFromTexture(Vector3 pos, InsectEmitter _insectEmitter, int bakeResolution)
        {
            float size = _insectEmitter.simulationSize;
            Vector3 origin = _insectEmitter.GetEmissionOrigin();

            float normalizedX = (pos.x - origin.x + size / 2) / size;
            float normalizedY = (pos.z - origin.z + size / 2) / size;

            normalizedX = Mathf.Clamp01(normalizedX);
            normalizedY = Mathf.Clamp01(normalizedY);

            int x = Mathf.FloorToInt(normalizedX * (bakeResolution - 1));
            int y = Mathf.FloorToInt(normalizedY * (bakeResolution - 1));

            x = Mathf.Clamp(x, 0, bakeResolution - 1);
            y = Mathf.Clamp(y, 0, bakeResolution - 1);

            return GetHeightFromTexture(x, y, _insectEmitter);
        }

        private static float GetHeightFromTexture(int x, int y, InsectEmitter _insectEmitter)
        {

            if (x >= 0 && x < _insectEmitter.navigationTexture.width && y >= 0 &&
                y < _insectEmitter.navigationTexture.height)
            {
                float pixelValue = _insectEmitter.navigationTexture.GetPixel(x, y).r;
                float normalizedValue = pixelValue * _insectEmitter.simulationHeight;
                return normalizedValue;
            }

            Debug.LogWarning($"Outside texture limits. Provided coordinates: ({x}, {y}). " +
                             $"Texture size: {_insectEmitter.navigationTexture.width}x{_insectEmitter.navigationTexture.height}");

            return 0f;
        }

        public static int GetBakeResolution(BakeResolutions resolution)
        {
            int bakeResolutionSize;
            switch (resolution)
            {
                case BakeResolutions.Low:
                    bakeResolutionSize = 124;
                    break;
                case BakeResolutions.Medium:
                    bakeResolutionSize = 256;
                    break;
                case BakeResolutions.High:
                    bakeResolutionSize = 512;
                    break;
                case BakeResolutions.Ultra:
                    bakeResolutionSize = 1024;
                    break;
                default:
                    bakeResolutionSize = 128;
                    break;
            }

            return bakeResolutionSize;
        }

        public enum BakeResolutions
        {
            [System.ComponentModel.Description("128 x 128")]
            Low,

            [System.ComponentModel.Description("256 x 256")]
            Medium,

            [System.ComponentModel.Description("512 x 512")]
            High,

            [System.ComponentModel.Description("1024 x 1024")]
            Ultra,
            Custom
        }
    }

}