using LTX.ChanneledProperties.Conditions;
using UnityEngine;
using UnityEngine.UI;

namespace LTX.ChanneledProperties.Samples.Sample.Conditions
{
    public class SampleCPManager : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        private Toggle[] toggles;

        [SerializeField]
        private Condition condition;

        private void Awake()
        {
            toggles = GetComponentsInChildren<Toggle>();
            condition = new Condition();
            for (int i = 0; i < toggles.Length; i++)
            {
                var t = toggles[i];
                condition.AddCondition(t, t.isOn);

                t.onValueChanged.AddListener(ctx => condition.Write(t, ctx));
            }

            condition.AddOnValueChangeCallback(ctx => text.text = ctx.ToString(), true);
        }
    }
}