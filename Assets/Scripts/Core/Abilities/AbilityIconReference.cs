using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace OverBang.ExoWorld.Core.Abilities
{
    public class AbilityIconReference : MonoBehaviour
    {
        [field: SerializeField] public Image IconOutline { get; private set; }
        [field: SerializeField] public Image CooldownFill { get; private set; }
        [field: SerializeField] public Image AbilityIcon { get; private set; }
        [field: SerializeField] public TMP_Text CooldownText { get; private set; }
        [field: SerializeField] public Image AbilityHighlight { get; private set; }

        [Space] 
        [SerializeField] private Color orange;
        [SerializeField] private Color black;
        [SerializeField] private Color white;
    }
}