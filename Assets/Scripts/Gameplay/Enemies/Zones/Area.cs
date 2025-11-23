using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Area : MonoBehaviour
    {
        [Header("Bounds Parameters")]
        [field: SerializeField, Self, HideInInspector] public Transform Center { get; private set; }
        [field: SerializeField] public Vector3 Size { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        
        [field: SerializeField] public List<EnemySpawner> Spawners { get; private set; }
        
        private Bounds Bound;
        
        private void OnEnable()
        {
            AreaManager.Instance.Register(this);
            
        }

        private void OnDisable()
        {
            AreaManager.Instance.Unregister(this);
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Awake()
        {
            Bound = Bounds();
        }

        public Bounds Bounds()
        {
            return new Bounds(Center.position, Size);
        }

        public bool CheckForPlayers(params Transform[] transforms)
        {
            for (var index = 0; index < transforms.Length; index++)
            {
                var playerTranform = transforms[index];
                if (Bound.Contains(playerTranform.position))
                {
                    return true;
                }
            }

            return false;
        }
        
        private void OnDrawGizmos()
        {
            Bound = Bounds();

            Gizmos.color = Color;
            Gizmos.DrawWireCube(Bound.center, Bound.size);
        }
        
        
    }
}