using KBCore.Refs;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace OverBang.ExoWorld.Core.Abilities
{
    public class AbilityIconReference : MonoBehaviour
    {
        [field: SerializeField, Child] public Image TargetGraphic { get; private set; }

        private void OnValidate()
        {
            this.ValidateRefs();
        }
    }
}