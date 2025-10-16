using DG.Tweening;
using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Quests
{
    public class RepairArea : MonoBehaviour
    {
        [SerializeField, Self] private MeshRenderer mr;
        
        private void OnValidate() => this.ValidateRefs();

        public void OnEnable()
        {
            float duration = 1f;
            float minAlpha = 0.1f;
            float maxAlpha = 1f;

            Color c = mr.material.color;
            c.a = minAlpha;
            mr.material.color = c;

            mr.material.DOFade(maxAlpha, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
        }
    }
}