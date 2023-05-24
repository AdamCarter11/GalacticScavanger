using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private void Start()
    {
        //set to -1 before they select a character just in case
        PlayerPrefs.SetInt("Player1Character", -1);
        PlayerPrefs.SetInt("Player2Character", -1);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("DANIEL LEVEL DONT CHANGE NAME");
    }
    public void NavigatorSelect()
    {
        PlayerPrefs.SetInt("Player1Character", 1);
    }
    public void PilotSelect()
    {
        PlayerPrefs.SetInt("Player1Character", 2);
    }
    public void GunnerSelect()
    {
        PlayerPrefs.SetInt("Player2Character", 1);
    }
    public void MechanicSelect()
    {
        PlayerPrefs.SetInt("Player2Character", 2);
    }
}
