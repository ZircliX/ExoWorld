using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    [CreateAssetMenu(menuName = "OverBang/Metrics/GameMetrics")]
    public partial class GameMetrics : ScriptableObject
    {
        public static GameMetrics Global => GameController.Metrics;
        
        [field: SerializeField, FoldoutGroup("Scenes")] public SceneCollection SceneCollection { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Camera")] public CameraIDs CameraIDs { get; private set; }
        
        [field: SerializeField, FoldoutGroup("UI")] public GameObject CharacterSelectionPrefab { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Const ID")] public ConstID ConstID { get; private set; }

        [field: SerializeField, FoldoutGroup("Sessions")]
        public int MaxPasswordLenght { get; private set; } = 8;
        
        [field: SerializeField, FoldoutGroup("Prefabs")] public NetworkObject PlayerControllerPrefab { get; private set; }
       
        [field: SerializeField, FoldoutGroup("Layers")] public LayerMask HittableLayers { get; private set; }
        
        [field: SerializeField, FoldoutGroup("DEBUG")] public GameObject DebugInputs { get; private set; }
        [field: SerializeField, FoldoutGroup("DEBUG")] public bool LightFlickerInEditMode { get; private set; }
        
    }
}