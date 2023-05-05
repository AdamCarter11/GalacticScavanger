using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private void Start()
    {
        //set to -1 before they select a character just in case
        PlayerPrefs.SetInt("WhichCharacter", -1);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }
    public void NavigatorSelect()
    {
        PlayerPrefs.SetInt("WhichCharacter", 1);
    }
    public void GunnerSelect()
    {
        PlayerPrefs.SetInt("WhichCharacter", 2);
    }
    public void MechanicSelect()
    {
        PlayerPrefs.SetInt("WhichCharacter", 3);
    }
}
