using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Dfo : Ability<DfoData>
    {
        protected override void OnBegin()
        {
            Debug.Log("Dfo BEGIN");
        }

        protected override void OnUpdate(float deltaTime)
        {
            Debug.Log("#UPDATE Dfo UPDATE");
        }

        protected override void OnEnd()
        {
            Debug.Log("Dfo END");
        }
    }
}