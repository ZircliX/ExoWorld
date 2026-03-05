using System;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Components;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Interactions.Clothing
{
    public class ClothColliderReceiver : BaseDetector
    {
        [SerializeField, Child] private Cloth[] cloths;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            DetectionArea.SetRequireInterface<CapsuleCollider>();
            DetectionArea.SetRequireInterface<SphereCollider>();
        }

        protected override void OnEnter(Collider other, object target)
        {
            if (target is CapsuleCollider capsuleCollider)
            {
                foreach (Cloth cloth in cloths)
                {
                    CapsuleCollider[] colliders = cloth.capsuleColliders;
                    Array.Resize(ref colliders, colliders.Length + 1);
                    colliders[^1] = capsuleCollider;
                    cloth.capsuleColliders = colliders;
                }
            }
            else if (target is SphereCollider sphereCollider)
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAA");
                foreach (Cloth cloth in cloths)
                {
                    ClothSphereColliderPair[] pairs = cloth.sphereColliders;
                    Array.Resize(ref pairs, pairs.Length + 1);
                    pairs[^1] = new ClothSphereColliderPair(sphereCollider);
                    cloth.sphereColliders = pairs;
                }
            }
        }

        protected override void OnExit(Collider other, object target)
        {
            if (target is CapsuleCollider capsuleCollider)
            {
                foreach (Cloth cloth in cloths)
                {
                    cloth.capsuleColliders = Array.FindAll(
                        cloth.capsuleColliders, c => c != capsuleCollider
                    );
                }
            }
            else if (target is SphereCollider sphereCollider)
            {
                foreach (Cloth cloth in cloths)
                {
                    cloth.sphereColliders = Array.FindAll(
                        cloth.sphereColliders, p => p.first != sphereCollider && p.second != sphereCollider
                    );
                }
            }
        }
    }
}