using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Components
{
    public class ParticleSystemReference : MonoBehaviour
    {
        [field: SerializeField] public ParticleSystem MainParticleSystem { get; private set; }
        [field: SerializeField] public ParticleSystem[] SubParticleSystem { get; private set; }
        [field: SerializeField] public bool Looping { get; private set; } = true;
        [field: SerializeField] public bool PlayOnAwake { get; private set; } = true;
        
        public float MaxDuration => MainParticleSystem?.main.duration ?? SubParticleSystem.Max(ps => ps.main.duration);

        private void OnValidate()
        {
            if (MainParticleSystem != null)
            {
                ParticleSystem.MainModule main = MainParticleSystem.main;
                main.loop = Looping;
                main.playOnAwake = PlayOnAwake;
            }

            for (int i = 0; i < SubParticleSystem.Length; i++)
            {
                ParticleSystem.MainModule mainModule = SubParticleSystem[i].main;
                mainModule.loop = Looping;
                mainModule.playOnAwake = PlayOnAwake;
            }
        }

        public void Play()
        {
            if (MainParticleSystem != null)
                MainParticleSystem.Play();

            for (int i = 0; i < SubParticleSystem.Length; i++)
            {
                ParticleSystem ps = SubParticleSystem[i];
                ps.Play();
            }
        }

#if UNITY_EDITOR
        private float simulationTime;
        private bool isSimulating;

        private ParticleSystem[] AllParticles => MainParticleSystem != null
            ? new[] { MainParticleSystem }.Concat(SubParticleSystem).ToArray()
            : SubParticleSystem;
        
        private ParticleSystem FirstParticle => MainParticleSystem != null 
            ? MainParticleSystem 
            : SubParticleSystem.Length > 0 ? SubParticleSystem[0] : null;

        [Button]
        private void PlayInEditor()
        {
            if (FirstParticle == null) return;
            
            simulationTime = 0f;
            isSimulating = true;
            UnityEditor.EditorApplication.update += SimulateStep;
        }

        [Button]
        private void StopInEditor()
        {
            isSimulating = false;
            UnityEditor.EditorApplication.update -= SimulateStep;

            foreach (ParticleSystem ps in AllParticles)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void SimulateStep()
        {
            if (!isSimulating) return;
            if (FirstParticle == null) { StopInEditor(); return; }

            bool restart = simulationTime == 0f; // ← restart only on first frame
            simulationTime += 0.05f;

            foreach (ParticleSystem ps in AllParticles)
                ps.Simulate(simulationTime, true, restart);

            UnityEditor.SceneView.RepaintAll();

            if (!Looping && simulationTime >= FirstParticle.main.duration)
                StopInEditor();
        }
#endif
    }
}