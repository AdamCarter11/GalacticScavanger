using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// code source: https://www.youtube.com/watch?v=3fzX-BX8hlI
public class CharacterSwitcher : MonoBehaviour
{
    private int index = 0;
    [SerializeField] private List<GameObject> characters = new List<GameObject>();
    private PlayerInputManager manager;

    private void Start()
    {
        manager = GetComponent<PlayerInputManager>();
        index = 0;//Random.Range(0, characters.Count);
        manager.playerPrefab = characters[index];
    }

    public void SwitchNextSpawnCharacter(PlayerInput input)
    {
        index = 1;//Random.Range(0, characters.Count);
        manager.playerPrefab = characters[index];
    }
}
