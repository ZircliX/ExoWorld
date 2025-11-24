using System;
using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        private List<PlayerController> players;

        private void Awake()
        {
            players = new List<PlayerController>();
        }
        
        public void RegisterPlayer(PlayerController player)
        {
            players.Add(player);
            Debug.Log($"Registered player {player.name}");
        }

        public void UnregisterPlayer(PlayerController player)
        {
            players.Remove(player);
        }

        public IEnumerator<Transform> GetPlayerPosition()
        {
            foreach (PlayerController playerController in players)
            {
                yield return playerController.playerTransform;
            }
        }

        public Transform[] GetPlayerTransforms()
        {
            List<Transform> transforms = new List<Transform>();
            foreach (PlayerController playerController in players)
            {
                transforms.Add(playerController.playerTransform);
            }
            return transforms.ToArray();
        }
        
    }
}