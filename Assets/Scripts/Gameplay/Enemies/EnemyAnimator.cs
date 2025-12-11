using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class EnemyAnimator : MonoBehaviour
    {
        [field : SerializeField] public Animator ModelAnimator { get; private set; }

        public void SetBool(string root, bool value)
        {
            ModelAnimator.SetBool(root, value);
        }
    }
}