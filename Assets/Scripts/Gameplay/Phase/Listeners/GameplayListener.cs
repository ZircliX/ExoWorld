using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Gameplay.Listeners
{
    public abstract class GameplayListener : MonoBehaviour
    {
        protected GameplayPhase current;

        protected void OnEnable()
        {
            GameplayPhase.OnNewPhaseBegins += OnInit;
            GameplayPhase.OnNewPhaseEnds += OnRelease;
        }

        protected void OnDisable()
        {
            GameplayPhase.OnNewPhaseBegins -= OnInit;
            GameplayPhase.OnNewPhaseEnds -= OnRelease;
        }

        private void OnInit(GameplayPhase phase)
        {
            if (current == null)
            {
                current = phase;
                Initialize(phase);
            }
        }

        private void OnRelease(GameplayPhase phase)
        {
            if (current == phase)
            {
                current = null;
                Release(phase);
            }
        }

        protected abstract void Initialize(GameplayPhase phase);
        protected abstract void Release(GameplayPhase phase);
    }
}