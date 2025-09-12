using KBCore.Refs;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Managers;
using OverBang.GameName.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Network
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerControllerNetwork))]
    public class PlayerControllerNetworkAdapter : NetworkBehaviour, IPlayerReady

    {
    [field: SerializeField, Self] public PlayerController PlayerController { get; private set; }
    [field: SerializeField, Self] public PlayerControllerNetwork PlayerControllerNetwork { get; private set; }
    
    public bool IsReady => PlayerControllerNetwork.IsReady.Value;

    private readonly float updateRate = 1f / 20f;
    private float timer;

    private void OnValidate() => this.ValidateRefs();

    private void OnEnable()
    {
        PlayerController.OnReadyKeyPressed += ToggleReady;
        PlayerController.OnShootKeyPressed += RequestFireServerRpc;
    }
    
    private void OnDisable()
    {
        PlayerController.OnReadyKeyPressed -= ToggleReady;
        PlayerController.OnShootKeyPressed -= RequestFireServerRpc;
    }

    private void Awake()
    {
        if (PlayerController == null) Debug.LogError($"Player {nameof(PlayerController)} is null.");
        if (PlayerControllerNetwork == null) Debug.LogError($"PlayerNetworkController is null.");
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerController.EnableLocalControls();
        }
        else
        {
            PlayerController.DisableRemoteControls(
                PlayerControllerNetwork.PlayerState.Value.Position,
                PlayerControllerNetwork.PlayerState.Value.Rotation);
        }

        if (PlayerManagerNetworkAdapter.HasInstance && PlayerManagerNetworkAdapter.Instance.IsSpawned)
        {
            InitializePlayer();
        }
        else
        {
            Debug.Log("Waiting for PlayerManagerNetworkAdapter to spawn");
            PlayerManagerNetworkAdapter.OnInstanceCreated += InitializePlayer;
        }
    }

    private void InitializePlayer()
    {
        Debug.Log($"[PlayerNetworkAdapter] InitializePlayer");
        PlayerManagerNetworkAdapter.Instance.RegisterPlayer(this);
        PlayerManager.OnInstanceCreated -= InitializePlayer;
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && PlayerManagerNetworkAdapter.HasInstance)
            PlayerManagerNetworkAdapter.Instance.UnregisterPlayer(this);
    }

    private void FixedUpdate()
    {
        if (!IsOwner || !PlayerControllerNetwork.IsSpawned) return;

        timer += Time.fixedDeltaTime;
        if (timer >= updateRate)
        {
            timer = 0f;
            (Vector3 pos, Quaternion rot) = PlayerController.CaptureState();
            PlayerControllerNetwork.PlayerState.Value = new PlayerNetworkTransform()
            {
                Position = pos,
                Rotation = rot
            };
        }
    }

    private void Update()
    {
        if (!PlayerControllerNetwork.IsSpawned) return;

        if (!IsOwner)
        {
            PlayerController.ApplyNetworkState(PlayerControllerNetwork.PlayerState.Value.Position,
                PlayerControllerNetwork.PlayerState.Value.Rotation);
        }
        else
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            SceneManager.Instance.ChangeScene("Test");
            Debug.Log("Try to change scene to Test");
        }
    }
    
    public void ToggleReady()
    {
        bool newReady = !PlayerControllerNetwork.IsReady.Value;
        PlayerControllerNetwork.RequestSetReadyRpc(newReady);
    }

    [Rpc(SendTo.Server)]
    private void RequestFireServerRpc(Vector3 direction)
    {
        FireClientRpc(direction);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void FireClientRpc(Vector3 direction)
    {
        if (IsOwner) return;
        PlayerController.Weapon.Shoot(direction);
    }
    }
}