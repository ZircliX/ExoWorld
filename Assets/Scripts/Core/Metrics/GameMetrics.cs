using EditorAttributes;
using OverBang.GameName.Managers;
using OverBang.GameName.Network;
using OverBang.GameName.Player;
using UnityEngine;

namespace OverBang.GameName.Metrics
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
        [field: SerializeField, HideProperty] public PlayerControllerNetworkAdapter PlayerControllerNetworkAdapter { get; private set; }
        [field: SerializeField, HideProperty] public PlayerControllerOfflineAdapter PlayerControllerOfflineAdapter { get; private set; }
        
        [FoldoutGroup("UI", nameof(CharacterSelectionPrefab))]
        [SerializeField] private Void uiFoldout;
        [field: SerializeReference, HideProperty] public GameObject CharacterSelectionPrefab { get; private set; }
    }
}