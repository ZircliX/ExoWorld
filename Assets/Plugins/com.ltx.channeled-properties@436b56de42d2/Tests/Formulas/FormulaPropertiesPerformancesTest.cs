using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Formulas;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class FormulaPropertiesPerformancesTest : MonoBehaviour
{
    [SerializeField]
    private Slider executionCountSlider;
    [SerializeField]
    private Slider formulaSizeSlider;

    private ChannelKey[] keys;

    private void Awake()
    {
        var maxValue = (int)formulaSizeSlider.maxValue;
        keys = new ChannelKey[maxValue];
        for (int i = 0; i < maxValue; i++)
            keys[i] = ChannelKey.GetUniqueChannelKey();
    }

    public void Run()
    {
        int count = (int)executionCountSlider.value;
        int size = (int)formulaSizeSlider.value;

        Stopwatch st = new Stopwatch();
        Profiler.BeginSample($"Calculating {count} formulas of {size} size");
        st.Start();
        for (int i = 0; i < count; i++)
        {
            Profiler.BeginSample($"Formula construction");
            Formula<int> property = new Formula<int>(1, size, false);
            for (int j = 0; j < size; j++)
                property.Multiply(keys[j], 5);
            Profiler.EndSample();

            Profiler.BeginSample($"Formula evaluation");
            int value = property.Value;
            Profiler.EndSample();
        }
        st.Stop();
        Profiler.EndSample();

        var elapsedMilliseconds = st.Elapsed.Milliseconds;
        var med = (float)elapsedMilliseconds / count;
        Debug.Log($"Completed {count} formulas of {size} size in {elapsedMilliseconds} ms so an average of {med}ms/formula");
        Debug.Break();
    }
}