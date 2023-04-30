using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// code source: https://www.youtube.com/watch?v=l9HrraxtdGY&t=648s
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField]
    private List<Transform> startingPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;
    
    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);
        player.transform.position = startingPoints[players.Count - 1].position;

        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        player.GetComponentInChildren<CinemachineBrain>().gameObject.layer = layerToAdd;
        player.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
        player.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");
    }
}
