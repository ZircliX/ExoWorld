using OverBang.GameName.Core.Scenes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.GameName.Core.Metrics
{
    [CreateAssetMenu(menuName = "OverBang/Metrics/GameMetrics")]
    public partial class GameMetrics : ScriptableObject
    {
        public static GameMetrics Global => GameController.Metrics;
        
        [field: SerializeField, FoldoutGroup("Scenes")] public SceneCollection SceneCollection { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Camera")] public CameraID PlayerView { get; private set; }
        [field: SerializeField, FoldoutGroup("Camera")] public CameraID PlayerSpectate { get; private set; }
        [field: SerializeField, FoldoutGroup("Camera")] public CameraID MainMenu { get; private set; }
        
        [field: SerializeField, FoldoutGroup("UI")] public GameObject CharacterSelectionPrefab { get; private set; }
        
        [field: SerializeField, FoldoutGroup("DEBUG")] public GameObject DebugInputs { get; private set; }
    }
}