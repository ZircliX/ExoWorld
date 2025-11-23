using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        private List<PlayerController> players;

        private void Awake()
        {
            
            if (Instance == null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            players = new List<PlayerController>();
            
            DontDestroyOnLoad(gameObject);
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