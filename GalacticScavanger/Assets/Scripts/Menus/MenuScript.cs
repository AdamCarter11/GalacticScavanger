using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuScript : MonoBehaviour
{
    [SerializeField] TMP_Text character1DisplayText;
    [SerializeField] TMP_Text character2DisplayText;
    [SerializeField] GameObject tutorialPanel;

    private void Start()
    {
        //set to -1 before they select a character just in case
        if(SceneManager.GetActiveScene().name == "GameOver")
        {
            PlayerPrefs.SetInt("Player1Character", -1);
            PlayerPrefs.SetInt("Player2Character", -1);
        }
        
    }
    public void StartGame()
    {
        if(PlayerPrefs.GetInt("Player1Character") != -1 && PlayerPrefs.GetInt("Player2Character") != -1 || SceneManager.GetActiveScene().name == "GameOver")
            SceneManager.LoadScene("DANIEL LEVEL DONT CHANGE NAME");
    }
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void NavigatorSelect()
    {
        PlayerPrefs.SetInt("Player1Character", 2);
        character1DisplayText.text = "Navigator";
    }
    public void PilotSelect()
    {
        PlayerPrefs.SetInt("Player1Character", 1);
        character1DisplayText.text = "Pilot";
    }
    public void GunnerSelect()
    {
        PlayerPrefs.SetInt("Player2Character", 1);
        character2DisplayText.text = "Gunner";
    }
    public void MechanicSelect()
    {
        PlayerPrefs.SetInt("Player2Character", 2);
        character2DisplayText.text = "Mechanic";
    }
}
