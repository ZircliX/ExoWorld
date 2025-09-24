using System.Collections;
using System.Collections.Generic;
using LTX.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Samples
{
    public class SampleAnnotation : MonoBehaviour
    {
        [SerializeField]
        private Annotation<float> annotation;
    }
}