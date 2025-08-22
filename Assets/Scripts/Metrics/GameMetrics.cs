using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Metrics
{
    [CreateAssetMenu(menuName = "OverBang/NomDuJeu/Metrics/GameMetrics")]
    public class GameMetrics : ScriptableObject
    {
        public static GameMetrics Global => GameController.Metrics;
        
        [field: SerializeField] public CameraID PlayerView { get; private set; }
        [field: SerializeField] public CameraID PlayerSpectate { get; private set; }
        [field: SerializeField] public CameraID MainMenu { get; private set; }
    }
}