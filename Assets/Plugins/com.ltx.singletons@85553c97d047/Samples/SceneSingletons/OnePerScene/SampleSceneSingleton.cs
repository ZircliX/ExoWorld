using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Singletons
{
    public class SampleSceneSingleton : SceneSingleton<SampleSceneSingleton>
    {
        public void Log()
        {
            Debug.Log(Instance.name);
        }
    }
}