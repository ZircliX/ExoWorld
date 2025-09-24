using System;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Tools
{
    [System.Serializable]
    public class Annotable
    {
#if UNITY_EDITOR
        [SerializeField]
        private string annotation = "";

        [SerializeField]
        private float fontSize = 15f;


        public Annotable(string annotation = "", float size = 15)
        {
            this.annotation = annotation;
        }

        public string Annotation
        {
            get => annotation;
            private set => annotation = value;
        }


        public float FontSize
        {
            get => fontSize;
            private set => fontSize = value;
        }
#endif
    }
    [System.Serializable]
    public class Annotation<T> : Annotable, IEquatable<T>
    {
        [SerializeField]
        public T value;

        public override string ToString() => value.ToString();

        public static implicit operator T(Annotation<T> annotation) => annotation.value;

        bool IEquatable<T>.Equals(T other) => EqualityComparer<T>.Default.Equals(value, other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(value, obj))
                return true;

            if (obj.GetType() != typeof(T))
                return false;

            return Equals((T)obj);
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => value.GetHashCode();
    }
}