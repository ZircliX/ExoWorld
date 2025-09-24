using UnityEngine;

namespace LTX.ChanneledProperties.Samples.Sample.Formulas.Scripts
{
    public class FormulaGroupUI : MonoBehaviour
    {
        public int Group => transform.GetSiblingIndex();

        private FormulaUI formulaUI;

        public void Destroy() => Destroy(gameObject);
    }
}