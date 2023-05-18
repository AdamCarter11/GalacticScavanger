using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleOnPlayerJoin : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;

    private void Awake()
    {
        if (!playerInputManager)
        {
            playerInputManager = FindObjectOfType<PlayerInputManager>();
        }
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += ToggleThis;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= ToggleThis;
    }

    private void Update()
    {
        if (GameObject.FindWithTag("Ship"))
        {
            this.ToggleThis(PlayerInput.GetPlayerByIndex(0));
        }
    }

    private void ToggleThis(PlayerInput player)
    {
        Debug.Log("Toggling off dummy camera");
        this.gameObject.SetActive(false);
        Destroy(this.GameObject());
    }
}
