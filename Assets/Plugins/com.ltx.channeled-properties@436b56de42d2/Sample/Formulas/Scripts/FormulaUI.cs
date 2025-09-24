using System.Globalization;
using LTX.ChanneledProperties.Formulas;
using TMPro;
using UnityEngine;

namespace LTX.ChanneledProperties.Samples.Sample.Formulas.Scripts
{
    public class FormulaUI : MonoBehaviour
    {
        [SerializeField]
        private float startValue = 50;
        [SerializeField]
        private TextMeshProUGUI result;
        [SerializeField]
        private Transform content;
        [SerializeField]
        private FormulaGroupUI prefab;

        [field: SerializeField]
        public Formula<float> Formula { get; private set; }

        private void Awake()
        {
            Formula = new Formula<float>(startValue, 64, true);
            Formula.AddOnValueChangeCallback(ctx =>
            {
                result.text = ctx.ToString(CultureInfo.InvariantCulture);
            }, true);

            Formula.AddOperation(ChannelKey.GetUniqueChannelKey("Formula"));
            Formula.AddOperation(ChannelKey.GetUniqueChannelKey());
        }


        public void AddNew() => Instantiate(prefab, content);
    }
}