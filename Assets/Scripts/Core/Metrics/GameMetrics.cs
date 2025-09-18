using EditorAttributes;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Core.Metrics
{
    [CreateAssetMenu(menuName = "OverBang/Metrics/GameMetrics")]
    public partial class GameMetrics : ScriptableObject
    {
        public static GameMetrics Global => GameController.Metrics;
        
        [FoldoutGroup("Camera", nameof(PlayerView), nameof(PlayerSpectate), nameof(MainMenu))]
        [SerializeField] private Void cameraFoldout;
        [field: SerializeField, HideProperty] public CameraID PlayerView { get; private set; }
        [field: SerializeField, HideProperty] public CameraID PlayerSpectate { get; private set; }
        [field: SerializeField, HideProperty] public CameraID MainMenu { get; private set; }

        [FoldoutGroup("Players", nameof(PlayerControllerNetworkAdapter), nameof(PlayerControllerOfflineAdapter))]
        [SerializeField] private Void playersFoldout;
        [field: SerializeField, HideProperty] public GameObject PlayerControllerNetworkAdapter { get; private set; }
        [field: SerializeField, HideProperty] public GameObject PlayerControllerOfflineAdapter { get; private set; }
        
        [FoldoutGroup("UI", nameof(CharacterSelectionPrefab))]
        [SerializeField] private Void uiFoldout;
        [field: SerializeReference, HideProperty] public GameObject CharacterSelectionPrefab { get; private set; }
        
        [FoldoutGroup("DEBUG", nameof(DebugInputs))]
        [SerializeField] private Void debugFoldout;
        [field: SerializeReference, HideProperty] public GameObject DebugInputs { get; private set; }
    }
}